/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Push CRUD Dispatchers                     ****/
        /***************************************************/
        // These methods dispatch calls to different CRUD methods as required by the Push.

        [Description("Performs the only the Create for the specified objects and, if Config.HandleDependencies is true, their dependencies.")]
        [Input("objectLevel", "Indicates the level of recursion of the algorithm, so first-level objects and their possible dependencies can be treated differently.")]
        protected virtual bool CreateOnly<T>(IEnumerable<T> objectsToPush, string tag = "", ActionConfig actionConfig = null, int objectLevel = 0) where T : IBHoMObject
        {
            bool callDistinct = objectLevel == 0 ? m_AdapterSettings.CreateOnly_DistinctObjects : m_AdapterSettings.CreateOnly_DistinctDependencies;

            List<T> newObjects = !callDistinct ? objectsToPush.ToList() : objectsToPush.Distinct(Engine.Adapter.Query.GetComparerForType<T>(this)).ToList();

            // Tag the objects, if tag is given.
            if (tag != "")
                newObjects.ForEach(x => x.Tags.Add(tag));

            // We may treat dependencies differently: like calling distinct only for them.
            if (m_AdapterSettings.HandleDependencies)
            {
                var dependencyTypes = Engine.Adapter.Query.GetDependencyTypes<T>(this);
                var dependencyObjects = Engine.Adapter.Query.GetDependencyObjects(newObjects, dependencyTypes, tag); //first-level dependencies

                foreach (var kv in dependencyObjects)
                {
                    if (!CreateOnly(kv.Value as dynamic, tag, actionConfig, objectLevel++))
                        return false;
                }
            }

            // Assign Id if required
            if (m_AdapterSettings.UseAdapterId)
                AssignNextFreeId(newObjects);

            // Create objects
            if (!ICreate(newObjects, actionConfig))
                return false;

            if (callDistinct && m_AdapterSettings.UseAdapterId)
            {
                // Map Ids to the original set of objects (before we extracted the distincts elements from it).
                // If some objects of the original set were not Created (because e.g. they were already existing in the external model and had already an id, 
                // therefore no new id was assigned to them) they will not get mapped, so the original set will be left with them intact.
                IEqualityComparer<T> comparer = Engine.Adapter.Query.GetComparerForType<T>(this);
                foreach (T item in objectsToPush)
                {
                    // Fetch any existing IAdapterId fragment and assign it to the item.
                    // This preserves any additional property other than `Id` that may be in the fragment.
                    IFragment fragment;
                    newObjects.First(x => comparer.Equals(x, item)).Fragments.TryGetValue(AdapterIdFragmentType, out fragment);

                    item.SetAdapterId(fragment as IAdapterId);
                }
            }

            return true;
        }

    }
}


