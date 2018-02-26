using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.DataStructure;
using System;
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

            // Make sure objects  are tagged
            if (tag != "")
                newObjects.ForEach(x => x.Tags.Add(tag));

            //Read all the existing objects of that type
            IEnumerable<T> existing = Read(typeof(T)).Cast<T>();

            // Merge and push the dependencies
            if (Config.SeparateProperties)
            {
                if (!ReplaceDependencies<T>(newObjects, tag))
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

                // Map Ids to the original set of objects (before we extracted the distincts elements from it)
                IEqualityComparer<T> comparer = Comparer<T>();
                foreach (T item in objectsToPush)
                    item.CustomData[AdapterId] = newObjects.First(x => comparer.Equals(x, item)).CustomData[AdapterId].ToString();
            }

            // Create objects
            if (!Create(objectsToCreate, overwriteObjects))
                return false;

            return true;
        }

        /***************************************************/
        /**** Helper Methods                            ****/
        /***************************************************/

        public bool ReplaceDependencies<T>(IEnumerable<T> objects, string tag) where T: IBHoMObject
        {

            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (Type t in DependencyTypes<T>())
            {

                IEnumerable<object> merged = objects.DistinctProperties<T>(t);
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { t });

                var list = miListObject.Invoke(merged, new object[] { merged });

                if (!Replace(list as dynamic, tag))
                    return false;

            }

            return true;
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
                VennDiagram<T> diagram = Engine.DataStructure.Create.VennDiagram(newObjects, multiTaggedObjects.Concat(nonTaggedObjects), Comparer<T>());
                diagram.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));
                newObjects = diagram.OnlySet1;
            }

            // Generate the list of objects to push
            IEnumerable<T> objectsToCreate = multiTaggedObjects.Concat(nonTaggedObjects).Concat(newObjects);
            return objectsToCreate;
        }

        /***************************************************/

        protected IEnumerable<T> ReplaceThroughAPI<T>(IEnumerable<T> newObjects, IEnumerable<T> existingObjects, string tag) where T : IBHoMObject
        {
            //Check if objects contains tag
            IEnumerable<T> taggedObjects = existingObjects.Where(x => x.Tags.Contains(tag)).ToList(); //ToList() necessary for the method to work correctly. The for each loop below removes the items from the IEnumerable when the tag is removed if not copied to a new list before hand. To be investigated
            IEnumerable<T> nonTaggedObjects = existingObjects.Where(x => !x.Tags.Contains(tag)).ToList();

            //Remove tag from existing objects
            foreach (T item in taggedObjects)
                item.Tags.Remove(tag);

            // Delete object without a tag left
            Delete(typeof(T), taggedObjects.Where(x => x.Tags.Count == 0).Select(x => x.CustomData[AdapterId]));

            // Get objects without the tag that can potentially be merged with the new objects
            IEqualityComparer<T> comparer = Comparer<T>();
            VennDiagram<T> diagram1 = Engine.DataStructure.Create.VennDiagram(newObjects, nonTaggedObjects, comparer);

            // Check and map properties
            diagram1.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

            // Get objectsmultiple tags that can potentially be merged with the new objects
            VennDiagram<T> diagram2 = Engine.DataStructure.Create.VennDiagram(diagram1.OnlySet1, taggedObjects.Where(x => x.Tags.Count > 0), comparer);

            // Check and map properties
            diagram2.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

            //Update the tags
            UpdateProperty(typeof(T), diagram2.OnlySet2.Select(x => x.CustomData[AdapterId]), "Tags", diagram2.OnlySet2.Select(x => x.Tags));

            //Update the objects in the venn diagram intersections
            UpdateObjects(diagram1.Intersection.Select(x => x.Item1).Concat(diagram2.Intersection.Select(x => x.Item1)));

            // return the objects to push
            return diagram2.OnlySet1;
        }

    }
}
