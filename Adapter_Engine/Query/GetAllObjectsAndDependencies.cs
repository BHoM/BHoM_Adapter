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
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Fetches all dependancy objects of types provided from the list of the objects. Firsts checks for any DependencyModules, if no present matching the type, tries to scan any property returning the types.")]
        public static Dictionary<Type, List<IBHoMObject>> GetAllObjectsAndDependencies<T>(this IEnumerable<T> objects, Dictionary<Type, List<Type>> allDependencyTypes, IBHoMAdapter adapter = null) where T : IBHoMObject
        {
            // Group the objects by their specific type.
            var typeGroups = objects.GroupBy(x => x.GetType());

            Dictionary<Type, List<IBHoMObject>> allObjectsPerType = new Dictionary<Type, List<IBHoMObject>>();

            foreach (var typeGroup in typeGroups)
            {
                if (allObjectsPerType.ContainsKey(typeGroup.Key))
                    allObjectsPerType[typeGroup.Key].AddRange(typeGroup.Cast<IBHoMObject>());
                else
                    allObjectsPerType[typeGroup.Key] = typeGroup.Cast<IBHoMObject>().ToList();


                if (!allDependencyTypes.TryGetValue(typeGroup.Key, out List<Type> dependencyTypes))
                    continue;

                //MethodInfo enumCastMethod_specificType = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(new[] { typeGroup.Key });
                //dynamic objList_specificType = enumCastMethod_specificType.Invoke(typeGroup, new object[] { typeGroup });

                //Dictionary<Type, IEnumerable> dependencyObjects = GetDependecyObjectWithCasting(typeGroup.First() as dynamic, typeGroup as dynamic, dependencyTypes, adapter);
                Dictionary<Type, IEnumerable> dependencyObjects = Engine.Adapter.Query.GetDependencyObjects(CastToFirstType(typeGroup.First() as dynamic, typeGroup as dynamic), dependencyTypes, adapter);

                // Dictionary<Type, IEnumerable> dependencyObjects = Engine.Adapter.Query.GetDependencyObjects(objList_specificType, dependencyTypes, adapter);
                foreach (var kv in dependencyObjects)
                {
                    if (allObjectsPerType.ContainsKey(kv.Key))
                        allObjectsPerType[kv.Key].AddRange(kv.Value.Cast<IBHoMObject>());
                    else
                        allObjectsPerType[kv.Key] = kv.Value.Cast<IBHoMObject>().ToList();

                    var recursedResult = kv.Value.OfType<IBHoMObject>().GetAllObjectsAndDependencies(allDependencyTypes, adapter);
                    foreach (var recursedRes in recursedResult)
                        if (allObjectsPerType.ContainsKey(recursedRes.Key))
                            throw new Exception("");
                        else
                            allObjectsPerType[recursedRes.Key] = recursedRes.Value;
                }
            }

            return allObjectsPerType;
        }

        /***************************************************/

        private static Dictionary<Type, IEnumerable> GetDependecyObjectWithCasting<T>(T item, IEnumerable<IBHoMObject> ienumerable, List<Type> dependencyTypes, IBHoMAdapter adapter) where T : IBHoMObject
        {
            return Engine.Adapter.Query.GetDependencyObjects(ienumerable.Cast<T>(), dependencyTypes, adapter);
        }

        private static IEnumerable<T> CastToFirstType<T>(T type, IEnumerable<IBHoMObject> objects)
        {
            return objects.Cast<T>();
        }
    }
}



