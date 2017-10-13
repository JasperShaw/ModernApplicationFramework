namespace ModernApplicationFramework.Utilities.Interfaces
{

    /// <inheritdoc />
    /// <summary>
    /// Extends the <see cref="IEnvironmentVariables" /> interface to settings aware variables
    /// </summary>
    public interface IExtendedEnvironmentVariables : IEnvironmentVariables
    {
        /// <summary>
        /// The current storage location of the settings file
        /// </summary>
        string SettingsFilePath { get; set; }

        /// <summary>
        /// The key of the environment variable to the file path
        /// </summary>
        string SettingsDirectoryKey { get; }
    }
}