using BH.oM.Adapter;
using BH.oM.Reflection.Attributes;
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
        [Description("Returns an externalIdFragment containing the next available ID for object creation.")]
        [Input("objectType", "Type of the object whose next available id should be returned.")]
        [Input("refresh", "To say if it is the first of many calls during the same pass of the adapter " +
               "so you only need to ask the adapter once, then increment.")]
        protected virtual object NextFreeId(Type objectType, bool refresh = false)
        {
            return null;
        }
    }
}
