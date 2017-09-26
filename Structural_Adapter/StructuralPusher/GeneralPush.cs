using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter.Structural;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using BH.oM.Materials;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Base;
using BH.Adapter.Queries;
using BH.oM.DataStructure;
using BH.Engine.DataStructure;

namespace BH.Adapter.Structural
{
    public static partial class StructuralPusher
    {
        /***************************************************/
        /**** General Push Methods                      ****/
        /***************************************************/

        //Method assuming index-based FE software
        private static bool GeneralPush<T>(IStructuralAdapter adapter, IEnumerable<T> objectsToPush, IEqualityComparer<T> comparer, string tag) where T : BH.oM.Base.BHoMObject
        {
            /********** Assure Objects to be pushed are distincts and have tags **********/

            //Get a distinct set of the non id-materials to create
            List<T> objectsToCreate = objectsToPush.Distinct(comparer).ToList();

            //Make sure objects being pushed are tagged
            objectsToCreate.ForEach(x => x.Tags.Add(tag));


            /**********   Check tags **********/

            IEnumerable<T> existingObjects = adapter.Pull(new List<IQuery> { new FilterQuery(typeof(T)) }).Cast<T>();

            //Check if objects contains tag
            List<T> taggedObjects = existingObjects.Where(x => x.Tags.Contains(tag)).ToList();
            List<T> nonTaggedObjects = existingObjects.Where(x => !x.Tags.Contains(tag)).ToList();

            //Remove tag from existing objects
            foreach (T item in taggedObjects)
                item.Tags.Remove(tag);

            //Delete objects with only the specified tag
            adapter.Delete(GenerateDeleteFilterQuery(taggedObjects.Where(x => x.Tags.Count == 0), adapter.AdapterId));

            //Get objects that have tags left
            List<T> multiTagObjects = taggedObjects.Where(x => x.Tags.Count > 0).ToList();

            /**********   Compare to objects without the provided tag **********/

            //Compare objects to the existing objects without the specified tag.
            VennDiagram<T> noTagDiagram = nonTaggedObjects.CreateVennDiagram<T>(objectsToCreate, comparer);

            //Map properties from existing to the objects to be created
            noTagDiagram.Intersection.ForEach(x => x.Item2.MapSpecialProperties(x.Item1, adapter.AdapterId));

            /**********   Compare to objects with the provided tag   **********/

            //Compare objects to the existing objects that had the specified tag and other tag.
            VennDiagram<T> multiTagDiagram = multiTagObjects.CreateVennDiagram<T>(noTagDiagram.OnlySet2, comparer);

            //Map properties from existing to the objects to be created
            multiTagDiagram.Intersection.ForEach(x => x.Item2.MapSpecialProperties(x.Item1, adapter.AdapterId));

            //Update the tags for the objects to update
            adapter.UpdateTags(multiTagDiagram.OnlySet1);

            //Tag untagged objects with adapter ID
            SetIdToObjectsFromAdapter(multiTagDiagram.OnlySet2, adapter);

            //Create delete queries for the objects to replace (being deleted here, to be replaced by the objects created). Note: Call not neccesary for GSA and robot
            adapter.Delete(GenerateDeleteFilterQuery(noTagDiagram.Intersection.Select(x => x.Item2).Concat(multiTagDiagram.Intersection.Select(x => x.Item2)), adapter.AdapterId));


            /**********   Create Objects      **********/

            //Create objects. Return false if something went wrong during the creation of the objects
            if (!adapter.Create(objectsToCreate)) return false;

            //Make sure every material is tagged with id
            foreach (T item in objectsToPush)
                item.CustomData[adapter.AdapterId] = objectsToCreate.First(x => comparer.Equals(x, item)).CustomData[adapter.AdapterId].ToString();

            return true;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static FilterQuery GenerateDeleteFilterQuery<T>(IEnumerable<T> objects, string adapterId) where T : BH.oM.Base.BHoMObject
        {
            FilterQuery filter = new FilterQuery();
            filter.Type = typeof(T);
            filter.Equalities["Indices"] = objects.Select(x => x.CustomData[adapterId].ToString()).ToList();
            return filter;
        }

        /***************************************************/

        private static void SetIdToObjectsFromAdapter<T>(IEnumerable<T> objects, IStructuralAdapter adapter) where T : BH.oM.Base.BHoMObject
        {
            bool refresh = true;
            string idKey = adapter.AdapterId;
            foreach (T item in objects)
            {
                item.CustomData[idKey] = adapter.GetNextIndex(typeof(T), refresh);
                refresh = false;
            }
        }


    }
}
