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
using BH.oM.Adapter;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Push CRUD Dispatchers                     ****/
        /***************************************************/
        // These methods dispatch calls to different CRUD methods as required by the Push.

        [Description("Performs the only the Create for the specified objects and, if Config.HandleDependencies is true, their dependencies.")]
        protected virtual bool CreateOnly<T>(IEnumerable<T> objectsToPush, string tag = "", ActionConfig actionConfig = null) where T : IBHoMObject
        {
            List<T> newObjects = !m_AdapterSettings.CreateOnly_DistinctObjects ? objectsToPush.ToList() : objectsToPush.Distinct(Engine.Adapter.Query.GetComparerForType<T>(this)).ToList(); // *removed* Distinct() on root objects.

            if (tag != "" && typeof(IBHoMObject).IsAssignableFrom(typeof(T)))
                newObjects.ForEach(x => x.Tags.Add(tag));

            // We may treat dependencies differently: like calling distinct only for them. Hence another method to Create them.
            if (m_AdapterSettings.HandleDependencies)
            {
                var dependencyTypes = Engine.Adapter.Query.GetDependencyTypes<T>(this);
                var dependencyObjects = Engine.Adapter.Query.GetDependencyObjects(newObjects, dependencyTypes, tag); //first-level dependencies

                foreach (var kv in dependencyObjects)
                {
                    if (!DependenciesCreateOnly(kv.Value as dynamic, tag))
                        return false;
                }
            }

            // Assign Id if required
            if (m_AdapterSettings.UseAdapterId)
                AssignNextFreeId(newObjects);

            // Create objects
            if (!ICreate(newObjects))
                return false;

            if (m_AdapterSettings.CreateOnly_DistinctObjects && m_AdapterSettings.UseAdapterId)
            {
                // Map Ids to the original set of objects (before we extracted the distincts elements from it).
                // If some objects of the original set were not Created (because e.g. they were already existing in the external model and had already an id, 
                // therefore no new id was assigned to them) they will not get mapped, so the original set will be left with them intact.
                IEqualityComparer<T> comparer = Engine.Adapter.Query.GetComparerForType<T>(this);
                foreach (T item in objectsToPush)
                {
                    item.CustomData[AdapterIdName] = newObjects.First(x => comparer.Equals(x, item)).CustomData[AdapterIdName];
                }
            }

            return true;
        }

        [Description("Called by CreateOnly() in order to recursively create the dependencies of the objects.")]
        protected virtual bool DependenciesCreateOnly<T>(IEnumerable<T> objectsToCreate, string tag = "", ActionConfig actionConfig = null) where T : IBHoMObject
        {
            // Make sure the dependencies objects are distinct 
            List<T> objects = !m_AdapterSettings.CreateOnly_DistinctDependencies ? objectsToCreate.ToList() : objectsToCreate.Distinct(Engine.Adapter.Query.GetComparerForType<T>(this)).ToList();

            // Make sure objects are tagged
            if (tag != "")
                objects.ForEach(x => x.Tags.Add(tag));

            // Create any sub-dependency
            var dependencyTypes = Engine.Adapter.Query.GetDependencyTypes<T>(this);
            var dependencyObjects = Engine.Adapter.Query.GetDependencyObjects<T>(objectsToCreate, dependencyTypes, tag);
            foreach (var depObj in dependencyObjects)
                DependenciesCreateOnly(depObj.Value as dynamic);

            // Assign Id if required
            if (m_AdapterSettings.UseAdapterId)
                AssignNextFreeId(objects);

            // Create objects
            if (!ICreate(objects))
                return false;

            if (m_AdapterSettings.CreateOnly_DistinctDependencies && m_AdapterSettings.UseAdapterId)
            {
                // Map Ids to the original set of objects (before we extracted the distincts elements from it).
                // If some objects of the original set were not Created (because e.g. they were already existing in the external model and had already an id, 
                // therefore no new id was assigned to them) they will not get mapped, so the original set will be left with them intact.
                IEqualityComparer<T> comparer = Engine.Adapter.Query.GetComparerForType<T>(this);
                foreach (T item in objectsToCreate)
                {
                    item.CustomData[AdapterIdName] = objects.First(x => comparer.Equals(x, item)).CustomData[AdapterIdName];
                }
            }

            return true;
        }


    }
}
