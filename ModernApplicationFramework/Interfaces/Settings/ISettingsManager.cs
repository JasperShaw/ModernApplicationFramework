using System;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Interfaces.Settings
{
    public enum GetValueResult
    {
        Success,
        Created,
        Missing,
        Corrupt,
        IncompatibleType,
        ObsoleteFormat,
        UnknownError
    }

    public interface IPropteryValueManager
    {
        GetValueResult GetOrCreatePropertyValue<T>(string propertyPath, string propertyName, out T value, T defaultValue,
            bool navigateAttributeWise, bool createNew);

        GetValueResult GetPropertyValue<T>(string propertyPath, string propertyName, out T value,
            bool navigateAttributeWise);

        Task SetPropertyValueAsync(string path, string propertyName, string value, bool navigateAttributeWise);
    }

    public interface ISettingsManager : IPropteryValueManager
    {
        event EventHandler SettingsLocationChanged;
        event EventHandler Initialized;

        IExtendedEnvironmentVarirables EnvironmentVarirables { get; }

        void ChangeSettingsFileLocation(string path, bool deleteCurrent);

        void CreateNewSettingsFile();

        void DeleteCurrentSettingsFile();

        /// <summary>
        /// Loads or Creates a SettingsFile and stores it's values into the SettingsCategories. 
        /// Also updates the existing SettingsFile if new SettingsCategories have been added
        /// </summary>
        void Initialize();

        void LoadCurrent();

        void SaveCurrent();

        void Close();
    }
}
