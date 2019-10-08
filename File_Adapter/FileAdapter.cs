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
using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.Engine.Base;

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
                folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BHoM", "DataSets", folder);

            m_FilePath = Path.Combine(folder, fileName);
            string ext = Path.GetExtension(m_FilePath);

            if (!Path.HasExtension(m_FilePath))
            {
                Engine.Reflection.Compute.RecordNote($"No extension specified in the FileName input. Default is .json.");
                ext = ".json";
                m_FilePath += ext;
            }

            if (ext != ".json" && ext != ".bson")
                throw new Exception($"File_Adapter currently supports only .json and .bson extension types.\nSpecified file extension: {ext}");

            m_isJSON = ext == ".json";
            this.Config.UseAdapterId = false;
        }

        /***************************************************/
        /**** Public Adapter Methods overrides          ****/
        /***************************************************/

        public override List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            if (!Path.HasExtension(m_FilePath))
            {
                Engine.Reflection.Compute.RecordError($"Please include the extension type in the FileName input.");
                return null;
            }

            CreateFileAndFolder();

            // Wrap non-BHoM objects into a Custom BHoMObject to make them work as BHoMObjects. 
            List<IObject> objectsToPush = Modify.WrapNonBHoMObjects(objects, Config, tag, config).ToList();

            // Clone the objects for immutability in the UI. CloneBeforePush should always be true, except for very specific cases.
            objectsToPush = Config.CloneBeforePush ? objectsToPush.Select(x => x.DeepClone()).ToList() : objects.ToList();

            IEnumerable<IBHoMObject> bhomObjects = objectsToPush.Where(x => x is IBHoMObject).Cast<IBHoMObject>();

            if (bhomObjects.Count() != objects.Count())
                Engine.Reflection.Compute.RecordWarning("The file adapter can currently only be used with BHoMObjects." + Environment.NewLine +
                    "If you want to push non-BHoMobject, specify a push config with the option `WrapNonBHoMObject` set to true.");

            bool success = this.Replace<IBHoMObject>(bhomObjects, tag);

            return success ? objectsToPush : new List<IObject>();
        }

        public override IEnumerable<object> Pull(IRequest request, Dictionary<string, object> config = null)
        {
            if (!System.IO.File.Exists(m_FilePath))
            {
                Engine.Reflection.Compute.RecordError($"File not found: {m_FilePath} - Cannot pull from this file");
                return null;
            }
            else if (!Path.HasExtension(m_FilePath))
            {
                Engine.Reflection.Compute.RecordError($"Please include the extension type in the FileName input.");
                return null;
            }

            return base.Pull(request, config);
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
