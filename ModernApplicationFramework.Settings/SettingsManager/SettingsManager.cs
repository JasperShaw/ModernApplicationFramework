using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.SettingsBase;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Settings;
using ModernApplicationFramework.Settings.SettingsDialog;

namespace ModernApplicationFramework.Settings.SettingsManager
{
    //TODO: Try to move all the settings stuff into a new assembly as this might not be required for all users of the framework

    [Export(typeof(ISettingsManager))]
    public class SettingsManager : ISettingsManager
    {
        private readonly object _lockObject = new object();

        public event EventHandler SettingsLocationChanged;
        public event EventHandler Initialized;

        public IEnvironmentVarirables EnvironmentVarirables { get; }

        public ISettingsFile SettingsFile { get; protected set; }

        protected SettingsValueSerializer ValueSerializer { get; }

        [ImportingConstructor]
        public SettingsManager(IEnvironmentVarirables environmentVarirables)
        {
            EnvironmentVarirables = environmentVarirables;
            ValueSerializer = new SettingsValueSerializer();
            Initialized += SettingsManager_Initialized;
        }

        public virtual void CreateNewSettingsFile()
        {
            CreateNewSettingsFileInternal(EnvironmentVarirables.SettingsFilePath);
        }

        public virtual void LoadCurrent()
        {
            SettingsFile = SettingsFactory.Open(EnvironmentVarirables.SettingsFilePath, this);
        }

        public void SaveCurrent()
        {
            SettingsFile.Save();
        }

        public void Close()
        {
            SettingsFile.Dispose();
            SettingsFile = null;
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
            else
                LoadCurrent();

            var settingsModels = IoC.GetAll<ISettingsDataModel>();
            var settingsCategories = IoC.GetAll<SettingsCategory>();
            foreach (var settingsCategory in settingsCategories)
            {
                var name = settingsCategory.Name;
                var node = SettingsFile.GetSingleNode(name);
                if (node != null)
                    continue;
                if (settingsCategory.IsToolsOptionsCategory)
                    SettingsFile.AddToolsOptionsCategoryElement(name);   
                else
                    throw new NotImplementedException();
            }

            foreach (var settingsModel in settingsModels)
            {
                if (SettingsFile.GetSingleNode(settingsModel.SettingsFilePath) == null)
                {
                    if (settingsModel.Category.IsToolsOptionsCategory)
                        SettingsFile.AddToolsOptionsModelElement(settingsModel.Name, settingsModel.Category.Name);
                    else
                        throw new NotImplementedException();
                }
                settingsModel.LoadOrCreate();
            }
            Initialized?.Invoke(this, EventArgs.Empty);
        }


        public GetValueResult GetOrCreatePropertyValue<T>(string propertyPath, string propertyName, out T value, T defaultValue = default(T),
            bool navigateAttributeWise = true, bool createNew = true)
        {
            value = defaultValue;
            var data = SettingsFile.GetPropertyValueData(propertyPath, propertyName, navigateAttributeWise);
            if (data == null)
            {
                var node = SettingsFile.GetSingleNode(propertyPath);
                if (node == null)
                    return GetValueResult.Missing;
                SettingsFile.AddPropertyValueElement(node, propertyName, value?.ToString());
                return GetValueResult.Created;
            }
            return TryGetValue(data, out value, defaultValue);
        }

        public GetValueResult GetPropertyValue<T>(string propertyPath, string propertyName, out T value,
            bool navigateAttributeWise = true)
        {
            return GetOrCreatePropertyValue(propertyPath, propertyName, out value, default(T), navigateAttributeWise, false);
        }

        public Task SetPropertyValueAsync(string path, string propertyName, string value,
            bool navigateAttributeWise = true)
        {
            var t = new Task(() =>
            {
                try
                {
                    SettingsFile.SetPropertyValueData(path, propertyName, value, navigateAttributeWise);
                }
                catch (Exception)
                {
                    //Ignored
                }
            });
            t.Start();
            return t;
        }

        protected void CreateNewSettingsFileInternal(string path)
        {
            SettingsFile = SettingsFactory.Create(EnvironmentVarirables.SettingsFilePath, this);
        }

        protected GetValueResult TryGetValue<T>(string data, out T value, T defaultValue = default(T))
        {
            value = defaultValue;
            try
            {
                if (data == null)
                    return GetValueResult.Missing;
                var serializedData = ValueSerializer.Serialize(data, typeof(T));
                return TryDeserialize(serializedData, out value, defaultValue);
            }
            catch (Exception)
            {
                return GetValueResult.Corrupt;
            }
        }

        private void SettingsManager_Initialized(object sender, EventArgs e)
        {
            Initialized -= SettingsManager_Initialized;
            //TODO: Read real settingspath from file and update if neccessary
        }

        private GetValueResult TryDeserialize<T>(string data, out T result, T defaultValue = default(T))
        {
            result = defaultValue;
            try
            {
                return ValueSerializer.Deserialize(data, out result, defaultValue);
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
}