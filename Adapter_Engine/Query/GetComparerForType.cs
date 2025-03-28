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

        [Description("Returns the comparer to be used with a certain object type.")]
        public static IEqualityComparer<T> GetComparerForType<T>(this IBHoMAdapter bHoMAdapter, ActionConfig actionConfig = null) where T : IBHoMObject
        {
            Type type = typeof(T);

            if (bHoMAdapter.AdapterComparers.ContainsKey(type))
                return bHoMAdapter.AdapterComparers[type] as IEqualityComparer<T>;

            var interfaceComparer = bHoMAdapter.AdapterComparers.Where(x => x.Key.IsAssignableFrom(type));

            if (interfaceComparer.Any())
                return interfaceComparer.First().Value as IEqualityComparer<T>;

            if (actionConfig != null && actionConfig.AllowHashForComparing)
                return new HashComparer<T>(actionConfig.DiffingConfig.ComparisonConfig); // by default the hash doesn't consider GUIDs, Fragments and CustomData. You can set different exceptions in the ActionConfig's DiffConfig.

            return EqualityComparer<T>.Default;
        }
    }
}






