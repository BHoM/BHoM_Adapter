using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Adapter
{
    public static partial class Push
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        private static void CreatePreProcess<T>(IAdapter adapter, IEnumerable<T> objects) where T : BHoMObject
        {
            _CreatePreProcess(adapter as dynamic, objects);
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static void _CreatePreProcess<T>(IAdapter adapter, IEnumerable<T> objects) where T : BHoMObject
        {

        }

        /***************************************************/

        private static void _CreatePreProcess<T>(IIndexAdapter adapter, IEnumerable<T> objects) where T : BHoMObject
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
