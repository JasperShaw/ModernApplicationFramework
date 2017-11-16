using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Settings.SettingDataModel
{
    /// <summary>
    /// Abstract implementation of an <see cref="ISettingsDataModel" /></summary>
    /// <seealso cref="ISettingsDataModel" />
    /// <inheritdoc />
    /// <seealso cref="ISettingsDataModel" />
    public abstract class SettingsDataModel : ISettingsDataModel
    {
        public event EventHandler SettingsChanged;


        /// <inheritdoc />
        /// <summary>
        /// The category of the data model
        /// </summary>
        public abstract ISettingsCategory Category { get; }

        /// <inheritdoc />
        /// <summary>
        /// The name of the data model
        /// </summary>
        public abstract string Name { get; }

        /// <inheritdoc />
        /// <summary>
        /// The path of the setting inside the document
        /// </summary>
        public virtual string SettingsFilePath => $"{Category.Name}";

        /// <summary>
        /// The instance an implementation of an <see cref="ISettingsManager"/>
        /// </summary>
        protected ISettingsManager SettingsManager { get; set; }


        /// <inheritdoc />
        /// <summary>
        /// Loads all settings entries from the settings file or creates them if they don't exist.
        /// </summary>
        public abstract void LoadOrCreate();

        /// <inheritdoc />
        /// <summary>
        /// Stores all settings into memory.
        /// <remarks>This should not write the file to disk due to performance and possible mutexes.</remarks>
        /// </summary>
        public abstract void StoreSettings();

        /// <summary>
        /// Fills a property from a settings value. Creates a new settings entry if it was not found
        /// </summary>
        /// <typeparam name="T">The type of Property</typeparam>
        /// <param name="settingsName">Name of the PropertyValue name element</param>
        /// <param name="propertyName">Name of the property on this data model</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidCastException">The type of the Property and the given type T do not match</exception>
        protected void SetClassPropertyFromPropertyValue<T>(string settingsName, string propertyName, T defaultValue = default(T))
        {
            SetClassPropertyFromPropertyValue(this, settingsName, propertyName, defaultValue);
        }

        /// <summary>
        /// Sets a property of different object from settings.
        /// </summary>
        /// <typeparam name="T">The type of Property</typeparam>
        /// <param name="instance">The object which property should be filled</param>
        /// <param name="settingsName">Name of the settings.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidCastException">The type of the Property and the given type T do not match</exception>
        protected void SetClassPropertyFromPropertyValue<T>(object instance, string settingsName, string propertyName,
            T defaultValue = default(T))
        {
            var propertyInfo = instance.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentNullException($"The Property {propertyName} does not exist in this context");
            if (typeof(T) != propertyInfo.PropertyType)
                if (!(propertyInfo.PropertyType.IsEnum && (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))))
                throw new InvalidCastException("The type of the Property and the given type T do not match");

            var result = SettingsManager.GetOrCreatePropertyValue(SettingsFilePath, settingsName, out T value, defaultValue, true);
            propertyInfo.SetValue(instance, value);

            if (result == GetValueResult.Corrupt)
                SetPropertyValue(settingsName, value);
        }

        /// <summary>
        /// Gets or creates a PropertyValue setting value from memory.
        /// </summary>
        /// <typeparam name="T">The expected type of the returned value</typeparam>
        /// <param name="settingsName">Name of the PropertyValue name element</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        protected T GetOrCreatePropertyValueSetting<T>(string settingsName, T defaultValue = default(T))
        {
            var result = SettingsManager.GetOrCreatePropertyValue(SettingsFilePath, settingsName, out T value, defaultValue, true);
            return result == GetValueResult.Corrupt ? defaultValue : value;
        }

        /// <summary>
        /// Gets a PropertyValue setting value from memory.
        /// </summary>
        /// <typeparam name="T">The expected type of the returned value</typeparam>
        /// <param name="settingsName">Name of the PropertyValue name element</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        protected T GetPropertyValueSetting<T>(string settingsName, T defaultValue = default(T))
        {
            var result = SettingsManager.GetPropertyValue(SettingsFilePath, settingsName, out T value, true);
            return result == GetValueResult.Corrupt ? defaultValue : value;
        }

        /// <summary>
        /// Sets a settings value in memory.
        /// </summary>
        /// <typeparam name="T">The type of the value to </typeparam>
        /// <param name="settingsProperty">The settings property.</param>
        /// <param name="value">The value.</param>
        protected void SetPropertyValue<T>(string settingsProperty, T value)
        {
            SettingsManager.SetPropertyValueAsync(SettingsFilePath, settingsProperty, value.ToString(), true);
        }

        /// <summary>
        /// Sets a settings value in memory asynchronous.
        /// </summary>
        /// <typeparam name="T">The type of the value to </typeparam>
        /// <param name="settingsProperty">The settings property.</param>
        /// <param name="value">The value.</param>
        protected async Task SetPropertyValueAsync<T>(string settingsProperty, T value)
        {
            await SettingsManager.SetPropertyValueAsync(SettingsFilePath, settingsProperty, value.ToString(), true);
        }


        /// <summary>
        /// Sets and replaces a XML model into the settings file.
        /// </summary>
        /// <typeparam name="T">The type of the model</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="insertRoot">if set to <see langword="true"/> the XML's root will be inserted also</param>
        protected void SetSettingsModel<T>(T model, bool insertRoot = false)
        {
            SettingsManager.RemoveModelAsync(SettingsFilePath);
            var document = new XmlDocument();
            var nav = document.CreateNavigator();

            using (var writer = nav.AppendChild())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(writer, model, ns);
            }
            SettingsManager.SetDocumentAsync(SettingsFilePath, document, insertRoot);
        }

        /// <summary>
        /// Inserts a XML model into the settings file.
        /// </summary>
        /// <typeparam name="T">The type of the model</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="insertRoot">if set to <see langword="true"/> the XML's root will be inserted also</param>
        protected void InsertSettingsModel<T>(T model, bool insertRoot = false)
        {
            var document = new XmlDocument();
            var nav = document.CreateNavigator();

            using (var writer = nav.AppendChild())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(writer, model, ns);
            }
            SettingsManager.SetDocumentAsync(SettingsFilePath, document, insertRoot);
        }

        /// <summary>
        /// Get a single deserialized model object from the settings file. The stored model will be handled as one model only. 
        /// For getting all stored data models use <see cref="GetAllDataModel{T}"/>
        /// </summary>
        /// <typeparam name="T">The expected type of the model</typeparam>
        /// <param name="model">The model</param>
        protected void GetSingleDataModel<T>(out T model)
        {
            model = default(T);
            var node = SettingsManager.GetDataModelNode(SettingsFilePath);
            var stm = new MemoryStream();
            var stw = new StreamWriter(stm);
            stw.Write(node.OuterXml);
            stw.Flush();
            stm.Position = 0;
            var ser = new XmlSerializer(typeof(T));
            model = (T)ser.Deserialize(stm);
        }

        /// <summary>
        /// Get all deserialized model objects from the settings file
        /// For getting a single stored data model use <see cref="GetDataModelAt{T}"/> or <see cref="GetSingleDataModel{T}"/> if the stored value should be handled as one model
        /// </summary>
        /// <typeparam name="T">The expected type of the models</typeparam>
        /// <param name="models"><see cref="ICollection{T}"/> of the models</param>
        protected void GetAllDataModel<T>(out ICollection<T> models)
        {
            models = new List<T>();
            var node = SettingsManager.GetDataModelNode(SettingsFilePath); 
            foreach (XmlNode childNode in node.ChildNodes)
            {
                var stm = new MemoryStream();
                var stw = new StreamWriter(stm);
                stw.Write(childNode.OuterXml);
                stw.Flush();
                stm.Position = 0;
                var ser = new XmlSerializer(typeof(T));
                var model = (T) ser.Deserialize(stm);
                models.Add(model);
            }
        }

        /// <summary>
        /// Get a single deserialized model object from the settings file. The stored model will be handled as one model only. 
        /// For getting all stored data models use <see cref="GetAllDataModel{T}"/>. For handling the stored data a one model only use <see cref="GetSingleDataModel{T}"/>
        /// </summary>
        /// <typeparam name="T">The expected type of the model</typeparam>
        /// <param name="model">The model</param>
        /// <param name="index">The index of the model</param>
        protected void GetDataModelAt<T>(out T model, int index)
        {
            model = default(T);
            var node = SettingsManager.GetDataModelNode(SettingsFilePath);
            var stm = new MemoryStream();
            var stw = new StreamWriter(stm);
            if (node.ChildNodes[index] == null)
                throw new IndexOutOfRangeException();
            stw.Write(node.ChildNodes[index].OuterXml);
            stw.Flush();
            stm.Position = 0;
            var ser = new XmlSerializer(typeof(T));
            model = (T)ser.Deserialize(stm);
        }

        /// <summary>
        /// Removes all models.
        /// </summary>
        protected void RemoveAllModels()
        {
            SettingsManager.RemoveModelAsync(SettingsFilePath);
        }

        protected void OnSettingsChanged(EventArgs args = null)
        {
            SettingsChanged?.Invoke(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}