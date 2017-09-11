using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        protected abstract bool Create(IEnumerable<object> objects)
        {

        }
    }
}
