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
using BH.Engine.Base;
using BH.oM.Data.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter 
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public string AdapterId { get; set; }

        public Guid BHoM_Guid { get; set; } = Guid.NewGuid();

        public List<string> ErrorLog { get; set; } = new List<string>();

        protected AdapterConfig Config { get; set; } = new AdapterConfig();



        /***************************************************/
        /**** Public Adapter Methods                    ****/
        /***************************************************/

        public virtual List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> pushConfig = null)
        {
            bool success = true;

            // Get the Push Type from the pushConfig.
            string pushType;
            object ptObj;
            if (pushConfig != null && pushConfig.TryGetValue("PushType", out ptObj))
                pushType = ptObj.ToString();
            else
                pushType = "Replace";

            // Wrap non-BHoM objects into a Custom BHoMObject to make them work as BHoMObjects.
            List<IObject> objectsToPush = Modify.WrapNonBHoMObjects(objects, Config, tag, pushConfig).ToList();

            // Clone the objects for immutability in the UI. CloneBeforePush should always be true, except for very specific cases.
            objectsToPush = Config.CloneBeforePush ? objectsToPush.Select(x => x.DeepClone()).ToList() : objects.ToList();

            // Perform the actual Push.
            Type iBHoMObjectType = typeof(IBHoMObject);
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objectsToPush.GroupBy(x => x.GetType()))
            {
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                var list = miListObject.Invoke(typeGroup, new object[] { typeGroup });

                if (iBHoMObjectType.IsAssignableFrom(typeGroup.Key))
                {
                    if (pushType == "Replace")
                        success &= CRUD(list as dynamic, tag);
                    else if (pushType == "UpdateOnly")
                    {
                        success &= UpdateOnly(list as dynamic, tag);
                    }
                } 
            }

            return success ? objectsToPush : new List<IObject>();
        }

        /***************************************************/

        public virtual IEnumerable<object> Pull(IRequest request, Dictionary<string, object> config = null)
        {
            // Make sure this is a FilterRequest
            FilterRequest filterReq = request as FilterRequest;
            if (filterReq == null)
                return Read(request);

            // Read the IBHoMObjects
            if (typeof(IBHoMObject).IsAssignableFrom(filterReq.Type))
                return Read(filterReq);
            
            // Read the IResults
            if (typeof(BH.oM.Common.IResult).IsAssignableFrom(filterReq.Type))
            {
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

                List<BH.oM.Common.IResult> results = ReadResults(filterReq.Type, objectIds, cases, divisions).ToList();
                results.Sort();
                return results;
            }

            // Read the IResultCollections
            if (typeof(BH.oM.Common.IResultCollection).IsAssignableFrom(filterReq.Type))
            {               
                List<BH.oM.Common.IResultCollection> results = ReadResults(filterReq).ToList();
                return results;
            }

            return new List<object>();
        }

        /***************************************************/

        public virtual bool Move(BHoMAdapter to, IRequest request, Dictionary<string, object> pullConfig = null, Dictionary<string, object> pushConfig = null)
        {
            string tag = "";
            if (request is FilterRequest)
                tag = (request as FilterRequest).Tag;

            IEnumerable<object> objects = Pull(request, pullConfig);
            int count = objects.Count();
            return to.Push(objects.Cast<IObject>(), tag, pushConfig).Count() == count;
        }

        /***************************************************/

        public virtual int UpdateProperty(FilterRequest filter, string property, object newValue, Dictionary<string, object> config = null)
        {
            return PullUpdatePush(filter, property, newValue); 
        }

        /***************************************************/

        public virtual int Delete(FilterRequest filter, Dictionary<string, object> config = null)
        {
            return Delete(filter.Type, filter.Tag);
        }

        /***************************************************/

        public virtual bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, object> config = null)
        {
            return false;
        }



        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected virtual IEqualityComparer<T> Comparer<T>()
        {
            return EqualityComparer<T>.Default;
        }

        /***************************************************/

        protected virtual List<Type> DependencyTypes<T>()
        {
            return new List<Type>();
        }

        /***************************************************/

        protected virtual object NextId(Type objectType, bool refresh = false)
        {
            return null;
        }



        /***************************************************/
        /**** Public Events                             ****/
        /***************************************************/

        public event EventHandler DataUpdated;

        /***************************************************/

        protected virtual void OnDataUpdated()
        {
            if (DataUpdated != null)
                DataUpdated.Invoke(this, new EventArgs());
        }
     
    }
}
