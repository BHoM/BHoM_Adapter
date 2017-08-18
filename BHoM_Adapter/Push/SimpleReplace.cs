using BH.Adapter.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public static partial class Push
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool SimpleReplace<T>(this IAdapter adapter, List<T> objectsToPush, string tag, Action<T> customDataWriter = null) where T : BH.oM.Base.BHoMObject
        {
            // Make sure objects being pushed are tagged
            objectsToPush.ForEach(x => x.Tags.Add(tag));

            // Delete objects that have the tag
            adapter.Delete(new FilterQuery(typeof(T), tag));

            // Add custom data to the objects to write
            if (customDataWriter != null)
                objectsToPush.ForEach(x => customDataWriter(x));

            // Finally Create the objects
            return false;
            //return adapter.Create(objectsToPush, tag);
        }
    }
}
