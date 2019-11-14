/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using BH.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter : BHoMAdapter
    {
        public override List<IObject> Push(IEnumerable<IObject> objects, string tag = "", PushOption pushOption = PushOption.Unset, Dictionary<string, object> config = null)
        {
            if (!ProcessExtension(ref m_FilePath))
                return null;

            CreateFileAndFolder();

            // Clone the objects for immutability in the UI. CloneBeforePush should always be true, except for very specific cases.
            List<IObject> objectsToPush = AdapterSettings.CloneBeforePush ? objects.Select(x => x.DeepClone()).ToList() : objects.ToList();

            // Wrap non-BHoM objects into a Custom BHoMObject to make them compatible with the CRUD.
            // The boolean Config.WrapNonBHoMObjects regulates this, checked inside the method itself to allow overriding on-the-fly.
            Modify.WrapNonBHoMObjects(objectsToPush, AdapterSettings, tag, config);

            IEnumerable<IBHoMObject> bhomObjects = objectsToPush.Where(x => x is IBHoMObject).Cast<IBHoMObject>();

            if (bhomObjects.Count() != objects.Count())
                Engine.Reflection.Compute.RecordWarning("The file adapter can currently only be used with BHoMObjects." + Environment.NewLine +
                    "If you want to push non-BHoMobject, specify a push config with the option `WrapNonBHoMObject` set to true.");

            bool success = this.CRUD<IBHoMObject>(bhomObjects, tag);

            return success ? objectsToPush : new List<IObject>();
        }
    }
}
