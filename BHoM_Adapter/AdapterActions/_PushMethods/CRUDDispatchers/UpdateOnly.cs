/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Data.Collections;
using BH.Engine.Adapter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Adapter;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Push CRUD Dispatchers                     ****/
        /***************************************************/
        // These methods dispatch calls to different CRUD methods as required by the Push.

        [Description("Performs the only the Update for the specified objects and, if Config.HandleDependencies is true, does the full CRUD for their dependencies.")]
        protected virtual bool UpdateOnly<T>(IEnumerable<T> objectsToPush, string tag = "",  ActionConfig actionConfig = null) where T : IBHoMObject
        {
            List<T> newObjects = objectsToPush.ToList();

            // Make sure objects  are tagged
            if (tag != "")
                newObjects.ForEach(x => x.Tags.Add(tag));

            // Merge and push the dependencies
            if (m_AdapterSettings.HandleDependencies)
            {
                var dependencyTypes = Engine.Adapter.Query.GetDependencyTypes<T>(this);
                var dependencyObjects = Engine.Adapter.Query.GetDependencyObjects<T>(objectsToPush, dependencyTypes, this); //first-level dependencies

                foreach (var depObj in dependencyObjects)
                    if (!FullCRUD(depObj.Value as dynamic, PushType.FullPush, tag, actionConfig))
                        return false;
            }

            return IUpdate(newObjects, actionConfig);
        }

        
    }
}


