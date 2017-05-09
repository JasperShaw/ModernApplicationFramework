using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.SettingsDialog;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Settings.General
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralVisualExperienceSettingsViewModel : ViewModelBase, ISettingsPage
    {
        private readonly IThemeManager _manager;
        private readonly EnvironmentGeneralOptions _generalOptions;

        private Theme _selectedTheme;
        private bool _useTitleCaseOnMenu;
        private bool _autoAdjustExperience;
        private bool _useHardwareAcceleration;
        private bool _useRichVisualExperience;
        private string _renderingStatusText;

        uint ISettingsPage.SortOrder => uint.MinValue;
        string ISettingsPage.Name => GeneralSettingsResources.GeneralSettings_Name;
        SettingsCategory ISettingsPage.Category => SettingsCategories.EnvironmentCategory;


        public IEnumerable<Theme> Themes => _manager.Themes;

        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme != null && value.Equals(_selectedTheme))
                    return;
                _selectedTheme = value;
                OnPropertyChanged();
            }
        }

        public bool UseTitleCaseOnMenu
        {
            get => _useTitleCaseOnMenu;
            set
            {
                if (_useTitleCaseOnMenu == value)
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
                if (_autoAdjustExperience == value)
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
                if (_useHardwareAcceleration == value)
                    return;
                _useHardwareAcceleration = value;
                OnPropertyChanged();
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
            }
        }

        public string RenderingStatusText
        {
            get => _renderingStatusText;
            set
            {
                if (_renderingStatusText == value)
                    return;
                _renderingStatusText = value;
                OnPropertyChanged();
            }
        }


        [ImportingConstructor]
        public GeneralVisualExperienceSettingsViewModel(IThemeManager manager, EnvironmentGeneralOptions generalOptions)
        {
            _generalOptions = generalOptions;
            _manager = manager;

            SelectedTheme = Themes.FirstOrDefault(x => x.GetType() == _manager.Theme?.GetType());
            UseTitleCaseOnMenu = _generalOptions.UseTitleCaseOnMenu;
            AutoAdjustExperience = _generalOptions.AutoAdjustExperience;
            UseHardwareAcceleration = _generalOptions.UseHardwareAcceleration;
            UseRichVisualExperience = _generalOptions.UseRichVisualExperience;

            UpdateStatusText();
        }

        public bool Apply()
        {
            _manager.Theme = SelectedTheme;
            _generalOptions.UseTitleCaseOnMenu = UseTitleCaseOnMenu;
            _generalOptions.UseHardwareAcceleration = UseHardwareAcceleration;
            _generalOptions.UseRichVisualExperience = UseRichVisualExperience;
            _generalOptions.AutoAdjustExperience = AutoAdjustExperience;
            _generalOptions.Save();
            return true;
        }

        public bool CanApply()
        {
            return SelectedTheme != null;
        }

        public void Load()
        {
            UpdateStatusText();
        }

        private void UpdateVisualExperience()
        {
            if (!_autoAdjustExperience)
                return;
            UseHardwareAcceleration = _generalOptions.IsHardwareAccelerationSupported;
            UseRichVisualExperience = _generalOptions.VisualEffectsAllowed >= 1;
        }

        private void UpdateStatusText()
        {
            var message = _generalOptions.UseHardwareAcceleration
                ? GeneralVisualExperienceSettingsResources.HardwareRenderingStatus
                : GeneralVisualExperienceSettingsResources.SoftwareRendereingStatus;
            if (_generalOptions.AutoAdjustExperience)
                message += GeneralVisualExperienceSettingsResources.AutoAdjustVisualExperienceStatus;
            RenderingStatusText = message;
        }
    }
}