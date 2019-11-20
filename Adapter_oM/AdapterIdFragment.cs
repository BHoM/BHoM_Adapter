/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapter
{
    public class AdaptersIdFragment : IBHoMFragment
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        [Description("The `string` key is the 'Software Name' e.g. Speckle; " +
            "the `object` value is the id assigned to a specific IBHoMObject." +
            "We use the general type `object` because ids come in different forms (int, string, GUID, etc.).")]
        public Dictionary<string, object> AdaptersId { get; set; } = new Dictionary<string, object>();
        // This can store contemporarily, for a same IBHoMObject: 
        // the 'SAP' id --> 56 , the 'Unreal' id --> "14", the 'Speckle' id --> {xxxaaaggg}, etc.
    }
}
