/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.Engine.Reflection;
using BH.oM.Base;
using BH.Engine.Base;
using BH.oM.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BH.oM.Adapter;


namespace BH.Engine.Adapter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Push Support methods                      ****/
        /***************************************************/
        // These are support methods required by other methods in the Push process.

        [Description("Returns the comparer to be used with a certain object type.")]
        public static IEqualityComparer<T> GetComparerForType<T>(this IBHoMAdapter bHoMAdapter, PushConfig pushConfig = null) where T : IBHoMObject
        {
            Type type = typeof(T);

            if (pushConfig != null && pushConfig.AllowHashForComparing)
            {
                var propertiesToIgnore = new List<string>() { "BHoM_Guid", "CustomData" };

                return new HashFragmComparer<T>(propertiesToIgnore);
            }

            if (bHoMAdapter.AdapterComparers.ContainsKey(type))
                return bHoMAdapter.AdapterComparers[type] as IEqualityComparer<T>;

            return EqualityComparer<T>.Default;
        }
    }
}

