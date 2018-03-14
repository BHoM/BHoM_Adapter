using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            CreateFileAndFolder();

            return base.Push(objects, tag, config);
        }
    }
}
