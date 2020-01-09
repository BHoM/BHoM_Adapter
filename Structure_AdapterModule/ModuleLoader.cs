using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter;

namespace BH.Adapter.Modules
{
    public static class ModuleLoader
    {
        [Description("Invoke this method in any Toolkit Adapter constructor to load functionality from the Structural Adapter Module.")]
        public static void LoadStructuralModules(this BHoMAdapter adapter)
        {
            adapter.AdapterModules.Add(new CopyNodeProperties());
        }
    }
}
