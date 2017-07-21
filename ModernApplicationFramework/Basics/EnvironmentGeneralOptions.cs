using System;
using System.ComponentModel.Composition;
using System.Windows.Interop;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.SettingsBase;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Settings;

namespace ModernApplicationFramework.Basics
{
    /// <inheritdoc />
    /// <summary>
    /// A settings data model that manages the environment's general options
    /// </summary>
    /// <seealso cref="AbstractSettingsDataModel" />
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
        public EnvironmentGeneralOptions(ISettingsManager settingsManager)
        {
            Instance = this;
            Category = SettingsCategories.EnvironmentCategory;
            SettingsManager = settingsManager;
        }

        /// <summary>
        /// The category of the data model
        /// </summary>
        /// <inheritdoc />
        public override ISettingsCategory Category { get; }

        /// <summary>
        /// The name of the data model
        /// </summary>
        /// <inheritdoc />
        public override string Name => "General";


        /// <summary>
        /// Loads all settings entries from the settings file or creates them if they don't exist.
        /// </summary>
        /// <inheritdoc />
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

        /// <summary>
        /// Stores all settings into memory.
        /// <remarks>This should not write the file to disk due to performance and possible mutexes.</remarks>
        /// </summary>
        /// <inheritdoc />
        public override void StoreSettings()
        {
            SetSettingsValue("ShowStatusBar", ShowStatusBar);
            SetSettingsValue("WindowMenuContainsNItems", WindowListItems);
            SetSettingsValue("MRUListContainsNItems", MRUListItems);
            SetSettingsValue("AutohidePinActiveTabOnly", DockedWinAuto);
            SetSettingsValue("CloseButtonActiveTabOnly", DockedWinClose);
            SetSettingsValue("UseTitleCaseOnMenu", UseTitleCaseOnMenu);
            SetSettingsValue("UseHardwareAcceleration", UseHardwareAcceleration);
            SetSettingsValue("RichClientExperienceOptions", UseRichVisualExperience);
            SetSettingsValue("AutoAdjustExperience", AutoAdjustExperience);
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