using System;

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

    /// <summary>
    /// This iterface provides the basic structure of a Settings Manager that holds settings locally on the computer
    /// </summary>
    public interface ISettingsManager : IPropteryValueManager
    {
        /// <summary>
        /// Fired when the storage location of the settings file was changed
        /// </summary>
        event EventHandler SettingsLocationChanged;

        /// <summary>
        /// Fired when the settings manager was completely initialized
        /// </summary>
        event EventHandler Initialized;

        /// <summary>
        /// An instance the the environment varibales object
        /// </summary>
        IExtendedEnvironmentVarirables EnvironmentVarirables { get; }

        /// <summary>
        /// Changes the current storage location of the settings file
        /// </summary>
        /// <param name="path">The new storage path</param>
        /// <param name="deleteCurrent">Indicates whether the old file should be delete. <see langword="true"/> if yes; <see langword="false"/> if not.</param>
        void ChangeSettingsFileLocation(string path, bool deleteCurrent);

        /// <summary>
        /// Creates a new settings file at the current spezified storage location path.
        /// </summary>
        void CreateNewSettingsFile();

        /// <summary>
        /// Deletes the current settings file
        /// </summary>
        void DeleteCurrentSettingsFile();

        /// <summary>
        /// Loads or Creates a SettingsFile and stores it's values into the SettingsCategories. 
        /// Also updates the existing SettingsFile if new SettingsCategories have been added
        /// </summary>
        void Initialize();

        /// <summary>
        /// Loads the current settings file into memory
        /// </summary>
        void LoadCurrent();

        /// <summary>
        /// Stores all settings into the settings file
        /// </summary>
        void SaveCurrent();

        /// <summary>
        /// Closes and removes the settings file from memory
        /// </summary>
        void Close();
    }
}
