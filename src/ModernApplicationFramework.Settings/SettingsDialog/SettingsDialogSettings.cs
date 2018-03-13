using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Settings.SettingsDialog
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(ISettingsDialogSettings))]
    internal class SettingsDialogSettings : SettingsDataModel, ISettingsDialogSettings
    {
        public int SettingsDialogWidth
        {
            get => GetOrCreatePropertyValueSetting(nameof(SettingsDialogWidth), 0);
            set
            {
                if (SettingsDialogWidth == value || !TrySetLocalInt32Setting(nameof(SettingsDialogWidth), value))
                    return;
                OnPropertyChanged();
            }
        }

        public int SettingsDialogHeight
        {
            get => GetOrCreatePropertyValueSetting(nameof(SettingsDialogHeight), 0);
            set
            {
                if (SettingsDialogHeight == value || !TrySetLocalInt32Setting(nameof(SettingsDialogHeight), value))
                    return;
                OnPropertyChanged();
            }
        }

        public override ISettingsCategory Category => SettingsCategories.SettingsDialogSubCategory;

        public override string Name => "SettingsDialog";

        [ImportingConstructor]
        public SettingsDialogSettings(ISettingsManager settingsManager) : base(settingsManager)
        {
        }

        public override void LoadOrCreate()
        {
        }

        public override void StoreSettings()
        {
            SetPropertyValue(nameof(SettingsDialogWidth), SettingsDialogWidth);
            SetPropertyValue(nameof(SettingsDialogHeight), SettingsDialogHeight);
        }

        private bool TrySetLocalInt32Setting(string settingsName, int value)
        {
            try
            {
                SetPropertyValue(settingsName, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
