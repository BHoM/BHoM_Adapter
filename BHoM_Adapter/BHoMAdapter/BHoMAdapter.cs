﻿using BH.Adapter.Queries;
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter 
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public string AdapterId { get; set; }

        public List<string> ErrorLog { get; set; } = new List<string>();

        protected AdapterConfig Config { get; set; } = new AdapterConfig();



        /***************************************************/
        /**** Public Adapter Methods                    ****/
        /***************************************************/

        public virtual bool Push(IEnumerable<BHoMObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            bool success = true;
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objects.GroupBy(x => x.GetType()))
            {            
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key});

                var list = miListObject.Invoke(typeGroup, new object[] {typeGroup});

                success &= Replace(list as dynamic, tag);
            }
                


            return success;
        }

        /***************************************************/

        public virtual IEnumerable<object> Pull(IQuery query, Dictionary<string, object> config = null)
        {
            // Make sure this is a FilterQuery
            FilterQuery filter = query as FilterQuery;
            if (filter == null)
                return new List<object>();

            // Read the objects
            return Read(filter.Type, filter.Tag);
        }

        /***************************************************/

        public virtual int UpdateProperty(FilterQuery filter, string property, object newValue, Dictionary<string, object> config = null)
        {
            return PullUpdatePush(filter, property, newValue); 
        }

        /***************************************************/

        public virtual int Delete(FilterQuery filter, Dictionary<string, object> config = null)
        {
            return Delete(filter.Type, filter.Tag);
        }

        /***************************************************/

        public virtual bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, object> config = null)
        {
            return false;
        }


        /***************************************************/
        /**** Protected Abstract CRUD Methods           ****/
        /***************************************************/

        // Level 1 - Always required

        protected abstract bool Create<T>(IEnumerable<T> objects, bool replaceAll = false) where T : BHoMObject;

        protected abstract IEnumerable<BHoMObject> Read(Type type, IList ids);


        // Level 2 - Optional 

        public virtual int UpdateProperty(Type type, IEnumerable<object> ids, string property, object newValue)
        {
            return 0;
        }

        protected virtual int Delete(Type type, IEnumerable<object> ids)
        {
            return 0;
        }


        // Optional Id query

        protected virtual object GetNextId(Type objectType, bool refresh = false)
        {
            return null;
        }


        /***************************************************/
        /**** Protected Type Methods                    ****/
        /***************************************************/

        protected virtual IEqualityComparer<T> GetComparer<T>()
        {
            return EqualityComparer<T>.Default;
        }

        /***************************************************/

        protected virtual List<Type> GetDependencyTypes<T>()
        {
            return new List<Type>();
        }

    }
}
