using System;
using System.ComponentModel.Composition;
using System.Xml;
using System.Xml.Serialization;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.KeyBindingScheme
{
    [Export(typeof(ISettingsDataModel))]
    public class KeyBindingsSettings : SettingsDataModel
    {
        public override ISettingsCategory Category => EnvironmentGroupSettingsCategories.KeyBindingsCategory;
        public override string Name => string.Empty;
        
        public KeyboardShortcutsSettingsCategory ShortcutsSettings { get; set; }

        [ImportingConstructor]
        public KeyBindingsSettings(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public override void LoadOrCreate()
        {
            var c = new KeyboardShortcutsSettingsCategory();

            c.Name = "Test";
            c.Version = "1.1.5";
            c.RegisteredName = "Test";
            c.Category = Guid.NewGuid().ToString("B");

            var detailDocument = new XmlDocument();
            var nav = detailDocument.CreateNavigator();

            using (var writer = nav.AppendChild())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var ser = new XmlSerializer(typeof(KeyboardShortcutsSettingsCategory));
                ser.Serialize(writer, c, ns);
            }

            SetDocument(detailDocument);
        }

        public override void StoreSettings()
        {
        }
    }

    public static class EnvironmentGroupSettingsCategories
    {
        [Export] public static ISettingsCategory KeyBindingsCategory =
            new SettingsCategory(GuidCollection.KeyBindingSettingsCategoryId, SettingsCategoryType.Normal,
                "Environment_KeyBindings", SettingsCategories.EnvironmentCategory);
    }
}
