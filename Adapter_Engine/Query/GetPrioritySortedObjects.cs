
using BH.oM.Adapter;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System;

/*This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BH.Engine.Adapter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Push Support methods                      ****/
        /***************************************************/
        // These are support methods required by other methods in the Push process.

        [Description("Groups of objects are sorted by priority order.")]
        [Input("objects", "Objects to group and sort by priority order.")]
        [Input("pushType", "PushType provided in the Push.")]
        [Input("bHoMAdapter", "The PriorityTypes that define the order of the output will be gathered from this Adapter instance.")]
        public static List<Tuple<Type, PushType, IEnumerable<object>>> GetPrioritySortedObjects(this List<Tuple<Type, PushType, IEnumerable<object>>> objects, PushType pushType, IBHoMAdapter bHoMAdapter)
        {
            List<Type> priorityTypes = bHoMAdapter?.PriorityTypes;

            if(objects == null || objects.Count == 0 || priorityTypes == null || priorityTypes.Count == 0)
                return objects;

            List<Tuple<Type, PushType, IEnumerable<object>>> prioritySortedObjects = objects.ToList();

            //Loop through the priority types backwards to ensure the first one in the list is moved to the top
            for (int i = priorityTypes.Count - 1; i >= 0; i--)
            {
                Type current = priorityTypes[i];
                //Loop through the object list backwards to keep previous sorting intact
                //Intentionally skipping index 0 (j >= 1) as object will be moved there anyway
                for (int j = prioritySortedObjects.Count - 1; j >= 1; j--)
                {
                    if (prioritySortedObjects[j].Item1 == current)
                    { 
                        var temp = prioritySortedObjects[j];
                        prioritySortedObjects.RemoveAt(j);
                        prioritySortedObjects.Insert(0, temp);
                        j++;                 //Increment index to test against the same index again (counteracting the j-- in the for loop) as list will have been shifted
                    }
                }
            }

            return prioritySortedObjects;
        }

        /***************************************************/
    }
}



