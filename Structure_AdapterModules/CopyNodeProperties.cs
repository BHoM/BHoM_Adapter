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
using BH.oM.Adapter;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Structure;
using BH.Engine.Geometry;

namespace BH.Adapter.Modules
{
    public class CopyNodeProperties : ICopyPropertiesModule<Node>
    {
        [Description("Gets called during the Push, when you have overlapping nodes." +
            "Takes properties specified from the source Node and assigns them to the target Node.")]
        public void CopyProperties(Node target, Node source)
        {
            // If source is constrained and target is not, add source constraint to target
            if (source.Support != null)
            {
                if (target.Support == null)
                    target.Support = source.Support;
                else
                {
                    string desc1 = target.Support.Description();
                    string desc2 = source.Support.Description();
                    if(desc1 != desc2)
                        Engine.Base.Compute.RecordNote($"Node in position ({target.Position.X},{target.Position.Y},{target.Position.Z}) contains conflicting supports. Support {desc1} will be used on the node.");
                }
            }

            // If source has a defined orientation and target does not, add local orientation from the source
            if (source.Orientation != null)
            {
                if (target.Orientation == null)
                    target.Orientation = source.Orientation;
                else if(!source.Orientation.IsEqual(target.Orientation))
                    BH.Engine.Base.Compute.RecordNote($"Node in position ({target.Position.X}, {target.Position.Y}, {target.Position.Z}) contains conflicting orientaions. Orientation with Normal vector ({target.Orientation.Z.X}, {target.Orientation.Z.Y}, {target.Orientation.Z.Z}) will be used on the node.");
            }
        }
    }
}






