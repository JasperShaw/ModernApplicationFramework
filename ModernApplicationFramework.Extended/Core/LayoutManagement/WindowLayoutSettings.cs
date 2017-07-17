using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.SettingsBase;
using ModernApplicationFramework.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.Core.LayoutManagement
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(IWindowLayoutSettings))]
    internal class WindowLayoutSettings : AbstractSettingsDataModel, IWindowLayoutSettings
    {

        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;
        public override string Name => "TabsAndWindows";

        public bool SkipApplyLayoutConfirmation
        {
            get => GetSettingsValue(nameof(SkipApplyLayoutConfirmation), false);
            set
            {
                if (SkipApplyLayoutConfirmation == value)
                    return;
                StoreSettingsValue(nameof(SkipApplyLayoutConfirmation), value);
                OnPropertyChanged();
            }
        }
        public int ManageLayoutsDialogWidth
        {
            get => GetSettingsValue(nameof(ManageLayoutsDialogWidth), 0);
            set
            {
                if (ManageLayoutsDialogWidth == value || !TrySetLocalInt32Setting(nameof(ManageLayoutsDialogWidth), value))
                    return;
                OnPropertyChanged();
            }
        }

        public int ManageLayoutsDialogHeight
        {
            get => GetSettingsValue(nameof(ManageLayoutsDialogHeight), 0);
            set
            {
                if (ManageLayoutsDialogHeight == value || !TrySetLocalInt32Setting(nameof(ManageLayoutsDialogHeight), value))
                    return;
                OnPropertyChanged();
            }
        }

        [ImportingConstructor]
        public WindowLayoutSettings(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public override void LoadOrCreate()
        {
        }

        public override void StoreSettings()
        {
            StoreSettingsValue(nameof(ManageLayoutsDialogWidth), ManageLayoutsDialogWidth);
            StoreSettingsValue(nameof(ManageLayoutsDialogHeight), ManageLayoutsDialogHeight);
            StoreSettingsValue(nameof(SkipApplyLayoutConfirmation), SkipApplyLayoutConfirmation);
        }

        private bool TrySetLocalInt32Setting(string settingsName, int value)
        {
            try
            {
                StoreSettingsValue(settingsName, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}