using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.Core.LayoutManagement
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(IWindowLayoutSettings))]
    internal class WindowLayoutSettings : SettingsDataModel, IWindowLayoutSettings
    {
        public override ISettingsCategory Category => Settings.ToolsOptionsSubCategories.TabsAndWindowsSubCategory;
        public override string Name => "TabsAndWindows";

        public bool SkipApplyLayoutConfirmation
        {
            get => GetOrCreatePropertyValueSetting(nameof(SkipApplyLayoutConfirmation), false);
            set
            {
                if (SkipApplyLayoutConfirmation == value)
                    return;
                SetPropertyValue(nameof(SkipApplyLayoutConfirmation), value);
                OnPropertyChanged();
            }
        }
        public int ManageLayoutsDialogWidth
        {
            get => GetOrCreatePropertyValueSetting(nameof(ManageLayoutsDialogWidth), 0);
            set
            {
                if (ManageLayoutsDialogWidth == value || !TrySetLocalInt32Setting(nameof(ManageLayoutsDialogWidth), value))
                    return;
                OnPropertyChanged();
            }
        }

        public int ManageLayoutsDialogHeight
        {
            get => GetOrCreatePropertyValueSetting(nameof(ManageLayoutsDialogHeight), 0);
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
            SetPropertyValue(nameof(ManageLayoutsDialogWidth), ManageLayoutsDialogWidth);
            SetPropertyValue(nameof(ManageLayoutsDialogHeight), ManageLayoutsDialogHeight);
            SetPropertyValue(nameof(SkipApplyLayoutConfirmation), SkipApplyLayoutConfirmation);
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