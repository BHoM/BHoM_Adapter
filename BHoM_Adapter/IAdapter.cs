using BH.Adapter.Queries;
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public interface IAdapter
    {
        bool Push(IEnumerable<object> objects, string tag = "", Dictionary<string, string> config = null);

        IEnumerable<object> Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null);

        int Update(FilterQuery filter, string property, object newValue, Dictionary<string, string> config = null);

        int Delete(FilterQuery filter, Dictionary<string, string> config = null);

        bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, string> config = null);

        List<string> ErrorLog { get; set; }
    }
}
