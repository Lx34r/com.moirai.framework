using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

namespace Moirai.Atropos.Save
{
    public class JsonEncryptedSaveHandler : SaveEncryptor, ISaveHandler
    {
        /// <summary>
        /// 将指定位置的指定对象保存到磁盘上，转换为json并加密
        /// </summary>
        /// <param name="objectToSave"></param>
        /// <param name="saveFile"></param>
        public Task Save(object objectToSave, FileStream saveFile)
        {
            string json = JSONUtility.ToJson(objectToSave);
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamWriter streamWriter = new StreamWriter(memoryStream))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                memoryStream.Position = 0;
                Encrypt(memoryStream, saveFile, Key);
            }
            saveFile.Close();

            return Task.CompletedTask;
        }

        /// <summary>
        /// 加载指定的文件，对其进行解密和解码
        /// </summary>
        /// <param name="saveFile"></param>
        /// <returns></returns>
        public Task<T> Load<T>(FileStream saveFile)
        {
            T savedObject;
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                try
                {
                    Decrypt(saveFile, memoryStream, Key);
                }
                catch (CryptographicException ce)
                {
                    Debug.LogError("[SaveHandler] Decryption failed for JSON save data: " + ce);
                    return Task.FromResult<T>(default);
                }
                memoryStream.Position = 0;
                savedObject = JSONUtility.ToObject<T>(streamReader.ReadToEnd());
            }
            saveFile.Close();

            return Task.FromResult(savedObject);
        }
    }
}