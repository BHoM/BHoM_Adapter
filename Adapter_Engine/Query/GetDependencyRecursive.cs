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

        public static Dictionary<Tuple<Type, PushType>, List<IBHoMObject>> GetObjectsAndRecursiveDependencies(this IEnumerable<IBHoMObject> objects, PushType pushType, IBHoMAdapter adapter)
        {
            // Group the objects by their specific type.
            var typeGroups = objects.GroupBy(x => x.GetType());

            Dictionary<Tuple<Type, PushType>, List<IBHoMObject>> allObjectsPerType = new Dictionary<Tuple<Type, PushType>, List<IBHoMObject>>();

            foreach (var typeGroup in typeGroups)
            {
                var key = new Tuple<Type, PushType>(typeGroup.Key, pushType);
                if (allObjectsPerType.ContainsKey(key))
                    allObjectsPerType[key].AddRange(typeGroup.Cast<IBHoMObject>());
                else
                    allObjectsPerType[key] = typeGroup.Cast<IBHoMObject>().ToList();

                MethodInfo enumCastMethod_specificType = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(new[] { typeGroup.Key });
                dynamic objList_specificType = enumCastMethod_specificType.Invoke(typeGroup, new object[] { typeGroup });

                PushType dependecyPushType = pushType == PushType.UpdateOnly ? PushType.FullPush : pushType;

                GetDependencyObjectsRecursive(objList_specificType, allObjectsPerType, dependecyPushType, adapter);
            }

            return allObjectsPerType;
        }

        private static void GetDependencyObjectsRecursive<T>(this IEnumerable<T> objects, Dictionary<Tuple<Type, PushType>, List<IBHoMObject>> gatheredDependecies, PushType pushType, IBHoMAdapter adapter) where T : IBHoMObject
        {
            List<Type> dependencies = GetDependencyTypes<T>(adapter);
            Dictionary<Type, IEnumerable> dependencyObjects = Engine.Adapter.Query.GetDependencyObjects(objects, dependencies, adapter);

            foreach (var depObj in dependencyObjects)
            {
                var key = new Tuple<Type, PushType>(depObj.Key, pushType);
                if (gatheredDependecies.ContainsKey(key))
                    gatheredDependecies[key].AddRange(depObj.Value.Cast<IBHoMObject>());
                else
                    gatheredDependecies[key] = depObj.Value.Cast<IBHoMObject>().ToList();

                GetDependencyObjectsRecursive(depObj.Value as dynamic, gatheredDependecies, pushType, adapter);
            }
        }
    }
}

