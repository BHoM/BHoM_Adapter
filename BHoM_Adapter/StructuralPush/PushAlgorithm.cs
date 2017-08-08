using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter.Interfaces;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using BH.oM.Materials;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.Engine.Base;

using BH.Adapter.Queries;

namespace BH.Adapter
{
    public static partial class StructuralPush
    {
        /***************************************************/
        /**** General Push Methods                      ****/
        /***************************************************/

        //Method assuming indexbased FE software
        private static bool GeneralPush<T>(IStructuralAdapter adapter, List<T> objectsToPush, List<T> existingObjects, IEqualityComparer<T> comparer, string tag, out List<string> ids) where T : BH.oM.Base.BHoMObject
        {
            //Create the id list for output
            ids = new List<string>();

            //Get a distinct set of the non id-materials to create
            List<T> objectsToCreate = objectsToPush.Distinct(comparer).ToList();

            //Make sure objects being pushed are tagged
            objectsToCreate.ForEach(x => x.Tags.Add(tag));


            /**********   Check tags **********/

            //Check if objects contains key
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
            List<T> createEexistingNoTag, existingNoTagEin;
            List<Tuple<T, T>> createUexistingNoTag;
            VennDiagram(objectsToCreate, nonTaggedObjects, out createUexistingNoTag, out createEexistingNoTag, out existingNoTagEin, comparer);

            //Map properties from existing to the objects to be created
            createUexistingNoTag.ForEach(x => MapProperties(x.Item1, x.Item2, adapter.AdapterId));

            /**********   Compare to objects with the provided tag   **********/

            //Compare objects to the existing objects that had the specified tag and other tag.
            List<T> createEexistingTagged, existingTaggedEcreate;
            List<Tuple<T, T>> createUexistingTagged;
            VennDiagram(createEexistingNoTag, multiTagObjects, out createUexistingTagged, out createEexistingTagged, out existingTaggedEcreate, comparer);

            //Map properties from existing to the objects to be created
            createUexistingTagged.ForEach(x => MapProperties(x.Item1, x.Item2, adapter.AdapterId));

            //Update the tags for the objects to update
            adapter.UpdateTags(existingTaggedEcreate);

            //Tag untagged objects with adapter ID
            SetIdToObjectsFromAdapter(createEexistingTagged, adapter);

            //Create delete queries for the objects to replace (being deleted here, to be replaced by the objects created). Note: Call not neccesary for GSA and robot
            adapter.Delete(GenerateDeleteFilterQuery(createUexistingNoTag.Select(x => x.Item2).Concat(createUexistingTagged.Select(x => x.Item2)), adapter.AdapterId));


            /**********   Create Objects      **********/

            //Create objects. Return false if something went wrong during the creation of the objects
            if (!adapter.CreateObjects(objectsToCreate)) return false;

            //Make sure every material is tagged with id
            TagPushObjectsFromCreatedObjects(objectsToCreate, objectsToPush, comparer, adapter.AdapterId, out ids);

            return true;
        }


        /***************************************************/

        private static void VennDiagram<T>(IEnumerable<T> setA, IEnumerable<T> setB, out List<Tuple<T,T>> aUb, out List<T> aEb, out List<T> bEa, IEqualityComparer<T> comparer)
            where T : BH.oM.Base.BHoMObject
        {
            aUb = new List<Tuple<T, T>>();
            aEb = new List<T>();
            bEa = new List<T>();
            foreach (T a in setA)
            {
                bool found = false;
                foreach (T b in setB)
                {
                    //Check if object exists
                    if (comparer.Equals(a, b))
                    {
                        aUb.Add(new Tuple<T, T>(a, b));
                        found = true;
                        break;
                    }
                }
                if (!found)
                    aEb.Add(a);
            }

            bEa = setB.Except(aUb.Select(x => x.Item2)).ToList();
        }


        /***************************************************/

        private static FilterQuery GenerateDeleteFilterQuery<T>(IEnumerable<T> objects, string adapterId) where T : BH.oM.Base.BHoMObject
        {
            FilterQuery filter = new FilterQuery();
            filter.Equalities["Type"] = typeof(T);
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

        /***************************************************/

        private static void TagPushObjectsFromCreatedObjects<T>(IEnumerable<T> createdObjects, IEnumerable<T> pushObjects, IEqualityComparer<T> comparer, string adapterId, out List<string> ids) where T : BH.oM.Base.BHoMObject
        {
            ids = new List<string>();
            foreach (T item in pushObjects)
            {
                string id = createdObjects.First(x => comparer.Equals(x, item)).CustomData[adapterId].ToString();
                item.CustomData[adapterId] = id;
                ids.Add(id);
            }
        }

       
    }
}
