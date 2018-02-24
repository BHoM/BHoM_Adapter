using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** BHoM Adapter Methods                      ****/
        /***************************************************/

        protected IEnumerable<IBHoMObject> Read(Type type, string tag = "")
        {
            // Get the objects based on the ids
            IEnumerable<IBHoMObject> objects = Read(type, null as List<object>);

            // Filter by tag if any 
            if (tag == "")
                return objects;
            else
                return objects.Where(x => x.Tags.Contains(tag));
        }
    }
}
