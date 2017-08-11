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
using BH.Engine.Structure;

using BH.Adapter.Queries;
using BH.oM.Base;

namespace BH.Adapter
{
    public static partial class StructuralPusher
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool PushByType(IStructuralAdapter adapter, IEnumerable<object> objects, string key, Dictionary<string, string> config = null)
        {
            bool success = true;
            List<string> ids = new List<string>();
            foreach (IEnumerable<object> typeGroup in objects.GroupBy(x => x.GetType()))
            {
                success &= Push(adapter as dynamic, objects as dynamic, out ids, key);
            }
            return success;
        }

        /***************************************************/

        public static bool Push(IBarAdapter adapter, List<Bar> bars, out List<string> ids, string tag, Dictionary<string, string> config = null)
        {
            //Get existing bars
            List<Bar> exisitingBars = adapter.PullBars();


            //Shallowclone the bars and their custom data
            bars.ForEach(x => x = (Bar)x.GetShallowClone());
            bars.ForEach(x => x.CustomData = new Dictionary<string, object>(x.CustomData));

            //////////// Dependent properties ///////////

            //Get unique section properties and clone the ones that does not contain a gsa ID
            Dictionary<Guid, SectionProperty> sectionProperties = bars.Select(x => x.SectionProperty).GetDistinctDictionary();
            Dictionary<Guid, SectionProperty> clonedSecProps = CloneObjects(sectionProperties);

            //Create the section properties
            List<string> propIds;
            if (!Push(adapter,clonedSecProps.Values.ToList(), out propIds, tag))
            {
                ids = new List<string>();
                return false;
            }

            //Assign the clones section properties to the bars
            bars.ForEach(x => x.SectionProperty = clonedSecProps[x.SectionProperty.BHoM_Guid]);

            //Get unique nodes and clone the ones that does not contain a gsa ID
            Dictionary<Guid, Node> nodes = bars.SelectMany(x => new List<Node> { x.StartNode, x.EndNode }).GetDistinctDictionary();
            Dictionary<Guid, Node> clonedNodes = CloneObjects(nodes);

            //Create nodes
            List<string> nodeIDs;
            if (!Push(adapter, clonedNodes.Values.ToList(), out nodeIDs, tag))
            {
                ids = new List<string>();
                return false;
            }
            //Assign the cloned nodes to the bars
            bars.ForEach(x => x.StartNode = clonedNodes[x.StartNode.BHoM_Guid]);
            bars.ForEach(x => x.EndNode = clonedNodes[x.EndNode.BHoM_Guid]);

            //////////// End dependent properties ///////////


            //Construct comparer, use default comparer for now TODO: might need a better comparer here?
            IEqualityComparer<Bar> comparer = EqualityComparer<Bar>.Default;

            return GeneralPush(adapter, bars, exisitingBars, comparer, tag, out ids);

        }

        /***************************************************/

        public static bool Push(IMaterialAdapter adapter, List<Material> materials, out List<string> ids, string key, Dictionary<string, string> config = null)
        {

            //Get existing materials
            List<Material> existingMaterialList = adapter.PullMaterials();

            //Construct comparer
            IEqualityComparer<Material> comparer = new BHoMObjectNameComparer();

            //Push materials
            return GeneralPush(adapter, materials, existingMaterialList, comparer, key, out ids);
        }

        /***************************************************/

        public static bool Push(ISectionPropertyAdapter adapter, List<SectionProperty> sectionProperties, out List<string> ids, string key, Dictionary<string, string> config = null)
        {
            //Get exisiting Properties
            ids = new List<string>();
            List<SectionProperty> existingProperties = adapter.PullSectionProperties();


            //////////// Dependent properties ///////////

            //Get all unique materials from imported sectionproperties
            Dictionary<Guid, Material> materials = sectionProperties.Select(x => x.Material).GetDistinctDictionary();

            //Clone materials
            Dictionary<Guid, Material> clonedMaterials = CloneObjects(materials);

            //Assign cloned materials to section properties
            sectionProperties.ForEach(x => x.Material = clonedMaterials[x.Material.BHoM_Guid]);

            //Create all new materials
            List<string> materialIds;
            Push(adapter, clonedMaterials.Values.ToList(), out materialIds, key);

            //////////// End dependent properties ///////////

            IEqualityComparer<SectionProperty> comparer = new BHoMObjectNameOrToStringComparer();

            return GeneralPush(adapter, sectionProperties, existingProperties, comparer, key, out ids);
        }

        /***************************************************/

        public static bool Push(INodeAdapter adapter, List<Node> nodes, out List<string> ids, string key, Dictionary<string, string> config = null)
        {
            List<Node> existingNodeList = adapter.PullNodes();

            //Create node distancecomaprer, to check within 3 decimalplaces
            IEqualityComparer<Node> comparer = new NodeDistanceComparer(3);

            return GeneralPush(adapter, nodes, existingNodeList, comparer, key, out ids);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dictionary<Guid, T> GetDistinctDictionary<T>(this IEnumerable<T> list) where T : BHoMObject
        {
            return list.GroupBy(x => x.BHoM_Guid).Select(x => x.First()).ToDictionary(x => x.BHoM_Guid);
        }

        /***************************************************/

        public static Dictionary<Guid, T> CloneObjects<T>(Dictionary<Guid, T> dict) where T : BHoMObject
        {
            Dictionary<Guid, T> clones = new Dictionary<Guid, T>();

            foreach (KeyValuePair<Guid, T> kvp in dict)
            {
                T obj = (T)kvp.Value.GetShallowClone();
                obj.CustomData = new Dictionary<string, object>(kvp.Value.CustomData);
                clones.Add(kvp.Key, obj);
            }

            return clones;
        }
    }
}
