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
using BH.Engine.Adapter;
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
    public abstract partial class BHoMAdapter : IBHoMAdapter
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        [Description("Name of the child Adapter targeting an external software e.g. 'SpeckleAdapter'.")]
        public string AdapterName { get; private set; }

        [Description("Different default settings for specific implementations may be set in the constructor.")]
        protected AdapterSettings AdapterSettings { get; set; }

        [Description("Can be used to store any kind of additional data to be used in any Adapter method." +
            "Re-initialisation happens in the BHoM_UI every time an Adapter Action (e.g. Push) is activated," +
            "so the data is not shared between different Actions.")]
        public Dictionary<string, object> ActionConfig { get; set; }

        public static Dictionary<Type, Dictionary<Type, int>> LastId { get; set; }

        public Guid BHoM_Guid { get; set; } = Guid.NewGuid();


        /***************************************************/
        /**** Constructor                               ****/
        /***************************************************/

        public BHoMAdapter()
        {
            // Set the adapter name through reflection based on the child class name (e.g. "SpeckleAdapter")
            AdapterName = GetType().Name;

            // Set the AdapterId Key Name (e.g. "Speckle_id") for the CustomData dictionary.
            // Used only as a Key for the CustomData dictionary; corresponding Value will be the id for the specific Adapter instance.
            // Might be superseded soon by the ID-as-fragment change.
            AdapterId = AdapterName.Split(new string[] { "Adapter" }, StringSplitOptions.None)[0]; // e.g. SpeckleAdapter -> "Speckle_id"

            // You can change the default AdapterSettings values in your Toolkit's Adapter constructor 
            // e.g. AdapterSettings.WrapNonBHoMObjects = true;
            AdapterSettings = new AdapterSettings();

            // First initialisation of the ActionConfig.
            // Re-initialisation happens in the BHoM_UI every time an Adapter action is activated.
            ActionConfig = new Dictionary<string, object>();

            LastId = new Dictionary<Type, Dictionary<Type, int>>();
            LastId[this.GetType()] = new Dictionary<Type, int>();
        }

        /***************************************************/
        /**** Protected Fields                          ****/
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
            // refresh: to say if it is the first of many calls during the same pass of the adapter
            // so you only need to ask the adapter once, then increment
            // useful for some softwares
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
            objects.First().AddAdapterId(new TestIdFragment(9));
            objects.First().AddAdapterId(9);


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

        [Description("Used only as a key for the CustomData dictionary; corresponding value will be the id for the specific Adapter instance.")]
        protected string AdapterId { get; set; }

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
