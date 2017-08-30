using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;


namespace BH.Adapter
{
    public static partial class Update
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/


        public static void MapObjectAttributes<T>(this IAdapter adapter, List<Tuple<T, T>> objects)
        {
            _MapObjectAttributes(adapter as dynamic, objects);
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static void _MapObjectAttributes<T>(IAdapter adapter, List<Tuple<T, T>> objects)
        {

        }

        /***************************************************/

        private static void _MapObjectAttributes<T>(IIndexAdapter adapter, List<Tuple<T, T>> objects) where T : BHoMObject
        {
            objects.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, adapter.AdapterId));
        }

        /***************************************************/
    }
}
