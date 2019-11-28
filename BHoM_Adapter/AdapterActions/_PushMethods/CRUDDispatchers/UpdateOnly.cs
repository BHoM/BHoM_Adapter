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
using BH.oM.Data.Collections;
using BH.Engine.Adapter;
using BH.Engine.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Push CRUD Dispatchers                     ****/
        /***************************************************/
        // These methods dispatch calls to different CRUD methods as required by the Push.

        [Description("Performs the only the Update for the specified objects and, if Config.HandleDependencies is true, does the full CRUD for their dependencies.")]
        protected virtual bool UpdateOnly<T>(IEnumerable<T> objectsToPush, string tag = "") where T : IBHoMObject
        {
            List<T> newObjects = objectsToPush.ToList();

            // Make sure objects  are tagged
            if (tag != "")
                newObjects.ForEach(x => x.Tags.Add(tag));

            // Merge and push the dependencies
            if (AdapterSettings.HandleDependencies)
            {
                var dependencyTypes = DependencyTypes<T>();
                var dependencyObjects = Engine.Adapter.Query.GetDependencyObjects<T>(newObjects, dependencyTypes, tag); //first-level dependencies

                foreach (var depObj in dependencyObjects)
                    if (!CRUD(depObj.Value as dynamic, tag))
                        return false;
            }

            return Update(newObjects);
        }

        [Description("Called by UpdateOnly() in order to recursively update the dependencies of the objects.")]
        protected virtual bool DependenciesUpdateOnly<T>(IEnumerable<T> objectsToUpdate, string tag = "") where T : IBHoMObject
        {
            // Make sure objects are distinct 
            List<T> objects = objectsToUpdate.Distinct(Comparer<T>()).ToList();

            // Make sure objects are tagged
            if (tag != "")
                objects.ForEach(x => x.Tags.Add(tag));

            // Create any sub-dependency 
            var dependencyTypes = DependencyTypes<T>();
            var dependencyObjects = Engine.Adapter.Query.GetDependencyObjects<T>(objectsToUpdate, dependencyTypes, tag);
            foreach (var depObj in dependencyObjects)
                DependenciesUpdateOnly(depObj.Value as dynamic);

            return Update(objects);
        }
    }
}