using BH.oM.Adapter;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        [Description("Assigns to the object the next available id, obtained calling the NextFreeId method.")]
        protected virtual void AssignNextFreeId<T>(IEnumerable<T> objects) where T : IBHoMObject
        {
            bool refresh = true;
            foreach (T item in objects)
            {
                if (!item.CustomData.ContainsKey(AdapterIdName))
                {
                    item.CustomData[AdapterIdName] = NextFreeId(typeof(T), refresh);
                    refresh = false;
                }
            }
        }
    }
}
