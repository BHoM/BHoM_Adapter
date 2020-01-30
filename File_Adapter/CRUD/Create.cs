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

using BH.oM.Base;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BH.Engine.Serialiser;
using BH.oM.Adapter;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {

            bool clearFile = m_AdapterSettings.ProcessInMemory;

            if (m_isJSON)
                return CreateJson((IEnumerable<IBHoMObject>)objects, clearFile);
            else
                return CreateBson((IEnumerable<IBHoMObject>)objects, clearFile);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private bool CreateBson(IEnumerable<IBHoMObject> objects, bool clearFile = false)
        {
            try
            {
                FileStream stream = new FileStream(m_FilePath, clearFile ? FileMode.Create : FileMode.Append);
                var writer = new BsonBinaryWriter(stream);
                BsonSerializer.Serialize(writer, typeof(object), objects);
                stream.Flush();
                stream.Close();
            }
            catch (Exception e)
            {
                BH.Engine.Reflection.Compute.RecordError(e.Message);
                return false;
            }

            return true;
        }

        /***************************************************/

        private bool CreateJson(IEnumerable<IBHoMObject> objects, bool clearFile = false)
        {
            try
            {
                if (clearFile)
                    File.WriteAllLines(m_FilePath, objects.Select(x => x.ToJson()));
                else
                    File.AppendAllLines(m_FilePath, objects.Select(x => x.ToJson()));
            }
            catch (Exception e)
            {
                BH.Engine.Reflection.Compute.RecordError(e.Message);
                return false;
            }

            return true;
        }
    }
}

