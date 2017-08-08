﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Materials;

namespace BH.Adapter.Interfaces
{
    public interface IMaterialAdapter : IAdapter, IStructuralAdapter
    {
        List<string> GetMaterials(out List<Material> materials, List<string> ids = null);
    }
}
