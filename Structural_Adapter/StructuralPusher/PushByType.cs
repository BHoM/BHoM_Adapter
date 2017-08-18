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
using BH.Engine.Structure;

using BH.Adapter.Queries;
using BH.oM.Base;
using System.Reflection;
using BH.Adapter;

namespace BH.Adapter.Strutural
{
    public static partial class StructuralPusher
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool PushByType(IStructuralAdapter adapter, IEnumerable<object> objects, string tag, Dictionary<string, string> config = null)
        {
            bool success = true;
            foreach (IEnumerable<object> typeGroup in objects.GroupBy(x => x.GetType()))
                success &= Push(adapter as dynamic, objects as dynamic, tag);
   
            return success;
        }

        /***************************************************/

        public static bool Push(IBarAdapter adapter, List<Bar> bars, string tag, Dictionary<string, string> config = null)
        {
            // Shallowclone the bars and their custom data
            bars.ForEach(x => x = (Bar)x.GetShallowClone());
            bars.ForEach(x => x.CustomData = new Dictionary<string, object>(x.CustomData));

            // Merge and push the section properties
            List<SectionProperty> mergedSections = bars.MergePropertyObjects<Bar, SectionProperty>();
            if (!Push(adapter, mergedSections, tag))
                return false;

            // Merge and push the nodes
            List<Node> mergedNodes = bars.MergePropertyObjects<Bar, Node>();
            if (!Push(adapter, mergedNodes, tag))
                return false;

            // Push the bars themselves
            return GeneralPush(adapter, bars, EqualityComparer<Bar>.Default, tag);
        }

        /***************************************************/

        public static bool Push(IMaterialAdapter adapter, List<Material> materials, string tag, Dictionary<string, string> config = null)
        {
            return GeneralPush(adapter, materials, new BHoMObjectNameComparer(), tag);
        }

        /***************************************************/

        public static bool Push(ISectionPropertyAdapter adapter, List<SectionProperty> sectionProperties, string tag, Dictionary<string, string> config = null)
        {
            // Merge and push the section properties
            List<Material> mergedMaterials = sectionProperties.MergePropertyObjects<SectionProperty, Material>();
            if (!Push(adapter, mergedMaterials, tag))
                return false;

            // Push the section properties themselves
            return GeneralPush(adapter, sectionProperties, new BHoMObjectNameOrToStringComparer(), tag);
        }

        /***************************************************/

        public static bool Push(INodeAdapter adapter, List<Node> nodes, string tag, Dictionary<string, string> config = null)
        {
            return GeneralPush(adapter, nodes, new NodeDistanceComparer(3), tag);
        }

    }
}
