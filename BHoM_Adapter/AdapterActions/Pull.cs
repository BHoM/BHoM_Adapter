/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
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

        [Description("Performs a set up for the Request input of the Pull Action.")]
        public virtual bool SetupPullRequest(object request, out IRequest actualRequest)
        {
            // Always assure there is a Request. Allow to input a Type to generate a FilterRequest.
            actualRequest = null;

            if (request == null)
                actualRequest = new FilterRequest();
            else if (request is IRequest)
                actualRequest = request as IRequest;
            else if (request is Type)
                actualRequest = BH.Engine.Data.Create.FilterRequest((Type)request, "");
            else
                return false;

            return true;
        }

        /******************************************************/

        [Description("Performs a set up for the ActionConfig of the Pull Action.")]
        public virtual bool SetupPullConfig(ActionConfig actionConfig, out ActionConfig pullConfig)
        {
            // If null, set the actionConfig to a new ActionConfig.
            pullConfig = actionConfig == null ? new ActionConfig() : actionConfig;

            return true;
        }

        /******************************************************/

        [Description("Performs an action prior to any pull actions. For example can be used to open up a file for repeated pull actions.")]
        public virtual bool BeforePull(IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            m_HasRunPrePullAction = true;
            return true;
        }

        /******************************************************/

        [Description("Performs an action after any pull actions. For example can be used to close a file that was opened for repeated pull actions.")]
        public virtual bool AfterPull(IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            m_HasRunPrePullAction = false; //Reset to false for the next pull
            return true;
        }

        /******************************************************/

        [Description("Pulls objects from an external software using the basic CRUD/Read method as appropriate.")]
        public virtual IEnumerable<object> Pull(IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            this.ClearCache();
            // `(this as dynamic)` casts the abstract BHoMAdapter to its instance type (e.g. Speckle_Adapter), so all public ReadResults methods are captured
            if (request is IResultRequest)
                return (this as dynamic).ReadResults(request as dynamic, actionConfig);

            // `(this as dynamic)` casts the abstract BHoMAdapter to its instance type (e.g. Speckle_Adapter), so all public Read methods are captured
            if (request is IRequest)
                return (this as dynamic).Read(request as dynamic, actionConfig);

            IEnumerable<object> read = IRead(null, null, actionConfig);
            this.ClearCache();
            return read;
        }
    }
}