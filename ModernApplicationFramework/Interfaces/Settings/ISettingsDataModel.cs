using System.ComponentModel;

namespace ModernApplicationFramework.Interfaces.Settings
{
    public interface ISettingsDataModel : INotifyPropertyChanged
    {
        ISettingsCategory Category { get; }

        string Name { get; }

        string SettingsFilePath { get; }

        /// <summary>
        /// Loads all settings entries from the settings file or creates them if they don't exist.
        /// </summary>
        void LoadOrCreate();

        /// <summary>
        /// Stores all settings into the Settings file. 
        /// <remarks>This should not write the file to disk due to perfomence and possible mutexes.</remarks>
        /// </summary>
        void StoreSettings();
    }
}