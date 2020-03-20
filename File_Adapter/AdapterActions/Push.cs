/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Adapter;
using BH.Engine.Base;
using BH.Engine.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter : BHoMAdapter
    {
        public override List<object> Push(IEnumerable<object> objects, string tag = "", PushType pushType = PushType.AdapterDefault, ActionConfig actionConfig = null)
        {
            // --------------- SET-UP ------------------

            // Process the objects (verify they are valid; DeepClone them, wrap them, etc).
            IEnumerable<IBHoMObject> objectsToPush = ProcessObjectsForPush(objects, actionConfig); // Note: default Push only supports IBHoMObjects.

            if (objectsToPush.Count() == 0)
            {
                Engine.Reflection.Compute.RecordError("Input objects were invalid.");
                return new List<object>();
            }

            // If unset, set the actionConfig to a new ActionConfig.
            actionConfig = actionConfig == null ? new ActionConfig() : actionConfig;

            // If unset, set the pushType to AdapterSettings' value (base AdapterSettings default is FullCRUD).
            if (pushType == PushType.AdapterDefault)
                pushType = m_AdapterSettings.DefaultPushType;

            // ------------- ACTUAL PUSH ---------------

            if (!ProcessExtension(ref m_FilePath))
                return null;

            CreateFileAndFolder(pushType);

            if (objectsToPush.Count() != objects.Count())
                Engine.Reflection.Compute.RecordWarning("The file adapter can currently only be used with BHoMObjects." + Environment.NewLine +
                    "If you want to push non-BHoMobject, specify a push config with the option `WrapNonBHoMObject` set to true.");

            bool success = this.FullCRUD(objectsToPush, pushType, tag, actionConfig);

            return success ? objectsToPush.Cast<object>().ToList() : new List<IObject>().Cast<object>().ToList();
        }
    }
}

