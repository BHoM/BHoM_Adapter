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

using BH.Engine.Reflection;
using BH.oM.Base;
using BH.Engine.Base;
using BH.oM.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BH.oM.Adapter;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Push Support methods                      ****/
        /***************************************************/
        // These are support methods required by other methods in the Push process.

        [Description("Prepares the objects for the Push.")]
        protected virtual IEnumerable<IBHoMObject> ProcessObjects(IEnumerable<object> objects) 
        {
            // Get the value for WrapNonBHoMObjects.
            bool wrapNonBHoMObjects = false;

            // If ActionConfig has a value for `WrapNonBHoMObjects`, it has precedence over the default value in AdapterSettings.
            object wrapNonBHoMObjs_actionConfig;
            if (ActionConfig.TryGetValue(nameof(AdapterSettings.WrapNonBHoMObjects), out wrapNonBHoMObjs_actionConfig))
                wrapNonBHoMObjects = (bool)wrapNonBHoMObjs_actionConfig;
            else
                wrapNonBHoMObjects = AdapterSettings.WrapNonBHoMObjects;

            // Verify that the input objects are IBHoMObjects.
            var iBHoMObjects = objects.OfType<IBHoMObject>(); //this also filters non-null objs.
            if (iBHoMObjects.Count() != objects.Count() && !wrapNonBHoMObjects)
            {
                Engine.Reflection.Compute.RecordWarning("Only non-null BHoMObjects are supported by the default Push. " + // = you can override if needed; 
                    "\nConsider specifying actionConfig['WrapNonBHoMObjects'] to true."); 
            }

            // Clone the objects for immutability in the UI. CloneBeforePush should always be true, except for very specific cases.
            List<IBHoMObject> objectsToPush = AdapterSettings.CloneBeforePush ? iBHoMObjects.Select(x => x.DeepClone()).ToList() : iBHoMObjects.ToList();

            // Wrap non-BHoM objects into a Custom BHoMObject to make them compatible with the CRUD.
            if (wrapNonBHoMObjects)
                Engine.Adapter.Convert.WrapNonBHoMObjects(objectsToPush);

            return objectsToPush;
        }
    }
}