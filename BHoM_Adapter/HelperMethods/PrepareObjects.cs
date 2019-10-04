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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<IObject> PrepareObjects(IEnumerable<IObject> objects, AdapterConfig adapterConfig, string tag = "", Dictionary<string, object> pushConfig = null)
        {
            bool wrapNonBHoMObjects = adapterConfig.WrapNonBHoMObjects;
            object wrapNonBHoMObjValue;

            if (pushConfig != null && pushConfig.TryGetValue("WrapNonBHoMObjects", out wrapNonBHoMObjValue)) wrapNonBHoMObjects = (bool)wrapNonBHoMObjValue;


            List<IObject> objectsToPush = objects.Select(x =>
            {
                if (x is BHoMObject)
                    return ((BHoMObject)x).DeepClone(); // Deep clone the object for immutability in the UI

                if (wrapNonBHoMObjects)
                    return new CustomObject() { CustomData = new Dictionary<string, object> { { "WrappedObject", x } } }; // Wraps non-IBHoMObject in a custom BHoMObject

                // If none of the above applies, return the non-BHoMObject untouched
                return x;
            })
            .ToList(); //ToList() necessary for the return collection to function properly for cloned objects

            return objectsToPush;
        }
    }
}