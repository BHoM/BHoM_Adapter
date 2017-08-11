using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Properties;

namespace BH.Adapter.Interfaces
{
    public interface ISectionPropertyAdapter : IAdapter, IStructuralAdapter, IMaterialAdapter
    {
        List<SectionProperty> PullSectionProperties(List<string> ids = null);
    }
}
