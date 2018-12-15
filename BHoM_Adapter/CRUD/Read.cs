using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using BH.oM.DataManipulation.Queries;
using BH.oM.Base.CRUD;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** BHoM Adapter Methods                      ****/
        /***************************************************/

        protected virtual IEnumerable<IBHoMObject> Read(Type type, string tag = "", CrudConfig config = null)
        {
            // Get the objects based on the ids
            IEnumerable<IBHoMObject> objects = Read(type, null as List<object>, config);

            // Filter by tag if any 
            if (tag == "")
                return objects;
            else
                return objects.Where(x => x.Tags.Contains(tag));
        }

        /***************************************************/

        public virtual IEnumerable<IBHoMObject> Read(FilterQuery query, CrudConfig config = null)
        {
            IList objectIds = null;
            object idObject;
            if (query.Equalities.TryGetValue("ObjectIds", out idObject) && idObject is IList)
                objectIds = idObject as IList;

            // Get the objects based on the ids
            IEnumerable<IBHoMObject> objects = Read(query.Type, objectIds, config);

            // Filter by tag if any 
            if (query.Tag == "")
                return objects;
            else
                return objects.Where(x => x.Tags.Contains(query.Tag));
        }
    }
}
