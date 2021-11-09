using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityGame
{
    /// <summary>
    /// Implement this interface to be abble to use SetData/GetData/HaveData methods.
    /// </summary>
    public interface IDatabaseData
    {
        public abstract void Reset();
    }

    /// <summary>
    /// Property to prevent a property to be serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DoNotSerializePropertyAttribute : Attribute
    {
    }

    /// <summary>
    /// Controller to save and load any data.
    /// </summary>
    public class DataController : BaseController
    {
        private string _dataPath;
        private DatabaseData _data;

        public override void Init()
        {
            _dataPath = string.Format("{0}/{1}/Data/GameData.json",
                Application.persistentDataPath, Application.identifier);
            LoadDataFromFile();
        }

        private void OnApplicationQuit()
        {
            SaveDataToFile();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveDataToFile();
            }
        }

        /// <summary>
        /// Save all data to file.
        /// </summary>
        private void SaveDataToFile()
        {
            if(_data == null)
            {
                LogWrapper.LogError("[DataController] Data is null!");
                return;
            }

            string dir = Path.GetDirectoryName(_dataPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string json = JsonConvert.SerializeObject(_data, Formatting.Indented, GetSerializerSettings());
            using (StreamWriter writer = new StreamWriter(_dataPath, false))
            {
                writer.WriteLine(json);
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            LogWrapper.Log($"[DataController] Save finished. FilePath: {_dataPath}; Data: {json};");
        }

        /// <summary>
        /// Load all data from file.
        /// </summary>
        private void LoadDataFromFile()
        {
            if (File.Exists(_dataPath))
            {
                using (StreamReader reader = new StreamReader(_dataPath))
                {
                    string json = reader.ReadToEnd();
                    _data = JsonConvert.DeserializeObject<DatabaseData>(json, GetSerializerSettings());
                    
                    LogWrapper.Log($"[DataController] Loaded data from {_dataPath}; NewData: {false}; Data: {json};");
                    
                    return;
                }
            }

            _data = new DatabaseData();

            LogWrapper.Log($"[DataController] Loaded data from {_dataPath}; NewData: {true};");
        }

        /// <summary>
        /// Add/Changed some data in database.
        /// </summary>
        public void SetData(IDatabaseData data)
        {
            string key = data.GetType().ToString();

            if (key == null)
                throw new Exception("key is null!");
            if (data == null)
                throw new Exception("data is null!");

            if (_data.Data.ContainsKey(key))
            {
                _data.Data[key] = data;
            }
            else
            {
                _data.Data.Add(key, data);
            }
            _data.Version += 1;
        }

        /// <summary>
        /// Get data from database.
        /// </summary>
        public T GetData<T>() where T : IDatabaseData
        {
            string key = typeof(T).ToString();

            if (key == null)
                throw new Exception("key is null!");

            if (HaveData<T>())
            {
                return (T)_data.Data[key];
            }


            var objectType = typeof(T);
            var newData = Activator.CreateInstance(objectType);
            _data.Data[key] = (T)newData;
            return (T)newData;
        }

        /// <summary>
        /// Check if data with given key exists.
        /// </summary>
        public bool HaveData<T>() where T : IDatabaseData
        {
            string key = typeof(T).ToString();

            if (key == null)
                throw new Exception("key is null!");

            if (_data.Data.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        private JsonSerializerSettings GetSerializerSettings()
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                SerializationBinder = new SerializationBinder(),
                ContractResolver = new GetOnlyContractResolver()
            };
            return serializerSettings;
        }

        private class DatabaseData
        {
            public int Version;
            public Dictionary<string, IDatabaseData> Data = new Dictionary<string, IDatabaseData>();
        }

        private class GetOnlyContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                if (property != null && property.Writable)
                {
                    var attributes = property.AttributeProvider.GetAttributes(typeof(DoNotSerializePropertyAttribute), true);
                    if (attributes != null && attributes.Count > 0)
                        property.Writable = false;
                }
                return property;
            }
        }

        private class SerializationBinder : ISerializationBinder
        {
            public Type BindToType(string assemblyName, string typeName)
            {
                return Type.GetType(typeName);
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                typeName = serializedType.FullName;
            }
        }
    }
}
