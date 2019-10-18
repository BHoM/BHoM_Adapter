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

using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.Engine.Reflection;
using BH.Engine.Data;
using System;

namespace BH.Adapter
{
    // NOTE: CRUD folder methods
    // All methods in the CRUD folder are used as "back-end" methods by the Adapter itself.
    // They are meant to be implemented at the Toolkit level.
    public abstract partial class BHoMAdapter
    {

        /***************************************************/
        /**** Basic Methods                             ****/
        /***************************************************/
        // These methods provide the basic functionalities for the CRUD to work.

        // This method is different from the normal Update method: it only updates a single property of an object without re-writing the whole object.
        // Its main usage is to update the Tags of an IBHoMObject in the CRUD method.
        // It needs to be implemented at the Toolkit level for the full CRUD to work.
        public virtual int UpdateProperty(Type type, IEnumerable<object> ids, string property, object newValue)
        {
            return 0;
        }
    }
}
