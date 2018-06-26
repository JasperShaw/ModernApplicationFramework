using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.Input.KeyBindingScheme
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

        public KeyboardShortcutsObject KeyboardShortcuts => ShortcutsSettings.KeyboardShortcuts;

        private KeyBindingsSettingsDataModel ShortcutsSettings { get; set; }

        [ImportingConstructor]
        public KeyBindingsSettings(ISettingsManager settingsManager) : base(settingsManager)
        {
        }

        public override void LoadOrCreate()
        {
            GetSingleDataModel<KeyBindingsSettingsDataModel>(out var model);
            if (model?.KeyboardShortcuts == null)
                model = new KeyBindingsSettingsDataModel { KeyboardShortcuts = new KeyboardShortcutsObject() };
            ShortcutsSettings = model;
            StoreSettings();
        }

        public override void StoreSettings()
        {
            Version = IoC.Get<IEnvironmentVariables>().ApplicationVersion;
            SetSettingsModel(ShortcutsSettings);
        }
    }

    internal static class EnvironmentGroupSettingsCategories
    {
        [Export] public static ISettingsCategory KeyBindingsCategory =
            new SettingsCategory(SettingsGuids.KeyBindingSettingsCategoryId, SettingsCategoryType.Normal,
                "Environment_KeyBindings", SettingsCategories.EnvironmentCategory);
    }
}
