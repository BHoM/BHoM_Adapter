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

        [Description("Performs a set up for the Request input of the Remove Action.")]
        public virtual bool SetupRemoveRequest(object obj, out IRequest removeRequest)
        {
            return SetupPullRequest(obj, out removeRequest);
        }

        [Description("Performs a set up for the ActionConfig of the Remove Action.")]
        public virtual bool SetupRemoveConfig(ActionConfig actionConfig, out ActionConfig removeConfig)
        {
            // If null, set the actionConfig to a new ActionConfig.
            removeConfig = actionConfig == null ? new ActionConfig() : actionConfig;

            return true;
        }

        [Description("Calls the basic CRUD/Delete method as appropriate." +
            "The base implementation just calls Delete. It can be overridden in Toolkits.")]
        public virtual int Remove(IRequest request, ActionConfig actionConfig = null)
        {
            this.ClearCache();
            return Delete(request as dynamic, actionConfig);
        }

        /******************************************************/

        [Description("Performs an action prior to any remove actions. For example can be used to open up a file for repeated remove actions.")]
        public virtual bool BeforeRemove(IRequest request, ActionConfig actionConfig = null)
        {
            m_HasRunPreRemoveAction = true;
            return true;
        }

        /******************************************************/

        [Description("Performs an action after any remove actions. For example can be used to close a file for repeated remove actions.")]
        public virtual bool AfterRemove(IRequest request, ActionConfig actionConfig = null)
        {
            m_HasRunPreRemoveAction = false; //Reset for next remove action
            return true;
        }

        /******************************************************/
    }
}





