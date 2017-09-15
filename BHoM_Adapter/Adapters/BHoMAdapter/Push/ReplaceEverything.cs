using BH.Adapter.Queries;
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
        /**** Protected Methods                         ****/
        /***************************************************/

        public bool ReplaceEverything(List<BHoMObject> objectsToPush, string tag = "", Action<object> customDataWriter = null)
        {
            // Make sure objects being pushed are tagged
            if (tag != "")
                objectsToPush.ForEach(x => x.Tags.Add(tag));

            // Add custom data to the objects to write
            if (customDataWriter != null)
                objectsToPush.ForEach(x => customDataWriter(x));

            // Finally Create the objects
            return Create(objectsToPush, true);
        }
    }
}
