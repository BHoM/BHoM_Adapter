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
        //public override bool Push(IEnumerable<object> objects, string tag = "", Dictionary<string, object> config = null)
        //{
        //    // Make sure objects being pushed are tagged
        //    List<BHoMObject> objectsToPush = objects.Cast<BHoMObject>().ToList();
        //    objectsToPush.ForEach(x => x.Tags.Add(tag));

        //    if (m_Readable)
        //        return PushJson(objectsToPush);
        //    else
        //        return PushBson(objectsToPush);
        //}

        //private bool PushBson(List<BHoMObject> objects)
        //{
        //    try
        //    {
        //        FileStream mongoStream = new FileStream(m_FilePath, FileMode.Create);
        //        var writer = new BsonBinaryWriter(mongoStream);
        //        BsonSerializer.Serialize(writer, typeof(object), objects);
        //        mongoStream.Flush();
        //        mongoStream.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorLog.Add(e.Message);
        //        return false;
        //    }

        //    return true;
        //}


        //private bool PushJson(List<BHoMObject> objects)
        //{
        //    try
        //    {
        //        File.WriteAllLines(m_FilePath, objects.Select(x => x.ToJson()));
        //    }
        //    catch(Exception e)
        //    {
        //        ErrorLog.Add(e.Message);
        //        return false;
        //    }

        //    return true;
        //}
  
    }
}
