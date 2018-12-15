using BH.oM.Base;
using BH.oM.Base.CRUD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        protected override int Delete(Type type, IEnumerable<object> ids, CrudConfig config = null)
        {
            IEnumerable<BHoMObject> everything = m_Readable ? ReadJson() : ReadBson();
            int initialCount = everything.Count();

            HashSet<Guid> toDelete = new HashSet<Guid>(ids.Cast<Guid>());

            everything = everything.Where(x => (type == null || !type.IsAssignableFrom(x.GetType())) && (toDelete.Contains((Guid)x.CustomData[AdapterId])));

            bool ok = true;
            if (m_Readable)
                ok = CreateJson(everything, true);
            else
                ok = CreateBson(everything, true);

            if (!ok)
            {
                throw new FieldAccessException();
            }

            return initialCount - everything.Count();
        }

        /***************************************************/
    }
}
