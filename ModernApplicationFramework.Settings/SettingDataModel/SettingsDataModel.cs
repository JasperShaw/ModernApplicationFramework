using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using ModernApplicationFramework.Settings.Interfaces;
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


        protected void SetDocument(XmlDocument document)
        {
            SettingsManager.SetDocumentAsync(SettingsFilePath, document);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}