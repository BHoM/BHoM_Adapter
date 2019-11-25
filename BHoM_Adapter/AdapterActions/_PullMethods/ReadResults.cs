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
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using BH.oM.Data.Requests;
using BH.oM.Reflection.Attributes;
using BH.oM.Common;

namespace BH.Adapter
{

    /***************************************************/
    /**** Pull Methods                              ****/
    /***************************************************/
    // These methods call the CRUD methods as needed from the Pull perspective.


    public abstract partial class BHoMAdapter
    {
        // Called directly by the Pull. Returns structural results. 
        // `Type` and `Ids` are the ones of the objects owning the IResult to be retrieved.
        // If needed, it has to be implemented at the Toolkit level. Its implementation is facultative.
        protected virtual IEnumerable<BH.oM.Common.IResult> ReadResults(Type type, IList ids = null, IList cases = null, int divisions = 5)
        {
            Engine.Reflection.Compute.RecordError($"ReadResults for {type.Name} is not implemented in {(this as dynamic).GetType().Name}.");
            return new List<BH.oM.Common.IResult>();
        }

        /***************************************************/
        /**** Wrapper methods                           ****/
        /***************************************************/

        /******* IRequest Wrapper methods *******/
        /* These methods have to be implemented if the Toolkit needs to support the Read for any generic IRequest. */
        protected virtual IEnumerable<IResult> ReadResults(IRequest request)
        {
            // The implementation must:
            // 1. extract all the needed info from the IRequest
            // 2. return a call to the Basic Method ReadResult() with the extracted info.
            Engine.Reflection.Compute.RecordError($"ReadResults for {request.GetType().Name} is not implemented in {(this as dynamic).GetType().Name}.");
            return new List<BH.oM.Common.IResult>();
        }
   
        /******* Additional Wrapper methods *******/
        /* These methods contain some additional logic to avoid boilerplate.
           If needed, they can be overriden at the Toolkit level, but the new implementation must always call the appropriate Basic Method. */

        protected virtual IEnumerable<IResult> ReadResults(FilterRequest filterRequest)
        {
            List<IResult> results = new List<IResult>();

            // Read the IResults
            if (typeof(BH.oM.Common.IResult).IsAssignableFrom(filterRequest.Type))
            {
                IList cases, objectIds;
                int divisions;
                object caseObject, idObject, divObj;

                if (filterRequest.Equalities.TryGetValue("Cases", out caseObject) && caseObject is IList)
                    cases = caseObject as IList;
                else
                    cases = null;

                if (filterRequest.Equalities.TryGetValue("ObjectIds", out idObject) && idObject is IList)
                    objectIds = idObject as IList;
                else
                    objectIds = null;

                if (filterRequest.Equalities.TryGetValue("Divisions", out divObj))
                {
                    if (divObj is int)
                        divisions = (int)divObj;
                    else if (!int.TryParse(divObj.ToString(), out divisions))
                        divisions = 5;
                }
                else
                    divisions = 5;

                results = ReadResults(filterRequest.Type, objectIds, cases, divisions).ToList();
                results.Sort();
            }

            return results;
        }
    }
}
