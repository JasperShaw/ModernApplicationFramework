using System;

namespace ModernApplicationFramework.Utilities.Interfaces
{
    /// <summary>
    /// Provides a service to handle application specific environment variables
    /// </summary>
    public interface IEnvironmentVariables
    {
        /// <summary>
        /// The name of the running application
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// The version of the running application
        /// </summary>
        string ApplicationVersion { get; }

        /// <summary>
        /// A constant key where the user directory of the application is stored
        /// </summary>
        string ApplicationUserDirectoryKey { get; }

        /// <summary>
        /// Sets the provider up
        /// </summary>
        void Setup();

        /// <summary>
        /// Gets an environment variable
        /// </summary>
        /// <param name="key">The the key of the variable</param>
        /// <param name="value">Pointer to the value of the variable. Is <see langword="null"/> of key was not found</param>
        /// <returns>Status whether the operation was successful</returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool GetEnvironmentVariable(string key, out string value);

        /// <summary>
        ///   Replaces the name of each application and operation system environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
        /// </summary>
        /// <param name="name">
        ///   A string containing the names of zero or more environment variables. 
        /// Each environment variable is quoted with the percent sign character (%).
        /// </param>
        /// <returns>
        ///   A string with each environment variable replaced by its value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="name" /> ist <see langword="null" />.
        /// </exception>
        string ExpandEnvironmentVariables(string name);

        /// <summary>
        /// Gets or creates a HKCU registry entry
        /// </summary>
        /// <param name="key">The key of the registry entry</param>
        /// <param name="path">The relative registry path to the entry.
        ///  When <see cref="path"/> is <see langword="null"/> only the root path will be used</param>
        /// <param name="defaultValue">The value which should be used when the entry must be created</param>
        /// <returns>The value of the registry entry</returns>
        string GetOrCreateRegistryVariable(string key, string path, string defaultValue);

        /// <summary>
        /// Sets a HKCU registry entry
        /// </summary>
        /// <param name="key">The key of the registry entry</param>
        /// <param name="value">The value to set</param>
        /// <param name="path">The relative registry path to the entry.
        ///  When <see cref="path"/> is <see langword="null"/> only the root path will be used</param>
        void SetRegistryVariable(string key, string value, string path);
    }
}