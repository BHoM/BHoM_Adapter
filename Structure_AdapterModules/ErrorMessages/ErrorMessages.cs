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

using System;
using BH.oM.Structure.Requests;
using BH.oM.Structure.Results;
using BH.oM.Structure.Loads;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base;

namespace BH.Adapter.Modules.Structure
{
    public static partial class ErrorMessages
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void ReadResultsError(Type resultType)
        {
            Type requestType = null;
            if (typeof(BarResult).IsAssignableFrom(resultType))
                requestType = typeof(BarResultRequest);
            else if (typeof(MeshResult).IsAssignableFrom(resultType) || typeof(MeshElementResult).IsAssignableFrom(resultType))
                requestType = typeof(MeshResultRequest);
            else if (typeof(StructuralGlobalResult).IsAssignableFrom(resultType))
                requestType = typeof(GlobalResultRequest);
            else if (typeof(NodeResult).IsAssignableFrom(resultType))
                requestType = typeof(NodeResultRequest);

            ReadResultsError(resultType, requestType);
        }

        /***************************************************/

        public static void ReadResultsError(Type resultType, Type requestType)
        {
            string message = resultType.Name + " cannot be extracted using a FilterRequest.";

            if (requestType != null)
                message += " Please instead make use of the " + requestType.Name + " that gives more options for the result extraction.";
            else
                message += " Please instead make use of the appropriate ResultRequest that gives more options for the result extraction.";

            Engine.Reflection.Compute.RecordError(message);
        }

        /***************************************************/

        public static void LoadsWithoutObejctIdsAssignedError(string loadName = "", Type loadType = null, Type objectType = null)
        {
            string loadDesc = loadType == null ? "load" : loadType.Name;
            string objDesc = objectType == null ? "objects" : objectType.Name + "s";
            char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
            string anOrA;
            if (vowels.Contains(loadDesc.ToLower().ToCharArray().First()))
                anOrA = "an";
            else
                anOrA = "a";

            string message;

            if (string.IsNullOrWhiteSpace(loadName))
                message = "The " + objDesc + " assigned to " + anOrA + " " + loadDesc;
            else
                message = "The " + objDesc + " assigned to the " + loadDesc + " named " + loadName;

            message += " being pushed does not contain any id-information and can not be identified by the software.\nPlease make sure all " + objDesc + " assigned to the " + loadDesc + " have been pulled from the package to ensure they contain the necessary information.";

            Engine.Reflection.Compute.RecordError(message);
        }

        /***************************************************/

        public static void LoadsWithoutObejctIdsAssignedError<T>(IElementLoad<T> load) where T : IBHoMObject
        {
            LoadsWithoutObejctIdsAssignedError(load.Name, load.GetType(), typeof(T));
        }

        /***************************************************/
    }
}
