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
using BH.oM.Structure.Elements;
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

        [Description("Gets called during the Push. Takes properties specified from the source IBHoMObject and assigns them to the target IBHoMObject.")]
        protected virtual void PortBHoMObjectProperties<T>(T target, T source) where T : class, IBHoMObject
        {
            // Port tags from source to target
            foreach (string tag in source.Tags)
                target.Tags.Add(tag);

            // If target does not have name, port the source name
            if (string.IsNullOrWhiteSpace(target.Name))
                target.Name = source.Name;

            // Get id of the source and port it to the target
            IBHoMFragment source_adapterIdFragment = source.FindFragment<IBHoMFragment>(AdapterIdFragmentType);
            target.Fragments.AddOrReplace(source_adapterIdFragment);
        }
    }
}