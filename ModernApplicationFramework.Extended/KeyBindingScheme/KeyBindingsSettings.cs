using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.KeyBindingScheme
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(KeyBindingsSettings))]
    public sealed class KeyBindingsSettings : SettingsDataModel
    {
        public override ISettingsCategory Category => EnvironmentGroupSettingsCategories.KeyBindingsCategory;
        public override string Name => string.Empty;

        public string Version
        {
            get => ShortcutsSettings.Version;
            set => ShortcutsSettings.Version = value;
        }

        public TestKeyboardShortcuts KeyboardShortcuts => ShortcutsSettings.KeyboardShortcuts;

        private KeyBindingsSettingsDataModel ShortcutsSettings { get; set; }

        [ImportingConstructor]
        public KeyBindingsSettings(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public override void LoadOrCreate()
        {
            GetDataModel<KeyBindingsSettingsDataModel>(out var model);
            if (model == null || model.Version == null && model.KeyboardShortcuts == null)
            {
                model = new KeyBindingsSettingsDataModel();
                model.KeyboardShortcuts = new TestKeyboardShortcuts();
            }
            ShortcutsSettings = model;
            Version = IoC.Get<IEnvironmentVariables>().ApplicationVersion;
            StoreSettings();
        }

        public override void StoreSettings()
        {
            SetSettingsModel(ShortcutsSettings);
        }
    }

    public static class EnvironmentGroupSettingsCategories
    {
        [Export] public static ISettingsCategory KeyBindingsCategory =
            new SettingsCategory(GuidCollection.KeyBindingSettingsCategoryId, SettingsCategoryType.Normal,
                "Environment_KeyBindings", SettingsCategories.EnvironmentCategory);
    }
}
