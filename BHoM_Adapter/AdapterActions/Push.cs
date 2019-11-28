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
    public abstract partial class BHoMAdapter : IBHoMAdapter
    {
        /******************************************************/
        /**** Public Adapter Methods "Adapter ACTIONS"    *****/
        /******************************************************/
        /* These methods represent Actions that the Adapter can complete. 
           They are publicly available in the UI as individual components, e.g. in Grasshopper, under BHoM/Adapters tab. */

        [Description("Pushes the input objects using either the full CRUD, or only Create or Update, depending on the option set in the PushType.")]
        public virtual List<object> Push(IEnumerable<object> objects, string tag = "", PushType pushType = PushType.AdapterDefault, Dictionary<string, object> actionConfig = null)
        {
            bool success = true;

            // ---------- READ CONFIGURATIONS ------------

            // Set the PushType to Adapter's default if unset (base Adapter default is FullCRUD).
            if (pushType == PushType.AdapterDefault)
                pushType = AdapterSettings.DefaultPushType;

            // Add the PushType to the actionConfig dictionary.
            actionConfig[nameof(PushType)] = pushType;

            // Read actionConfig `WrapNonBHoMObjects`. If present, that overrides the `WrapNonBHoMObjects` of the Adapter Settings.
            bool wrapNonBHoMObjects = AdapterSettings.WrapNonBHoMObjects;
            object wrapNonBHoMObjs_actionConfig;
            if (actionConfig != null && actionConfig.TryGetValue("WrapNonBHoMObjects", out wrapNonBHoMObjs_actionConfig))
                wrapNonBHoMObjects |= (bool)wrapNonBHoMObjs_actionConfig;

            // ------------ OBJECTS SET-UP --------------

            // Verify that the input objects are IBHoMObjects
            var iBHoMObjects = objects.OfType<IBHoMObject>();
            if (iBHoMObjects.Count() != objects.Count() && !wrapNonBHoMObjects)
            {
                Engine.Reflection.Compute.RecordError("Only BHoMObjects are supported by the default Push."); // = you can override if needed; 
                // if the specific adapter supports it, consider setting actionConfig['WrapNonBHoMObjects'] to true.
                return null;
            }

            // Clone the objects for immutability in the UI. CloneBeforePush should always be true, except for very specific cases.
            List<IBHoMObject> objectsToPush = AdapterSettings.CloneBeforePush ? iBHoMObjects.Select(x => x.DeepClone()).ToList() : iBHoMObjects.ToList();

            // Wrap non-BHoM objects into a Custom BHoMObject to make them compatible with the CRUD.
            if (wrapNonBHoMObjects)
                Engine.Adapter.Convert.WrapNonBHoMObjects(objectsToPush, AdapterSettings, tag, actionConfig);
         

            // ------------- ACTUAL PUSH ---------------

            // Group the objects by their specific type.
            var typeGroups = objects.GroupBy(x => x.GetType());

            foreach (var typeGroup in typeGroups)
            {
                // Cast the objects to their specific types
                MethodInfo enumCastMethod_specificType = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(new[] { typeGroup.Key });
                var objList_specificType = enumCastMethod_specificType.Invoke(typeGroup, new object[] { typeGroup });

                if (typeof(IBHoMObject).IsAssignableFrom(typeGroup.Key))
                {
                    if (pushType == PushType.FullCRUD)
                        success &= CRUD(objList_specificType as dynamic, tag);
                    else if (pushType == PushType.CreateOnly)
                    {
                        success &= CreateOnly(objList_specificType as dynamic, tag);
                    }
                    else if (pushType == PushType.UpdateOnly)
                    {
                        success &= UpdateOnly(objList_specificType as dynamic, tag);
                    }
                }
                else
                {
                    Engine.Reflection.Compute.RecordError("Some objects could not be pushed.");
                    success = false;
                }
            }

            return success ? objectsToPush.Cast<object>().ToList() : new List<IObject>().Cast<object>().ToList();
        }

    }
}
