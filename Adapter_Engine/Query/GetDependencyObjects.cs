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
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Adapter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Fetches all dependancy objects of types provided from the list of the objects. Firsts checks for any DependencyModules, if no present matching the type, tries to scan any property returning the types.")]
        public static Dictionary<Type, IEnumerable> GetDependencyObjects<T>(this IEnumerable<T> objects, List<Type> dependencyTypes, IBHoMAdapter adapter = null) where T : IBHoMObject
        {
            if (objects == null || !objects.Any() || dependencyTypes == null || !dependencyTypes.Any())
                return new Dictionary<Type, IEnumerable>();

            Dictionary<Type, IEnumerable> dict = new Dictionary<Type, IEnumerable>();

            var method = typeof(Query)
                        .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                        .Single(m => m.Name == nameof(GetDependencyObjects) && m.IsGenericMethodDefinition && m.GetParameters().Count() == 2);


            foreach (Type t in dependencyTypes)
            {
                MethodInfo generic = method.MakeGenericMethod(new Type[] { typeof(T), t });
                var list = generic.Invoke(null, new object[] { objects, adapter });

                dict.Add(t, list as IEnumerable);
            }

            return dict;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<P> GetDependencyObjects<T, P>(this IEnumerable<T> objects, IBHoMAdapter adapter) where T : IBHoMObject where P : IBHoMObject
        {
            //If adapter is provided and not null, check for dependency modules
            if (adapter != null)
            {
                //Look for any GetDependencyModules of the current type
                List<IGetDependencyModule<T, P>> dependencyModules = adapter.AdapterModules.OfType<IGetDependencyModule<T, P>>().ToList();
                if (dependencyModules.Count != 0)
                {
                    return dependencyModules.SelectMany(x => x.GetDependencies(objects)).ToList();
                }
            }

            //No modules found, instead rely on reflection to extract the dependency properties
            return objects.DistinctProperties<T, P>().ToList();

        }

        /***************************************************/
    }
}



