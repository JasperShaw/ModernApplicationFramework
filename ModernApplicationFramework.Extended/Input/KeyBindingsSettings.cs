using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.Input
{
    [Export(typeof(ISettingsDataModel))]
    public class KeyBindingsSettings : AbstractSettingsDataModel
    {
        public override ISettingsCategory Category => EnvironmentGroupSettingsCategories.KeyBindingsCategory;
        public override string Name => string.Empty;
        
        public string Version { get; set; }

        [ImportingConstructor]
        public KeyBindingsSettings(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public override void LoadOrCreate()
        {
            GetOrCreatePropertyValueSetting("Version", "Test");
        }

        public override void StoreSettings()
        {
        }
    }

    public static class EnvironmentGroupSettingsCategories
    {
        [Export] public static SettingsCategory EnvironmentCategory =
            new SettingsCategory("Environment_Group", string.Empty, 0, false);

        [Export]
        public static SettingsCategory KeyBindingsCategory =
            new SettingsCategory("Environment_KeyBindings", string.Empty, 0, EnvironmentCategory, false);
    }
}
