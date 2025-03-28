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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace BH.Engine.Adapter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void SetAdapterId<T>(this IBHoMObject bHoMObject, Type adapterIdFragmentType, T id)
        {
            if(bHoMObject == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set an adapter ID to a null object.");
                return;
            }

            // Check if the specified `adapterIdFragmentType` is effectively an `IAdapterId`.
            if (!typeof(IAdapterId).IsAssignableFrom(adapterIdFragmentType))
            {
                BH.Engine.Base.Compute.RecordError($"The `{adapterIdFragmentType.Name}` is not a valid `{typeof(IAdapterId).Name}`.");
                return;
            }

            // If the input `id` is already an instance of the specified `adapterIdFragmentType`, simply add it to the fragments.
            if (id != null && id.GetType() == adapterIdFragmentType)
            {
                bHoMObject.Fragments.AddOrReplace(id as IFragment);
                return;
            }

            // Create an instance of the specified `adapterIdFragmentType`, set its `Id` property, then add it to the fragments.
            IAdapterId newAdapterIdFragment = (IAdapterId)Activator.CreateInstance(adapterIdFragmentType);

            newAdapterIdFragment.Id = id;

            bHoMObject.Fragments.AddOrReplace(newAdapterIdFragment);
        }

        public static void SetAdapterId(this IBHoMObject bHoMObject, IAdapterId id)
        {
            if (bHoMObject == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set an adapter ID to a null object.");
                return;
            }

            bHoMObject.Fragments.AddOrReplace(id as IFragment);
        }
    }
}





