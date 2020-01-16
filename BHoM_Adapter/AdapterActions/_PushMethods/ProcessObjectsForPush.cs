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
        protected virtual IEnumerable<IBHoMObject> ProcessObjectsForPush(IEnumerable<object> objects, ActionConfig actionConfig) 
        {
            // -------------------------------- // 
            //            READ CONFIG           // 
            // -------------------------------- // 

            // If ActionConfig has a value for `WrapNonBHoMObjects`, it has precedence over the default value in AdapterSettings.
            bool wrapNonBHoMObjects = AdapterSettings.WrapNonBHoMObjects;
            if (actionConfig != null)
                wrapNonBHoMObjects = actionConfig.WrapNonBHoMObjects;


            // -------------------------------- // 
            //              CHECKS              // 
            // -------------------------------- // 

            // Verify that the input objects are IBHoMObjects.
            if (objects.OfType<IBHoMObject>().Count() != objects.Count() & !wrapNonBHoMObjects)
            {
                Engine.Reflection.Compute.RecordWarning("Only non-null BHoMObjects are supported by the default Push. " + // = you can override if needed; 
                    "\nConsider specifying actionConfig['WrapNonBHoMObjects'] to true."); 
            }

            // -------------------------------- // 
            //        OBJECT PREPARATION        // 
            // -------------------------------- // 

            IEnumerable<IBHoMObject> objectsToPush = new List<IBHoMObject>();

            // Wrap non-BHoM objects into a Custom BHoMObject to make them compatible with the CRUD.
            if (wrapNonBHoMObjects)
                objectsToPush = Engine.Adapter.Modify.WrapNonBHoMObjects(objects);
            else
                objectsToPush = objects.OfType<IBHoMObject>();

            // Clone the objects for immutability in the UI. CloneBeforePush should always be true, except for very specific cases.
            if (AdapterSettings.CloneBeforePush)
                objectsToPush = objectsToPush.Select(x => x.DeepClone()).ToList();

            return objectsToPush;
        }
    }
}