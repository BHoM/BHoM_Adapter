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

        protected AdapterConfig AdapterConfig { get; set; } = new AdapterConfig();



        /***************************************************/
        /**** Public Adapter Methods                    ****/
        /***************************************************/

        public virtual List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> pushConfig = null)
        {
            bool success = true;

            string pushType;

            object ptObj;
            if (pushConfig != null && pushConfig.TryGetValue("PushType", out ptObj))
                pushType = ptObj.ToString();
            else
                pushType = "Replace";

            List<IObject> objectsToPush = Modify.PrepareObjects(objects, AdapterConfig, tag, pushConfig);

            Type iBHoMObjectType = typeof(IBHoMObject);
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objectsToPush.GroupBy(x => x.GetType()))
            {
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                var list = miListObject.Invoke(typeGroup, new object[] { typeGroup });

                if (iBHoMObjectType.IsAssignableFrom(typeGroup.Key))
                {
                    if (pushType == "Replace")
                        success &= Replace(list as dynamic, tag);
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
            // Make sure this is a FilterQuery
            FilterRequest filter = request as FilterRequest;
            if (filter == null)
                return new List<object>();

            // Read the IBHoMObjects
            if (typeof(IBHoMObject).IsAssignableFrom(filter.Type))
                return Read(filter);
            
            // Read the IResults
            if (typeof(BH.oM.Common.IResult).IsAssignableFrom(filter.Type))
            {
                IList cases, objectIds;
                int divisions;
                object caseObject, idObject, divObj;

                if (filter.Equalities.TryGetValue("Cases", out caseObject) && caseObject is IList)
                    cases = caseObject as IList;
                else
                    cases = null;

                if (filter.Equalities.TryGetValue("ObjectIds", out idObject) && idObject is IList)
                    objectIds = idObject as IList;
                else
                    objectIds = null;

                if (filter.Equalities.TryGetValue("Divisions", out divObj))
                {
                    if (divObj is int)
                        divisions = (int)divObj;
                    else if (!int.TryParse(divObj.ToString(), out divisions))
                        divisions = 5;
                }
                else
                    divisions = 5;

                List<BH.oM.Common.IResult> results = ReadResults(filter.Type, objectIds, cases, divisions).ToList();
                results.Sort();
                return results;
            }

            // Read the IResultCollections
            if (typeof(BH.oM.Common.IResultCollection).IsAssignableFrom(filter.Type))
            {               
                List<BH.oM.Common.IResultCollection> results = ReadResults(filter).ToList();
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
        /**** Public Events                             ****/
        /***************************************************/

        public event EventHandler DataUpdated;

        /***************************************************/

        protected virtual void OnDataUpdated()
        {
            if (DataUpdated != null)
                DataUpdated.Invoke(this, new EventArgs());
        }


        /***************************************************/
        /**** Protected Abstract CRUD Methods           ****/
        /***************************************************/

        // Level 1 - Always required

        protected abstract bool Create<T>(IEnumerable<T> objects, bool replaceAll = false) where T : IObject;

        protected abstract IEnumerable<IBHoMObject> Read(Type type, IList ids);


        // Level 2 - Optional 

        public virtual int UpdateProperty(Type type, IEnumerable<object> ids, string property, object newValue)
        {
            return 0;
        }

        protected virtual int Delete(Type type, IEnumerable<object> ids)
        {
            return 0;
        }

        protected virtual IEnumerable<BH.oM.Common.IResult> ReadResults(Type type, IList ids = null, IList cases = null, int divisions = 5)
        {
            return new List<BH.oM.Common.IResult>();
        }

        protected virtual IEnumerable<BH.oM.Common.IResultCollection> ReadResults(FilterRequest request)
        {
            return new List<BH.oM.Common.IResultCollection>();
        }

        protected virtual bool UpdateObjects<T>(IEnumerable<T> objects) where T:IObject
        {
            Type objectType = typeof(T);
            if (AdapterConfig.UseAdapterId && typeof(IBHoMObject).IsAssignableFrom(objectType))
            {
                Delete(typeof(T), objects.Select(x => ((IBHoMObject)x).CustomData[AdapterId]));
            }
            return Create(objects);
        }


        // Optional Id query

        protected virtual object NextId(Type objectType, bool refresh = false)
        {
            return null;
        }


        /***************************************************/
        /**** Protected Type Methods                    ****/
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

    }
}
