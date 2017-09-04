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

        protected override void CreatePreProcess<T>(IEnumerable<T> objects) 
        {
            bool refresh = true;
            foreach (T item in objects)
            {
                item.CustomData[AdapterId] = GetNextIndex(typeof(T), refresh);
                refresh = false;
            }
        }
    }
}
