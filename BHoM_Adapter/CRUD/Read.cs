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

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter
    {

        /***************************************************/
        /**** Protected CRUD Methods                    ****/
        /***************************************************/

        // This is the most basic Read method that returns objects depending on their Type and Id. 
        // It's needed for the CRUD method to work, and it must be implemented at the Toolkit level.
        protected abstract IEnumerable<IBHoMObject> Read(Type type, IList ids); //CRUD

        // This is the most basic Read method that returns `IResult`s depending on Type, Ids of the objects owning the IResult, Load Cases and Divisions.
        // If needed, it has to be implemented at the Toolkit level. Its implementation is facultative.
        protected virtual IEnumerable<BH.oM.Common.IResult> ReadResults(Type type, IList ids = null, IList cases = null, int divisions = 5)
        {
            return new List<BH.oM.Common.IResult>();
        } // CRUD

        // This method should be used to return a an IEnumerable<IResult>.
        // The default implementation here will return results only in case the request is a FilterRequest.
        // It should be implemented (overrided) at the Toolkit level in order for it to work with other implementations of IRequest.
        protected virtual IEnumerable<BH.oM.Common.IResult> ReadResults(IRequest request)
        {
            // Check if it is a filterRequest.
            FilterRequest filterReq = request as FilterRequest;
            if (filterReq == null)
                return ReadResults(filterReq);

            return new List<BH.oM.Common.IResult>();
        } //ADAPTER

        // This is a default implementation that returns an Enumerable<IResults> from a FilterRequest.
        // It can be overridden at the Toolkit level if needed.
        protected virtual IEnumerable<BH.oM.Common.IResult> ReadResults(FilterRequest filterReq)
        {
            if (!typeof(BH.oM.Common.IResult).IsAssignableFrom(filterReq.Type))
                return new List<BH.oM.Common.IResult>();

            // Extract all needed information from the FilterRequest
            IList cases, objectIds;
            int divisions;
            object caseObject, idObject, divObj;

            if (filterReq.Equalities.TryGetValue("Cases", out caseObject) && caseObject is IList)
                cases = caseObject as IList;
            else
                cases = null;

            if (filterReq.Equalities.TryGetValue("ObjectIds", out idObject) && idObject is IList)
                objectIds = idObject as IList;
            else
                objectIds = null;

            if (filterReq.Equalities.TryGetValue("Divisions", out divObj))
            {
                if (divObj is int)
                    divisions = (int)divObj;
                else if (!int.TryParse(divObj.ToString(), out divisions))
                    divisions = 5;
            }
            else
                divisions = 5;

            // Call the ReadResults basic method using all the extracted information from the FilterRequest
            List<BH.oM.Common.IResult> results = ReadResults(filterReq.Type, objectIds, cases, divisions).ToList();
            results.Sort();
            return results;
        } //ADAPTER

        // This method should be used to return a IEnumerable<IResultCollection>.
        // The default implementation here will return results only in case the request is a FilterRequest.
        // It should be implemented (overrided) at the Toolkit level in order for it to work with other implementations of IRequest.
        protected virtual IEnumerable<BH.oM.Common.IResultCollection> ReadResultCollection(IRequest request)
        {
            // Check if it is a filterRequest.
            FilterRequest filterReq = request as FilterRequest;
            if (filterReq == null)
                return ReadResultCollection(filterReq);

            return new List<BH.oM.Common.IResultCollection>();
        } //ADAPTER

        // This method should be used to return a IResultCollection.
        // It must be implemented (overrided) at the Toolkit level.
        protected virtual IEnumerable<BH.oM.Common.IResultCollection> ReadResultCollection(FilterRequest filterReq) 
        {
            if (!typeof(BH.oM.Common.IResultCollection).IsAssignableFrom(filterReq.Type))
                return new List<BH.oM.Common.IResultCollection>();

            return new List<BH.oM.Common.IResultCollection>();
        } //ADAPTER

        /***************************************************/
        /**** BHoM Adapter Methods                      ****/
        /***************************************************/

        protected IEnumerable<IBHoMObject> Read(Type type, string tag = "")
        {
            // Get the objects based on the ids
            IEnumerable<IBHoMObject> objects = Read(type, null as List<object>);

            // Filter by tag if any 
            if (tag == "")
                return objects;
            else
                return objects.Where(x => x.Tags.Contains(tag));
        }

        /***************************************************/

        public virtual IEnumerable<object> Read(IRequest request)
        {
            return new List<IBHoMObject>();
        }

        /***************************************************/

        public virtual IEnumerable<IBHoMObject> Read(FilterRequest request)
        {
            IList objectIds = null;
            object idObject;
            if (request.Equalities.TryGetValue("ObjectIds", out idObject) && idObject is IList)
                objectIds = idObject as IList;

            // Get the objects based on the ids
            IEnumerable<IBHoMObject> objects = Read(request.Type, objectIds);

            // Filter by tag if any 
            if (request.Tag == "")
                return objects;
            else
                return objects.Where(x => x.Tags.Contains(request.Tag));
        }
    }
}
