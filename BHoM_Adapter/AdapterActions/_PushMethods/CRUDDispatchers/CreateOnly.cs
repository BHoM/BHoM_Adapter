/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Adapter;
using BH.oM.Base.Attributes;

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

            List<T> newObjects;
            IEnumerable<IGrouping<T, T>> distinctGroups = null;
            if (!callDistinct)
                newObjects = objectsToPush.ToList();
            else
            {
                distinctGroups = GroupAndCopyProperties(objectsToPush, actionConfig);
                newObjects = distinctGroups.Select(x => x.Key).ToList();
            }

            // Tag the objects, if tag is given.
            if (tag != "")
                newObjects.ForEach(x => x.Tags.Add(tag));

            // Assign Id if required
            if (m_AdapterSettings.UseAdapterId)
                AssignNextFreeId(newObjects);

            // Create objects
            if (m_AdapterSettings.CacheCRUDobjects)
            {
                if (!CreateAndCache(newObjects, actionConfig))
                    return false;
            }
            else if(!ICreate(newObjects, actionConfig))
                return false;

            if (callDistinct && m_AdapterSettings.UseAdapterId && distinctGroups != null)
            {
                // Map Ids to the original set of objects (before we extracted the distincts elements from it).
                // If some objects of the original set were not Created (because e.g. they were already existing in the external model and had already an id, 
                // therefore no new id was assigned to them) they will not get mapped, so the original set will be left with them intact.
                foreach (var group in distinctGroups)
                {
                    IFragment idFragment;
                    if (group.Key.Fragments.TryGetValue(AdapterIdFragmentType, out idFragment))
                    {
                        foreach (T item in group.Skip(1))   //Skip 1 as first instance is the key
                        {
                            item.SetAdapterId(idFragment as IAdapterId);
                        }
                    }
                }
            }

            return true;
        }

    }
}






