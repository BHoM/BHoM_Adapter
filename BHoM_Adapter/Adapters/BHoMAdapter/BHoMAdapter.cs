using BH.Adapter.Queries;
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public abstract partial class BHoMAdapter : IAdapter
    {
        /***************************************************/
        /**** Public Properties                         ****/
        /***************************************************/

        public string AdapterId { get; set; }

        public List<string> ErrorLog { get; set; } = new List<string>();


        /***************************************************/
        /**** Public Adapter Methods                    ****/
        /***************************************************/

        public virtual bool Push(IEnumerable<object> objects, string tag = "", Dictionary<string, string> config = null)
        {
            return PushByType(objects, tag, config);
        }

        /***************************************************/

        public virtual IEnumerable<object> Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null)
        {
            // Make sure there is at least one query
            if (query.Count() == 0)
                return new List<object>();

            // Make sure this is a FilterQuery
            FilterQuery filter = query.First() as FilterQuery;
            if (filter == null)
                return new List<object>();

            // Read the objects
            return Read(filter.Type, filter.Tag);
        }

        /***************************************************/

        public virtual int Update(FilterQuery filter, string property, object newValue, Dictionary<string, string> config = null)
        {
            return PullUpdatePush(filter, property, newValue, config); 
        }

        /***************************************************/

        public virtual int Delete(FilterQuery filter, Dictionary<string, string> config = null)
        {
            return Delete(filter.Type, filter.Tag);
        }

        /***************************************************/

        public virtual bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, string> config = null)
        {
            return false;
        }


        /***************************************************/
        /**** Protected Abstract CRUD Methods           ****/
        /***************************************************/

        protected abstract bool Create(IEnumerable<object> objects);

        protected abstract IEnumerable<BHoMObject> Read(Type type, string tag = "");

        protected abstract bool UpdateTags(IEnumerable<object> objects);

        protected abstract int Delete(Type type, string tag = "");


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


        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected virtual FilterQuery GenerateFilterQuery<T>(IEnumerable<T> objects) where T : BH.oM.Base.BHoMObject
        {
            FilterQuery filter = new FilterQuery();
            filter.Type = typeof(T);
            filter.Equalities[AdapterId] = objects.Select(x => x.CustomData[AdapterId].ToString()).ToList();
            return filter;
        }

    }
}
