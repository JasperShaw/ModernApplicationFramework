using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics
{
    public sealed class EnvironmentGeneralOptions : INotifyPropertyChanged
    {
        public const int MinFileListCount = 1;
        public const int MaxFileListCount = 24;
        public const int MinMruListCount = 1;
        public const int MaxMruListCount = 24;

        private bool _useTitleCaseOnMenu;
        private bool _autoAdjustExperience;
        private bool _useHardwareAcceleration;
        private bool _useRichVisualExperience;
        private int _windowListItems;
        private bool _showStatusBar;
        private bool _dockedWinClose;
        private bool _dockedWinAuto;
        private int _mruListItems;

        public event PropertyChangedEventHandler PropertyChanged;
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

        public EnvironmentGeneralOptions()
        {
            Instance = this;
        }

        public void Load()
        {
            ShowStatusBar = Properties.Settings.Default.ShowStatusBar;
            WindowListItems = Properties.Settings.Default.WindowMenuContainsNItems;
            MRUListItems = Properties.Settings.Default.MRUListContainsNItems;
            DockedWinAuto = Properties.Settings.Default.AutohidePinActiveTabOnly;
            DockedWinClose = Properties.Settings.Default.CloseButtonActiveTabOnly;
            UseTitleCaseOnMenu = Properties.Settings.Default.UseTitleCaseOnMenu;
            UseHardwareAcceleration = Properties.Settings.Default.UseHardwareAcceleration;
            UseRichVisualExperience = Properties.Settings.Default.UseRichVisualExperience;
            //Must be last
            AutoAdjustExperience = Properties.Settings.Default.AutoAdjustVisualExperience;
        }

        public void Save()
        {
            Properties.Settings.Default.UseTitleCaseOnMenu = UseTitleCaseOnMenu;
            Properties.Settings.Default.AutoAdjustVisualExperience = AutoAdjustExperience;
            Properties.Settings.Default.UseHardwareAcceleration = UseHardwareAcceleration;
            Properties.Settings.Default.UseRichVisualExperience = UseRichVisualExperience;

            Properties.Settings.Default.WindowMenuContainsNItems = WindowListItems;
            Properties.Settings.Default.MRUListContainsNItems = MRUListItems;
            Properties.Settings.Default.AutohidePinActiveTabOnly = DockedWinAuto;
            Properties.Settings.Default.CloseButtonActiveTabOnly = DockedWinClose;
            Properties.Settings.Default.ShowStatusBar = ShowStatusBar;
            Properties.Settings.Default.Save();
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

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}