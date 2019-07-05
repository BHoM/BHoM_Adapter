using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{

    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Protected Abstract CRUD Methods           ****/
        /***************************************************/

        // Level 1 - Always required

        protected abstract bool Create<T>(IEnumerable<T> objects, bool replaceAll = false) where T : IObject;
    }
}
