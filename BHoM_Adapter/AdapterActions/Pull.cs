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

using BH.oM.Base;
using BH.Engine.Base;
using BH.oM.Data.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /******************************************************/
        /**** Public Adapter Methods "Adapter ACTIONS"    *****/
        /******************************************************/
        /* These methods represent Actions that the Adapter can complete. 
           They are publicly available in the UI as individual components, e.g. in Grasshopper, under BHoM/Adapters tab. */

        // Calls the appropriate basic CRUD/Read method.
        public virtual IEnumerable<object> Pull(IRequest request, PullOption pullOption = PullOption.Unset, Dictionary<string, object> config = null)
        {
            // --------------------------------------------------------------------------------- //
            // *** Temporary retrocompatibility fix ***
            // If it's a FilterRequest, check if it should read IResults or Objects with that.
            // This should be replaced by an appropriate IResultRequest.
            FilterRequest filterReq = request as FilterRequest;
            if (filterReq != null)
                if (typeof(BH.oM.Common.IResult).IsAssignableFrom(filterReq.Type))
                    return ReadResults(filterReq);
            // --------------------------------------------------------------------------------- //

            //Casting the adapter to its instance type (this as dynamic) to make sure all ReadResults methods are captured
            if (request is IResultRequest)
                return (this as dynamic).ReadResults(request as dynamic);

            //Casting the adapter to its instance type (this as dynamic) to make sure all Read methods are captured
            if (request is IRequest)
                return (this as dynamic).Read(request as dynamic);

            return new List<object>();
        }

    }
}
