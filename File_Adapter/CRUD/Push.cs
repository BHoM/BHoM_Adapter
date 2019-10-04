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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

using System.IO;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            if (!Path.HasExtension(m_FilePath))
            {
                Engine.Reflection.Compute.RecordError($"Please include the extension type in the FileName input.");
                return null;
            }

            CreateFileAndFolder();

            List<IObject> objectsToPush = Modify.WrapNonBHoMObjects(objects, Config, tag, config);
            objectsToPush = Modify.CloneBHoMObjects(objectsToPush, Config);

            IEnumerable<IBHoMObject> bhomObjects = objectsToPush.Where(x => x is IBHoMObject).Cast<IBHoMObject>();

            if (bhomObjects.Count() != objects.Count())
                Engine.Reflection.Compute.RecordWarning("The file adapter can currently only be used with BHoMObjects. Please check your input data");

            bool success = this.Replace<IBHoMObject>(bhomObjects, tag);

            return success ? objectsToPush : new List<IObject>();
        }

        /***************************************************/
    }
}
