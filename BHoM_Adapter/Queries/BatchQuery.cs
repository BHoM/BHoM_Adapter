using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using BH.oM.Base;

namespace BH.Adapter.Queries
{
    public class BatchQuery : IQuery
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<IQuery> Queries { get; set; } = new List<IQuery>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public BatchQuery() { }

        /***************************************************/

        public BatchQuery(IEnumerable<IQuery> queries)
        {
            Queries = queries.ToList();
        }

        
    }
}
