using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter;

namespace BH.Adapter.Modules
{
    public static class ModuleLoader
    {
        public static void LoadStructuralModules(this BHoMAdapter adapter)
        {
            adapter.AdapterModules.Add(new CopyNodeProperties());
        }
    }
}
