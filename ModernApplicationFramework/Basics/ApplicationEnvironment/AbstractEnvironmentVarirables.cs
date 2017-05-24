using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.ApplicationEnvironment
{
    public abstract class AbstractEnvironmentVarirables : IEnvironmentVarirables
    {
        protected const string ApplicationLocationKey = "ApplicationLocation";
        protected const string DefaultSettingsDirectoryKey = "DefaultSettingsDirectory";
        protected const string ProfileKey = "Profile";
        protected const string SettingsFilePathKey = "SaveFile";

        private string _registryRootPath;
        private string _settingsFilePath;


        protected AbstractEnvironmentVarirables()
        {
            EnvironmentVariables = new Dictionary<string, string>();
        }

        public abstract string ApplicationName { get; }

        public string ApplicationUserDirectoryKey => "%maf_application_dir%";

        public virtual string ApplicationVersion => GetType().Assembly.GetName().Version.ToString(2);

        public Dictionary<string, string> EnvironmentVariables { get; }

        public string SettingsDirectoryKey => "%maf_settings_directory%";


        public string SettingsFilePath
        {
            get => ExpandEnvironmentVariables(_settingsFilePath);
            set
            {
                if (_settingsFilePath == value)
                    return;
                _settingsFilePath = value;
                RegirstryTools.SetValueCurrentUserRoot(Path.Combine(RegistryRootPath, ProfileKey), SettingsFilePathKey, value);
            }
        }

        protected virtual string DefaultApplicationDirectory => Path.Combine("%USERPROFILE%\\Documents\\",
            ApplicationName);


        protected virtual string DefaultLoggingDirectoryPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

        protected virtual string DefaultSettingsDirectory => Path.Combine(ApplicationUserDirectoryKey, "settings");

        protected virtual string DefaultSettingsFilePath => Path.Combine(DefaultSettingsDirectory, "CurrentSettings.mafsettings");

        protected virtual string RegistryRootPath => _registryRootPath ??
                                                  (_registryRootPath =
                                                      $"Software\\{ApplicationName.Replace(" ", string.Empty)}\\{ApplicationVersion}");


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

        public virtual void Setup()
        {
            if (!RegirstryTools.ExistsCurrentUserRoot(RegistryRootPath))
                RegirstryTools.CreateCurrentUserRoot(RegistryRootPath);
            SetupApplicationLocation();
            SetupProfile();
        }

        protected virtual void SetupApplicationLocation()
        {
            SetupRegistryPath(RegistryRootPath, ApplicationLocationKey, DefaultApplicationDirectory,
                ApplicationUserDirectoryKey);
        }

        protected virtual void SetupProfile()
        {
            var profilePath = Path.Combine(RegistryRootPath, ProfileKey);
            SetupRegistryPath(profilePath, DefaultSettingsDirectoryKey, DefaultSettingsDirectory, SettingsDirectoryKey);
            _settingsFilePath = SetupRegistryPath(profilePath, SettingsFilePathKey, DefaultSettingsFilePath);
        }


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