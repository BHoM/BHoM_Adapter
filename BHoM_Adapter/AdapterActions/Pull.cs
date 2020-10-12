/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Adapter;
using BH.oM.Data.Requests;
using BH.Engine.Base;
using BH.Engine.Adapter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /******************************************************/
        /**** Public Adapter Methods "Adapter ACTIONS"    *****/
        /******************************************************/
        /* These methods represent Actions that the Adapter can complete. 
           They are publicly available in the UI as individual components, e.g. in Grasshopper, under BHoM/Adapters tab. */

        public virtual IEnumerable<object> SetupThenPull(object request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            // ---------------------------------------------//
            // Mandatory Adapter Action set-up              //
            //----------------------------------------------//
            // The following are mandatory set-ups to be ALWAYS performed 
            // before the Adapter Action is called,
            // whether the Action is overrided at the Toolkit level or not.

            // If unset, set the actionConfig to a new ActionConfig.
            actionConfig = actionConfig == null ? new ActionConfig() : actionConfig;

            // Always assure there is a Request. Allow to input a Type to generate a FilterRequest.
            IRequest actualRequest = null;

            if (request == null)
                actualRequest = new FilterRequest();
            else if (request is Type)
                actualRequest = BH.Engine.Data.Create.FilterRequest((Type)request, "");
            else if (typeof(IRequest).IsAssignableFrom(request.GetType()))
                actualRequest = request as IRequest;

            return Pull(actualRequest, pullType, actionConfig);
        }

        [Description("Pulls objects from an external software using the basic CRUD/Read method as appropriate.")]
        public virtual IEnumerable<object> Pull(IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            // `(this as dynamic)` casts the abstract BHoMAdapter to its instance type (e.g. Speckle_Adapter), so all public ReadResults methods are captured
            if (request is IResultRequest)
                return (this as dynamic).ReadResults(request as dynamic, actionConfig);

            // `(this as dynamic)` casts the abstract BHoMAdapter to its instance type (e.g. Speckle_Adapter), so all public Read methods are captured
            if (request is IRequest)
                return (this as dynamic).Read(request as dynamic, actionConfig);

            return IRead(null, null, actionConfig);
        }
    }
}

