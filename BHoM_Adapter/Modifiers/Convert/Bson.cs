using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BH.Adapter
{
    public static partial class Convert
    {
        public static BsonDocument ToBson(this object obj)
        {
            if (obj is string)
            {
                BsonDocument document;
                BsonDocument.TryParse(obj as string, out document);
                return document;
            }
            else if (obj is CustomObject)
            {
                CustomObject co = obj as CustomObject;
                BsonDocument doc = co.CustomData.ToBsonDocument();
                if (co.Name.Length > 0)
                    doc["Name"] = co.Name;
                if (co.Tags.Count > 0)
                    doc["Tags"] = new BsonArray(co.Tags);
                return doc;
            }
            else
                return obj.ToBsonDocument();
        }

        /*******************************************/

        public static object FromBson(BsonDocument bson)
        {
            if (!m_TypesRegistered)
                RegisterTypes();

            bson.Remove("_id");
            object obj = BsonSerializer.Deserialize(bson, typeof(object));
            if (obj is ExpandoObject)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>(obj as ExpandoObject);
                CustomObject co = new CustomObject();
                if (dic.ContainsKey("Name"))
                {
                    co.Name = dic["Name"] as string;
                    dic.Remove("Name");
                }   
                if (dic.ContainsKey("Tags"))
                {
                    co.Tags = new HashSet<string>(((List<object>)dic["Tags"]).Cast<string>());
                    dic.Remove("Tags");
                }
                co.CustomData = dic;
                return co;
            }
            else
                return obj;
        }

        /*******************************************/

        private static void RegisterTypes()
        {
            foreach (Type type in BH.Engine.Reflection.Create.TypeList())
            {
                if (!type.IsGenericType)
                    BsonClassMap.LookupClassMap(type);
            }

            m_TypesRegistered = true;
        }


        private static bool m_TypesRegistered = false;
    }
}
