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

        protected virtual void Delete<T>(IEnumerable<T> objects, Dictionary<string, string> config = null) where T : BHoMObject
        {
            Delete(GenerateFilterQuery<T>(objects), config);
        }

    }
}
