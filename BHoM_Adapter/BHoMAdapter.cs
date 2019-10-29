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

        protected AdapterConfig Config { get; set; } = new AdapterConfig();


        /******************************************************/
        /**** Public Adapter Methods "Adapter ACTIONS"    *****/
        /******************************************************/
        /* These methods represent Actions that the Adapter can complete. 
           They are publicly available in the UI as individual components, e.g. in Grasshopper, under BHoM/Adapters tab. */

        // Performs the full CRUD if implemented, or calls the appropriate basic CRUD/Create method.
        public virtual List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            bool success = true;

            // Get the Push Type from the pushConfig.
            string pushType;
            object ptObj;
            if (config != null && config.TryGetValue("PushType", out ptObj))
                pushType = ptObj.ToString();
            else
                pushType = "Replace";

            // Wrap non-BHoM objects into a Custom BHoMObject to make them work as BHoMObjects.
            List<IObject> objectsToPush = Modify.WrapNonBHoMObjects(objects, Config, tag, config).ToList();

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

        // Calls the appropriate basic CRUD/Read method.
        public virtual IEnumerable<object> Pull(IRequest request, Dictionary<string, object> config = null)
        {
            // --------------------------------------------------------------------------------- //
            // *** Temporary retrocompatibility fix ***
            // If it's a FilterRequest, check if it should read IResults or Objects with that.
            // This should be replaced by an appropriate IResultRequest.
            FilterRequest filterReq = request as FilterRequest;
            if (filterReq != null)
                if (typeof(BH.oM.Common.IResult).IsAssignableFrom(filterReq.Type))
                    return ReadResults(filterReq);
            // --------------------------------------------------------------------------------- //

            if (request is IResultRequest)
                return ReadResults(request as dynamic);

            if (request is IRequest)
                return Read(request as dynamic);

            return new List<object>();
        }

        /***************************************************/

        // Performs a Pull and then a Push. Useful to move data between two different software without passing it through the UI.
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

        // Calls the basic CRUD/Delete method.
        public virtual int Delete(IRequest request, Dictionary<string, object> config = null)
        {
            // If the provided request is a FilterRequest, the specific wrapper method for FilterRequest is called.
            // For all other cases, Toolkits should implement specific IRequests and the related CRUD Wrapper method(s).

            FilterRequest filterReq = request as FilterRequest;
            if (filterReq != null)
                return Delete(filterReq);

            return Delete(request);
        }

        /***************************************************/

        // Used to send specific commands to the external software, if it supports it. It should be implemented (overridden) at the Toolkit level.
        public virtual bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, object> config = null)
        {
            return false;
        }


        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        // This field has to be implemented (overridden) at the Toolkit level for the CRUD method to work.
        // It tells the CRUD what kind of relationship (dependency) exists between the Types that must be Pushed.
        // E.g. A Line has dependency type of Points. See the wiki or look at existing Toolkit implementations for more info.
        protected virtual List<Type> DependencyTypes<T>()
        {
            return new List<Type>();
        }

        /***************************************************/

        protected virtual IEqualityComparer<T> Comparer<T>()
        {
            return EqualityComparer<T>.Default;
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
