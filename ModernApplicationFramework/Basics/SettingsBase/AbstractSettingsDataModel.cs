using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Interfaces.Settings;

namespace ModernApplicationFramework.Basics.SettingsBase
{
    public abstract class AbstractSettingsDataModel : ISettingsDataModel
    {
        public abstract ISettingsCategory Category { get; }
        public abstract string Name { get; }

        public virtual string SettingsFilePath => $"{Category.Name}/{Name}/";

        protected ISettingsManager SettingsManager { get; set; }

        public abstract void LoadOrCreate();

        public abstract void StoreSettings();

        protected void SetPropertyFromSettings<T>(string queryName, string propertyName, T defaultValue = default (T))
        {
            var propertyInfo = GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentNullException($"The Property {propertyName} does not exist in this context");
            if (typeof(T) != propertyInfo.PropertyType)
                throw new InvalidCastException("The type of the Property and the given type T do not match");

            var result = SettingsManager.GetOrCreatePropertyValue(SettingsFilePath, queryName, out T value, defaultValue, true, true);
            propertyInfo.SetValue(this, value);

            if (result == GetValueResult.Corrupt)
                StoreSettingsValue(queryName, value.ToString());
        }

        protected string GetSettingsValue(string queryName, string defaultValue = null)
        {
            var result = SettingsManager.GetOrCreatePropertyValue(SettingsFilePath, queryName, out string value, defaultValue, true, true);
            if (result == GetValueResult.Corrupt)
                return defaultValue;
            return value;
        }

        protected void StoreSettingsValue(string settingsProperty, string value)
        {
            SettingsManager.SetPropertyValueAsync(SettingsFilePath, settingsProperty, value, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}