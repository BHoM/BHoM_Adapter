/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected bool Replace<T>(IEnumerable<T> objectsToPush, string tag = "") where T : IBHoMObject
        {
            // Make sure objects are distinct 
            List<T> newObjects = objectsToPush.Distinct(Comparer<T>()).ToList();

            //Read all the existing objects of that type
            IEnumerable<T> existing;

            if (tag != "" || Comparer<T>() != EqualityComparer<T>.Default)
                existing = Read(typeof(T)).Where(x => x != null && x is T).Cast<T>();
            else
                existing = new List<T>();

            // Merge and push the dependencies
            if (Config.SeparateProperties)
            {
                var dependencyObjects = GetDependencyObjects<T>(objectsToPush, tag);

                foreach (var depObj in dependencyObjects)
                    if (!Replace(depObj.Value as dynamic, tag))
                        return false;
            }

            // Replace objects that overlap and define the objects that still have to be pushed
            IEnumerable<T> objectsToCreate = newObjects;
            bool overwriteObjects = false;

            if (Config.ProcessInMemory)
            {
                objectsToCreate = ReplaceInMemory(newObjects, existing, tag);
                overwriteObjects = true;
            }
            else
            {
                objectsToCreate = ReplaceThroughAPI(newObjects, existing, tag);
            }

            // Assign Id if needed
            if (Config.UseAdapterId)
            {
                AssignId(objectsToCreate);
            }

            // Create objects
            if (!Create(objectsToCreate, overwriteObjects))
                return false;
            else if (Config.UseAdapterId)
            {
                // Map Ids to the original set of objects (before we extracted the distincts elements from it)
                IEqualityComparer<T> comparer = Comparer<T>();
                foreach (T item in objectsToPush)
                    item.CustomData[AdapterId] = newObjects.First(x => comparer.Equals(x, item)).CustomData[AdapterId];
            }

            return true;
        }

        /***************************************************/
        /**** Helper Methods                            ****/
        /***************************************************/

        public Dictionary<Type, IEnumerable> GetDependencyObjects<T>(IEnumerable<T> objects, string tag) where T : IBHoMObject
        {
            Dictionary<Type, IEnumerable> dict = new Dictionary<Type, IEnumerable>();

            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (Type t in DependencyTypes<T>())
            {

                IEnumerable<object> merged = objects.DistinctProperties<T>(t);
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { t });

                var list = miListObject.Invoke(merged, new object[] { merged });

                dict.Add(t, list as IEnumerable);
            }

            return dict;
        }

        /***************************************************/

        protected void AssignId<T>(IEnumerable<T> objects) where T: IBHoMObject
        {
            bool refresh = true;
            foreach (T item in objects)
            {
                if (!item.CustomData.ContainsKey(AdapterId))
                {
                    item.CustomData[AdapterId] = NextId(typeof(T), refresh);
                    refresh = false;
                }
            }
        }


        /***************************************************/

        protected IEnumerable<T> ReplaceInMemory<T>(IEnumerable<T> newObjects, IEnumerable<T> existingOjects, string tag) where T : IBHoMObject
        {
            // Separate objects based on tags
            List<T> multiTaggedObjects = existingOjects.Where(x => x.Tags.Contains(tag) && x.Tags.Count > 1).ToList();
            IEnumerable<T> nonTaggedObjects = existingOjects.Where(x => !x.Tags.Contains(tag));

            // Remove the tag from the multi-tags objects
            multiTaggedObjects.ForEach(x => x.Tags.Remove(tag));

            // Merge objects if required
            if (Config.MergeWithComparer)
            {
                VennDiagram<T> diagram = Engine.Data.Create.VennDiagram(newObjects, multiTaggedObjects.Concat(nonTaggedObjects), Comparer<T>());
                diagram.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));
                newObjects = diagram.OnlySet1;
            }

            // Generate the list of objects to push
            IEnumerable<T> objectsToCreate = multiTaggedObjects.Concat(nonTaggedObjects).Concat(newObjects);
            return objectsToCreate;
        }

        /***************************************************/

        protected IEnumerable<T> ReplaceThroughAPI<T>(IEnumerable<T> objsToPush, IEnumerable<T> existingObjs, string tag) where T : IBHoMObject
        {
            IEqualityComparer<T> comparer = Comparer<T>();
            VennDiagram<T> diagram = Engine.Data.Create.VennDiagram(objsToPush, existingObjs, comparer);

            // Objects to push that do not have any overlap with the existing ones
            List<T> objsToPush_exclusive = diagram.OnlySet1.ToList();

            // Objects existing in the model that do not have any overlap with the objects being pushed
            List<T> existingObjs_exclusive = diagram.OnlySet2.ToList();

            // Do not consider exclusive existing objects that do not contain the currently specified tag. 
            // Those objects do not need any update, so they will be left as they are.
            existingObjs_exclusive.RemoveAll(x => !x.Tags.Contains(tag));

            // Remove the current tag from exclusive existing objects
            existingObjs_exclusive.ForEach(x => x.Tags.Remove(tag));

            // Delete exclusive existing objects that do not have any other tag except the current tag from the model
            Delete(typeof(T), existingObjs_exclusive.Where(x => x.Tags.Count == 0).Select(x => x.CustomData[AdapterId]));

            // Update the tags for the rest of the existing objects in the model
            UpdateProperty(typeof(T), 
                existingObjs_exclusive.Where(x => x.Tags.Count > 0).Select(x => x.CustomData[AdapterId]), 
                "Tags", 
                existingObjs_exclusive.Where(x => x.Tags.Count > 0).Select(x => x.Tags));

            // Map properties for the objects that overlap (between existing and pushed) and Update them
            diagram.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));
            UpdateObjects(diagram.Intersection.Select(x => x.Item1));

            // Return the objectsToPush that do not have any overlap with the existing ones; those will need to be created
            return objsToPush_exclusive;
        }

    }
}
