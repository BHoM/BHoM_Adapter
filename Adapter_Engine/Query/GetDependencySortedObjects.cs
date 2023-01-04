/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.Engine.Base;
using BH.oM.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Adapter;
using BH.oM.Base.Attributes;
using BH.Engine.Reflection;

namespace BH.Engine.Adapter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Push Support methods                      ****/
        /***************************************************/
        // These are support methods required by other methods in the Push process.

        [Description("Collects and groups all of the obejcts and dependencies of all the objects by type and pushtype. Groups are sorted by dependency order.")]
        [Input("objects", "Objects to group and sort by dependency order. The dependency of these objects will also be gathered recursively and included in the output.")]
        [Input("pushType", "PushType provided in the Push.")]
        [Input("bHoMAdapter", "The DependencyTypes that define the order of the output will be gathered from this Adapter instance.")]
        public static List<Tuple<Type, PushType, IEnumerable<object>>> GetDependencySortedObjects(this IEnumerable<IBHoMObject> objects, PushType pushType, IBHoMAdapter bHoMAdapter)
        {
            if ((!objects?.Any() ?? true) || bHoMAdapter == null)
                return new List<Tuple<Type, PushType, IEnumerable<object>>>();

            // Group the objects by their specific type.
            var typeGroups = objects.GroupBy(x => x.GetType());

            // Collect all objects and related dependency objects, recursively, and group them by type.
            Dictionary<Tuple<Type, PushType>, List<IBHoMObject>> allObjectsPerType = GetObjectsAndRecursiveDependencies(objects, pushType, bHoMAdapter);

            // Sort the groups by dependency order, so they can be pushed in the correct order.
            List<Tuple<Type, PushType, IEnumerable<object>>> orderedObjects = new List<Tuple<Type, PushType, IEnumerable<object>>>();

            List<Tuple<Type, PushType>> handledGroups = new List<Tuple<Type, PushType>>();
            foreach (var typeGroup in allObjectsPerType)
            {
                if (handledGroups.Contains(typeGroup.Key))
                    continue;

                // We can scan the rest of the input objects to see if they include dependencies of this current object type.
                List<Type> dependenciesToLookFor = new List<Type>();

                // Add direct dependencies of this current object type.
                if (bHoMAdapter.DependencyTypes.TryGetValue(typeGroup.Key.Item1, out List<Type> typeDeps))
                    dependenciesToLookFor.AddRange(typeDeps);

                // Check if the current object type has basetypes (interfaces) for which dependencies may have been specified.
                // E.g. for a CrossSection object we can get ISectionProperty which it implements,
                // which in turn is a type that commonly specifies additional dependencies (generally, IMaterialFragment).
                foreach (var baseType in typeGroup.Key.Item1.BaseTypes())
                {
                    if (bHoMAdapter.DependencyTypes.TryGetValue(baseType, out List<Type> baseTypeDeps))
                        dependenciesToLookFor.AddRange(baseTypeDeps);
                }

                // If this current object type does not have dependencies, add it at the start of the list and continue.
                if (!dependenciesToLookFor.Any())
                {
                    orderedObjects.Insert(0, new Tuple<Type, PushType, IEnumerable<object>>(typeGroup.Key.Item1, typeGroup.Key.Item2, typeGroup.Value.OfType<object>()));
                    handledGroups.Add(typeGroup.Key);
                    continue;
                }

                // Scan the rest of the input objects to see they include dependencies,
                // and add them to the orderedObjects list in order to prioritize them.
                foreach (var otherTypeGroup in allObjectsPerType)
                {
                    if (handledGroups.Contains(otherTypeGroup.Key) || otherTypeGroup.Key == typeGroup.Key)
                        continue;

                    if (dependenciesToLookFor.Contains(otherTypeGroup.Key.Item1) || dependenciesToLookFor.Any(d => d.IsAssignableFromIncludeGenericsAndRefTypes(otherTypeGroup.Key.Item1)))
                    {
                        orderedObjects.Add(new Tuple<Type, PushType, IEnumerable<object>>(otherTypeGroup.Key.Item1, otherTypeGroup.Key.Item2, otherTypeGroup.Value.OfType<object>()));
                        handledGroups.Add(otherTypeGroup.Key);
                    }
                }
            }

            // The non-handled groups can be added at the end in any order, because they don't have any dependency ordering.
            foreach (var typeGroup in allObjectsPerType)
            {
                if (handledGroups.Contains(typeGroup.Key))
                    continue;

                orderedObjects.Add(new Tuple<Type, PushType, IEnumerable<object>>(typeGroup.Key.Item1, typeGroup.Key.Item2, typeGroup.Value.OfType<object>()));
            }

            // Group per base type extracted from dependencies.
            // This is useful to reduce the number of CRUD calls.
            var allTypesInDependencies = bHoMAdapter.DependencyTypes.Values.SelectMany(v => v).Distinct();
            for (int i = 0; i < orderedObjects.Count; i++)
            {
                var kv = orderedObjects.ElementAt(i);

                foreach (Type baseType in kv.Item1.BaseTypes())
                {
                    bool found = false;
                    foreach (Type dependencyType in allTypesInDependencies)
                    {
                        if (baseType != dependencyType)
                            continue;

                        //Find matching item in the ordered obejct, matching the base type and push type.
                        var matchingItem = orderedObjects.FirstOrDefault(o => o.Item1 == baseType && o.Item2 == kv.Item2);

                        int matchingIndex;
                        //Get index of matching object if match is not null.
                        if (matchingItem != null)
                            matchingIndex = orderedObjects.IndexOf(matchingItem);
                        else
                            matchingIndex = -1;

                        if (matchingIndex == -1)
                        {
                            //Nothing found. Replace the current item with base type instead of concrete type.
                            orderedObjects[i] = new Tuple<Type, PushType, IEnumerable<object>>(baseType, kv.Item2, kv.Item3);
                        }
                        else
                        {
                            //If matching base type is found, concatenate the to sets together, to be CRUD together.
                            //For example, if the pushtype for both is the same, SteelSections and AluminiumSections will be grouped under ISectionProperty if ISectionProperty is in DependencyTypes.
                            var toAdd = new Tuple<Type, PushType, IEnumerable<object>>(baseType, orderedObjects[matchingIndex].Item2, orderedObjects[matchingIndex].Item3.Concat(kv.Item3));

                            int minIndex = Math.Min(i, matchingIndex);
                            int maxIndex = Math.Max(i, matchingIndex);

                            //Replace on minimum of the two indecies found and remove the other.
                            orderedObjects[minIndex] = toAdd;
                            orderedObjects.RemoveAt(maxIndex);
                            i--;    //Decrement i as item has been removed from the list.
                        }
                        found = true;
                        break;

                    }

                    if (found)
                        break;
                }
            }

            // If two types are subject to two different CRUD operations (e.g. UpdateOnly and FullCRUD),
            // make sure the order of CRUD operations is appropriate (e.g. UpdateOnly must happen before FullCRUD to avoid duplicates).
            // For example, this happens when both Nodes and Bars are sent via UpdateOnly during a same Push operation,
            // and the Nodes being updated are the same assigned to the endpoints of the Bars being updated).
            for (int i = 0; i < orderedObjects.Count; i++)
            {
                var kv1 = orderedObjects[i];

                for (int j = i + 1; j < orderedObjects.Count; j++)
                {
                    var kv2 = orderedObjects[j];

                    if (kv1.Item1 == kv2.Item1 && kv1.Item2 == PushType.FullPush && kv2.Item2 == PushType.UpdateOnly)
                    {
                        orderedObjects.RemoveAt(j);
                        orderedObjects.Insert(i, kv2);
                    }
                }
            }

            return orderedObjects;
        }
    }
}


