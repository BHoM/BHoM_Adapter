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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.Data.Requests;

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


        // Basic Delete method that deletes objects depending on their Type and Id. 
        // Its implementation is facultative (not needed for a simple export/import scenario). 
        // Toolkits need to implement (override) this only to get the full CRUD to work.
        protected virtual int IDelete(Type type, IEnumerable<object> ids, ActionConfig actionConfig = null)
        {
            Engine.Reflection.Compute.RecordError($"Delete for objects of type {type.Name} is not implemented in {(this as dynamic).GetType().Name}.");
            return 0;
        }

        /***************************************************/
        /**** Wrapper methods                           ****/
        /***************************************************/
        // These methods extend the functionality of the basic methods (they wrap them) to avoid boilerplate code.
        // They get called by the Adapter Actions (Push, Pull, etc.), and they are responsible for calling the basic methods.

        /******* IRequest Wrapper methods *******/
        /* These methods have to be implemented if the Toolkit needs to support the Read for any generic IRequest. */

        protected virtual int Delete(IRequest request, ActionConfig actionConfig = null)
        {
            Engine.Reflection.Compute.RecordError($"Delete for {request.GetType().Name} is not implemented in {(this as dynamic).GetType().Name}.");
            return 0;
        }

        /******* Additional Wrapper methods *******/
        /* These methods contain some additional logic to avoid boilerplate.
           If needed, they can be overriden at the Toolkit level, but the new implementation must always call the appropriate Basic Method. */

        protected virtual int Delete(FilterRequest request, ActionConfig actionConfig = null)
        {
            return Delete(request.Type, request.Tag);
        }

        protected virtual int Delete(Type type, string tag = "", ActionConfig actionConfig = null)
        {
            if (tag == "")
            {
                return IDelete(type, null as List<object>);
            }
            else
            {
                // Get all with tag
                IEnumerable<IBHoMObject> withTag = Read(type, tag, actionConfig);

                // Get indices of all with that tag only
                IEnumerable<object> ids = withTag.Where(x => x.Tags.Count == 1).Select(x => x.CustomData[AdapterIdName]).OrderBy(x => x);
                IDelete(type, ids);

                // Remove tag if other tags as well
                IEnumerable<IBHoMObject> multiTags = withTag.Where(x => x.Tags.Count > 1);
                IUpdateTags(type, multiTags.Select(x => x.CustomData[AdapterIdName]), multiTags.Select(x => x.Tags), actionConfig);

                return ids.Count();
            }
        }

    }
}
