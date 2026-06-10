using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Moirai.Atropos.Save
{
    /// <summary>
    /// 此保存加载方法将文件保存并加载为加密的二进制文件
    /// </summary>
    /// <remarks>
    /// SECURITY WARNING: BinaryFormatter is vulnerable to deserialization attacks (RCE).
    /// It has been deprecated by Microsoft and is removed in .NET 9+.
    /// Consider migrating to JsonEncryptedSaveHandler for new projects.
    /// See: https://learn.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide
    /// </remarks>
    [System.Obsolete("BinaryFormatter is insecure and deprecated. Use JsonEncryptedSaveHandler instead. See https://aka.ms/binaryformatter")]
    public class BinaryEncryptedSaveHandler : EncryptedSaveHandlerBase
    {
        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        protected override void SerializeToStream(object objectToSave, MemoryStream stream)
        {
            _formatter.Serialize(stream, objectToSave);
        }

        protected override T DeserializeFromStream<T>(MemoryStream stream)
        {
            return (T)_formatter.Deserialize(stream);
        }
    }
}
