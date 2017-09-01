using BH.Engine.DataStructure;
using BH.oM.Base;
using BH.oM.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public static partial class Merge
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Tuple<T,T>> AdjustToSet<T>(this IEnumerable<T> set, IEnumerable<T> other, IEqualityComparer<T> comparer, Action<T,T> propertyMapper) where T : BHoMObject 
        {
            VennDiagram<T> diagram = set.CreateVennDiagram<T>(other, comparer);

            //Map properties from source group to the target group
            diagram.Intersection.ForEach(x => propertyMapper(x.Item1, x.Item2));

            return diagram.Intersection;
        }
    }
}
