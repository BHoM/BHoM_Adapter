using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Structural.Properties;
using BH.Adapter.Queries;

namespace BH.Adapter
{
    public static partial class StructuralPush
    {
        /***************************************************/

        public static Dictionary<Guid, T> GetDistinctDictionary<T>(this IEnumerable<T> list) where T : IObject
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

        /***************************************************/


    }
}
