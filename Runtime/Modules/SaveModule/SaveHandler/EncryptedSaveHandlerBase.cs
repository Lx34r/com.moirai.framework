using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Moirai.Atropos.Save
{
    /// <summary>
    /// Base class for encrypted save handlers that provides common encryption/decryption
    /// workflow with error handling. Subclasses only need to implement the serialization
    /// and deserialization logic for their specific format.
    /// </summary>
    public abstract class EncryptedSaveHandlerBase : SaveEncryptor, ISaveHandler
    {
        public Task Save(object objectToSave, FileStream saveFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                SerializeToStream(objectToSave, memoryStream);
                memoryStream.Position = 0;
                Encrypt(memoryStream, saveFile, Key);
            }
            saveFile.Flush();
            saveFile.Close();

            return Task.CompletedTask;
        }

        public Task<T> Load<T>(FileStream saveFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    Decrypt(saveFile, memoryStream, Key);
                }
                catch (CryptographicException ce)
                {
                    Log.Error("[SaveHandler] Decryption failed: " + ce);
                    return Task.FromResult<T>(default);
                }
                memoryStream.Position = 0;
                T savedObject = DeserializeFromStream<T>(memoryStream);
                saveFile.Close();
                return Task.FromResult(savedObject);
            }
        }

        /// <summary>
        /// Serialize the object into the provided stream.
        /// </summary>
        protected abstract void SerializeToStream(object objectToSave, MemoryStream stream);

        /// <summary>
        /// Deserialize an object of type T from the provided stream.
        /// </summary>
        protected abstract T DeserializeFromStream<T>(MemoryStream stream);
    }
}
