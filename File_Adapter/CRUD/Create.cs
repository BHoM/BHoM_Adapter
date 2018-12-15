using BH.oM.Base;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BH.Engine.Serialiser;
using BH.oM.Base.CRUD;

namespace BH.Adapter.FileAdapter
{
    public partial class FileAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false, CrudConfig config = null)
        {
            if (m_Readable)
                return CreateJson((IEnumerable<IBHoMObject>)objects);
            else
                return CreateBson((IEnumerable<IBHoMObject>)objects);
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
                Engine.Reflection.Compute.RecordError(e.Message);
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
                Engine.Reflection.Compute.RecordError(e.Message);
                return false;
            }

            return true;
        }

        /***************************************************/
    }
}
