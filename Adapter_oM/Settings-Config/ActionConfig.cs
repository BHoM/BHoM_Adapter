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
using BH.oM.Diffing;
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

        [Description("If true, the Push action wraps any non-BHoM type into a BH.oM.Adapter.ObjectWrapper, " +
           "allowing them to make use of the full Adapter workflow.")]
        public virtual bool WrapNonBHoMObjects { get; set; } = false;

        [Description("If true and if no specific EqualityComparer is found for the type, Diffing hashes are computed and used to compare the objects.")]
        public virtual bool AllowHashForComparing { get; set; } = false;

        [Description("Configurations for the Diffing hashing. Requires `AllowHashForComparing` to be set to true.")]
        public virtual DiffConfig DiffConfig { get; set; } = new DiffConfig();
    }
}

