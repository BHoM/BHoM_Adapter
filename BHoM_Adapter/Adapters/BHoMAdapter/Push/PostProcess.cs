using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Protected  Methods                        ****/
        /***************************************************/

        protected virtual void CreatePostProcess<T>(IEnumerable<T> createdObjects, IEnumerable<T> pushedObjects, IEqualityComparer<T> comparer) where T : BHoMObject
        {
        }

    }
}
