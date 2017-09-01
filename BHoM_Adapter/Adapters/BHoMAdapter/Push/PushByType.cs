using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Materials;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using System;
using System.Collections;
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

        protected bool PushByType(IEnumerable<object> objects, string tag, Dictionary<string, string> config = null)
        {
            bool success = true;
            foreach (IEnumerable<object> typeGroup in objects.GroupBy(x => x.GetType()))
                success &= PushType(objects as dynamic, tag);

            return success;
        }

        /***************************************************/

        protected bool PushType(IList objectsToPush, string tag = "", bool applyMerge = true)
        {
            // If we end up here, it means that no custom method exists fr this type

            if (objectsToPush.Count == 0)
                return true;

            Type type = objectsToPush[0].GetType();
            return SimpleReplace(objectsToPush.OfType<BHoMObject>().ToList(), type, tag);
        }
    }
}
