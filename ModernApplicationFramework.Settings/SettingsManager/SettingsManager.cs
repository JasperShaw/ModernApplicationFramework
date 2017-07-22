using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces.Settings;
using ModernApplicationFramework.Utilities.Settings;
using ISettingsDataModel = ModernApplicationFramework.Settings.Interfaces.ISettingsDataModel;

namespace ModernApplicationFramework.Settings.SettingsManager
{
    [Export(typeof(ISettingsManager))]
    public class SettingsManager : ISettingsManager
    {
        private readonly object _lockObject = new object();

        public event EventHandler SettingsLocationChanged;
        public event EventHandler Initialized;

        public IExtendedEnvironmentVarirables EnvironmentVarirables { get; }

        protected ISettingsFile SettingsFile { get; set; }

        protected SettingsValueSerializer ValueSerializer { get; }

        [ImportingConstructor]
        public SettingsManager(IExtendedEnvironmentVarirables environmentVarirables)
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
            SettingsFile?.Save();
        }

        public void Close()
        {
            SettingsFile?.Dispose();
            SettingsFile = null;
        }

        public void ChangeSettingsFileLocation(string path, bool deleteCurrent)
        {
            lock (_lockObject)
            {
                var newDirectoryPath = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(newDirectoryPath))
                    throw new ArgumentNullException();
                new FileInfo(path).Directory.Create();
                if (!File.Exists(EnvironmentVarirables.SettingsFilePath))
                    CreateNewSettingsFileInternal(path);
                else
                    File.Copy(EnvironmentVarirables.SettingsFilePath, path, true);

                if (deleteCurrent)
                    DeleteCurrentSettingsFile();
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
            bool navigateAttributeWise = true)
        {
            return GetOrCreatePropertyValueInternal(propertyPath, propertyName, out value, true, defaultValue, navigateAttributeWise);
        }

        public GetValueResult GetPropertyValue<T>(string propertyPath, string propertyName, out T value,
            bool navigateAttributeWise = true)
        {
            return GetOrCreatePropertyValueInternal(propertyPath, propertyName, out value, false, default(T), navigateAttributeWise);
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

        protected GetValueResult GetOrCreatePropertyValueInternal<T>(string propertyPath, string propertyName,
            out T value, bool createNew, T defaultValue = default(T), bool navigateAttributeWise = true)
        {
            value = defaultValue;
            var data = SettingsFile.GetPropertyValueData(propertyPath, propertyName, navigateAttributeWise);
            //Could not find, but we can create a PropertyValue element
            if (data == null && createNew)
            {
                var node = SettingsFile.GetSingleNode(propertyPath);
                if (node == null)
                    return GetValueResult.Missing;
                SettingsFile.AddPropertyValueElement(node, propertyName, value?.ToString());
                return GetValueResult.Created;
            }
            //Don't create PropertyValue if data is null
            return data == null ? GetValueResult.Missing : TryGetValue(data, out value, defaultValue);
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