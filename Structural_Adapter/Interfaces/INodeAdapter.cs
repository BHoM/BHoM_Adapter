using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;

namespace BH.Adapter.Structural
{
    public interface INodeAdapter : IAdapter, IStructuralAdapter
    {
        List<Node> PullNodes(List<string> ids = null);
    }
}
