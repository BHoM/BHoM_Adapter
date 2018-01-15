using System.Collections.Generic;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using MongoDB.Bson;
using MongoDB.Bson.IO;


namespace Adapter_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            
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
