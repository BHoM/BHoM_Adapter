using BH.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter.Queries;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter : IAdapter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<string> ErrorLog { get; set; } = new List<string>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public FileAdapter(string folder = "C:\\", string fileName = "objects", bool readable = false)
        {
            if (!folder.EndsWith("\\"))
                folder += "\\";

            m_FilePath = folder + fileName + (readable ? ".json" : ".bson");
            m_Readable = readable;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private string m_FilePath;
        private bool m_Readable;
    }
}
