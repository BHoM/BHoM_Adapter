using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Base;

namespace BH.Adapter
{
    public static partial class Delete
    {

        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/


        public static int DeleteObjects<T>(this IAdapter adapter, IEnumerable<T> objects, string tag) where T: BHoMObject
        {
            return _DeleteObjects(adapter as dynamic, objects, tag);
        }


        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static int _DeleteObjects<T>(this IIndexAdapter adapter, IEnumerable<T> objects, string tag) where T : BHoMObject
        {
            return adapter.Delete(GenerateDeleteFilterQuery(objects, adapter.AdapterId));
        }

        /***************************************************/
    }
}
