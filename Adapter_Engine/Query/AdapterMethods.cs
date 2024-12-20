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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace BH.Engine.Adapter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of MethodInfo with all methods contained in classes whose name ends with `Adapter`.")]
        public static List<MethodInfo> AdapterMethods()
        {
            // If the list exists already, return it
            if (m_AdapterMethodsList != null && m_AdapterMethodsList.Count > 0)
                return m_AdapterMethodsList;
            else
            {
                List<MethodInfo> allMethods = BH.Engine.Base.Query.AllMethodList().OfType<MethodInfo>().ToList();
                m_AdapterMethodsList = allMethods.Where(x => x.DeclaringType.Name.EndsWith("Adapter")).ToList();
                // (if we moved the IBHoMAdapter interface from the BHoM_Adapter down in the base BH.oM, we could test for inheritance instead of "EndsWith")
            }
            return m_AdapterMethodsList;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<MethodInfo> m_AdapterMethodsList = new List<MethodInfo>();
    }
}

