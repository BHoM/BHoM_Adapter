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

using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System;

namespace BH.Engine.Adapter
{
    public static partial class Modify
    {
        public static void AddAdapterId<T>(this IBHoMObject obj, IAdapterIdFragment<T> adapterIdFragment)
        {
            obj.Fragments.AddOrReplace(adapterIdFragment);
        }

        public static Type CurrentAdapterIdFragmentType { get; set; } //would need to be always updated before calling the below

        public static void AddAdapterId<T>(this IBHoMObject obj, T id) 
        {
            var idFragm = CreateAdapterIdFragment(CurrentAdapterIdFragmentType, id);

            obj.Fragments.AddOrReplace(idFragm);
        }

        public static IAdapterIdFragment<T> CreateAdapterIdFragment<T>(Type AdapterIdFragmentType, T id)
        {
            return (IAdapterIdFragment<T>)Activator.CreateInstance(AdapterIdFragmentType, id);
        }
    }
}
