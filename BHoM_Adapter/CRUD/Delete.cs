using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;
using BH.oM.Base.CRUD;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Protected  Methods                        ****/
        /***************************************************/

        protected virtual int Delete(Type type, string tag = "", CrudConfig config = null) 
        {
            if (tag == "")
            {
                return Delete(type, null as List<object>, config);
            }
            else
            {
                // Get all with tag
                IEnumerable<IBHoMObject> withTag = Read(type, tag, config);

                // Get indices of all with that tag only
                IEnumerable<object> ids = withTag.Where(x => x.Tags.Count == 1).Select(x => x.CustomData[AdapterId]).OrderBy(x => x);
                Delete(type, ids, config);

                // Remove tag if other tags as well
                IEnumerable<IBHoMObject> multiTags = withTag.Where(x => x.Tags.Count > 1);
                UpdateProperty(type, multiTags.Select(x => x.CustomData[AdapterId]), "Tags", multiTags.Select(x => x.Tags), config);

                return ids.Count();
            }
        }

        /***************************************************/
    }
}
