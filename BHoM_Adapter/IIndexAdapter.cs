using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public interface IIndexAdapter : IAdapter
    {
        string AdapterId { get; }

        object GetNextIndex(Type objectType, bool refresh = false);

    }
}
