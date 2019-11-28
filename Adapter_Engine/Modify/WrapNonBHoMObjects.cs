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
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Adapter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/ 
        /***************************************************/

        public static bool WrapNonBHoMObjects(IEnumerable<object> objects)
        {
            // This method is triggered when either:
            // 1) You have set to true `AdapterSettings.WrapNonBHoMObjects` in your Toolkit (-> all Push actions will call this), OR 
            // 2) When Pushing, you input an ActionConfig dictionary with "WrapNonBHoMObjects" set to true.

            objects = objects.Select(x => typeof(IBHoMObject).IsAssignableFrom(x.GetType()) ?
                x : new CustomObject() { CustomData = new Dictionary<string, object> { { x.GetType().Name + "_IBHoMObjectWrap", x } } } // Wraps non-BHoMObject in a custom BHoMObject
                );

            return true;
        }
    }
}