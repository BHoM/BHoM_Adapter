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
using BH.oM.Base.Attributes;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /******************************************************/
        /**** Public Adapter Methods "Adapter ACTIONS"    *****/
        /******************************************************/
        /* These methods represent Actions that the Adapter can complete. 
           They are publicly available in the UI as individual components, e.g. in Grasshopper, under BHoM/Adapters tab. */

        [Description("Performs a set up for the ActionConfig of the Execute Action.")]
        public virtual bool SetupExecuteConfig(ActionConfig actionConfig, out ActionConfig executeConfig)
        {
            // If null, set the actionConfig to a new ActionConfig.
            executeConfig = actionConfig == null ? new ActionConfig() : actionConfig;

            return true;
        }

        /******************************************************/

        [Description("Performs an action prior to any ececute actions. For example can be used to open up a file for repeated execute actions.")]
        public virtual bool BeforeExecute()
        {
            m_HasRunPreExecuteAction = true;
            return true;
        }

        /******************************************************/

        [Description("Performs an action after any ececute actions. For example can be used to close a file for repeated execute actions.")]
        public virtual bool AfterExecute()
        {
            m_HasRunPreExecuteAction = false; //Reset for next execute action
            return true;
        }

        /******************************************************/

        [Description("To be implemented to send specific commands to the external software, through its API or other means." +
            "To be implemented (overridden) in Toolkits.")]
        [Output("Output<object, bool>", "Output is a tuple-like class with: " +
            "Item1 = System.Object containing any desired outcome of the Execute; " +
            "Item2 = A boolean indicating if the Execute was successful.")]
        public virtual Output<List<object>, bool> Execute(IExecuteCommand command, ActionConfig actionConfig = null)
        {
            BH.Engine.Base.Compute.RecordError($"Execute is not implemented in {this.GetType().Name}.");

            return new Output<List<object>, bool> { Item1 = null, Item2 = false};
        }
    }
}
