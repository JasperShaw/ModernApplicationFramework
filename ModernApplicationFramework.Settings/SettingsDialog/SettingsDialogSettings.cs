using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Settings.SettingsDialog
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(ISettingsDialogSettings))]
    internal class SettingsDialogSettings : AbstractSettingsDataModel, ISettingsDialogSettings
    {
        public int SettingsDialogWidth
        {
            get => GetOrCreateSettingsValue(nameof(SettingsDialogWidth), 0);
            set
            {
                if (SettingsDialogWidth == value || !TrySetLocalInt32Setting(nameof(SettingsDialogWidth), value))
                    return;
                OnPropertyChanged();
            }
        }

        public int SettingsDialogHeight
        {
            get => GetOrCreateSettingsValue(nameof(SettingsDialogHeight), 0);
            set
            {
                if (SettingsDialogHeight == value || !TrySetLocalInt32Setting(nameof(SettingsDialogHeight), value))
                    return;
                OnPropertyChanged();
            }
        }

        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;

        public override string Name => "SettingsDialog";

        [ImportingConstructor]
        public SettingsDialogSettings(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public override void LoadOrCreate()
        {
        }

        public override void StoreSettings()
        {
            SetSettingsValue(nameof(SettingsDialogWidth), SettingsDialogWidth);
            SetSettingsValue(nameof(SettingsDialogHeight), SettingsDialogHeight);
        }

        private bool TrySetLocalInt32Setting(string settingsName, int value)
        {
            try
            {
                SetSettingsValue(settingsName, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
