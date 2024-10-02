using System;
using System.Collections.Generic;
using System.Text;

namespace BH.oM.Adapter
{
    public class PriorityComparer : IComparer<Tuple<Type, PushType, IEnumerable<object>>>
    {
        // ATTRIBUTES
        Dictionary<Type, int> priorityTypes = new Dictionary<Type, int>();

        // CONSTRUCTOR
        public PriorityComparer(Dictionary<Type,int> priorityTypes) 
        {
            this.priorityTypes = priorityTypes;
        }

        // METHODS
        public int Compare(Tuple<Type, PushType, IEnumerable<object>> obj1, Tuple<Type, PushType, IEnumerable<object>> obj2)
        {
            int obj1Index, obj2Index;
            bool b1, b2;

            b1 = priorityTypes.TryGetValue(obj1.Item1, out obj1Index);
            b2 = priorityTypes.TryGetValue(obj2.Item1, out obj2Index);

            if (obj1Index < obj2Index) return -1;
            if (obj1Index > obj2Index) return 1;

            return 0;
        }
    }
}
