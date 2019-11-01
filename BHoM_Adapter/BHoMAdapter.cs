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
using BH.Engine.Base;
using BH.oM.Data.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public string AdapterId { get; set; }

        public Guid BHoM_Guid { get; set; } = Guid.NewGuid();

        protected AdapterConfig Config { get; set; } = new AdapterConfig();


        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        // This method has to be implemented (overridden) at the Toolkit level for the CRUD method to work.
        // It tells the CRUD what kind of relationship (dependency) exists between the Types that must be Pushed.
        // E.g. A Line has dependency type of Points. See the wiki or look at existing Toolkit implementations for more info.
        protected virtual List<Type> DependencyTypes<T>()
        {
            return new List<Type>();
        }

        /***************************************************/

        protected virtual IEqualityComparer<T> Comparer<T>()
        {
            return EqualityComparer<T>.Default;
        }

        /***************************************************/

        protected virtual object NextId(Type objectType, bool refresh = false)
        {
            return null;
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
