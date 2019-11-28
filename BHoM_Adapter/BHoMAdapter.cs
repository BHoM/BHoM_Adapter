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
using BH.oM.Structure.Elements;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter : IBHoMAdapter
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        [Description("Name of the child Adapter targeting an external software e.g. 'SpeckleAdapter'.")]
        public virtual string AdapterName { get; private set; }

        [Description("Different default settings for specific implementations may be set in the constructor.")]
        protected virtual AdapterSettings AdapterSettings { get; set; }

        [Description("Can be used to store any kind of additional data to be used in any Adapter method." +
            "Content is erased and repopulated every an Adapter Action (e.g. Push) is activated," +
            "so the data is not shared between different Actions.")]
        public virtual Dictionary<string, object> ActionConfig { get; set; }

        [Description("Properties of the objects that, in case of overlap, need to be ported from the ReadObjects to the ObjectsToPush.")]
        public virtual Dictionary<Type, List<string>> PropertiesToPort { get; set; }

        [Description("Push is executed first for any type in this list, in their order. Then all remaining types are pushed in no particular order.")]
        public virtual OrderedHashSet<Type> PushTypeOrder { get; set; }

        public virtual Guid AdapterGuid { get; set; } = Guid.NewGuid();


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

            PropertiesToPort = new Dictionary<Type, List<string>>
            {
                { typeof(Node), new List<string>(){nameof(Node.Support)} }
            };

            PushTypeOrder = new OrderedHashSet<Type>() { }; // typeof(Node), typeof(Bar), typeof(FEMesh), typeof(BarLoad), etc.
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
            // With new Id fragment, you can do AddAdapterId()
            // e.g. objects.First().AddAdapterId(new TestIdFragment(9));

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
