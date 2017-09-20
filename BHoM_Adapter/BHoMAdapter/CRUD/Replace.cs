using BH.Adapter.Queries;
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
            if (PushConfiguration.SeparateProperties)
            {
                if (!ReplaceDependencies<T>(newObjects, tag))
                    return false;
            }

            //Read all the existing objects of that type
            IEnumerable<T> existing = Read(typeof(T)).Cast<T>();
            IEnumerable<T> objectsToCreate = newObjects;

            if (PushConfiguration.ProcessInMemory)
            {
                // Separate objects based on tags
                List<T> multiTaggedObjects = existing.Where(x => x.Tags.Contains(tag) && x.Tags.Count > 1).ToList();
                IEnumerable<T> nonTaggedObjects = existing.Where(x => !x.Tags.Contains(tag));

                // Remove the tag from the multi-tags objects
                multiTaggedObjects.ForEach(x => x.Tags.Remove(tag));

                // Generate the list of objects to push
                objectsToCreate = multiTaggedObjects.Concat(nonTaggedObjects).Concat(newObjects);

                // Create objects
                if (!Create(objectsToCreate, true))
                    return false;
            }
            else
            {
                // Delete objects that have the tag
                IEnumerable<T> objectsToDelete = existing.Where(x => x.Tags.Contains(tag) && x.Tags.Count == 1);
                Delete(typeof(T), objectsToDelete.Select(x => x.CustomData[AdapterId]));

                //

                // Create objects
                if (!Create(objectsToCreate))
                    return false;
            }

            //Make sure every material is tagged with id
            IEqualityComparer<T> comparer = GetComparer<T>();
            foreach (T item in objectsToPush)
                item.CustomData[AdapterId] = objectsToCreate.First(x => comparer.Equals(x, item)).CustomData[AdapterId].ToString();

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

        public bool Replace<T>(IEnumerable<T> objectsToPush, string tag, Action<object> customDataWriter = null) where T: BHoMObject
        {
            List<T> objectList = objectsToPush.ToList();

            // Make sure objects being pushed are tagged
            if (tag != "")
                objectList.ForEach(x => x.Tags.Add(tag));

            // Delete objects that have the tag
            Delete(typeof(T), tag);

            // Add custom data to the objects to write
            if (customDataWriter != null)
                objectList.ForEach(x => customDataWriter(x));

            // Finally Create the objects
            return Create(objectsToPush);
        }

        /***************************************************/

        public bool Replace(List<BHoMObject> objectsToPush, string tag = "", Action<object> customDataWriter = null)
        {
            // Make sure objects being pushed are tagged
            if (tag != "")
                objectsToPush.ForEach(x => x.Tags.Add(tag));

            // Add custom data to the objects to write
            if (customDataWriter != null)
                objectsToPush.ForEach(x => customDataWriter(x));

            // Finally Create the objects
            return Create(objectsToPush, true);
        }

        /***************************************************/

        protected bool Replace<T>(List<T> objectsToPush, IEqualityComparer<T> comparer, List<Type> dependencies, string tag = "", bool applyMerge = true) where T : BHoMObject
        {
            // Assure Objects to be pushed are distincts and have tags 
            List<T> objectsToCreate = objectsToPush.Distinct(comparer).ToList();
            objectsToCreate.ForEach(x => x.Tags.Add(tag));

            // Get existing objects
            IEnumerable<T> existingObjects = Pull(new FilterQuery(typeof(T))).Cast<T>();

            // Merge and push the dependencies
            foreach (Type t in dependencies)
            {
                IEnumerable<object> merged = objectsToCreate.MergePropertyObjects<T>(t);
                foreach (var typeGroup in merged.GroupBy(x => x.GetType()))
                    if (!Replace(typeGroup as dynamic, tag))
                        return false;
            }

            //Check if objects contains tag
            List<T> taggedObjects = existingObjects.Where(x => x.Tags.Contains(tag)).ToList();
            List<T> nonTaggedObjects = existingObjects.Where(x => !x.Tags.Contains(tag)).ToList();

            //Remove tag from existing objects
            foreach (T item in taggedObjects)
                item.Tags.Remove(tag);


            Delete(taggedObjects.Where(x => x.Tags.Count == 0));


            if (applyMerge)
            {
                // Get objects without the tag that can potentially be merged with the new objects
                VennDiagram<T> diagram1 = objectsToCreate.CreateVennDiagram(nonTaggedObjects, comparer);

                // Check and map properties
                diagram1.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

                // Get objectsmultiple tags that can potentially be merged with the new objects
                VennDiagram<T> diagram2 = diagram1.OnlySet1.CreateVennDiagram(taggedObjects.Where(x => x.Tags.Count > 0), comparer);

                // Check and map properties
                diagram2.Intersection.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));

                //Update the tags
                UpdateProperty(typeof(T), diagram2.OnlySet2.Select(x => x.CustomData[AdapterId]).ToList(), "Tags", diagram2.OnlySet2.Select(x => x.Tags));

                //Preprocess unaffected objects
                Create_PreProcess(diagram2.OnlySet1);

                //Delete objects to be replaced
                Delete(diagram1.Intersection.Select(x => x.Item2).Concat(diagram2.Intersection.Select(x => x.Item2)));

            }
            else
            {
                //Preprocess objects to create
                Create_PreProcess(objectsToCreate);
            }

            //Create objects
            if (Create(objectsToCreate))
            {
                //Make sure every material is tagged with id
                foreach (T item in objectsToPush)
                    item.CustomData[AdapterId] = objectsToCreate.First(x => comparer.Equals(x, item)).CustomData[AdapterId].ToString();
                return true;
            }

            return false;
        }


        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private void Create_PreProcess<T>(IEnumerable<T> objects) where T: BHoMObject
        {
            bool refresh = true;
            foreach (T item in objects)
            {
                item.CustomData[AdapterId] = GetNextId(typeof(T), refresh);
                refresh = false;
            }
        }

    }
}
