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
      
        [Description("Performs a Pull and then a Push. Useful to move data between two different software without passing it through the UI.")]
        public virtual bool Move(IBHoMAdapter to, IRequest request,
            PullType pullType = PullType.AdapterDefault, ActionConfig pullConfig = null,
            PushType pushType = PushType.AdapterDefault, ActionConfig pushConfig = null)
        {
            string tag = "";
            if (request is FilterRequest)
                tag = (request as FilterRequest).Tag;

            IEnumerable<object> objects = Pull(request, pullType, pullConfig);
            int count = objects.Count();

            return to.Push(objects.Cast<IObject>(), tag, pushType, pushConfig).Count() == count;
        }

        /******************************************************/

        [Description("Performs an action prior to any move actions. For example can be used to open up a file for repeated move actions. This method is intended to be called by the context in which this Adapter is run, which typically is a UI supported by BHoM.")]
        public virtual bool BeforeMove(IBHoMAdapter to, IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig pullConfig = null, PushType pushType = PushType.AdapterDefault,  ActionConfig actionConfig = null)
        {
            m_HasRunPreMoveAction = true;
            return true;
        }

        /******************************************************/

        [Description("Performs an action after any move actions. For example can be used to close a file for repeated move actions. This method is intended to be called by the context in which this Adapter is run, which typically is a UI supported by BHoM.")]
        public virtual bool AfterMove(IBHoMAdapter to, IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig pullConfig = null, PushType pushType = PushType.AdapterDefault, ActionConfig actionConfig = null)
        {
            m_HasRunPreMoveAction = false; //Reset for next move action
            return true;
        }

        /******************************************************/
    }
}






