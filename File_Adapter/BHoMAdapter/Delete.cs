using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        protected override int Delete(Type type, string tag = "")
        {
            IEnumerable<BHoMObject> everything = m_Readable ? ReadJson() : ReadBson();
            int initialCount = everything.Count();

            everything = everything.Where(x => (type == null || !type.IsAssignableFrom(x.GetType())) && (tag.Length == 0 || !x.Tags.Contains(tag)));

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
    }
}
