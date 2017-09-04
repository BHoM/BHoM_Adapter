using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public abstract partial class IndexAdapter
    {
        /***************************************************/
        /**** BHoM Adapter Methods                      ****/
        /***************************************************/

        protected override IEnumerable<BHoMObject> Read(Type type, string tag = "")
        {
            // Get the objects based on the indices
            IEnumerable<BHoMObject> fromIndices = Read(type, null);

            // Filter by tag if any 
            if (tag == "")
                return fromIndices;
            else
                return fromIndices.Where(x => x.Tags.Contains(tag));
        }
    }
}
