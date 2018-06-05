using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected bool UpdateOnly<T>(IEnumerable<T> objectsToPush, string tag = "") where T : IBHoMObject
        {
            List<T> newObjects = objectsToPush.ToList();

            // Make sure objects  are tagged
            if (tag != "")
                newObjects.ForEach(x => x.Tags.Add(tag));

            // Merge and push the dependencies
            if (Config.SeparateProperties)
            {
                if (!ReplaceDependencies<T>(newObjects, tag))
                    return false;
            }


            return UpdateObjects(newObjects);

        }

        /***************************************************/
    }
}
