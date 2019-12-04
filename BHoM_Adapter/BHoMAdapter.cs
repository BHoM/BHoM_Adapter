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
using BH.oM.Adapter;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter : IBHoMAdapter
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        [Description("Used only as a key for the CustomData dictionary. E.g. key = Speckle_Id, value = 123")]
        public string AdapterIdName { get; set; } // value to be assigned in the specific Adapter constructor, e.g. = BH.Engine.GSA.Convert.AdapterId;

        public Guid AdapterGuid { get; set; }

        /***************************************************/
        /**** Protected Fields                          ****/
        /***************************************************/

        // You can change the default AdapterSettings values in your Toolkit's Adapter constructor 
        // e.g. AdapterSettings.WrapNonBHoMObjects = true;
        protected AdapterSettings m_AdapterSettings = new AdapterSettings();

        // Object comparers to be used within a specific Adapter.
        // E.g. A Structural Node can be compared only using its geometrical location.
        // Needed because different software need different rules for comparing objects.
        protected Dictionary<Type, object> m_adapterComparers = new Dictionary<Type, object>
        {
            // In your adapter constructor, populate this with values like:
            // {typeof(Node), new BH.Engine.Structure.NodeDistanceComparer(3) }
        };

        // Dependecies between different IBHoMObjects to be considered within a specific Adapter.
        // E.g. A Line has dependency type of Points. 
        // Needed because different software have different dependency relationships.
        protected Dictionary<Type, List<Type>> m_dependencyTypes = new Dictionary<Type, List<Type>>
        {
            // In your adapter constructor, populate this with values like:
            // {typeof(Bar), new List<Type> { typeof(ISectionProperty), typeof(Node) } }
        };

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
