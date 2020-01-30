/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.Adapter
{
    [Description("Configurations specific for an Adapter Action (Push, Pull, etc)."+
        "\nConsider that your tookit might have a more specific implementation available. Try to look for [your toolkit name]ActionConfig.")]
    public class ActionConfig : IObject
    {
        // You can make your own ActionConfig for your Toolkit, e.g. SpeckleActionConfig.
        // To do so, inherit this class. You will so be able to pass it to any method (like the Push) that accepts ActionConfig.
        // Then to use it, you will need to cast the actionConfig parameter to your own ActionConfig type within each method.

        public bool WrapNonBHoMObjects { get; set; } = false;
        public bool AllowHashForComparing { get; set; } = false;
    }
}

