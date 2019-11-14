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
using BH.oM.Data.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BH.oM.Base.Adapter;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /******************************************************/
        /**** Public Adapter Methods "Adapter ACTIONS"    *****/
        /******************************************************/
        /* These methods represent Actions that the Adapter can complete. 
           They are publicly available in the UI as individual components, e.g. in Grasshopper, under BHoM/Adapters tab. */

        // Performs the full CRUD if implemented, or calls the appropriate basic CRUD/Create method.
        public virtual List<IObject> Push(IEnumerable<IObject> objects, string tag = "", PushOption pushOption = PushOption.Unset, Dictionary<string, object> config = null)
        {
            bool success = true;

            // Set the Push Option to Adapter's default if unset. Base Adapter default is FullCRUD.
            if (pushOption == PushOption.Unset)
                pushOption = AdapterSettings.PushOption;

            // Clone the objects for immutability in the UI. CloneBeforePush should always be true, except for very specific cases.
            List<IObject> objectsToPush = AdapterSettings.CloneBeforePush ? objects.Select(x => x.DeepClone()).ToList() : objects.ToList();

            // Wrap non-BHoM objects into a Custom BHoMObject to make them compatible with the CRUD.
            // The boolean Config.WrapNonBHoMObjects regulates this, checked inside the method itself to allow overriding on-the-fly.
            WrapNonBHoMObjects(objectsToPush, AdapterSettings, tag, config);

            // Perform the actual Push.
            Type iBHoMObjectType = typeof(IBHoMObject);
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objectsToPush.GroupBy(x => x.GetType()))
            {
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                var list = miListObject.Invoke(typeGroup, new object[] { typeGroup });

                if (iBHoMObjectType.IsAssignableFrom(typeGroup.Key))
                {
                    if (pushOption == PushOption.FullCRUD)
                        success &= CRUD(list as dynamic, tag);
                    else if (pushOption == PushOption.CreateOnly)
                    {
                        success &= CreateOnly(list as dynamic, tag);
                    }
                    else if (pushOption == PushOption.UpdateOnly)
                    {
                        success &= UpdateOnly(list as dynamic, tag);
                    }
                }
            }

            return success ? objectsToPush : new List<IObject>();
        }

    }
}
