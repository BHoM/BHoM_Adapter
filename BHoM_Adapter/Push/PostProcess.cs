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

        private static void CreatePostProcess<T>(IAdapter adapter, IEnumerable<T> createdObjects, IEnumerable<T> pushedObjects, IEqualityComparer<T> comparer) where T : BHoMObject
        {
            _CreatePostProcess(adapter as dynamic, createdObjects, pushedObjects, comparer);
        }

        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static void _CreatePostProcess<T>(IAdapter adapter, IEnumerable<T> createdObjects, IEnumerable<T> pushedObjects, IEqualityComparer<T> comparer) where T : BHoMObject
        {

        }

        /***************************************************/

        private static void _CreatePostProcess<T>(IIndexAdapter adapter, IEnumerable<T> createdObjects, IEnumerable<T> pushedObjects, IEqualityComparer<T> comparer) where T : BHoMObject
        {
            //Make sure every material is tagged with id
            foreach (T item in pushedObjects)
                item.CustomData[adapter.AdapterId] = createdObjects.First(x => comparer.Equals(x, item)).CustomData[adapter.AdapterId].ToString();
        }

        /***************************************************/
    }
}
