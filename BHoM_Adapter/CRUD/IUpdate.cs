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

using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BH.Engine.Adapter;

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
        /* These methods provide the basic functionalities for the CRUD to work. */

        // Unlike the Create, Delete and Read, this method already exposes a simple implementation: it calls Delete and then Create.
        // It can be overridden at the Toolkit level if a more appropriate implementation is required.
        protected virtual bool IUpdate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null) where T : IBHoMObject
        {
            BH.Engine.Base.Compute.RecordNote($"The default IUpdate method for {typeof(T).Name} has been invoked by the Push.\n" +
                $"This method calls IDelete and then ICreate for the specified objects.");

            Type objectType = typeof(T);
            if (m_AdapterSettings.UseAdapterId && typeof(IBHoMObject).IsAssignableFrom(objectType))
            {
                IDelete(typeof(T), objects.Select(x => ((IBHoMObject)x).AdapterIds(AdapterIdFragmentType)), actionConfig);
            }
            return ICreate(objects, actionConfig);
        }

        // UpdateTag should be implemented to allow for the update of the objects' tags without re-writing the whole objects.
        // It needs to be implemented at the Toolkit level for the full CRUD to work.
        protected virtual int IUpdateTags(Type type, IEnumerable<object> ids, IEnumerable<HashSet<string>> newTags, ActionConfig actionConfig = null)
        {
            return 0;
        }
    }
}






