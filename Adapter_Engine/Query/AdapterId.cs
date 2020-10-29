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
using BH.Engine.Base;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace BH.Engine.Adapter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static object AdapterId(this IBHoMObject bHoMObject, Type adapterIdFragmentType)
        {
            if (!typeof(IAdapterId).IsAssignableFrom(adapterIdFragmentType))
            {
                BH.Engine.Reflection.Compute.RecordError($"The `{adapterIdFragmentType.Name}` is not a valid `{typeof(IAdapterId).Name}`.");
                return null;
            }

            List<IAdapterId> fragmentList = bHoMObject.GetAllFragments(adapterIdFragmentType).OfType<IAdapterId>().ToList();

            if (fragmentList.Count != 0)
            {
                IEnumerable<object> ids = fragmentList.Select(f => f.Id);

                if (ids.Count() == 1)
                    return ids.First();
                else
                    return ids;
            }
            else
                return null;
        }

        public static T AdapterId<T>(this IBHoMObject bHoMObject, Type adapterIdFragmentType)
        {
            object id = AdapterId(bHoMObject, adapterIdFragmentType);

            if (id == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning($"AdapterId is null or missing for an object of type {bHoMObject.GetType().Name}.");
                return default(T);
            }

            if (id is T)
            {
                return (T)id;
            }
            try
            {
                return (T)Convert.ChangeType(id, typeof(T));
            }
            catch (InvalidCastException)
            {
                BH.Engine.Reflection.Compute.RecordError($"Found Id of type `{id.GetType().Name}` that cannot be converted to the requested type of `{typeof(T).Name}`.");
                return default(T);
            }
        }

    }
}
