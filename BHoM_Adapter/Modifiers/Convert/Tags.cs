using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Adapter
{
    public static partial class Tag
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string GetTaggedName(this IObject obj)
        {
            string str = string.IsNullOrWhiteSpace(obj.Name) ? "" : obj.Name;

            if (obj.Tags.Count > 0)
            {
                str += " __Tags__:";

                foreach (string tag in obj.Tags)
                {
                    str += tag + "_/_";
                }

                str = str.TrimEnd("_/_");
            }

            return str;

        }

        /***************************************************/

        public static HashSet<string> ToTagHashSet(this string str)    //GetTagsFromString
        {
            string name = "";
            return str.ToTagHashSet(out name);
        }

        /***************************************************/

        public static HashSet<string> ToTagHashSet(this string str, out string name)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                name = "";
                return new HashSet<string>();
            }

            string[] arr = str.Split(new string[] { "__Tags__:" }, StringSplitOptions.None);

            name = arr[0].TrimEnd(" ");

            if (arr.Length < 2)
                return new HashSet<string>();

            return new HashSet<string>(arr[1].Split(new string[] { "_/_" }, StringSplitOptions.None));
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static string TrimEnd(this string input, string suffixToRemove)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else return input;
        }

        /***************************************************/

    }
}
