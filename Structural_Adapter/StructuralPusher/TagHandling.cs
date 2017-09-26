using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Adapter.Structural
{
    public static partial class StructuralPusher
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string GetNameAndTagString(this BHoMObject obj)
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

        public static HashSet<string> GetTagsFromString(this string str)
        {
            string name = "";
            return str.GetTagsFromString(out name);
        }

        /***************************************************/

        public static HashSet<string> GetTagsFromString(this string str, out string name)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                name = "";
                return new HashSet<string>();
            }

            string[] arr = str.Split(new string[] { "__Tags__:" }, StringSplitOptions.None);

            name = arr[0];

            if (arr.Length < 2)
                return new HashSet<string>();

            return new HashSet<string>(arr[1].Split(new string[] { "_/_" }, StringSplitOptions.None));
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public static string TrimEnd(this string input, string suffixToRemove)
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
