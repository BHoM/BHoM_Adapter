using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.oM.Data.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {

        /***************************************************/
        /**** Public Adapter Methods                    ****/
        /***************************************************/

        public virtual IEnumerable<object> Pull(IRequest request, Dictionary<string, object> config = null)
        {
            // Make sure this is a FilterQuery
            FilterRequest filter = request as FilterRequest;
            if (filter == null)
                return new List<object>();

            // Read the IBHoMObjects
            if (typeof(IBHoMObject).IsAssignableFrom(filter.Type))
                return Read(filter);

            // Read the IResults
            if (typeof(BH.oM.Common.IResult).IsAssignableFrom(filter.Type))
            {
                IList cases, objectIds;
                int divisions;
                object caseObject, idObject, divObj;

                if (filter.Equalities.TryGetValue("Cases", out caseObject) && caseObject is IList)
                    cases = caseObject as IList;
                else
                    cases = null;

                if (filter.Equalities.TryGetValue("ObjectIds", out idObject) && idObject is IList)
                    objectIds = idObject as IList;
                else
                    objectIds = null;

                if (filter.Equalities.TryGetValue("Divisions", out divObj))
                {
                    if (divObj is int)
                        divisions = (int)divObj;
                    else if (!int.TryParse(divObj.ToString(), out divisions))
                        divisions = 5;
                }
                else
                    divisions = 5;

                List<BH.oM.Common.IResult> results = ReadResults(filter.Type, objectIds, cases, divisions).ToList();
                results.Sort();
                return results;
            }

            // Read the IResultCollections
            if (typeof(BH.oM.Common.IResultCollection).IsAssignableFrom(filter.Type))
            {
                List<BH.oM.Common.IResultCollection> results = ReadResults(filter).ToList();
                return results;
            }

            return new List<object>();
        }

    }
}
