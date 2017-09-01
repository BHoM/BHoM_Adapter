using BH.Adapter.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public abstract partial class IndexAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public Adapter Methods                    ****/
        /***************************************************/

        public override int Delete(FilterQuery filter, Dictionary<string, string> config = null)
        {
            List<string> indices = (List<string>)filter.Equalities[AdapterId];

            if (indices != null /*&& indices.Count > 0*/)
                return Delete(filter.Type, indices.Select(x => int.Parse(x)).ToList());
            else
                return Delete(filter.Type, filter.Tag);
        }


        /***************************************************/
        /**** Protected Abstract Methods                ****/
        /***************************************************/

        protected abstract object GetNextIndex(Type objectType, bool refresh = false);

        protected abstract int Delete(Type type, List<int> indices);
    }
}
