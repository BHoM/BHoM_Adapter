/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapter
{
    [Description("Controls which type of export should be done by the Adapter `Push` action.")]
    public enum PushType //add error message if not picked up --> compliance check for this?
    {
        [Description("Calls all CRUD methods as appropriate.")]
        FullPush,
        [Description("Uses only the Create CRUD method to export the objects. This may create duplicates if the object already exists.")]
        CreateOnly,
        [Description("Same as FullPush, but does not update pre-existing objects.")]
        CreateNonExisting,
        [Description("Uses only the Update CRUD method to update the objects in the external software. All other objects in the model are left untouched.")]
        UpdateOnly,
        [Description("Attempt to Update the objects if possible, otherwise Create them. Deletion is not included in this type.")]
        UpdateOrCreateOnly,
        [Description("For all objects being Pushed, identifies their type, calls Delete to remove all of those types, then it Creates them.")]
        DeleteThenCreate,
        [Description("AdapterDefault - Picks the value hard-coded in the specific Adapter.")] // If this is chosen, then the m_AdapterSettings.DefaultPushType is picked
        AdapterDefault

    }
}


