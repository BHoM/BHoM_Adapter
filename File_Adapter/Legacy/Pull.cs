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

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        //public override IEnumerable<object> Pull(IEnumerable<IQuery> query, Dictionary<string, object> config = null)
        //{
        //    if (m_Readable)
        //        return PullJson();
        //    else
        //        return PullBson();
        //}


        //private IEnumerable<object> PullJson()
        //{
        //    string[] json = File.ReadAllLines(m_FilePath);
        //    return json.Select(x => Convert.FromJson(x));
        //}


        //private IEnumerable<object> PullBson()
        //{
        //    FileStream mongoReadStream = File.OpenRead(m_FilePath);
        //    var reader = new BsonBinaryReader(mongoReadStream);
        //    List<BsonDocument> readBson = BsonSerializer.Deserialize(reader, typeof(object)) as List<BsonDocument>;
        //    return readBson.Select(x => BsonSerializer.Deserialize(x, typeof(object)));
        //}
    }
}
