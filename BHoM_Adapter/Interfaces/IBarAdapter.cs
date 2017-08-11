using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;

namespace BH.Adapter.Interfaces
{
    public interface IBarAdapter : IAdapter, IStructuralAdapter, INodeAdapter, ISectionPropertyAdapter
    {
        List<Bar> PullBars(List<string> ids = null);
    }
}
