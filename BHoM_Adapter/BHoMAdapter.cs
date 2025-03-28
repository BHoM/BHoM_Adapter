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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter : IBHoMAdapter
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public Type AdapterIdFragmentType { get; set; }

        [Description("Modules containing adapter functionality")]
        public ModuleSet AdapterModules { get; set; } = new ModuleSet();

        [Description("Object comparers to be used within a specific Adapter." +
            "E.g. A Structural Node can be compared only using its geometrical location." +
            "Needed because different software need different rules for comparing objects.")]
        public Dictionary<Type, object> AdapterComparers { get; protected set; } =  new Dictionary<Type, object>
        {
            // In your adapter constructor, populate this with values like:
            // {typeof(Node), new BH.Engine.Structure.NodeDistanceComparer(3) }
        };

        [Description("Dependecies between different IBHoMObjects to be considered within a specific Adapter." +
            "E.g. A Line has dependency type of Points. " +
            "Needed because different software have different dependency relationships.")]
        public Dictionary<Type, List<Type>> DependencyTypes { get; protected set; } = new Dictionary<Type, List<Type>>
        {
            // In your adapter constructor, populate this with values like:
            // {typeof(Bar), new List<Type> { typeof(ISectionProperty), typeof(Node) } }
        };

        public List<Type> PriorityTypes { get; protected set; } = new List<Type>()
        {
            // If you are using the dependency types, this only needs to cover cases that are not already handled by this system
            // e.g., if Bars are set to have a dependency on Nodes, then there is no need to add them in order in the priority types
            // In your adapter constructor, populate this with values like:
            // {typeof(Level), typeof(Node), typeof(Bar), ...}

        };

        public Guid AdapterGuid { get; set; } = Guid.NewGuid();

        /***************************************************/
        /**** Protected Fields                          ****/
        /***************************************************/

        // You can change the default AdapterSettings values in your Toolkit's Adapter constructor 
        // e.g. AdapterSettings.WrapNonBHoMObjects = true;
        protected AdapterSettings m_AdapterSettings { get; set; } = new AdapterSettings();

        protected bool m_HasRunPrePullAction { get; set; } = false; //Flag to determine whether the adapter has run pre-pull actions
        protected bool m_HasRunPrePushAction { get; set; } = false; //Flag to determine whether the adapter has run pre-push actions
        protected bool m_HasRunPreExecuteAction { get; set; } = false; //Flag to determine whether the adapter has run pre-execute actions
        protected bool m_HasRunPreMoveAction { get; set; } = false; //Flag to determine whether the adapter has run pre-move actions
        protected bool m_HasRunPreRemoveAction { get; set; } = false; //Flag to determine whether the adapter has run pre-remove actions

        /***************************************************/
        /**** Public Events                             ****/
        /***************************************************/

        public event EventHandler DataUpdated;

        /***************************************************/

        protected virtual void OnDataUpdated()
        {
            if (DataUpdated != null)
                DataUpdated.Invoke(this, new EventArgs());
        }
    }
}






