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
    public interface IData
    {
        public event Action Changed;

        public abstract void Reset();
        public abstract IData Clone();
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
        private Dictionary<Type, Action<IData>> _dataChangedSubscribers = new Dictionary<Type, Action<IData>>();
        private Dictionary<string, Action> _internalDataChangedSubscribers = new Dictionary<string, Action>();

        public override void Init()
        {
            _dataPath = string.Format("{0}/{1}/Data/GameData.json",
                Application.persistentDataPath, Application.identifier);
            LoadDataFromFile();

            foreach (var pair in _data.Data)
            {
                TrySubscribeToDataInternal(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Add/Change some data in database.
        /// </summary>
        public void ChangeOrAddData<T>(T data) where T : IData
        {
            if (data == null)
                throw new Exception("data is null!");

            Type type = data.GetType();
            string key = type.ToString();

            IData oldData;
            if (_data.Data.TryGetValue(key, out oldData))
            {
                if(!((T)oldData).Equals(data))
                {
                    Action oldListeners = _internalDataChangedSubscribers[key];
                    oldData.Changed -= oldListeners;
                    data.Changed += oldListeners;
                    PublishDataChanged(data);
                }

                _data.Data[key] = data;
            }
            else
            {
                _data.Data.Add(key, data);
                TrySubscribeToDataInternal(data);
            }

            _data.Version += 1;
        }

        /// <summary>
        /// Get data from database.
        /// </summary>
        public T GetData<T>() where T : IData
        {
            string key = typeof(T).ToString();

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
        /// Subscribe to data changed of given data type.
        /// </summary>
        public void SubscribeDataChanged<T>(Action<T> callback) where T : IData
        {
            Type key = typeof(T);

            Action<IData> changed = (data) =>
            {
                callback.Invoke((T)data);
            };

            if (_dataChangedSubscribers.ContainsKey(key))
            {
                _dataChangedSubscribers[key] += changed;
            }
            else
            {
                _dataChangedSubscribers[key] = changed;
            }
        }

        /// <summary>
        /// Check if data with given key exists.
        /// </summary>
        public bool HaveData<T>() where T : IData
        {
            string key = typeof(T).ToString();

            if (_data.Data.ContainsKey(key))
            {
                return true;
            }

            return false;
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
            if (_data == null)
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

        private void PublishDataChanged<T>(T data)
        {
            Type key = typeof(T);
            PublishDataChanged(key, data);
        }

        private void PublishDataChanged<T>(Type type, T data)
        {
            if (_dataChangedSubscribers.ContainsKey(type))
            {
                _dataChangedSubscribers[type].Invoke((IData)data);
            }
        }

        private bool TrySubscribeToDataInternal<T>(T data) where T : IData
        {
            string key = typeof(T).ToString();

            Action action;
            if (!_internalDataChangedSubscribers.ContainsKey(key))
            {
                Action changed = () =>
                {
                    // Send copy of the data to prevent modification.
                    PublishDataChanged(data.Clone());
                };
                data.Changed += changed;

                _internalDataChangedSubscribers.Add(key, changed);

                return true;
            }
            return false;
        }

        private bool TrySubscribeToDataInternal(string typeName, IData data)
        {
            Type type = Type.GetType(typeName);
            if (!_internalDataChangedSubscribers.ContainsKey(typeName))
            {
                Action changed = () =>
                {
                    // Send copy of the data to prevent modification.
                    PublishDataChanged(type, data.Clone());
                };
                data.Changed += changed;

                _internalDataChangedSubscribers.Add(typeName, changed);
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
            public Dictionary<string, IData> Data = new Dictionary<string, IData>();
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
