using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Interfaces
{
    public interface IStructuralAdapter : IAdapter
    {
        string AdapterId { get; }

        object GetNextIndex(Type objectType, bool refresh = false);

        bool CreateObjects(IEnumerable<object> objects);

        bool UpdateTags(IEnumerable<object> objects);

    }
}
