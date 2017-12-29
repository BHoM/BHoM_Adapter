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
