using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public static partial class Update
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool UpdateProperty(this List<BHoMObject> objects, Type objectType, string property, object newValue)
        {
            PropertyInfo propInfo = objectType.GetProperty(property);
            Action<object, object> setProp = (Action<object, object>)Delegate.CreateDelegate(typeof(Action<object, object>), propInfo.GetSetMethod());

            if (newValue is IList && newValue.GetType() != propInfo.PropertyType)
            {
                IList values = ((IList)newValue);

                // Check that the two lists are of equal length
                if (objects.Count() != values.Count)
                    return false;

                // Set their property
                for (int i = 0; i < values.Count; i++)
                    setProp(objects[i], values[i]);
            }
            else
            {
                // Set the same property to all objects
                foreach (object obj in objects)
                    setProp(obj, newValue);
            }

            return true;
        }

    }
}
