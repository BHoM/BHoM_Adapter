using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BHC = BH.Adapter.Convert;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using BH.Adapter.Queries;
using BH.Adapter;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        public IEnumerable<object> Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null)
        {
            if (m_Readable)
                return PullJson();
            else
                return PullBson();
        }


        private IEnumerable<object> PullJson()
        {
            string[] json = File.ReadAllLines(m_FilePath);
            return json.Select(x => Convert.FromJson(x));
        }


        private IEnumerable<object> PullBson()
        {
            FileStream mongoReadStream = File.OpenRead(m_FilePath);
            var reader = new BsonBinaryReader(mongoReadStream);
            List<BsonDocument> readBson = BsonSerializer.Deserialize(reader, typeof(object)) as List<BsonDocument>;
            return readBson.Select(x => BsonSerializer.Deserialize(x, typeof(object)));
        }
    }
}
