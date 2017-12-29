using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        protected override IEnumerable<BHoMObject> Read(Type type, IList ids)
        {
            IEnumerable<BHoMObject> everything = m_Readable ? ReadJson() : ReadBson();

            if (type != null)
                everything = everything.Where(x => type.IsAssignableFrom(x.GetType()));

            if (ids != null)
            {
                HashSet<Guid> toDelete = new HashSet<Guid>(ids.Cast<Guid>());
                everything = everything.Where(x => !toDelete.Contains((Guid)x.CustomData[AdapterId]));
            }
                

            return everything;
        }


        private IEnumerable<BHoMObject> ReadJson()
        {
            string[] json = File.ReadAllLines(m_FilePath);
            return json.Select(x => Engine.Serialiser.Convert.FromJson(x) as BHoMObject);
        }


        private IEnumerable<BHoMObject> ReadBson()
        {
            FileStream mongoReadStream = File.OpenRead(m_FilePath);
            var reader = new BsonBinaryReader(mongoReadStream);
            List<BsonDocument> readBson = BsonSerializer.Deserialize(reader, typeof(object)) as List<BsonDocument>;
            return readBson.Select(x => BsonSerializer.Deserialize(x, typeof(object)) as BHoMObject);
        }
    }
}
