using System.IO;
using ModernApplicationFramework.Basics.ApplicationEnvironment;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Extended.ApplicationEnvironment
{
    public abstract class ExtendedEnvironmentVarirables : AbstractEnvironmentVarirables, IExtendedEnvironmentVarirables
    {
        protected const string DefaultSettingsDirectoryKey = "DefaultSettingsDirectory";
        protected const string SettingsFilePathKey = "SaveFile";

        private string _settingsFilePath;

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


        protected virtual string DefaultSettingsDirectory => Path.Combine(ApplicationUserDirectoryKey, "settings");

        protected virtual string DefaultSettingsFilePath => Path.Combine(DefaultSettingsDirectory, "CurrentSettings.mafsettings");

        protected override void SetupProfile()
        {
            var profilePath = Path.Combine(RegistryRootPath, ProfileKey);
            SetupRegistryPath(profilePath, DefaultSettingsDirectoryKey, DefaultSettingsDirectory, SettingsDirectoryKey);
            _settingsFilePath = SetupRegistryPath(profilePath, SettingsFilePathKey, DefaultSettingsFilePath);
        }
    }
}