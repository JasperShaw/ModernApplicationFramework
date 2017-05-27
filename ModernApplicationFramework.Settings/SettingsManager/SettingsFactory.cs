using System;
using System.IO;
using System.Xml;
using ModernApplicationFramework.Interfaces.Settings;

namespace ModernApplicationFramework.Settings.SettingsManager
{
    public static class SettingsFactory
    {
        public static ISettingsFile Open(string path, ISettingsManager settingsManager)
        {
            if (!File.Exists(path))
                throw new SettingsManagerException("Could not load the settings file, as it does not exists");
            var settingsFile = new UserSettingsFile(settingsManager);
            settingsFile.TryRead(path);
            return settingsFile;
        }

        public static ISettingsFile Create(string path, ISettingsManager settingsManager, XmlDocument presetDocument)
        {
            if (presetDocument == null)
                throw new ArgumentNullException();
            presetDocument.Save(path);
            return new UserSettingsFile(settingsManager) {SettingsSotrage = presetDocument};
        }

        public static ISettingsFile Create(string path, ISettingsManager settingsManager)
        {
            var settingsFile = new UserSettingsFile(settingsManager);
            var document = settingsFile.CreateNewSettingsStore();
            document.Save(path);
            return settingsFile;
        }
    }
}