using System;
using System.Threading.Tasks;
using System.Xml;

namespace ModernApplicationFramework.Utilities.Interfaces.Settings
{
    /// <inheritdoc />
    /// <summary>
    /// This interface provides the basic structure of a Settings Manager that holds settings locally on the computer
    /// </summary>
    public interface ISettingsManager : IPropertyValueManager
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
        /// An instance the the environment variables object
        /// </summary>
        IExtendedEnvironmentVariables EnvironmentVariables { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Changes the current storage location of the settings file
        /// </summary>
        /// <param name="path">The new storage path</param>
        /// <param name="deleteCurrent">Indicates whether the old file should be delete. <see langword="true"/> if yes; <see langword="false"/> if not.</param>
        void ChangeSettingsFileLocation(string path, bool deleteCurrent);

        /// <summary>
        /// Creates a new settings file at the current specified storage location path.
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

        /// <summary>
        /// Inserts a <see cref="XmlDocument"/> into the settings file
        /// </summary>
        /// <param name="path">The path where to insert the document.</param>
        /// <param name="document">The document.</param>
        /// <param name="insertRootNode">if set to <see langword="true"/> the root element of the document will also be inserted</param>
        /// <returns></returns>
        Task SetDocumentAsync(string path, XmlDocument document, bool insertRootNode);

        /// <summary>
        /// Gets a <see cref="XmlNode"/> from settings file
        /// </summary>
        /// <param name="settingsFilePath">The path where to expect the node</param>
        /// <returns>The <see cref="XmlNode"/></returns>
        XmlNode GetDataModelNode(string settingsFilePath);

        /// <summary>
        /// Removes a complete subtree of the settings file
        /// </summary>
        /// <param name="settingsFilePath">The path where to expect the subtree</param>
        /// <returns></returns>
        Task RemoveModelAsync(string settingsFilePath);
    }
}
