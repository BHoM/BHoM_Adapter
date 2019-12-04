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
using BH.oM.Data;
using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Adapter
{
    public abstract partial class StructuralAnalysisAdapter
    {
        /***************************************************/
        /**** Push Support methods                      ****/
        /***************************************************/
        // These are support methods required by other methods in the Push process.

        [Description("Gets called during the Push. Takes properties specified from the source object and assigns them to the target object.")]
        protected override void ICopySpecificProperties(object target, object source)
        {
            (this as dynamic).PortTypeSpecificProperties(target as dynamic, source as dynamic);
            return;
        }

        [Description("Gets called during the Push. Takes properties specified from the source Node and assigns them to the target Node.")]
        protected virtual void PortTypeSpecificProperties(Node target, Node source)
        {
            // If source is constrained and target is not, add source constraint to target
            if (source.Support != null && target.Support == null)
                target.Support = source.Support;
        }
    }
}