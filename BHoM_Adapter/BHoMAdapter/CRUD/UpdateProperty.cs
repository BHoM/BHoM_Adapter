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

        public int PullUpdatePush(FilterQuery filter, string property, object newValue) 
        {
            if (Config.ProcessInMemory)
            {
                IEnumerable<BHoMObject> objects = UpdateInMemory(filter, property, newValue);
                Create(objects, true);
                return objects.Count();
            }
            else
                return UpdateThroughAPI(filter, property, newValue);
        }


        /***************************************************/
        /**** Helper Methods                            ****/
        /***************************************************/

        public IEnumerable<BHoMObject> UpdateInMemory(FilterQuery filter, string property, object newValue)
        {
            // Pull the objects to update
            IEnumerable<BHoMObject> objects = Read(filter.Type);

            // Set the property of the objects matching the filter
            filter.Filter(objects).ToList().UpdateProperty(filter.Type, property, newValue);

            return objects;
        }

        /***************************************************/

        public int UpdateThroughAPI(FilterQuery filter, string property, object newValue)
        {
            IEnumerable<object> ids = Pull(filter).Select(x => ((BHoMObject)x).CustomData[AdapterId]);
            return UpdateProperty(filter.Type, ids, property, newValue);
        }
    }
}
