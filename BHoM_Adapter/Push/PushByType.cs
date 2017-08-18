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

        public static bool PushByType(this IAdapter adapter, IEnumerable<object> objects, string tag, Dictionary<string, string> config = null)
        {
            bool success = true;
            foreach (var typeGroup in objects.GroupBy(x => x.GetType()))
            {
                if (!m_AdapterTypes.ContainsKey(typeGroup.Key))
                    return false;
                AdapterType adapterType = m_AdapterTypes[typeGroup.Key];
                //success &= adapter.PushType(typeGroup.ToList(), adapterType.Comparer, adapterType.DependencyTypes);
            }
               


            return success;
        }

        /***************************************************/
    }
}
