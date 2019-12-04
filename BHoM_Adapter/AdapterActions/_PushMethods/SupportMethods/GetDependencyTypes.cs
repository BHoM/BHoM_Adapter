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

using BH.Engine.Reflection;
using BH.oM.Base;
using BH.Engine.Base;
using BH.oM.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BH.oM.Adapter;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        /***************************************************/
        /**** Push Support methods                      ****/
        /***************************************************/
        // These are support methods required by other methods in the Push process.

        [Description("Tells the CRUD what kind of relationship (dependency) exists between the Types that must be Pushed." +
            "E.g. A Line has dependency type of Points. Needed because not all software have the same dependency relationship." +
            "See the wiki or look at existing Adapter implementations in the Toolkits for more info.")]
        protected virtual List<Type> GetDependencyTypes<T>()
        {
            Type type = typeof(T);

            if (m_DependencyTypes.ContainsKey(type))
                return m_DependencyTypes[type];

            else if (type.BaseType != null && m_DependencyTypes.ContainsKey(type.BaseType))
                return m_DependencyTypes[type.BaseType];

            else
            {
                foreach (Type interType in type.GetInterfaces())
                {
                    if (m_DependencyTypes.ContainsKey(interType))
                        return m_DependencyTypes[interType];
                }
            }

            return new List<Type>();
        }


    }
}