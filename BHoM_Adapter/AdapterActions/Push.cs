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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {

        /******************************************************/
        /**** Public Adapter Methods "Adapter ACTIONS"    *****/
        /******************************************************/
        /* These methods represent Actions that the Adapter can complete. 
           They are publicly available in the UI as individual components, e.g. in Grasshopper, under BHoM/Adapters tab. */

        [Description("Pushes input objects using either the 'Full CRUD', 'CreateOnly' or 'UpdateOnly', depending on the PushType.")]
        public virtual List<object> Push(IEnumerable<object> objects, string tag = "", PushType pushType = PushType.AdapterDefault, ActionConfig actionConfig = null)
        {
            bool success = true;

            // ----------------------------------------//
            //                 SET-UP                  //
            // ----------------------------------------//

            // If unset, set the pushType to AdapterSettings' value (base AdapterSettings default is FullPush).
            if (pushType == PushType.AdapterDefault)
                pushType = m_AdapterSettings.DefaultPushType;

            // Process the objects (verify they are valid; DeepClone them, wrap them, etc).
            IEnumerable<IBHoMObject> objectsToPush = ProcessObjectsForPush(objects, actionConfig); // Note: default Push only supports IBHoMObjects.

            if (objectsToPush.Count() == 0)
            {
                Engine.Reflection.Compute.RecordError("Input objects were invalid.");
                return new List<object>(); 
            }

            // ----------------------------------------//
            //               ACTUAL PUSH               //
            // ----------------------------------------//

            // Group the objects by their specific type.
            var typeGroups = objectsToPush.GroupBy(x => x.GetType());

            foreach (var typeGroup in typeGroups)
            {
                // Cast the objects to their specific types
                MethodInfo enumCastMethod_specificType = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(new[] { typeGroup.Key });
                var objList_specificType = enumCastMethod_specificType.Invoke(typeGroup, new object[] { typeGroup });

                if (pushType == PushType.FullPush || pushType == PushType.CreateNonExisting || pushType == PushType.UpdateOrCreateOnly)
                    success &= FullCRUD(objList_specificType as dynamic, pushType, tag, actionConfig);
                else if (pushType == PushType.CreateOnly)
                {
                    success &= CreateOnly(objList_specificType as dynamic, tag, actionConfig);
                }
                else if (pushType == PushType.UpdateOnly)
                {
                    success &= UpdateOnly(objList_specificType as dynamic, tag, actionConfig);
                }
            }

            return success ? objectsToPush.Cast<object>().ToList() : new List<IObject>().Cast<object>().ToList();
        }

    }
}

