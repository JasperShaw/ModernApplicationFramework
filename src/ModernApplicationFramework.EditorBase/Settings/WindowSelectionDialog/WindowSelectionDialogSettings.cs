using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Settings;

namespace ModernApplicationFramework.EditorBase.Settings.WindowSelectionDialog
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(IWindowSelectionDialogSettings))]
    internal class WindowSelectionDialogSettings : SettingsDataModel, IWindowSelectionDialogSettings
    {
        [Export]
        public static ISettingsCategory WindowSelectionDialogCategory =
            new SettingsCategory(Guids.WindowSelectionDialogSettingsCategoryId,
                SettingsCategoryType.Normal, "Environment_WindowSelectionDialog", SettingsCategories.EnvironmentCategory);

        public override ISettingsCategory Category => WindowSelectionDialogCategory;

        public override string Name => "WindowSelectionDialog";

        [ImportingConstructor]
        public WindowSelectionDialogSettings(ISettingsManager settingsManager) : base(settingsManager)
        {
        }

        public int WindowSelectionDialogWidth
        {
            get => GetOrCreatePropertyValueSetting(nameof(WindowSelectionDialogWidth), 0);
            set
            {
                if (WindowSelectionDialogWidth == value || !TrySetLocalInt32Setting(nameof(WindowSelectionDialogWidth), value))
                    return;
                OnPropertyChanged();
            }
        }

        public int WindowSelectionDialogHeight
        {
            get => GetOrCreatePropertyValueSetting(nameof(WindowSelectionDialogHeight), 0);
            set
            {
                if (WindowSelectionDialogHeight == value || !TrySetLocalInt32Setting(nameof(WindowSelectionDialogHeight), value))
                    return;
                OnPropertyChanged();
            }
        }

        public int NameColumnWidth
        {
            get => GetOrCreatePropertyValueSetting(nameof(NameColumnWidth), 0);
            set
            {
                if (NameColumnWidth == value || !TrySetLocalInt32Setting(nameof(NameColumnWidth), value))
                    return;
                OnPropertyChanged();
            }
        }

        public int? LastSelectedColumnIndex
        {
            get => GetOrCreatePropertyValueSetting(nameof(LastSelectedColumnIndex), 0);
            set
            {
                if (LastSelectedColumnIndex == value || !TrySetLocalNullableIntSetting(nameof(LastSelectedColumnIndex), value))
                    return;
                OnPropertyChanged();
            }
        }


        public ListSortDirection LastSortDirection
        {
            get => (ListSortDirection) GetOrCreatePropertyValueSetting<int>(nameof(LastSortDirection));
            set
            {
                if (LastSortDirection == value || !TrySetLocalEnumSetting(nameof(LastSortDirection), value))
                    return;
                OnPropertyChanged();
            }
        }

        public override void StoreSettings()
        {
            SetPropertyValue(nameof(WindowSelectionDialogWidth), WindowSelectionDialogWidth);
            SetPropertyValue(nameof(WindowSelectionDialogHeight), WindowSelectionDialogHeight);
            SetPropertyValue(nameof(NameColumnWidth), NameColumnWidth);
            SetPropertyValue(nameof(LastSelectedColumnIndex), LastSelectedColumnIndex);
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

        private bool TrySetLocalNullableIntSetting(string settingsName, int? value)
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

        private bool TrySetLocalEnumSetting(string settingsName, ListSortDirection value)
        {
            try
            {
                SetPropertyValue(settingsName, (int) value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public interface IWindowSelectionDialogSettings
    {
        int WindowSelectionDialogWidth { get; set; }

        int WindowSelectionDialogHeight { get; set; }

        int NameColumnWidth { get; set; }

        int? LastSelectedColumnIndex { get; set; }

        ListSortDirection LastSortDirection { get; set; }
    }
}
