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

        protected bool ReplaceMergePush<T>(List<T> objectsToPush, IEqualityComparer<T> comparer, List<Type> dependencies, string tag = "", bool applyMerge = true) where T : BHoMObject
        {
            // Assure Objects to be pushed are distincts and have tags 
            List<T> objectsToCreate = objectsToPush.Distinct(comparer).ToList();
            objectsToCreate.ForEach(x => x.Tags.Add(tag));

            // Get existing objects
            IEnumerable<T> existingObjects = Pull(new List<IQuery> { new FilterQuery(typeof(T)) }).Cast<T>();

            // Merge and push the dependencies
            foreach (Type t in dependencies)
            {
                if (!PushByType(objectsToCreate.MergePropertyObjects<T>(t), tag))
                    return false;
            }

            //Check if objects contains tag
            List<T> taggedObjects = existingObjects.Where(x => x.Tags.Contains(tag)).ToList();
            List<T> nonTaggedObjects = existingObjects.Where(x => !x.Tags.Contains(tag)).ToList();

            //Remove tag from existing objects
            foreach (T item in taggedObjects)
                item.Tags.Remove(tag);


            DeleteObjects(taggedObjects.Where(x => x.Tags.Count == 0));


            if (applyMerge)
            {
                // Get objects without the tag that can potentially be merged with the new objects
                VennDiagram<T> diagram1 = objectsToCreate.CreateVennDiagram(nonTaggedObjects, comparer);

                // Check and map properties
                MapObjectAttributes(diagram1.Intersection);

                // Get objectsmultiple tags that can potentially be merged with the new objects
                VennDiagram<T> diagram2 = diagram1.OnlySet1.CreateVennDiagram(taggedObjects.Where(x => x.Tags.Count > 0), comparer);

                // Check and map properties
                MapObjectAttributes(diagram2.Intersection);

                //Update the tags
                UpdateTags(diagram2.OnlySet2);

                //Preprocess unaffected objects
                CreatePreProcess(diagram2.OnlySet1);

                //Delete objects to be replaced
                DeleteObjects(diagram1.Intersection.Select(x => x.Item2).Concat(diagram2.Intersection.Select(x => x.Item2)));

            }
            else
            {
                //Preprocess objects to create
                CreatePreProcess(objectsToCreate);
            }

            //Create objects
            if (Create(objectsToCreate))
            {
                //Post process
                CreatePostProcess(objectsToCreate, objectsToPush, comparer);
                return true;
            }

            return false;
        }
    }
}
