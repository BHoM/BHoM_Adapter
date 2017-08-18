using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Materials;
using BH.oM.Base;
using BH.oM.Structural.Elements;
using BH.Adapter.Queries;

namespace BH.Adapter
{
    public static partial class Update
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static int PullUpdatePush<T, P>(this IAdapter adapter, FilterQuery filter, string property, object newValue, Dictionary<string, string> config = null) where T : BHoMObject
        {
            // Pull the objects to update
            List<T> objects = adapter.Pull(new List<IQuery> { filter }).Cast<T>().ToList();

            // Set their property
            Action<T, P> setProp = (Action<T, P>)Delegate.CreateDelegate(typeof(Action<T>), typeof(T).GetProperty(property).GetSetMethod());
            if (newValue is IEnumerable<P>)
            {
                // Case of a list of properties
                List<P> values = ((IEnumerable<P>)newValue).ToList();
                if (values.Count == objects.Count)
                {
                    for (int i = 0; i < values.Count; i++)
                        setProp(objects[i], values[i]);
                }
            }
            else
            {
                // Case of a single common property
                P value = (P)newValue;
                foreach (T obj in objects)
                    setProp(obj, value);
            }

            // Push the objects back
            adapter.Push(objects);

            return objects.Count;
        }
    }
}
