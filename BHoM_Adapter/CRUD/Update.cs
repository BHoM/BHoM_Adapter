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
    // NOTE: CRUD folder methods
    // All methods in the CRUD folder are used as "back-end" methods by the Adapter itself.
    // They are meant to be implemented at the Toolkit level.
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Basic Methods                             ****/
        /***************************************************/
        /* These methods provide the basic functionalities for the CRUD to work. */

        // Unlike the Create, Delete and Read, this method already exposes a simple implementation: it calls Delete and then Create.
        // It can be overridden at the Toolkit level if a more appropriate implementation is required.
        protected virtual bool UpdateObjects<T>(IEnumerable<T> objects) where T : IObject
        {
            Type objectType = typeof(T);
            if (Config.UseAdapterId && typeof(IBHoMObject).IsAssignableFrom(objectType))
            {
                Delete(typeof(T), objects.Select(x => ((IBHoMObject)x).CustomData[AdapterId]));
            }
            return Create(objects);
        }

        // UpdateProperty should only update a single property of an object without re-writing the whole object.
        // Currently, its main usage is to update the Tags of an IBHoMObject in the CRUD method, and
        // it needs to be implemented at the Toolkit level for the full CRUD to work.
        public virtual int UpdateProperty(Type type, IEnumerable<object> ids, string property, object newValue)
        {
            return 0;
        }

        /***************************************************/
        /**** Wrapper methods                           ****/
        /***************************************************/
        /* These methods extend the functionality of the basic methods (they wrap them) to avoid boilerplate code.
           They get called by the Adapter Actions (Push, Pull, etc.), and they are responsible for calling the basic methods. */

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
