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

        protected virtual bool PushByType(IEnumerable<object> objects, string tag, Dictionary<string, string> config = null)
        {
            bool success = true;
            foreach (IEnumerable<object> typeGroup in objects.GroupBy(x => x.GetType()))
                success &= PushType(objects as dynamic, tag);

            return success;
        }

        /***************************************************/

        protected virtual bool PushType<T>(List<T> objectsToPush, string tag = "") where T : BHoMObject
        {
            if (objectsToPush.Count == 0)
                return true;

            Type type = objectsToPush[0].GetType();
            return ReplaceAndMerge<T>(objectsToPush, GetComparer<T>(), GetDependencyTypes<T>(), tag);
        }
    }
}
