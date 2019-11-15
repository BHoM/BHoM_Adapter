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

using BH.oM.Base;
using BH.oM.Structure.Elements;

namespace BH.Engine.Adapter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool MapSpecialProperties(this IBHoMObject target, IBHoMObject source, string adapterKey) 
        {
            // Add tags from source to target
            foreach (string tag in source.Tags)
                target.Tags.Add(tag);

            // Map Properties Special Properties
            _MapSpecialProperties(target as dynamic, source as dynamic);

            // Check for id of the source and apply to the target
            bool found = true;
            object id;
            if (source.CustomData.TryGetValue(adapterKey, out id))
            {
                target.CustomData[adapterKey] = id;
                found = true;
            }
            return found;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void _MapSpecialProperties(IBHoMObject target, IBHoMObject source)
        {
        }

        /***************************************************/

        private static void _MapSpecialProperties(Node target, Node source)
        {
            //Check if the source is constraint och taget not, if so add source constraint to target
            if (source.Support != null && target.Support == null)
                target.Support = source.Support;

            //If target does not have name, take sources name //TODO: could that be done for all BHoM objects?
            if (string.IsNullOrWhiteSpace(target.Name))
                target.Name = source.Name;
        }

        /***************************************************/
    }
}
