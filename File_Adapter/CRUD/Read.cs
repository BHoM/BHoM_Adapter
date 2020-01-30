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

using BH.oM.Adapter;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        protected override IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {
            IEnumerable<BHoMObject> everything = m_isJSON ? ReadJson() : ReadBson();

            if (type != null)
                everything = everything.Where(x => type.IsAssignableFrom(x.GetType()));

            if (ids != null)
            {
                HashSet<Guid> toDelete = new HashSet<Guid>(ids.Cast<Guid>());
                everything = everything.Where(x => !toDelete.Contains((Guid)x.CustomData[AdapterIdName]));
            }
                

            return everything;
        }


        private IEnumerable<BHoMObject> ReadJson()
        {
            string[] json = File.ReadAllLines(m_FilePath);
            var converted = json.Select(x => Engine.Serialiser.Convert.FromJson(x) as BHoMObject).Where(x => x != null);
            if (converted.Count() < json.Count())
                BH.Engine.Reflection.Compute.RecordWarning("Could not convert some object to BHoMObject.");
            return converted;
        }


        private IEnumerable<BHoMObject> ReadBson()
        {
            FileStream mongoReadStream = File.OpenRead(m_FilePath);
            var reader = new BsonBinaryReader(mongoReadStream);
            List<BsonDocument> readBson = BsonSerializer.Deserialize(reader, typeof(object)) as List<BsonDocument>;
            return readBson.Select(x => BsonSerializer.Deserialize(x, typeof(object)) as BHoMObject);
        }
    }
}

