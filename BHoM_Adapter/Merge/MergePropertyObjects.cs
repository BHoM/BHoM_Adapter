using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public static partial class Merge
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<Type, List<P>> MergePropertyObjects<T, P>(List<T> objects, IEnumerable<Type> propertyTypes) where T : BHoMObject where P : BHoMObject
        {
            Dictionary<Type, List<P>> mergedPropertyObjects = new Dictionary<Type, List<P>>();

            Dictionary<Type, List<PropertyInfo>> propertyDictionary = typeof(T).GetProperties().GroupBy(x => x.PropertyType).ToDictionary(x => x.Key, x => x.ToList());

            foreach (Type type in propertyTypes)
            {
                if (!propertyDictionary.ContainsKey(type))
                    continue;

                Dictionary<PropertyInfo, Action<T, P>> setters = new Dictionary<PropertyInfo, Action<T, P>>();
                Dictionary<PropertyInfo, Func<T, P>> getters = new Dictionary<PropertyInfo, Func<T, P>>();

                List<P> propertyObjects = new List<P>();

                foreach (PropertyInfo property in propertyDictionary[type])
                {
                    // Optimisation using this article: https://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
                    Action<T, P> setProp = (Action<T, P>)Delegate.CreateDelegate(typeof(Action<T>), property.GetSetMethod());
                    Func<T, P> getProp = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), property.GetSetMethod());

                    // Keep those for later
                    setters.Add(property, setProp);
                    getters.Add(property, getProp);

                    // Collect the objects from this property
                    propertyObjects.AddRange(objects.Select(x => getProp(x)));
                }

                // Clone the distinct property objects
                Dictionary<Guid, P> cloneDictionary = CloneObjects<P>(propertyObjects.GetDistinctDictionary());


                //Assign cloned distinct property objects back into input objects
                foreach (PropertyInfo property in propertyDictionary[type])
                {
                    Action<T, P> setProp = setters[property];
                    Func<T, P> getProp = getters[property];
                    objects.ForEach(x => setProp(x, cloneDictionary[getProp(x).BHoM_Guid]));
                }

                //Return the disticnt property objects
                mergedPropertyObjects[type] = cloneDictionary.Values.ToList();
            }

            return mergedPropertyObjects;
        }


        /***************************************************/

        public static List<P> MergePropertyObjects<T, P>(this List<T> objects) where T : BHoMObject where P : BHoMObject
        {
            Type propertyType = typeof(P);
            Dictionary<Type, List<P>> mergedPropertyObjects = new Dictionary<Type, List<P>>();

            Dictionary<Type, List<PropertyInfo>> propertyDictionary = typeof(T).GetProperties().GroupBy(x => x.PropertyType).ToDictionary(x => x.Key, x => x.ToList());

            if (!propertyDictionary.ContainsKey(propertyType))
                return new List<P>();

            Dictionary<PropertyInfo, Action<T, P>> setters = new Dictionary<PropertyInfo, Action<T, P>>();
            Dictionary<PropertyInfo, Func<T, P>> getters = new Dictionary<PropertyInfo, Func<T, P>>();

            List<P> propertyObjects = new List<P>();

            foreach (PropertyInfo property in propertyDictionary[propertyType])
            {
                // Optimisation using this article: https://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
                Action<T, P> setProp = (Action<T, P>)Delegate.CreateDelegate(typeof(Action<T>), property.GetSetMethod());
                Func<T, P> getProp = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), property.GetSetMethod());

                // Keep those for later
                setters.Add(property, setProp);
                getters.Add(property, getProp);

                // Collect the objects from this property
                propertyObjects.AddRange(objects.Select(x => getProp(x)));
            }

            // Clone the distinct property objects
            Dictionary<Guid, P> cloneDictionary = CloneObjects<P>(propertyObjects.GetDistinctDictionary());


            //Assign cloned distinct property objects back into input objects
            foreach (PropertyInfo property in propertyDictionary[propertyType])
            {
                Action<T, P> setProp = setters[property];
                Func<T, P> getProp = getters[property];
                objects.ForEach(x => setProp(x, cloneDictionary[getProp(x).BHoM_Guid]));
            }

            //Return the disticnt property objects
            return cloneDictionary.Values.ToList();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dictionary<Guid, T> GetDistinctDictionary<T>(this IEnumerable<T> list) where T : BHoMObject
        {
            return list.GroupBy(x => x.BHoM_Guid).Select(x => x.First()).ToDictionary(x => x.BHoM_Guid);
        }

        /***************************************************/

        private static Dictionary<Guid, T> CloneObjects<T>(Dictionary<Guid, T> dict) where T : BHoMObject
        {
            Dictionary<Guid, T> clones = new Dictionary<Guid, T>();

            foreach (KeyValuePair<Guid, T> kvp in dict)
            {
                T obj = (T)kvp.Value.GetShallowClone();
                obj.CustomData = new Dictionary<string, object>(kvp.Value.CustomData);
                clones.Add(kvp.Key, obj);
            }

            return clones;
        }

    }
}
