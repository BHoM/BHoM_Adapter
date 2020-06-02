/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.Engine.Base;
using BH.oM.Adapter;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructor                               ****/
        /***************************************************/
        [Input("folder", "Defaults to the path of your default drive (usually C://)")]
        [Input("fileName", "Insert filename with extension.\nCurrently supports only .json and .bson file types.")]
        public FileAdapter(string folder = null, string fileName = "")
        {
            if (folder == null)
                folder = Path.GetPathRoot(Environment.SystemDirectory);

            if (string.IsNullOrEmpty(fileName))
                fileName = "objects.json";

            if (folder.Count() > 2 && folder.ElementAt(1) != ':')
                folder = Path.Combine(@"C:\ProgramData\BHoM\DataSets", folder);

            m_FilePath = Path.Combine(folder, fileName);

            ProcessExtension(ref m_FilePath);

            m_isJSON = Path.GetExtension(m_FilePath) == ".json";
            this.m_AdapterSettings.UseAdapterId = false;
        }

        /***************************************************/
        /**** Public Adapter Methods overrides          ****/
        /***************************************************/

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private bool ProcessExtension(ref string filePath)
        {
            string ext = Path.GetExtension(filePath);

            if (!Path.HasExtension(m_FilePath))
            {
                Engine.Reflection.Compute.RecordNote($"No extension specified in the FileName input. Default is .json.");
                ext = ".json";
                filePath += ext;
            }

            if (ext != ".json" && ext != ".bson")
            {
                Engine.Reflection.Compute.RecordError($"File_Adapter currently supports only .json and .bson extension types.\nSpecified file extension: {ext}");
                return false;
            }

            return true;
        }

        private void CreateFileAndFolder(PushType pushType)
        {
            string directoryPath = Path.GetDirectoryName(m_FilePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (!File.Exists(m_FilePath) || pushType == PushType.DeleteThenCreate)
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

