using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Structural.Elements;
using BH.Adapter;
using BH.oM.Base;
using BH.oM.Geometry;

using BH.oM.Chrome.Views;
using BH.oM.Chrome.Dimensions;
using BH.oM.Chrome.Domains;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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
                    {"B", new Point(1,2,3) },
                    {"C", new Node() }
                }
            };

            Node node = new Node(new Point(1, 2, 3), "testNode");

            object input = bubbles;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            /*BsonDocument doc = input.ToBsonDocument();
            string json = doc.ToJson<BsonDocument>(jsonWriterSettings);
            object obj = BsonSerializer.Deserialize(doc, typeof(object));
            BubbleChart result = obj as BubbleChart;*/

            BsonDocument doc2 = BH.Adapter.Convert.ToBson(bubbles);
            string json2 = doc2.ToJson(jsonWriterSettings);
            object obj2 = BH.Adapter.Convert.FromBson(doc2);
            BubbleChart result2 = obj2 as BubbleChart;
        }


        private static void TestColourToBson()
        {
            System.Drawing.Color colour = System.Drawing.Color.Aquamarine;

            string direct = colour.ToJson();
            string bh = BH.Adapter.Convert.ToJson(colour);
        }

        private static void TestFileAdapter()
        {
            List<Node> nodes = new List<Node>
            {
                new Node {Point = new BH.oM.Geometry.Point(1, 2, 3), Name = "A"},
                new Node {Point = new BH.oM.Geometry.Point(4, 5, 6), Name = "B"},
                new Node {Point = new BH.oM.Geometry.Point(7, 8, 9), Name = "C"}
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
