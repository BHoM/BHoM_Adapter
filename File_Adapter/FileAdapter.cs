/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System.Linq;
using System;
using System.IO;
using BH.oM.Reflection.Attributes;

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
        [Input("folder", "Defaults to the path of your default drive (usually C://)")]
        [Input("fileName","Insert filename with extension.\nCurrently supports only .json and .bson file types.")]
        public FileAdapter(string folder = null, string fileName = "")
        {
            bool valueInserted = true;

            if (folder == null)
            {
                valueInserted = false;
                folder = Path.GetPathRoot(Environment.SystemDirectory);
            }

            if (string.IsNullOrEmpty(fileName))
            {
                valueInserted = false;
                fileName = "objects.json";
            }

            if (folder.Count() > 2 && folder.ElementAt(1) != ':')
            {
                folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\BHoM\DataSets\", folder);
            }

            m_FilePath = Path.Combine(folder, fileName);

            if (!Path.HasExtension(m_FilePath))
            {
                Engine.Reflection.Compute.RecordError($"Please include the extension type in the FileName input.");
                return;
            }

            if (valueInserted && !System.IO.File.Exists(m_FilePath))
            {
                Engine.Reflection.Compute.RecordWarning($"File not found:\n{m_FilePath}.");
                return;
            }

            string ext = Path.GetExtension(m_FilePath);

            if (ext != ".json" && ext != ".bson")
                    Engine.Reflection.Compute.RecordError($"File_Adapter currently supports only .json and .bson extension types.\nSpecified file extension: {ext}");

            m_isJSON = ext == ".json";
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
        private bool m_isJSON;
    }
}
