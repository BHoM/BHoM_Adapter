/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using BH.oM.Data.Requests;
using BH.oM.Base.Attributes;
using BH.oM.Analytical.Results;
using BH.oM.Adapter;

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

        // This is the most basic Read method and it must be implemented at the Toolkit level.
        // It must implement only logic for reading things (e.g. through API calls), without modifying objects.
        protected virtual IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {
            BH.Engine.Base.Compute.RecordError($"Read for objects of type {type.Name} is not implemented in {(this as dynamic).GetType().Name}.");
            return new List<IBHoMObject>();
        }

        /***************************************************/
        /**** Wrapper methods                           ****/
        /***************************************************/
        // These methods extend the functionality of the basic methods (they wrap them) to avoid boilerplate code.
        // They get called by the Adapter Actions (Push, Pull, etc.), and they are responsible for calling the basic methods.

        /******* IRequest Wrapper methods *******/
        /* These methods have to be implemented if the Toolkit needs to support the Read for any generic IRequest. */
        protected virtual IEnumerable<IBHoMObject> Read(IRequest request, ActionConfig actionConfig = null)
        {
            // The implementation must:
            // 1. extract all the needed info from the IRequest
            // 2. return a call to the Basic Method Read() with the extracted info.

            BH.Engine.Base.Compute.RecordError($"Read for {request.GetType().Name} is not implemented in {(this as dynamic).GetType().Name}.");
            return new List<IBHoMObject>();
        }

        /******* Additional Wrapper methods *******/
        /* These methods contain some additional logic to avoid boilerplate.
           If needed, they can be overriden at the Toolkit level, but the new implementation must always call the appropriate Basic Method. */

        protected virtual IEnumerable<IBHoMObject> Read(FilterRequest filterRequest, ActionConfig actionConfig = null)
        {
            // Extract the Ids from the FilterRequest
            IList objectIds = null;
            object idObject;
            if (filterRequest.Equalities.TryGetValue("ObjectIds", out idObject) && idObject is IList)
                objectIds = idObject as IList;

            // Call the Basic Method Read() to get the objects based on the Ids
            IEnumerable<IBHoMObject> objects = IRead(filterRequest.Type, objectIds, actionConfig);

            // Null guard
            objects = objects ?? new List<IBHoMObject>();

            // If the FilterRequest contains a Tag, use it to further filter the objects
            if (filterRequest.Tag == "")
                return objects;
            else
                return objects.Where(x => x.Tags.Contains(filterRequest.Tag));
        }


        protected virtual IEnumerable<IBHoMObject> Read(Type type, string tag = "", ActionConfig actionConfig = null)
        {
            // Call the Basic Method Read() to get the objects based on the ids
            IEnumerable<IBHoMObject> objects = IRead(type, null as List<object>, actionConfig);

            // Null guard
            objects = objects ?? new List<IBHoMObject>();

            // Filter by tag if any 
            if (tag == "")
                return objects;
            else
                return objects.Where(x => x.Tags.Contains(tag));
        }
    }
}






