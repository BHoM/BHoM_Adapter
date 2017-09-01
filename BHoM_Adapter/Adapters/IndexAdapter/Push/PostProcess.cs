using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Adapter
{
    public abstract partial class IndexAdapter
    {
        /***************************************************/
        /**** Protected  Methods                        ****/
        /***************************************************/

        protected override void CreatePostProcess<T>(IEnumerable<T> createdObjects, IEnumerable<T> pushedObjects, IEqualityComparer<T> comparer) 
        { 
            //Make sure every material is tagged with id
            foreach (T item in pushedObjects)
                item.CustomData[AdapterId] = createdObjects.First(x => comparer.Equals(x, item)).CustomData[AdapterId].ToString();
        }

    }
}
