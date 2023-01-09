/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using System.Collections;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Loads;


namespace BH.Adapter.Modules
{
    [Description("Get all elements that does not contain an adapter ID of the expected type. Avoids the need to again read and check against elements already in the model.")]
    public class GetLoadElementsWithoutID<T> : IGetDependencyModule<IElementLoad<T>, T> where T : IBHoMObject
    {
        /***************************************************/
        /**** Interface method                          ****/
        /***************************************************/

        public IEnumerable<T> GetDependencies(IEnumerable<IElementLoad<T>> objects)
        {
            List<T> noIdLoadObjects = new List<T>();
            foreach (IElementLoad<T> load in objects)
            {
                if(load?.Objects?.Elements != null)
                    noIdLoadObjects.AddRange(load.Objects.Elements.Where(x => x != null && !x.Fragments.Contains(m_adapterIdType)));
            }
            return noIdLoadObjects;
        }

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GetLoadElementsWithoutID(IBHoMAdapter adapter)
        {
            m_adapterIdType = adapter.AdapterIdFragmentType;
        }

        /***************************************************/

        private Type m_adapterIdType;

        /***************************************************/
    }
}


