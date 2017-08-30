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

        private static bool _PushType<T>(this IAdapter adapter, List<T> objectsToPush, IEqualityComparer<T> comparer, List<Type> dependencies, string tag = "", bool applyMerge = true) where T : BHoMObject
        {
            // Assure Objects to be pushed are distincts and have tags 
            List<T> objectsToCreate = objectsToPush.Distinct(comparer).ToList();
            objectsToCreate.ForEach(x => x.Tags.Add(tag));

            // Get existing objects
            IEnumerable<T> existingObjects = adapter.Pull(new List<IQuery> { new FilterQuery(typeof(T)) }).Cast<T>();

            // Merge and push the dependencies
            foreach (Type t in dependencies)
            {
                if (!adapter.PushByType(objectsToCreate.MergePropertyObjects<T>(t), tag))
                    return false;
            }

            //Check if objects contains tag
            List<T> taggedObjects = existingObjects.Where(x => x.Tags.Contains(tag)).ToList();
            List<T> nonTaggedObjects = existingObjects.Where(x => !x.Tags.Contains(tag)).ToList();

            //Remove tag from existing objects
            foreach (T item in taggedObjects)
                item.Tags.Remove(tag);


            adapter.DeleteObjects(taggedObjects.Where(x => x.Tags.Count == 0), tag);


            if (applyMerge)
            {
                // Get objects without the tag that can potentially be merged with the new objects
                VennDiagram<T> diagram1 = objectsToCreate.CreateVennDiagram(nonTaggedObjects, comparer);

                // Check and map properties
                adapter.MapObjectAttributes(diagram1.Intersection);

                // Get objectsmultiple tags that can potentially be merged with the new objects
                VennDiagram<T> diagram2 = diagram1.OnlySet1.CreateVennDiagram(taggedObjects.Where(x => x.Tags.Count > 0), comparer);

                // Check and map properties
                adapter.MapObjectAttributes(diagram2.Intersection);

                //Update the tags
                adapter.UpdateTags(diagram2.OnlySet2);

                //Preprocess unaffected objects
                CreatePreProcess(adapter, diagram2.OnlySet1);

                //Delete objects to be replaced
                adapter.DeleteObjects(diagram1.Intersection.Select(x => x.Item2).Concat(diagram2.Intersection.Select(x => x.Item2)), tag);

            }
            else
            {
                //Preprocess objects to create
                CreatePreProcess(adapter, objectsToCreate);
            }

            //Create objects
            if (adapter.Create(objectsToCreate))
            {
                //Post process
                CreatePostProcess(adapter, objectsToCreate, objectsToPush, comparer);
                return true;
            }

            return false;
        }
    }
}
