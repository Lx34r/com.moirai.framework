using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Moirai.Atropos
{
    /// <summary>
    /// 默认游戏配置辅助器。
    /// </summary>
    public class DefaultSettingHelper : SettingUtility.ISettingHelper
    {
        private const string SETTING_FILE_NAME = "Setting.dat";

        private string _filePath = null;
        private DefaultSetting _settings = null;
        private DefaultSettingSerializer _serializer = null;

        public int Count => _settings?.Count ?? 0;

        /// <summary>
        /// 获取游戏配置存储文件路径。
        /// </summary>
        public string FilePath => _filePath;

        /// <summary>
        /// 获取游戏配置。
        /// </summary>
        public DefaultSetting Setting => _settings;

        /// <summary>
        /// 获取游戏配置序列化器。
        /// </summary>
        public DefaultSettingSerializer Serializer => _serializer;

        public void OnInit()
        {
            _filePath = PathUtility.FormatToUnityPath(Path.Combine(Application.persistentDataPath, SETTING_FILE_NAME));
            _settings = new DefaultSetting();
            _serializer = new DefaultSettingSerializer();
            _serializer.RegisterSerializeCallback(0, SerializeDefaultSettingCallback);
            _serializer.RegisterDeserializeCallback(0, DeserializeDefaultSettingCallback);
            
            // 加载游戏配置。
            Load();
        }
        
        private bool SerializeDefaultSettingCallback(Stream stream, DefaultSetting defaultSetting)
        {
            _settings.Serialize(stream);
            return true;
        }

        private DefaultSetting DeserializeDefaultSettingCallback(Stream stream)
        {
            _settings.Deserialize(stream);
            return _settings;
        }
        
        public bool Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return true;
                }

                using (FileStream fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    _serializer.Deserialize(fileStream);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Log.Warning("Load settings failure with exception '{0}'.", exception);
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                using (FileStream fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
                {
                    return _serializer.Serialize(fileStream, _settings);
                }
            }
            catch (Exception exception)
            {
                Log.Warning("Save settings failure with exception '{0}'.", exception);
                return false;
            }
        }

        public string[] GetAllSettingNames()
        {
            return _settings.GetAllSettingNames();
        }

        public void GetAllSettingNames(List<string> results)
        {
            _settings.GetAllSettingNames(results);
        }

        public bool HasSetting(string settingName)
        {
            return _settings.HasSetting(settingName);
        }

        public bool RemoveSetting(string settingName)
        {
            return _settings.RemoveSetting(settingName);
        }

        public void RemoveAllSettings()
        {
            _settings.RemoveAllSettings();
        }

        public bool GetBool(string settingName)
        {
            return _settings.GetBool(settingName);
        }

        public bool GetBool(string settingName, bool defaultValue)
        {
            return _settings.GetBool(settingName, defaultValue);
        }

        public void SetBool(string settingName, bool value)
        {
            _settings.SetBool(settingName, value);
        }

        public int GetInt(string settingName)
        {
            return _settings.GetInt(settingName);
        }

        public int GetInt(string settingName, int defaultValue)
        {
            return _settings.GetInt(settingName, defaultValue);
        }

        public void SetInt(string settingName, int value)
        {
            _settings.SetInt(settingName, value);
        }

        public float GetFloat(string settingName)
        {
            return _settings.GetFloat(settingName);
        }

        public float GetFloat(string settingName, float defaultValue)
        {
            return _settings.GetFloat(settingName, defaultValue);
        }

        public void SetFloat(string settingName, float value)
        {
            _settings.SetFloat(settingName, value);
        }

        public string GetString(string settingName)
        {
            return _settings.GetString(settingName);
        }

        public string GetString(string settingName, string defaultValue)
        {
            return _settings.GetString(settingName, defaultValue);
        }

        public void SetString(string settingName, string value)
        {
            _settings.SetString(settingName, value);
        }
    }
}