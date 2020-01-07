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

using BH.Engine.Reflection;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapter
{
    public partial class Modify
    {
        public static bool CopySpecificProperties<T>(this T target, T source, IEnumerable<string> propertyNames) where T : class, IBHoMObject
        {
            bool success = true;

            // Get the list of properties corresponding to type T
            Dictionary<string, PropertyInfo> propertyDictionary = typeof(T).GetProperties().ToList().ToDictionary(x => x.Name, x => x);

            foreach (string propertyName in propertyNames)
            {
                if (!propertyDictionary.ContainsKey(propertyName))
                {
                    Compute.RecordWarning($"While trying to copy properties, could not find property {propertyName} in {nameof(T)}.");
                    success &= false;
                    continue;
                }

                var propertyInfo = propertyDictionary[propertyName];

                Func<T, dynamic> getProp = (Func<T, dynamic>)Delegate.CreateDelegate(typeof(Func<T, dynamic>), propertyInfo.GetGetMethod());
                Action<T, dynamic> setProp = (Action<T, dynamic>)Delegate.CreateDelegate(typeof(Action<T, dynamic>), propertyInfo.GetSetMethod());

                dynamic sourcePropValue = getProp(source);
                dynamic targetPropValue = getProp(target);

                if (targetPropValue != null)
                {
                    // Assigning a value when the target object has some value assigned to it is dangerous. Better to return an error. 
                    // We then might want to handle these kind of conflicts on property-by-property basis, which would require some specific framework infrastructure.
                    Compute.RecordError($"Cannot copy value of overlapping property {propertyName} for an object of type {nameof(T)}." +
                        $"\nSource object: {source.BHoM_Guid}\nTarget object: {target.BHoM_Guid}");
                    return false;
                }

                setProp(target, sourcePropValue);
            }

            return success;
        }
    }
}
