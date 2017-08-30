using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter.Queries;

namespace BH.Adapter
{
    public static partial class Delete
    {
        private static FilterQuery GenerateDeleteFilterQuery<T>(IEnumerable<T> objects, string adapterId) where T : BH.oM.Base.BHoMObject
        {
            FilterQuery filter = new FilterQuery();
            filter.Type = typeof(T);
            filter.Equalities["Indices"] = objects.Select(x => x.CustomData[adapterId].ToString()).ToList();
            return filter;
        }
    }
}
