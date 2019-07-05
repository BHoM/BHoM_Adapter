using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.oM.Data.Requests;
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

        public virtual bool PullTo(BHoMAdapter to, IRequest request, Dictionary<string, object> config = null)
        {
            string tag = "";
            if (request is FilterRequest)
                tag = (request as FilterRequest).Tag;

            IEnumerable<object> objects = this.Pull(request, config);
            int count = objects.Count();
            return to.Push(objects.Cast<IObject>(), tag).Count() == count;
        }

        /***************************************************/

        public virtual bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, object> config = null)
        {
            return false;
        }
    }
}
