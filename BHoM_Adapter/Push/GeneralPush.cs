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
    public static partial class Push
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        private static bool PushType<T>(this IAdapter adapter, List<T> objectsToPush, IEqualityComparer<T> comparer, List<Type> dependencies, string tag = "", bool applyMerge = true) where T : BHoMObject
        {
            // Assure Objects to be pushed are distincts and have tags 
            List<T> objectsToCreate = objectsToPush.Distinct(comparer).ToList();
            objectsToCreate.ForEach(x => x.Tags.Add(tag));

            // Get existing objects
            IEnumerable<T> existingObjects = adapter.Pull(new List<IQuery> { new FilterQuery(typeof(T)) }).Cast<T>();

            // Merge and push the dependencies
            foreach (Type t in dependencies)
            {
                if (!adapter.Push(existingObjects.MergePropertyObjects<T>(t), tag))
                    return false;
            }
            
            //Remove tag from existing objects
            foreach (T item in existingObjects)
                item.Tags.Remove(tag);

            return false;
            // TODO: Find a way to make this work
            // Delete objects left without a tag
            //adapter.Delete(adapter.CreateFilterQuery(existingObjects.Where(x => x.Tags.Count == 0)));

            //// Handle object merging if required
            //if (applyMerge)
            //{
            //    // Get objects that can potentially be merged with the new objects
            //    IEnumerable<T> potentialSet = existingObjects.Where(x => x.Tags.Count > 0);

            //    //Map properties from existing objects to objects to create that are mmatching
            //    VennDiagram<T> diagram = existingObjects.CreateVennDiagram<T>(potentialSet, comparer);
            //    diagram.Intersection.ForEach(x => adapter.MapSpecialProperties(x.Item1, x.Item2));

            //    // Delete matching existing objects
            //    adapter.Delete(adapter.CreateFilterQuery(diagram.Intersection.Select(x => x.Item2)));
            //}

            ////Create objects. Return false if something went wrong during the creation of the objects
            //return adapter.Create(objectsToCreate);
        }
    }
}
