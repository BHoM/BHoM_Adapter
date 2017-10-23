using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
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
            else
                return obj.ToBsonDocument();
        }

        /*******************************************/

        public static object FromBson(BsonDocument bson)
        {
            /*Type type = BH.Engine.Reflection.Create.Type(bson.GetValue("_t").AsString);
            if (!BsonClassMap.IsClassMapRegistered(type))
                BsonClassMap.RegisterClassMap(new BsonClassMap(type));*/

            if (!m_TypesRegistered)
                RegisterTypes();

            bson.Remove("_id");
            return BsonSerializer.Deserialize(bson, typeof(object));
        }

        /*******************************************/

        private static void RegisterTypes()
        {
            foreach (Type type in BH.Engine.Reflection.Create.TypeList())
            {
                if (!type.IsGenericType)
                {
                    BsonClassMap.LookupClassMap(type);
                    /*if (!BsonClassMap.IsClassMapRegistered(type))
                        BsonClassMap.RegisterClassMap(new BsonClassMap(type));*/
                }
            }

            m_TypesRegistered = true;
        }


        private static bool m_TypesRegistered = false;
    }
}
