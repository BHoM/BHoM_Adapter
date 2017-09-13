using BH.oM.Base;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        protected override bool Create(IEnumerable<object> objects)
        {
            IEnumerable<BHoMObject> objectsToPush = objects.Cast<BHoMObject>();

            if (m_Readable)
                return CreateJson(objectsToPush);
            else
                return CreateBson(objectsToPush);
        }

        private bool CreateBson(IEnumerable<BHoMObject> objects, bool clearFile = false)
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
                ErrorLog.Add(e.Message);
                return false;
            }

            return true;
        }


        private bool CreateJson(IEnumerable<BHoMObject> objects, bool clearFile = false)
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
                ErrorLog.Add(e.Message);
                return false;
            }

            return true;
        }
    }
}
