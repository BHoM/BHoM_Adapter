using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
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
        /**** Private Methods                   ****/
        /*******************************************/

        static Convert()
        {
            RegisterTypes();
        }

        /*******************************************/

        private static void RegisterTypes()
        {
            try
            {
                BsonSerializer.RegisterSerializer(typeof(object), new ObjectSerializer());
                BsonSerializer.RegisterSerializer(typeof(List<object>), new ObjectListSerializer());
                BsonSerializer.RegisterSerializer(typeof(CustomObject), new CustomObjectSerializer());
            }
            catch(Exception)
            {
                Console.WriteLine("Problem with initialisation of the Bson Serializer");
            }

            foreach (Type type in BH.Engine.Reflection.Query.GetBHoMTypeList())
            {
                if (!type.IsGenericType)
                    BsonClassMap.LookupClassMap(type);
            }

            m_TypesRegistered = true;
        }


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private static bool m_TypesRegistered = false;
    }


    /*******************************************/
    /**** Bson Serializers                  ****/
    /*******************************************/

    public class CustomObjectSerializer : SerializerBase<CustomObject>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CustomObject value)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(value.CustomData);
            data["BHoM_Guid"] = value.BHoM_Guid;

            context.Writer.WriteStartDocument();

            if (value.Tags.Count > 0)
            {
                context.Writer.WriteName("Name");
                BsonSerializer.Serialize(context.Writer, value.Name);
            }

            if (value.Name.Length > 0)
            {
                context.Writer.WriteName("Tags");
                context.Writer.WriteStartArray();
                foreach (string tag in value.Tags)
                    context.Writer.WriteString(tag);
                context.Writer.WriteEndArray();
            }

            foreach (KeyValuePair<string, object> kvp in data)
            {
                context.Writer.WriteName(kvp.Key);
                BsonSerializer.Serialize(context.Writer, kvp.Value);
            }
            context.Writer.WriteEndDocument();
        }
    }

    /*******************************************/

    public class ObjectSerializer : MongoDB.Bson.Serialization.Serializers.ObjectSerializer
    {
        CustomObjectSerializer customObjectSerializer = new CustomObjectSerializer();
        ObjectListSerializer listSerializer = new ObjectListSerializer();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value is CustomObject)
                customObjectSerializer.Serialize(context, args, value as CustomObject);
            else if (value is List<object>)
                listSerializer.Serialize(context, args, value as List<object>);
            else
                base.Serialize(context, args, value);
        }
    }

    /*******************************************/

    public class ObjectListSerializer : SerializerBase<List<object>>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, List<object> value)
        {
            context.Writer.WriteStartArray();
            foreach(object item in value)
            {
                BsonSerializer.Serialize(context.Writer, item);
            }
            context.Writer.WriteEndArray();
        }
    }

}
