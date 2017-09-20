using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Protected  Methods                        ****/
        /***************************************************/

        protected virtual int Delete(Type type, string tag = "", Dictionary<string, string> config = null) 
        {
            if (tag == "")
            {
                return Delete(type, null as List<object>);
            }
            else
            {
                // Get all with tag
                IEnumerable<BHoMObject> withTag = Read(type, tag);

                // Get indices of all with that tag only
                IEnumerable<object> ids = withTag.Where(x => x.Tags.Count == 1).Select(x => x.CustomData[AdapterId]).OrderBy(x => x);
                Delete(type, ids);

                // Remove tag if other tags as well
                IEnumerable<BHoMObject> multiTags = withTag.Where(x => x.Tags.Count > 1);
                UpdateProperty(type, multiTags.Select(x => x.CustomData[AdapterId]), "Tags", multiTags.Select(x => x.Tags));

                return ids.Count();
            }
        }

    }
}
