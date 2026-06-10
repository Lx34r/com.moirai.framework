using System.IO;

namespace Moirai.Atropos.Save
{
    /// <summary>
    /// 将指定位置的指定对象保存到磁盘上，转换为json并加密
    /// </summary>
    public class JsonEncryptedSaveHandler : EncryptedSaveHandlerBase
    {
        protected override void SerializeToStream(object objectToSave, MemoryStream stream)
        {
            string json = JSONUtility.ToJson(objectToSave);
            using (StreamWriter streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8, 1024, leaveOpen: true))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }
        }

        protected override T DeserializeFromStream<T>(MemoryStream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream, System.Text.Encoding.UTF8, true, 1024, leaveOpen: true))
            {
                return JSONUtility.ToObject<T>(streamReader.ReadToEnd());
            }
        }
    }
}
