using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;


namespace BH.Adapter
{
    public abstract partial class IndexAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        public override void MapObjectAttributes<T>(List<Tuple<T, T>> objects)
        {
            objects.ForEach(x => x.Item1.MapSpecialProperties(x.Item2, AdapterId));
        }

        /***************************************************/
    }
}
