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
using BH.Engine.Base;
using BH.oM.Data.Requests;
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
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        [Description("Name of the external software followed by `_id`. E.g. `Speckle` -> `Speckle_id`. Automatically extracted from derived class name (e.g. Speckle_Adapter).")]
        public string AdapterId { get; set; }

        [Description("Different default settings for specific implementations may be set in the constructor.")]
        protected AdapterSettings AdapterSettings { get; set; } 

        [Description("Can be used to store any kind of additional data to be used in any Adapter method.")]
        public Dictionary<string, object> ActionConfig { get; set; }

        public Guid BHoM_Guid { get; set; } = Guid.NewGuid();


        /***************************************************/
        /**** Constructor                               ****/
        /***************************************************/

        public BHoMAdapter()
        {
            AdapterId = GetType().Name.Split('_')[0] + "_id"; // e.g. Speckle_Adapter -> "Speckle_id"

            AdapterSettings = new AdapterSettings(); // Change the default AdapterSettings values in your Toolkit's Adapter constructor, e.g. AdapterSettings.WrapNonBHoMObjects = true;

            ActionConfig = new Dictionary<string, object>();
        }

        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        [Description("To be implemented (overrided) at the Toolkit level for the full CRUD to work." +
            "Tells the CRUD what kind of relationship (dependency) exists between the Types that must be Pushed." +
            "E.g. A Line has dependency type of Points. Needed because not all software have the same dependency relationship." +
            "See the wiki or look at existing Adapter implementations in the Toolkits for more info.")]
        protected virtual List<Type> DependencyTypes<T>()
        {
            return new List<Type>();
        }

        /***************************************************/

        protected virtual object NextId(Type objectType, bool refresh = false)
        {
            return null;
        }

        /***************************************************/

        protected virtual IEqualityComparer<T> Comparer<T>()
        {
            return EqualityComparer<T>.Default;
        }

        /***************************************************/

        protected void AssignId<T>(IEnumerable<T> objects) where T : IBHoMObject
        {
            bool refresh = true;
            foreach (T item in objects)
            {
                if (!item.CustomData.ContainsKey(AdapterId))
                {
                    item.CustomData[AdapterId] = NextId(typeof(T), refresh);
                    refresh = false;
                }
            }
        }

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
