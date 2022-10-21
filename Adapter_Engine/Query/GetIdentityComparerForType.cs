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

using BH.Engine;
using BH.oM.Base;
using BH.Engine.Base;
using BH.oM.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Adapter;
using BH.Engine.Diffing;
using BH.Engine.Base.Objects;

namespace BH.Engine.Adapter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Push Support methods                      ****/
        /***************************************************/
        // These are support methods required by other methods in the Push process.

        [Description("Returns the identity comparer to be used with a certain object type, i.e. the comparer to check if the object is identical on every level as seen from the software. Defaults to the HashComparer.")]
        public static IEqualityComparer<T> GetIdentityComparerForType<T>(this IBHoMAdapter bHoMAdapter, ActionConfig actionConfig = null) where T : IBHoMObject
        {
            Type type = typeof(T);

            if (bHoMAdapter.IdentityComparers.ContainsKey(type))
                return bHoMAdapter.IdentityComparers[type] as IEqualityComparer<T>;

            var interfaceComparer = bHoMAdapter.IdentityComparers.Where(x => x.Key.IsAssignableFrom(type));

            if (interfaceComparer.Any())
                return interfaceComparer.First().Value as IEqualityComparer<T>;

            return new HashComparer<T>(actionConfig?.DiffingConfig?.ComparisonConfig ?? new BH.oM.Base.ComparisonConfig()); //Default to using the HashComparer
        }
    }
}



