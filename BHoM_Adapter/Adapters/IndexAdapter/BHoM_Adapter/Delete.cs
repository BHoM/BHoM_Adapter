using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public abstract partial class IndexAdapter
    {
        /***************************************************/
        /**** BHoM Adapter Methods                      ****/
        /***************************************************/

        protected override int Delete(Type type, string tag = "")
        {
            if (tag == "")
            {
                return Delete(type, null);
            }
            else
            {
                // Get all with tag
                IEnumerable<BHoMObject> withTag = Read(type, tag);

                // Get indices of all with that tag only
                List<int> indices = withTag.Where(x => x.Tags.Count == 1).Select(x => (int)x.CustomData[AdapterId]).OrderBy(x => x).ToList();
                Delete(type, indices);

                // Remove tag if other tags as well
                IEnumerable<BHoMObject> multiTags = withTag.Where(x => x.Tags.Count > 1);
                UpdateTags(multiTags);

                return indices.Count;
            }

        }
    }
}
