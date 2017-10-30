using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Windows.Interop;
using System.Windows.Media;
using Caliburn.Micro;
using JetBrains.Annotations;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics
{
    /// <inheritdoc />
    /// <summary>
    /// Holds the environment's general settings
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    [Export(typeof(EnvironmentGeneralOptions))]
    public sealed class EnvironmentGeneralOptions : INotifyPropertyChanged
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

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static EnvironmentGeneralOptions Instance { get; private set; }

        /// <summary>
        /// The level of visual effects are allowed.
        /// From 0 = no effects; to 2 = full effects allowed
        /// </summary>
        public int VisualEffectsAllowed => RenderCapability.Tier >> 16;

        /// <summary>
        /// Determinates if hardware acceleration is allowed. <see langword="true"/> if <see cref="VisualEffectsAllowed"/> is greater than 0.
        /// </summary>
        public bool IsHardwareAccelerationSupported => VisualEffectsAllowed > 0;

        /// <summary>
        /// Option to set all capital letters on the menu bar. <see langword="false"/> means menu items will be in capital only
        /// </summary>
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

        /// <summary>
        /// Option to let the environment decide about using hardware accelerated rendering.
        /// </summary>
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

        /// <summary>
        /// Option to use hardware acceleration
        /// </summary>
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

        /// <summary>
        /// Option to use enhanced visual animations, gradients and shadows
        /// </summary>
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

        /// <summary>
        /// The amount of listed windows inside the window menu 
        /// </summary>
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

        /// <summary>
        /// The amount of listed MRU files
        /// </summary>
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

        /// <summary>
        /// Option to show the environment's status bar
        /// </summary>
        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set
            {
                _showStatusBar = value;
                OnPropertyChanged();
                IoC.Get<IStatusBarDataModelService>().SetVisibility(Convert.ToUInt32(value));
            }
        }

        /// <summary>
        /// Option to close only the active tab of an anchorable container
        /// </summary>
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

        /// <summary>
        /// Option to auto-hide only the active tab of an anchorable container
        /// </summary>
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
        public EnvironmentGeneralOptions()
        {
            Instance = this;
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}