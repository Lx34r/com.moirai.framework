using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Moirai.Atropos;

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
    public class BinaryEncryptedSaveHandler : SaveEncryptor, ISaveHandler
    {
        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        /// <summary>
        /// 加密后将指定对象保存到指定位置的磁盘上
        /// </summary>
        /// <param name="objectToSave"></param>
        /// <param name="saveFile"></param>
        public Task Save(object objectToSave, FileStream saveFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            _formatter.Serialize(memoryStream, objectToSave);
            memoryStream.Position = 0;
            Encrypt(memoryStream, saveFile, Key);
            saveFile.Flush();
            memoryStream.Close();
            saveFile.Close();

            return Task.CompletedTask;
        }

        /// <summary>
        /// 从磁盘加载指定的文件，对其进行解密，然后对其进行反序列化
        /// </summary>
        /// <param name="saveFile"></param>
        /// <returns></returns>
        public Task<T> Load<T>(FileStream saveFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                Decrypt(saveFile, memoryStream, Key);
            }
            catch (CryptographicException ce)
            {
                Log.Error("[SaveHandler] Decryption failed for binary save data: " + ce);
                return Task.FromResult<T>(default);
            }
            memoryStream.Position = 0;
            T savedObject = (T)_formatter.Deserialize(memoryStream);
            memoryStream.Close();
            saveFile.Close();

            return Task.FromResult(savedObject);
        }
    }
}