using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Structural.Elements;
using BH.Adapter;
using BH.oM.Base;
using BH.oM.Geometry;

namespace Adapter_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }


        private static void TestFileAdapter()
        {
            List<Node> nodes = new List<Node>
            {
                new Node {Point = new Point(1, 2, 3), Name = "A"},
                new Node {Point = new Point(4, 5, 6), Name = "B"},
                new Node {Point = new Point(7, 8, 9), Name = "C"}
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
