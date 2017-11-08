using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public static partial class Convert
    {
        public static string ToJson(this object obj)
        {
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return Convert.ToBson(obj).ToJson<BsonDocument>(jsonWriterSettings);  
        }

        /*******************************************/

        public static object FromJson(string json)
        {
            BsonDocument document;
            if (BsonDocument.TryParse(json, out document))
            {
                return Convert.FromBson(document);
            }
            else
            {
                return null;
            }
        }
    }
}
