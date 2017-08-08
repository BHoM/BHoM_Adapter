using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;

namespace BH.Adapter.Interfaces
{
    public interface INodeAdapter : IAdapter, IStructuralAdapter
    {
        List<string> GetNodes(out List<Node> nodes, List<string> ids = null);
    }
}
