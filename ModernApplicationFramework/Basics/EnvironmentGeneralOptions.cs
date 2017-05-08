using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Basics
{
    public sealed class EnvironmentGeneralOptions : INotifyPropertyChanged
    {
        private bool _useTitleCaseOnMenu;
        private bool _autoAdjustExperience;
        private bool _useHardwareAcceleration;
        private bool _useRichVisualExperience;

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


        public EnvironmentGeneralOptions()
        {
            Instance = this;
        }

        public void Load()
        {
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