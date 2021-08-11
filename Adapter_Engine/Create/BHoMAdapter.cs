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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Adapter;
using System.Reflection;

namespace BH.Engine.Adapter
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a adapter of the specified type. Method makes use of the constructor with the largest number of arguments. Tries to match the provided arguments to best fit the arguments of the constructor.")]
        [Input("adapterType", "The type of adapter to create.")]
        [Input("filePath", "Optional file path parameter for the adapter. If the constructor of the adapter does not have a file path argument, this will be ignored.")]
        [Input("toolkitConfig", "Optional adapter config to be passed to the adapter. Needs to be of type matching the adapter. If nothing is provided, null will be used.")]
        [Input("active", "Boolean to trigger if the adapter should be active or not.")]
        [Output("adapter", "The created adapter of the specified type.")]
        public static IBHoMAdapter BHoMAdapter(Type adapterType, string filePath = "", object toolkitConfig = null, bool active = false)
        {
            if (adapterType == null)
            {
                Reflection.Compute.RecordError("Provided adapterType is null. Can not create adapter.");
                return null;
            }
            if (!active)
                return null;

            if (!typeof(IBHoMAdapter).IsAssignableFrom(adapterType))
            {
                Reflection.Compute.RecordError("The provided type is not an adapter type. Can not create the adapter.");
                return null;
            }

            //Get the constructor with the largest number of arguments
            ConstructorInfo constructor = adapterType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();

            List<object> arguments = new List<object>();

            foreach (var parameter in constructor.GetParameters())
            {
                if (parameter.ParameterType == typeof(string) && parameter.Name.ToLower().Contains("file"))
                    arguments.Add(filePath);
                else if (parameter.ParameterType == typeof(bool) && parameter.Name.ToLower().Contains("active"))
                    arguments.Add(active);
                else if (toolkitConfig != null && (parameter.Name.ToLower().Contains("setting") || parameter.Name.ToLower().Contains("config")))
                {
                    if (parameter.ParameterType.IsAssignableFrom(toolkitConfig.GetType()))
                        arguments.Add(toolkitConfig);
                    else
                    {
                        Reflection.Compute.RecordWarning($"Provided config is not of the correct type. Expecting a config of type {parameter.ParameterType.Name}. Null will be used instead.");
                        arguments.Add(null);
                    }
                }
                else
                {
                    if (parameter.ParameterType.IsValueType)
                        arguments.Add(Activator.CreateInstance(parameter.ParameterType));
                    else
                        arguments.Add(null);
                }
            }


            return constructor.Invoke(arguments.ToArray()) as IBHoMAdapter;
        }

        /***************************************************/
    }
}
