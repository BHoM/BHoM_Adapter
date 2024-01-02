/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.Engine.Adapter;
using BH.Engine.Base;
using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        [Description("Groups the objects by the coparer for the particular type, and then runs any CopyPropertiesModules available for the type.")]
        private IEnumerable<IGrouping<T, T>> GroupAndCopyProperties<T>(IEnumerable<T> objectsToPush, ActionConfig actionConfig = null) where T : IBHoMObject
        {
            IEnumerable<IGrouping<T, T>> grouped = objectsToPush.GroupBy(x => x, Engine.Adapter.Query.GetComparerForType<T>(this, actionConfig));

            List<ICopyPropertiesModule<T>> copyPropertiesModules = this.GetCopyPropertiesModules<T>();

            foreach (var group in grouped)
            {
                T keep = group.Key;
                foreach (T item in group.Skip(1))   //Skip 1 as first instance is the key
                {
                    CopyBHoMObjectProperties(keep, item);
                    foreach (var copyModule in copyPropertiesModules)
                    {
                        copyModule.CopyProperties(keep, item);
                    }
                }
            }

            return grouped;
        }
    }
}





