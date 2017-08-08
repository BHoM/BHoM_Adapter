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

namespace BH.Adapter
{
    public static partial class StructuralPush
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
                success &= PushObjects(adapter as dynamic, objects as dynamic, out ids, key);
            }
            return success;
        }

        /***************************************************/
        /**** Bar Push Methods                          ****/
        /***************************************************/

        public static bool PushObjects(IBarAdapter adapter, List<Bar> bars, out List<string> ids, string tag, Dictionary<string, string> config = null)
        {
            //Get existing bars
            List<Bar> exisitingBars;
            adapter.GetBars(out exisitingBars);


            //Shallowclone the bars and their custom data
            bars.ForEach(x => x = (Bar)x.GetShallowClone());
            bars.ForEach(x => x.CustomData = new Dictionary<string, object>(x.CustomData));

            //////////// Dependent properties ///////////

            //Get unique section properties and clone the ones that does not contain a gsa ID
            Dictionary<Guid, SectionProperty> sectionProperties = bars.Select(x => x.SectionProperty).GetDistinctDictionary();
            Dictionary<Guid, SectionProperty> clonedSecProps = CloneObjects(sectionProperties);

            //Create the section properties
            List<string> propIds;
            if (!PushObjects(adapter,clonedSecProps.Values.ToList(), out propIds, tag))
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
            if (!PushObjects(adapter, clonedNodes.Values.ToList(), out nodeIDs, tag))
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
        /**** Material Methods                          ****/
        /***************************************************/

        public static bool PushObjects(IMaterialAdapter adapter, List<Material> materials, out List<string> ids, string key, Dictionary<string, string> config = null)
        {

            //Get existing materials
            List<Material> existingMaterialList;
            adapter.GetMaterials(out existingMaterialList);

            //Construct comparer
            IEqualityComparer<Material> comparer = new BHoMObjectNameComparer();

            //Push materials
            return GeneralPush(adapter, materials, existingMaterialList, comparer, key, out ids);
        }

        /***************************************************/
        /**** Section Property Methods                  ****/
        /***************************************************/
        public static bool PushObjects(ISectionPropertyAdapter adapter, List<SectionProperty> sectionProperties, out List<string> ids, string key, Dictionary<string, string> config = null)
        {
            //Get exisiting Properties
            ids = new List<string>();
            List<SectionProperty> existingProperties;
            adapter.GetSectionProperties(out existingProperties);


            //////////// Dependent properties ///////////

            //Get all unique materials from imported sectionproperties
            Dictionary<Guid, Material> materials = sectionProperties.Select(x => x.Material).GetDistinctDictionary();

            //Clone materials
            Dictionary<Guid, Material> clonedMaterials = CloneObjects(materials);

            //Assign cloned materials to section properties
            sectionProperties.ForEach(x => x.Material = clonedMaterials[x.Material.BHoM_Guid]);

            //Create all new materials
            List<string> materialIds;
            PushObjects(adapter, clonedMaterials.Values.ToList(), out materialIds, key);

            //////////// End dependent properties ///////////

            IEqualityComparer<SectionProperty> comparer = new BHoMObjectNameOrToStringComparer();

            return GeneralPush(adapter, sectionProperties, existingProperties, comparer, key, out ids);
        }

        /***************************************************/
        /**** Node Methods                              ****/
        /***************************************************/

        public static bool PushObjects(INodeAdapter adapter, List<Node> nodes, out List<string> ids, string key, Dictionary<string, string> config = null)
        {
            List<Node> existingNodeList;
            adapter.GetNodes(out existingNodeList);

            //Create node distancecomaprer, to check within 3 decimalplaces
            IEqualityComparer<Node> comparer = new NodeDistanceComparer(3);

            return GeneralPush(adapter, nodes, existingNodeList, comparer, key, out ids);

        }

    }
}
