using BH.Engine.Base;
using BH.oM.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
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
        public class AdapterType
        {
            public IEqualityComparer Comparer { get; set; }
            public List<Type> DependencyTypes { get; set; }
        }

        public static Dictionary<Type, AdapterType> m_AdapterTypes = new Dictionary<Type, AdapterType>
        {
            { typeof(Bar), new AdapterType { Comparer = EqualityComparer<Bar>.Default, DependencyTypes = new List<Type> { typeof(SectionProperty), typeof(Node) } } },
            //{ typeof(SectionProperty), new AdapterType { Comparer = new BHoMObjectNameOrToStringComparer(), DependencyTypes = new List<Type> { typeof(Material) } } },
            //{ typeof(Material), new AdapterType { Comparer = new BHoMObjectNameComparer(), DependencyTypes = new List<Type> { } } }
        };


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //public static bool PushByType(this IAdapter adapter, IEnumerable<object> objects, string tag, Dictionary<string, string> config = null)
        //{
        //    bool success = true;
        //    foreach (var typeGroup in objects.GroupBy(x => x.GetType()))
        //    {
        //        if (!m_AdapterTypes.ContainsKey(typeGroup.Key))
        //            return false;
        //        AdapterType adapterType = m_AdapterTypes[typeGroup.Key];
        //        //success &= adapter.PushType(typeGroup.ToList(), adapterType.Comparer, adapterType.DependencyTypes);
        //    }



        //    return success;
        //}
        public static bool PushByType(this IAdapter adapter, IEnumerable<object> objects, string tag, Dictionary<string, string> config = null)
        {
            bool success = true;
            foreach (IEnumerable<object> typeGroup in objects.GroupBy(x => x.GetType()))
                success &= PushType(adapter as dynamic, objects as dynamic, tag);

            return success;
        }

        /***************************************************/

        public static bool PushType(this IAdapter adapter, List<Bar> objectsToPush, string tag = "", bool applyMerge = true)
        {
            IEqualityComparer<Bar> comparer = EqualityComparer<Bar>.Default;
            List<Type> dependencyTypes = new List<Type> { typeof(SectionProperty), typeof(Node) };
            return _PushType(adapter, objectsToPush, comparer, dependencyTypes, tag, applyMerge);
        }

        /***************************************************/

        public static bool PushType(this IAdapter adapter, List<Node> objectsToPush, string tag = "", bool applyMerge = true)
        {
            IEqualityComparer<Node> comparer = new BH.Engine.Structure.NodeDistanceComparer(3);
            List<Type> dependencyTypes = new List<Type>();
            return _PushType(adapter, objectsToPush, comparer, dependencyTypes, tag, applyMerge);
        }

        /***************************************************/

        public static bool PushType(this IAdapter adapter, List<SectionProperty> objectsToPush, string tag = "", bool applyMerge = true)
        {
            IEqualityComparer<SectionProperty> comparer = new BH.Engine.Base.BHoMObjectNameOrToStringComparer();
            List<Type> dependencyTypes = new List<Type> { typeof(Material)};
            return _PushType(adapter, objectsToPush, comparer, dependencyTypes, tag, applyMerge);
        }

        /***************************************************/

        public static bool PushType(this IAdapter adapter, List<Material> objectsToPush, string tag = "", bool applyMerge = true)
        {
            IEqualityComparer<Material> comparer = new BH.Engine.Base.BHoMObjectNameComparer();
            List<Type> dependencyTypes = new List<Type>();
            return _PushType(adapter, objectsToPush, comparer, dependencyTypes, tag, applyMerge);
        }
    }
}
