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
using BH.oM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Protected CRUD Method                     ****/
        /***************************************************/

        protected virtual bool UpdateObjects<T>(IEnumerable<T> objects) where T : IObject
        {
            Type objectType = typeof(T);
            if (Config.UseAdapterId && typeof(IBHoMObject).IsAssignableFrom(objectType))
            {
                Delete(typeof(T), objects.Select(x => ((IBHoMObject)x).CustomData[AdapterId]));
            }
            return Create(objects);
        }


        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected bool UpdateOnly<T>(IEnumerable<T> objectsToPush, string tag = "") where T : IBHoMObject
        {
            List<T> newObjects = objectsToPush.ToList();

            // Make sure objects  are tagged
            if (tag != "")
                newObjects.ForEach(x => x.Tags.Add(tag));

            // Merge and push the dependencies
            if (Config.HandleDependencies)
            {
                var dependencyObjects = GetDependencyObjects<T>(newObjects, tag);

                foreach (var depObj in dependencyObjects)
                    if (!CRUD(depObj.Value as dynamic, tag))
                        return false;
            }

            return UpdateObjects(newObjects);

        }

        /***************************************************/
    }
}