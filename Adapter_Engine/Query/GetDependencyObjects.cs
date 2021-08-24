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

using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BH.oM.Adapter;

namespace BH.Engine.Adapter
{
    public static partial class Query
    {
        public static Dictionary<Type, IEnumerable> GetDependencyObjects<T>(this IBHoMAdapter adapter, IEnumerable<T> objects, List<Type> dependencyTypes, string tag) where T : IBHoMObject
        {
            Dictionary<Type, IEnumerable> dict = new Dictionary<Type, IEnumerable>();

            List<IGetDependencyModule<T>> dependencyModules = adapter.AdapterModules.OfType<IGetDependencyModule<T>>().ToList();

            if (dependencyModules.Count != 0)
            {
                foreach (Type t in dependencyTypes)
                {
                    foreach (IGetDependencyModule<T> module in dependencyModules)
                    {
                        var dependencies = module.GetDependencies(objects, t);
                        if (dependencies.Key != null && dependencies.Value.Cast<object>().Any())
                            dict.Add(dependencies.Key, dependencies.Value);
                    }
                }
                return dict;
            }

            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (Type t in dependencyTypes)
            {
                IEnumerable<object> merged = objects.DistinctProperties<T>(t);
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { t });

                var list = miListObject.Invoke(merged, new object[] { merged });

                dict.Add(t, list as IEnumerable);
            }

            return dict;
        }
    }
}


