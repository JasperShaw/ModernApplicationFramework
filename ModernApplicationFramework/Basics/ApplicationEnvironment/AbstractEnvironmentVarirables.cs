using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.ApplicationEnvironment
{
    /// <summary>
    /// Basic implementation of a service that handles the application's environment variables
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Interfaces.IEnvironmentVarirables" />
    public abstract class AbstractEnvironmentVarirables : IEnvironmentVarirables
    {
        protected const string ApplicationLocationKey = "ApplicationLocation";
        protected const string ProfileKey = "Profile";

        private string _registryRootPath;

        protected AbstractEnvironmentVarirables()
        {
            EnvironmentVariables = new Dictionary<string, string>();
        }

        /// <summary>
        /// The name of the running application
        /// </summary>
        public abstract string ApplicationName { get; }

        /// <summary>
        /// A constant key where the user directory of the application is stored
        /// </summary>
        public string ApplicationUserDirectoryKey => "%maf_application_dir%";

        /// <summary>
        /// The version of the running application
        /// </summary>
        public virtual string ApplicationVersion => GetType().Assembly.GetName().Version.ToString(2);

        /// <summary>
        /// Gets all stored environment variables.
        /// </summary>
        public Dictionary<string, string> EnvironmentVariables { get; }


        /// <summary>
        /// Gets the default user application directory.
        /// </summary>
        protected virtual string DefaultApplicationDirectory => Path.Combine("%USERPROFILE%\\Documents\\",
            ApplicationName);


        /// <summary>
        /// Gets the default logging directory path.
        /// </summary>
        protected virtual string DefaultLoggingDirectoryPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);


        /// <summary>
        /// Gets the application's HKCU registry root path.
        /// </summary>
        protected virtual string RegistryRootPath => _registryRootPath ??
                                                  (_registryRootPath =
                                                      $"Software\\{ApplicationName.Replace(" ", string.Empty)}\\{ApplicationVersion}");


        /// <summary>
        /// Replaces the name of each application and operation system environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
        /// </summary>
        /// <param name="name">A string containing the names of zero or more environment variables.
        /// Each environment variable is quoted with the percent sign character (%).</param>
        /// <returns>
        /// A string with each environment variable replaced by its value.
        /// </returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public string ExpandEnvironmentVariables(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name.Length == 0)
                return name;

            var blob = new StringBuilder(100);

            var varArray = name.Split('%');
            foreach (var part in varArray)
            {
                if (part.Length == 0)
                    continue;
                var envVar = "%" + part + "%";
                blob.Append(GetExpandedRecursive(envVar));
            }
            return blob.ToString();
        }

        /// <summary>
        /// Gets or creates a HKCU registry entry
        /// </summary>
        /// <param name="key">The key of the registry entry</param>
        /// <param name="path">The relative registry path to the entry.
        /// When <see cref="path" /> is <see langword="null" /> only the root path will be used</param>
        /// <param name="defaultValue">The value which should be used when the entry must be created</param>
        /// <returns>
        /// The value of the registry entry
        /// </returns>
        public string GetOrCreateRegistryVariable(string key, string path, string defaultValue)
        {
            var fullPath = path == null ? RegistryRootPath : Path.Combine(RegistryRootPath, path);
            var value = RegirstryTools.GetValueCurrentUserRoot(fullPath, key);
            if (value == null)
            {
                value = defaultValue;
                RegirstryTools.SetValueCurrentUserRoot(fullPath, key, defaultValue);
            }

            return value.ToString();
        }

        /// <summary>
        /// Sets a HKCU regirsty entry
        /// </summary>
        /// <param name="key">The key of the registry entry</param>
        /// <param name="value">The value to set</param>
        /// <param name="path">The relative registry path to the entry.
        /// When <see cref="path" /> is <see langword="null" /> only the root path will be used</param>
        public void SetRegistryVariable(string key, string value, string path)
        {
            var fullPath = path == null ? RegistryRootPath : Path.Combine(RegistryRootPath, path);
            RegirstryTools.SetValueCurrentUserRoot(fullPath, key, value);
        }

        /// <summary>
        /// Gets an evironment variable
        /// </summary>
        /// <param name="key">The the key of the variable</param>
        /// <param name="value">Pointer to the value of the variable. Is <see langword="null" /> of key was not found</param>
        /// <returns>
        /// Status whether the operation was successful
        /// </returns>
        public virtual bool GetEnvironmentVariable(string key, out string value)
        {
            if (!EnvironmentVariables.ContainsKey(key))
            {
                value = null;
                return false;
            }
            EnvironmentVariables.TryGetValue(key, out value);
            return true;
        }

        /// <summary>
        /// Sets the provider up
        /// </summary>
        public virtual void Setup()
        {
            if (!RegirstryTools.ExistsCurrentUserRoot(RegistryRootPath))
                RegirstryTools.CreateCurrentUserRoot(RegistryRootPath);
            SetupApplicationLocation();
            SetupProfile();
        }

        /// <summary>
        /// Setups the application location.
        /// </summary>
        protected virtual void SetupApplicationLocation()
        {
            SetupRegistryPath(RegistryRootPath, ApplicationLocationKey, DefaultApplicationDirectory,
                ApplicationUserDirectoryKey);
        }

        /// <summary>
        /// Setups the profile.
        /// </summary>
        protected virtual void SetupProfile()
        {
        }


        /// <summary>
        /// Setups the registry path.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="regKeyName">Name of the registry key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="environmentVariableKey">The environment variable key.</param>
        /// <returns></returns>
        protected string SetupRegistryPath(string rootPath, string regKeyName, string defaultValue,
            string environmentVariableKey = null)
        {
            string result;

            if (!RegirstryTools.ExistsCurrentUserRoot(rootPath))
                RegirstryTools.CreateCurrentUserRoot(rootPath);

            var keyValue = RegirstryTools.GetValueCurrentUserRoot(rootPath, regKeyName);
            if (keyValue == null)
            {
                RegirstryTools.SetValueCurrentUserRoot(rootPath, regKeyName, defaultValue);
                result = defaultValue;
            }
            else
            {
                result = keyValue.ToString();
            }
            if (!string.IsNullOrEmpty(environmentVariableKey))
                EnvironmentVariables.Add(environmentVariableKey, result);
            return result;
        }

        private string GetExpandedRecursive(string part)
        {
            if (part.Length == 0)
                return part;
            var array = part.Split('%');
            if (array.Length <= 1)
                return part;

            if (GetEnvironmentVariable(part, out string newPart))
                return GetExpandedRecursive(newPart);
            newPart = Environment.ExpandEnvironmentVariables(part);
            return newPart == part ? newPart.Replace("%", "") : newPart;
        }
    }
}