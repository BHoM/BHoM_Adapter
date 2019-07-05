using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Public Adapter Methods                    ****/
        /***************************************************/

        public virtual List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            bool success = true;

            string pushType;

            object ptObj;
            if (config != null && config.TryGetValue("PushType", out ptObj))
                pushType = ptObj.ToString();
            else
                pushType = "Replace";

            List<IObject> objectsToPush = Config.CloneBeforePush ? objects.Select(x => x is BHoMObject ? ((BHoMObject)x).GetShallowClone() : x).ToList() : objects.ToList(); //ToList() necessary for the return collection to function properly for cloned objects

            Type iBHoMObjectType = typeof(IBHoMObject);
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objectsToPush.GroupBy(x => x.GetType()))
            {
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                var list = miListObject.Invoke(typeGroup, new object[] { typeGroup });

                if (iBHoMObjectType.IsAssignableFrom(typeGroup.Key))
                {
                    if (pushType == "Replace")
                        success &= Replace(list as dynamic, tag);
                    else if (pushType == "UpdateOnly")
                    {
                        success &= UpdateOnly(list as dynamic, tag);
                    }
                }
            }

            return success ? objectsToPush : new List<IObject>();
        }
    }
}
