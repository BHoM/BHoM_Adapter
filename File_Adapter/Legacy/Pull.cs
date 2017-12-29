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
