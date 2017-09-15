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

        public bool Replace(IEnumerable<BHoMObject> objectsToPush, Type type, string tag, Action<object> customDataWriter = null) 
        {
            List<BHoMObject> objectList = objectsToPush.ToList();

            // Make sure objects being pushed are tagged
            objectList.ForEach(x => x.Tags.Add(tag));

            // Delete objects that have the tag
            Delete(type, tag);

            // Add custom data to the objects to write
            if (customDataWriter != null)
                objectList.ForEach(x => customDataWriter(x));

            // Finally Create the objects
            return Create(objectsToPush);
        }
    }
}
