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
using BH.Engine.Base;
using BH.oM.Data.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Adapter;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /******************************************************/
        /**** Public Adapter Methods "Adapter ACTIONS"    *****/
        /******************************************************/
        /* These methods represent Actions that the Adapter can complete. 
           They are publicly available in the UI as individual components, e.g. in Grasshopper, under BHoM/Adapters tab. */

        [Description("Performs a set up, then calls the Remove Action.")]
        public virtual int SetupThenRemove(object request, ActionConfig actionConfig = null)
        {
            // This method includes following are set-ups to be performed before the Remove Action is called.
            // If you override this method, make sure you know what you're doing.

            // If unset, set the actionConfig to a new ActionConfig.
            actionConfig = actionConfig == null ? new ActionConfig() : actionConfig;

            // Always assure there is a Request. Allow to input a Type to generate a FilterRequest.
            IRequest actualRequest = null;

            if (request == null)
                actualRequest = new FilterRequest();

            if (request is Type)
                actualRequest = BH.Engine.Data.Create.FilterRequest((Type)request, "");

            return Remove(actualRequest, actionConfig);
        }

        [Description("Calls the basic CRUD/Delete method as appropriate." +
            "The base implementation just calls Delete. It can be overridden in Toolkits.")]
        public virtual int Remove(IRequest request, ActionConfig actionConfig = null)
        {
            return Delete(request as dynamic, actionConfig);
        }
    }
}

