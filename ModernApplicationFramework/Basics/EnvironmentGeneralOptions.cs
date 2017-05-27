using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Basics.SettingsBase;
using ModernApplicationFramework.Interfaces.Settings;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(EnvironmentGeneralOptions))]
    public sealed class EnvironmentGeneralOptions : AbstractSettingsDataModel
    {
        public const int MinFileListCount = 1;
        public const int MaxFileListCount = 24;
        public const int MinMruListCount = 1;
        public const int MaxMruListCount = 24;

        private bool _useTitleCaseOnMenu = true;
        private bool _autoAdjustExperience;
        private bool _useHardwareAcceleration;
        private bool _useRichVisualExperience;
        private int _windowListItems = 10;
        private bool _showStatusBar = true;
        private bool _dockedWinClose;
        private bool _dockedWinAuto;
        private int _mruListItems;

        public static EnvironmentGeneralOptions Instance { get; private set; }

        public int VisualEffectsAllowed => RenderCapability.Tier >> 16;

        public bool IsHardwareAccelerationSupported => VisualEffectsAllowed > 0;

        public bool UseTitleCaseOnMenu
        {
            get => _useTitleCaseOnMenu;
            set
            {
                if (value == _useTitleCaseOnMenu)
                    return;
                _useTitleCaseOnMenu = value;
                OnPropertyChanged();
            }
        }

        public bool AutoAdjustExperience
        {
            get => _autoAdjustExperience;
            set
            {
                if (value == _autoAdjustExperience)
                    return;
                _autoAdjustExperience = value;
                OnPropertyChanged();
                UpdateVisualExperience();
            }
        }

        public bool UseHardwareAcceleration
        {
            get => _useHardwareAcceleration;
            set
            {
                if (value == _useHardwareAcceleration)
                    return;
                _useHardwareAcceleration = value;
                OnPropertyChanged();
                UpdateVisualExperience();
            }
        }

        public bool UseRichVisualExperience
        {
            get => _useRichVisualExperience;
            set
            {
                if (value == _useRichVisualExperience)
                    return;
                _useRichVisualExperience = value;
                OnPropertyChanged();
                UpdateVisualExperience();
            }
        }

        public int WindowListItems
        {
            get => _windowListItems;
            set
            {
                if (value == _windowListItems) return;
                _windowListItems = value;
                OnPropertyChanged();
            }
        }

        public int MRUListItems
        {
            get => _mruListItems;
            set
            {
                if (value == _mruListItems) return;
                _mruListItems = value;
                OnPropertyChanged();
            }
        }

        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set
            {

                _showStatusBar = value;
                OnPropertyChanged();
                if (Application.Current.MainWindow?.DataContext is IMainWindowViewModel mainWindowViewModel)
                    mainWindowViewModel.UseStatusBar = value;
            }
        }

        public bool DockedWinClose
        {
            get => _dockedWinClose;
            set
            {
                if (value == _dockedWinClose)
                    return;
                _dockedWinClose = value;
                OnPropertyChanged();
            }
        }

        public bool DockedWinAuto
        {
            get => _dockedWinAuto;
            set
            {
                if (value == _dockedWinAuto)
                    return;
                _dockedWinAuto = value;
                OnPropertyChanged();
            }
        }

        [ImportingConstructor]
        public EnvironmentGeneralOptions(ISettingsManager settingsManager)
        {
            Instance = this;
            Category = SettingsCategories.EnvironmentCategory;
            SettingsManager = settingsManager;
        }

        public override ISettingsCategory Category { get; }

        public override string Name => "General";



        public override void LoadOrCreate()
        {
            SetPropertyFromSettings("ShowStatusBar", nameof(ShowStatusBar), true);
            SetPropertyFromSettings("WindowMenuContainsNItems", nameof(WindowListItems), 10);
            SetPropertyFromSettings("MRUListContainsNItems", nameof(MRUListItems), 10);
            SetPropertyFromSettings<bool>("AutohidePinActiveTabOnly", nameof(DockedWinAuto));
            SetPropertyFromSettings("CloseButtonActiveTabOnly", nameof(DockedWinClose), true);
            SetPropertyFromSettings("UseTitleCaseOnMenu", nameof(UseTitleCaseOnMenu), true);
            SetPropertyFromSettings("UseHardwareAcceleration", nameof(UseHardwareAcceleration), IsHardwareAccelerationSupported);
            SetPropertyFromSettings<bool>("RichClientExperienceOptions", nameof(UseRichVisualExperience));
            SetPropertyFromSettings<bool>("AutoAdjustExperience", nameof(AutoAdjustExperience)); 
        }

        public override void StoreSettings()
        {
            StoreSettingsValue("ShowStatusBar", ShowStatusBar.ToString());
            StoreSettingsValue("WindowMenuContainsNItems", WindowListItems.ToString());
            StoreSettingsValue("MRUListContainsNItems", MRUListItems.ToString());
            StoreSettingsValue("AutohidePinActiveTabOnly", DockedWinAuto.ToString());
            StoreSettingsValue("CloseButtonActiveTabOnly", DockedWinClose.ToString());
            StoreSettingsValue("UseTitleCaseOnMenu", UseTitleCaseOnMenu.ToString());
            StoreSettingsValue("UseHardwareAcceleration", UseHardwareAcceleration.ToString());
            StoreSettingsValue("RichClientExperienceOptions", UseRichVisualExperience.ToString());
            StoreSettingsValue("AutoAdjustExperience", AutoAdjustExperience.ToString());
        }

        private void UpdateVisualExperience()
        {
            if (_autoAdjustExperience)
            {
                EnvironmentRenderCapabilities.Current.VisualEffectsAllowed = VisualEffectsAllowed;
                RenderOptions.ProcessRenderMode = IsHardwareAccelerationSupported
                    ? RenderMode.Default
                    : RenderMode.SoftwareOnly;
                UseHardwareAcceleration = IsHardwareAccelerationSupported;
                UseRichVisualExperience = VisualEffectsAllowed >= 1;
            }
            else
            {
                RenderOptions.ProcessRenderMode = _useHardwareAcceleration
                    ? RenderMode.Default
                    : RenderMode.SoftwareOnly;
                EnvironmentRenderCapabilities.Current.VisualEffectsAllowed =
                    !UseRichVisualExperience ? 0 : VisualEffectsAllowed;
            }
        }
    }
}