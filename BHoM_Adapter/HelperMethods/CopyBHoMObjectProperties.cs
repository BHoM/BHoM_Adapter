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

using BH.oM.Adapter;
using BH.oM.Base;
using BH.Engine.Adapter;
using BH.Engine.Base;
using BH.oM.Data.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {
        [Description("Gets called during the Push. Takes properties specified from the source IBHoMObject and assigns them to the target IBHoMObject.")]
        public void CopyBHoMObjectProperties<T>(T target, T source) where T : IBHoMObject
        {
            // Port tags from source to target
            foreach (string tag in source.Tags)
                target.Tags.Add(tag);

            // If target does not have name, port the source name
            if (string.IsNullOrWhiteSpace(target.Name))
                target.Name = source.Name;

            // Get id of the source and port it to the target
            if (source.HasAdapterIdFragment(AdapterIdFragmentType))
                target.SetAdapterId(AdapterIdFragmentType, source.AdapterIds(AdapterIdFragmentType));
        }
    }
}




