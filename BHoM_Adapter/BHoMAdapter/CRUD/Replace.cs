﻿using BH.Adapter.Queries;
using BH.Engine.DataStructure;
using BH.oM.Base;
using BH.oM.DataStructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected bool Replace<T>(IEnumerable<T> objectsToPush, string tag = "") where T : BHoMObject
        {
            // Make sure objects are distinct 
            List<T> newObjects = objectsToPush.Distinct(GetComparer<T>()).ToList();

            // Make sure objects  are tagged
            if (tag != "")
                newObjects.ForEach(x => x.Tags.Add(tag));

            // Merge and push the dependencies
            if (Config.SeparateProperties)
            {
                if (!ReplaceDependencies<T>(newObjects, tag))
                    return false;
            }

            //Read all the existing objects of that type
            IEnumerable<T> existing = Read(typeof(T)).Cast<T>();

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
                IEqualityComparer<T> comparer = GetComparer<T>();
                foreach (T item in objectsToPush)
                    item.CustomData[AdapterId] = objectsToCreate.First(x => comparer.Equals(x, item)).CustomData[AdapterId].ToString();
            }


            // Create objects
            if (!Create(objectsToCreate, overwriteObjects))
                return false;

            return true;
        }

        /***************************************************/
        /**** Helper Methods                            ****/
        /***************************************************/

        public bool ReplaceDependencies<T>(IEnumerable<T> objects, string tag) where T: BHoMObject
        {
            foreach (Type t in GetDependencyTypes<T>())
            {
                IEnumerable<object> merged = objects.MergePropertyObjects<T>(t);
                foreach (var typeGroup in merged.GroupBy(x => x.GetType()))
                    if (!Replace(typeGroup as dynamic, tag))
                        return false;
            }

            return true;
        }

        /***************************************************/

        protected void AssignId<T>(IEnumerable<T> objects) where T: BHoMObject
        {
            bool refresh = true;
            foreach (T item in objects)
            {
                if (!item.CustomData.ContainsKey(AdapterId))
                {
                    item.CustomData[AdapterId] = GetNextId(item.GetType(), refresh);
                    refresh = false;
                }
            }
        }

        /***************************************************/

        protected bool MergeIntoSet<T>(IEnumerable<T> objects, IEnumerable<T> set, out IEnumerable<Tuple<T,T>> mergedObjects, out IEnumerable<T> unmergedObjects) where T : BHoMObject
        {
            IEqualityComparer<T> comparer = GetComparer<T>();

            VennDiagram<T> diagram = objects.CreateVennDiagram(set, comparer);
            diagram.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

            mergedObjects = diagram.Intersection;
            unmergedObjects = diagram.OnlySet1;

            return true;
        }

        /***************************************************/

        protected IEnumerable<T> ReplaceInMemory<T>(IEnumerable<T> newObjects, IEnumerable<T> existingOjects, string tag) where T : BHoMObject
        {
            // Separate objects based on tags
            List<T> multiTaggedObjects = existingOjects.Where(x => x.Tags.Contains(tag) && x.Tags.Count > 1).ToList();
            IEnumerable<T> nonTaggedObjects = existingOjects.Where(x => !x.Tags.Contains(tag));

            // Remove the tag from the multi-tags objects
            multiTaggedObjects.ForEach(x => x.Tags.Remove(tag));

            // Merge objects if required
            if (Config.MergeWithComparer)
            {
                VennDiagram<T> diagram = newObjects.CreateVennDiagram(multiTaggedObjects.Concat(nonTaggedObjects), GetComparer<T>());
                diagram.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));
                newObjects = diagram.OnlySet1;
            }

            // Generate the list of objects to push
            IEnumerable<T> objectsToCreate = multiTaggedObjects.Concat(nonTaggedObjects).Concat(newObjects);
            return objectsToCreate;
        }

        /***************************************************/

        protected IEnumerable<T> ReplaceThroughAPI<T>(IEnumerable<T> newObjects, IEnumerable<T> existingObjects, string tag) where T : BHoMObject
        {
            //Check if objects contains tag
            IEnumerable<T> taggedObjects = existingObjects.Where(x => x.Tags.Contains(tag));
            IEnumerable<T> nonTaggedObjects = existingObjects.Where(x => !x.Tags.Contains(tag));

            //Remove tag from existing objects
            foreach (T item in taggedObjects)
                item.Tags.Remove(tag);

            // Delete object without a tag left
            Delete(typeof(T), taggedObjects.Where(x => x.Tags.Count == 0).Select(x => x.CustomData[AdapterId]));

            // Get objects without the tag that can potentially be merged with the new objects
            IEqualityComparer<T> comparer = GetComparer<T>();
            VennDiagram<T> diagram1 = newObjects.CreateVennDiagram(nonTaggedObjects, comparer);

            // Check and map properties
            diagram1.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

            // Get objectsmultiple tags that can potentially be merged with the new objects
            VennDiagram<T> diagram2 = diagram1.OnlySet1.CreateVennDiagram(taggedObjects.Where(x => x.Tags.Count > 0), comparer);

            // Check and map properties
            diagram2.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

            //Update the tags
            UpdateProperty(typeof(T), diagram2.OnlySet2.Select(x => x.CustomData[AdapterId]), "Tags", diagram2.OnlySet2.Select(x => x.Tags));

            //Delete objects to be replaced
            IEnumerable<T> objectsToDelete = diagram1.Intersection.Select(x => x.Item2).Concat(diagram2.Intersection.Select(x => x.Item2));
            Delete(typeof(T), objectsToDelete.Select(x => x.CustomData[AdapterId]));

            // return the objects to push
            return newObjects;
        }

    }
}


// Legacy replace method

//protected bool Replace<T>(List<T> objectsToPush, IEqualityComparer<T> comparer, List<Type> dependencies, string tag = "", bool applyMerge = true) where T : BHoMObject
//{
//    // Assure Objects to be pushed are distincts and have tags 
//    List<T> objectsToCreate = objectsToPush.Distinct(comparer).ToList();
//    objectsToCreate.ForEach(x => x.Tags.Add(tag));

//    // Get existing objects
//    IEnumerable<T> existingObjects = Pull(new FilterQuery(typeof(T))).Cast<T>();

//    // Merge and push the dependencies
//    foreach (Type t in dependencies)
//    {
//        IEnumerable<object> merged = objectsToCreate.MergePropertyObjects<T>(t);
//        foreach (var typeGroup in merged.GroupBy(x => x.GetType()))
//            if (!Replace(typeGroup as dynamic, tag))
//                return false;
//    }

//    //Check if objects contains tag
//    List<T> taggedObjects = existingObjects.Where(x => x.Tags.Contains(tag)).ToList();
//    List<T> nonTaggedObjects = existingObjects.Where(x => !x.Tags.Contains(tag)).ToList();

//    //Remove tag from existing objects
//    foreach (T item in taggedObjects)
//        item.Tags.Remove(tag);


//    Delete(taggedObjects.Where(x => x.Tags.Count == 0));


//    if (applyMerge)
//    {
//        // Get objects without the tag that can potentially be merged with the new objects
//        VennDiagram<T> diagram1 = objectsToCreate.CreateVennDiagram(nonTaggedObjects, comparer);

//        // Check and map properties
//        diagram1.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

//        // Get objectsmultiple tags that can potentially be merged with the new objects
//        VennDiagram<T> diagram2 = diagram1.OnlySet1.CreateVennDiagram(taggedObjects.Where(x => x.Tags.Count > 0), comparer);

//        // Check and map properties
//        diagram2.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

//        //Update the tags
//        UpdateProperty(typeof(T), diagram2.OnlySet2.Select(x => x.CustomData[AdapterId]).ToList(), "Tags", diagram2.OnlySet2.Select(x => x.Tags));

//        //Preprocess unaffected objects
//        AssignId(diagram2.OnlySet1);

//        //Delete objects to be replaced
//        Delete(diagram1.Intersection.Select(x => x.Item2).Concat(diagram2.Intersection.Select(x => x.Item2)));

//    }
//    else
//    {
//        //Preprocess objects to create
//        AssignId(objectsToCreate);
//    }

//    //Create objects
//    if (Create(objectsToCreate))
//    {
//        //Make sure every material is tagged with id
//        foreach (T item in objectsToPush)
//            item.CustomData[AdapterId] = objectsToCreate.First(x => comparer.Equals(x, item)).CustomData[AdapterId].ToString();
//        return true;
//    }

//    return false;
//}