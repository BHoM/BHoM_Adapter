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

            IEnumerable<T> objectsToDelete;

            if (Config.ProcessInMemory)
            {
                objectsToCreate = ReplaceInMemory(newObjects, existing, tag);
                overwriteObjects = true;
                objectsToDelete = new List<T>();
            }
            else
            {
                objectsToCreate = ReplaceThroughAPI(newObjects, existing, tag, out objectsToDelete);
            }

            // Assign Id if needed
            if (Config.UseAdapterId)
            {
                AssignId(objectsToCreate);

                // Map Ids to the original set of objects (before we extracted the distincts elements from it)
                IEqualityComparer<T> comparer = Comparer<T>();
                foreach (T item in objectsToPush)
                    item.CustomData[AdapterId] = objectsToCreate.First(x => comparer.Equals(x, item)).CustomData[AdapterId].ToString();
            }

            //Delete elements to be replaced. Only applies to replace through API
            if(!Config.ProcessInMemory)
                Delete(typeof(T), objectsToDelete.Select(x => x.CustomData[AdapterId]));

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

                //foreach (var typeGroup in merged.GroupBy(x => x.GetType()))
                //{
                //    MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                //    var list = miListObject.Invoke(typeGroup.ToList(), new object[] { typeGroup.ToList() });

                //    if (!Replace(list as dynamic, tag))
                //        return false;
                //}

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

        protected bool MergeIntoSet<T>(IEnumerable<T> objects, IEnumerable<T> set, out IEnumerable<Tuple<T,T>> mergedObjects, out IEnumerable<T> unmergedObjects) where T : IBHoMObject
        {
            IEqualityComparer<T> comparer = Comparer<T>();

            VennDiagram<T> diagram = Engine.DataStructure.Create.VennDiagram(objects, set, comparer);
            diagram.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

            mergedObjects = diagram.Intersection;
            unmergedObjects = diagram.OnlySet1;

            return true;
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

        protected IEnumerable<T> ReplaceThroughAPI<T>(IEnumerable<T> newObjects, IEnumerable<T> existingObjects, string tag, out IEnumerable<T> objectsToDelete) where T : IBHoMObject
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

            //Define objects to be objects to be replaced for deletion
            objectsToDelete = diagram1.Intersection.Select(x => x.Item2).Concat(diagram2.Intersection.Select(x => x.Item2));

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