using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Materials;
using BH.oM.Base;
using BH.oM.Structural.Elements;
using BH.Adapter.Queries;
using System.Collections;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        public int PullUpdatePush(FilterQuery filter, string property, object newValue, Dictionary<string, string> config = null) 
        {
            // Get the type of object
            Type objectType = filter.Type;
            if (objectType == null)
                return 0;

            // Pull the objects to update
            List<object> objects = Pull(new List<IQuery> { filter }).ToList();

            // Set their property
            objects.UpdateProperty(objectType, property, newValue);

            // Push the objects back
            Create(objects);

            return objects.Count;
        }
    }
}
