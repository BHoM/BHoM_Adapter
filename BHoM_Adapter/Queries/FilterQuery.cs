using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace BH.Adapter.Queries
{
    public class FilterQuery : IQuery
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string Tag { get; set; } = "";

        public Type Type { get; set; } = null;

        //public Func<object, bool> Filter { get; set; }
        public Dictionary<string, object> Equalities { get; set; } = new Dictionary<string, object>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public FilterQuery() {}

        /***************************************************/

        public FilterQuery(Type type, string tag = "")
        {
            Type = type;
            Tag = Tag;
        }

        /***************************************************/

        //public FilterQuery(Func<object, bool> filter)
        //{
        //    Filter = filter;
        //}
    }
}
