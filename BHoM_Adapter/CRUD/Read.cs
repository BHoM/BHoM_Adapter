using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using BH.oM.DataManipulation.Queries;

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

        /***************************************************/

        public virtual IEnumerable<IBHoMObject> Read(FilterQuery query)
        {
            IList objectIds = null;
            object idObject;
            if (query.Equalities.TryGetValue("ObjectIds", out idObject) && idObject is IList)
                objectIds = idObject as IList;

            // Get the objects based on the ids
            IEnumerable<IBHoMObject> objects = Read(query.Type, objectIds);

            // Filter by tag if any 
            if (query.Tag == "")
                return objects;
            else
                return objects.Where(x => x.Tags.Contains(query.Tag));
        }
    }
}
