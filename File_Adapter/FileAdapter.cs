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
