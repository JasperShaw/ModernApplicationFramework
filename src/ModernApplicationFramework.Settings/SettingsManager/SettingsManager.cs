using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Caliburn.Micro;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Utilities;
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

        public IExtendedEnvironmentVariables EnvironmentVariables { get; }
        public bool IsInitialized { get; private set; }

        protected ISettingsFile SettingsFile { get; set; }

        protected SettingsValueSerializer ValueSerializer { get; }

        [ImportingConstructor]
        public SettingsManager(IExtendedEnvironmentVariables environmentVariables)
        {
            EnvironmentVariables = environmentVariables;
            ValueSerializer = new SettingsValueSerializer();
            Initialized += SettingsManager_Initialized;
        }

        public virtual void CreateNewSettingsFile()
        {
            CreateNewSettingsFileInternal(EnvironmentVariables.SettingsFilePath);
        }

        public virtual void LoadCurrent()
        {
            SettingsFile = SettingsFactory.Open(EnvironmentVariables.SettingsFilePath, this);
        }

        public void SaveCurrent()
        {
            SettingsFile?.Save();
        }

        public void Close()
        {
            var settingsModels = IoC.GetAll<ISettingsDataModel>();
            foreach (var settingsModel in settingsModels)
                settingsModel.StoreSettings();
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
                if (!File.Exists(EnvironmentVariables.SettingsFilePath))
                    CreateNewSettingsFileInternal(path);
                else
                    File.Copy(EnvironmentVariables.SettingsFilePath, path, true);

                if (deleteCurrent)
                    DeleteCurrentSettingsFile();
                EnvironmentVariables.SettingsFilePath = path;
            }
            SettingsLocationChanged?.Invoke(this, EventArgs.Empty);
        }

        public void DeleteCurrentSettingsFile()
        {
            if (File.Exists(EnvironmentVariables.SettingsFilePath))
                File.Delete(EnvironmentVariables.SettingsFilePath);
        }

        public void Initialize()
        {
            if (!File.Exists(EnvironmentVariables.SettingsFilePath))
                CreateNewSettingsFile();
            else
                LoadCurrent();

            var settingsModels = IoC.GetAll<ISettingsDataModel>();
            var settingsCategories = IoC.GetAll<ISettingsCategory>();
            foreach (var settingsCategory in settingsCategories)
                SettingsFile.AddCategoryElement(settingsCategory);

            foreach (var settingsModel in settingsModels)
            {
                if (SettingsFile.GetSingleNode(settingsModel.SettingsFilePath) == null)
                {
                    //if (settingsModel.Category.IsToolsOptionsCategory)
                    SettingsFile.AddToolsOptionsModelElement(settingsModel.Name, settingsModel.Category.Name);
                    //else
                    //    throw new NotImplementedException();
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

        public Task SetDocumentAsync(string path, XmlDocument document, bool insertRootNode)
        {
            var task = new Task(() =>
            {
                SettingsFile.InsertDocument(path, document, insertRootNode);
            });
            task.Start();
            return task;
        }

        public XmlNode GetDataModelNode(string settingsFilePath)
        {
            return SettingsFile.GetSingleNode(settingsFilePath);
        }

        public Task RemoveModelAsync(string settingsFilePath)
        {
            var task = new Task(() =>
            {
                SettingsFile.RemoveNodeContent(settingsFilePath);
            });
            task.Start();
            return task;
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
            SettingsFile = SettingsFactory.Create(EnvironmentVariables.SettingsFilePath, this);
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
            IsInitialized = true;
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