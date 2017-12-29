using BH.oM.Base;
using BH.oM.DataStructure;
using System;
using System.Collections.Generic;

namespace BH.Adapter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Tuple<T,T>> UpdatePropertiesFrom<T>(this IEnumerable<T> set, IEnumerable<T> other, IEqualityComparer<T> comparer, Action<T,T> propertyMapper) where T : BHoMObject 
        {
            VennDiagram<T> diagram = Engine.DataStructure.Create.VennDiagram<T>(set, other, comparer);

            //Map properties from source group to the target group
            diagram.Intersection.ForEach(x => propertyMapper(x.Item1, x.Item2));

            return diagram.Intersection;
        }
    }
}
