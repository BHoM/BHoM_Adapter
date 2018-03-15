using System.Linq;
using System;
using System.IO;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public FileAdapter(string folder = "C:\\", string fileName = "objects", bool readable = true)
        {

            if (folder.Count() > 2 && folder.ElementAt(1) != ':')
            {
                folder = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\BHoM\DataSets\" + folder;
            }

            if (!folder.EndsWith("\\"))
                folder += "\\";

            m_FilePath = folder + fileName + (readable ? ".json" : ".bson");



            m_Readable = readable;
            this.Config.UseAdapterId = false;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private void CreateFileAndFolder()
        {
            string directoryPath = Path.GetDirectoryName(m_FilePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);


            if (!File.Exists(m_FilePath))
            {
                FileStream stream = File.Create(m_FilePath);
                stream.Dispose();
                stream.Close();
            }
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private string m_FilePath;
        private bool m_Readable;
    }
}
