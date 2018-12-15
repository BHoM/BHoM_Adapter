using BH.oM.Base;
using BH.oM.Base.CRUD;
using BH.oM.DataManipulation.Queries;
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



        /***************************************************/
        /**** Public Adapter Methods                    ****/
        /***************************************************/

        public virtual List<IObject> Push(IEnumerable<IObject> objects, string tag = "", CrudConfig config = null)
        {
            bool success = true;

            if (config == null)
                config = new CrudConfig();

            List<IObject> objectsToPush = Config.CloneBeforePush ? objects.Select(x => x is BHoMObject ? ((BHoMObject)x).GetShallowClone() : x).ToList() : objects.ToList(); //ToList() necessary for the return collection to function properly for cloned objects

            Type iBHoMObjectType = typeof(IBHoMObject);
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objectsToPush.GroupBy(x => x.GetType()))
            {
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                var list = miListObject.Invoke(typeGroup, new object[] { typeGroup });

                if (iBHoMObjectType.IsAssignableFrom(typeGroup.Key))
                {
                    switch (config.ActionType)
                    {
                        case PushActionType.Replace:
                            success &= Replace(list as dynamic, tag, config);
                            break;
                        case PushActionType.UpdateOnly:
                            success &= UpdateOnly(list as dynamic, tag, config);
                            break;
                    }
                }
            }

            return success ? objectsToPush : new List<IObject>();
        }

        /***************************************************/

        public virtual IEnumerable<object> Pull(IQuery query, CrudConfig config = null)
        {
            // Make sure this is a FilterQuery
            FilterQuery filter = query as FilterQuery;
            if (filter == null)
                return new List<object>();

            // Read the IBHoMObjects
            if (typeof(IBHoMObject).IsAssignableFrom(filter.Type))
                return Read(filter, config);
            
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

                List<BH.oM.Common.IResult> results = ReadResults(filter.Type, objectIds, cases, divisions, config).ToList();
                results.Sort();
                return results;
            }

            // Read the IResultCollections
            if (typeof(BH.oM.Common.IResultCollection).IsAssignableFrom(filter.Type))
            {               
                List<BH.oM.Common.IResultCollection> results = ReadResults(filter, config).ToList();
                return results;
            }

            return new List<object>();
        }

        /***************************************************/

        public virtual bool PullTo(BHoMAdapter to, IQuery query, CrudConfig sourceConfig = null, CrudConfig targetConfig = null)
        {
            string tag = "";
            if (query is FilterQuery)
                tag = (query as FilterQuery).Tag;

            IEnumerable<object> objects = this.Pull(query, sourceConfig);
            int count = objects.Count();
            return to.Push(objects.Cast<IObject>(), tag, targetConfig).Count() == count;
        }

        /***************************************************/

        public virtual int UpdateProperty(FilterQuery filter, string property, object newValue, CrudConfig config = null)
        {
            return PullUpdatePush(filter, property, newValue, config); 
        }

        /***************************************************/

        public virtual int Delete(FilterQuery filter, CrudConfig config = null)
        {
            return Delete(filter.Type, filter.Tag, config);
        }

        /***************************************************/

        public virtual bool Execute(string command, IExecuteParams parameters = null, CrudConfig config = null)
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

        /***************************************************/
        // Level 1 - Always required

        protected abstract bool Create<T>(IEnumerable<T> objects, bool replaceAll = false, CrudConfig config = null) where T : IObject;

        /***************************************************/

        protected abstract IEnumerable<IBHoMObject> Read(Type type, IList ids, CrudConfig config = null);

        /***************************************************/
        // Level 2 - Optional 

        public virtual int UpdateProperty(Type type, IEnumerable<object> ids, string property, object newValue, CrudConfig config = null)
        {
            return 0;
        }

        /***************************************************/

        protected virtual int Delete(Type type, IEnumerable<object> ids, CrudConfig config = null)
        {
            return 0;
        }

        /***************************************************/

        protected virtual IEnumerable<BH.oM.Common.IResult> ReadResults(Type type, IList ids = null, IList cases = null, int divisions = 5, CrudConfig config = null)
        {
            return new List<BH.oM.Common.IResult>();
        }

        /***************************************************/

        protected virtual IEnumerable<BH.oM.Common.IResultCollection> ReadResults(FilterQuery query, CrudConfig config = null)
        {
            return new List<BH.oM.Common.IResultCollection>();
        }

        /***************************************************/

        protected virtual bool UpdateObjects<T>(IEnumerable<T> objects, CrudConfig config = null) where T:IObject
        {
            Type objectType = typeof(T);
            if (Config.UseAdapterId && typeof(IBHoMObject).IsAssignableFrom(objectType))
            {
                Delete(typeof(T), objects.Select(x => ((IBHoMObject)x).CustomData[AdapterId]), config);
            }
            return Create(objects, false, config);
        }

        /***************************************************/
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
