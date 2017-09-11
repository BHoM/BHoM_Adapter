using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BHC = BH.Adapter.Convert;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using BH.Adapter.Queries;
using BH.Adapter;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        public int Update(FilterQuery filter, string property, object newValue, Dictionary<string, string> config = null)
        {
            return 0;
        } 
    }
}
