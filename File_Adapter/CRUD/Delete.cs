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

using BH.oM.Adapter;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        protected override int IDelete(Type type, IEnumerable<object> ids, ActionConfig actionConfig = null)
        {
            IEnumerable<BHoMObject> everything = m_isJSON ? ReadJson() : ReadBson();
            int initialCount = everything.Count();

            HashSet<Guid> toDelete = new HashSet<Guid>(ids.Cast<Guid>());

            everything = everything.Where(x => (type == null || !type.IsAssignableFrom(x.GetType())) && (toDelete.Contains((Guid)x.CustomData[AdapterIdName])));

            bool ok = true;
            if (m_isJSON)
                ok = CreateJson(everything, true);
            else
                ok = CreateBson(everything, true);

            if (!ok)
            {
                throw new FieldAccessException();
            }

            return initialCount - everything.Count();
        }
    }
}

