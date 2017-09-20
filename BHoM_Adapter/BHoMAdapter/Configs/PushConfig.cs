﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public class AdapterConfig
    {
        public bool ProcessInMemory { get; set; } = true;
        public bool SeparateProperties { get; set; } = false;
        public bool MergeWithComparer { get; set; } = false;
        public bool UseAdapterId { get; set; } = true;
    }
}
