using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.SettingsManager
{
    [Export(typeof(ISettingsManager))]
    public class SettingsManager : ISettingsManager
    {
        private readonly object _lockObject = new object();

        public event EventHandler SettingsLocationChanged;
        public event EventHandler Initialized;

        public IEnvironmentVarirables EnvironmentVarirables { get; }

        public UserSettingsFile SettingsFile { get; protected set; }

        protected SettingsFileSerializer Serializer { get; }
        protected SettingsValueSerializer ValueSerializer { get; }

        [ImportingConstructor]
        public SettingsManager(IEnvironmentVarirables environmentVarirables)
        {
            EnvironmentVarirables = environmentVarirables;
            Initialized += SettingsManager_Initialized;
            Serializer = new SettingsFileSerializer(this);
            ValueSerializer = new SettingsValueSerializer();
        }

        public virtual void CreateNewSettingsFile()
        {
            CreateNewSettingsFileInternal(EnvironmentVarirables.SettingsFilePath);
        }

        public virtual void LoadCurrent()
        {
            SettingsFile = Serializer.Desrialize(EnvironmentVarirables.SettingsFilePath);
        }

        public void ChangeSettingsFileLocation(string path, bool deleteCurrent)
        {
            lock (_lockObject)
            {
                if (!File.Exists(EnvironmentVarirables.SettingsFilePath))
                    throw new FileNotFoundException(EnvironmentVarirables.SettingsFilePath);

                var newDirectoryPath = Path.GetDirectoryName(path);

                if (string.IsNullOrEmpty(newDirectoryPath))
                    throw new ArgumentNullException();
                if (!Directory.Exists(newDirectoryPath))
                    throw new DirectoryNotFoundException();

                File.Copy(EnvironmentVarirables.SettingsFilePath, path);

                if (deleteCurrent)
                    File.Delete(EnvironmentVarirables.SettingsFilePath);
                EnvironmentVarirables.SettingsFilePath = path;
            }
            SettingsLocationChanged?.Invoke(this, EventArgs.Empty);
        }

        public void DeleteCurrentSettingsFile()
        {
            if (File.Exists(EnvironmentVarirables.SettingsFilePath))
                File.Delete(EnvironmentVarirables.SettingsFilePath);
        }

        public void Initialize()
        {
            if (!File.Exists(EnvironmentVarirables.SettingsFilePath))
                CreateNewSettingsFile();
            LoadCurrent();
            Initialized?.Invoke(this, EventArgs.Empty);

            var t = GetValueOrDefault(SettingsFile.GetPropertyValueData("Environment/Documents/", "InitializeOpenFileFromCurrentDocument"), default(bool));
            var v = GetValueOrDefault(SettingsFile.GetAttributeValue("ApplicationIdentity", "version", false), default(string));
        }

        public GetValueResult TryGetValue<T>(string data, out T value)
        {
            value = default(T);
            try
            {
                if (data == null)
                    return GetValueResult.Missing;
                var serializedData = ValueSerializer.Serialize(data, typeof(T));
                return TryDeserialize(serializedData, out value);
            }
            catch (Exception)
            {
                return GetValueResult.Corrupt;
            }
        }


        public T GetValueOrDefault<T>(string xPath, T defaultValue = default(T))
        {
            return TryGetValue(xPath, out T obj) != GetValueResult.Success ? defaultValue : obj;
        }


        public Task SetValueAsync(string name, object value)
        {
            var t = new Task(() => { });
            t.Start();
            return t;
        }

        protected void CreateNewSettingsFileInternal(string path)
        {
            Serializer.Serialize(path);
        }

        private void SettingsManager_Initialized(object sender, EventArgs e)
        {
            Initialized -= SettingsManager_Initialized;
            //TODO: Read real settingspath from file and update if neccessary
        }


        private GetValueResult TryDeserialize<T>(string data, out T result)
        {
            result = default(T);
            try
            {
                return ValueSerializer.Deserialize(data, out result);
            }
            catch (Exception)
            {
                return GetValueResult.UnknownError;
            }
        }
    }

    public class SettingsManagerException : Exception
    {
        public SettingsManagerException()
        {
        }

        public SettingsManagerException(string message) : base(message)
        {
        }

        public SettingsManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SettingsManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public interface ISettingsManager
    {
        event EventHandler SettingsLocationChanged;
        event EventHandler Initialized;

        IEnvironmentVarirables EnvironmentVarirables { get; }

        void ChangeSettingsFileLocation(string path, bool deleteCurrent);

        void CreateNewSettingsFile();

        void DeleteCurrentSettingsFile();

        void Initialize();

        void LoadCurrent();

        //ISettingsSubset GetSubset(string namePattern);

        //string[] NamesStartingWith(string prefix);

        //ISettingsList GetOrCreateList(string name, bool isMachineLocal);

        GetValueResult TryGetValue<T>(string xPath, out T value);

        T GetValueOrDefault<T>(string xPath, T defaultValue);

        Task SetValueAsync(string name, object value);
    }

    public enum GetValueResult
    {
        Success,
        Missing,
        Corrupt,
        IncompatibleType,
        ObsoleteFormat,
        UnknownError
    }
}