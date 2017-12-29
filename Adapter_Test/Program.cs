using System.Collections.Generic;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.oM.Chrome.Views;
using BH.oM.Chrome.Dimensions;
using BH.oM.Chrome.Domains;
using MongoDB.Bson;
using MongoDB.Bson.IO;


namespace Adapter_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBsonConversion();
        }


        private static void TestBsonConversion()
        {
            BubbleChart bubbles = new BubbleChart
            {
                Parent = "body",
                Id = "bubbles",
                XDim = new AxisDimension()
                {
                    Property = "X",
                    InDomain = new NumberDomain(0, 10)
                },
                YDim = new AxisDimension("Y"),
                ColourDim = new ColourDimension
                {
                    OutDomain = new ColourDomain
                    {
                        Values = new List<System.Drawing.Color>
                        {
                            System.Drawing.Color.Aqua,
                            System.Drawing.Color.Red,
                            System.Drawing.Color.Green
                        }
                    }
                },
                Tags = new HashSet<string> { "Tag1", "Tag2" },
                CustomData = new Dictionary<string, object>
                {
                    {"A", 1 },
                    {"B", new Point { X = 1, Y = 2, Z = 3 } },
                    {"C", new Node() }
                }
            };

            Node node = new Node { Position = new Point { X = 1, Y = 2, Z = 3 }, Name = "testNode" };

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["Views"] = new List<View> { bubbles };
            dic["Data'"] = new List<object> { node };
            dic["Tag"] = "A";

            object input = dic;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            /*BsonDocument doc = input.ToBsonDocument();
            string json = doc.ToJson<BsonDocument>(jsonWriterSettings);
            object obj = BsonSerializer.Deserialize(doc, typeof(object));
            BubbleChart result = obj as BubbleChart;*/

            BsonDocument doc2 = BH.Engine.Serialiser.Convert.ToBson(input);
            string json2 = doc2.ToJson(jsonWriterSettings);
            //object obj2 = BH.Adapter.Convert.FromBson(doc2);
            //BubbleChart result2 = obj2 as BubbleChart;
        }


        private static void TestColourToBson()
        {
            System.Drawing.Color colour = System.Drawing.Color.Aquamarine;

            string direct = colour.ToJson();
            string bh = BH.Engine.Serialiser.Convert.ToJson(colour);
        }

        private static void TestFileAdapter()
        {
            List<Node> nodes = new List<Node>
            {
                new Node {Position = new Point { X = 1, Y = 2, Z = 3 }, Name = "A"},
                new Node {Position = new Point { X = 4, Y = 5, Z = 6 }, Name = "B"},
                new Node {Position = new Point { X = 7, Y = 8, Z = 9 }, Name = "C"}
            };

            List<Bar> bars = new List<Bar>
            {
                new Bar {StartNode = nodes[0], EndNode = nodes[1] },
                new Bar {StartNode = nodes[1], EndNode = nodes[2] }
            };


        }

        //private static void TestTags()
        //{
        //    Node node = new Node();

        //    node.Name = "Name hej";
        //    node.Tags.Add("tag1");
        //    node.Tags.Add("tag2");

        //    string nameAndTags = node.GetNameAndTagString();

        //    string name;
        //    HashSet<string> tags = nameAndTags.GetTagsFromString(out name);
        //}
    }
}
