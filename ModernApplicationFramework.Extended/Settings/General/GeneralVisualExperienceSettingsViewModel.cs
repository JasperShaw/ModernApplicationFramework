using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.SettingsBase;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Settings;
using ModernApplicationFramework.Settings.SettingsDialog;

namespace ModernApplicationFramework.Extended.Settings.General
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralVisualExperienceSettingsViewModel : AbstractSettingsPage
    {
        private readonly IThemeManager _manager;
        private readonly EnvironmentGeneralOptions _generalOptions;

        private Theme _selectedTheme;
        private bool _useTitleCaseOnMenu;
        private bool _autoAdjustExperience;
        private bool _useHardwareAcceleration;
        private bool _useRichVisualExperience;
        private string _renderingStatusText;

        public override uint SortOrder => uint.MinValue;
        public override string Name => GeneralSettingsResources.GeneralSettings_Name;
        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;


        public IEnumerable<Theme> Themes => _manager.Themes;

        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme != null && value.Equals(_selectedTheme))
                    return;
                DirtyObjectManager.SetData(_selectedTheme, value);
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
                DirtyObjectManager.SetData(_useTitleCaseOnMenu, value);
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
                DirtyObjectManager.SetData(_autoAdjustExperience, value);
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
                DirtyObjectManager.SetData(_useHardwareAcceleration, value);
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
                DirtyObjectManager.SetData(_useRichVisualExperience, value);
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
                DirtyObjectManager.SetData(_renderingStatusText, value);
                _renderingStatusText = value;
                OnPropertyChanged();
            }
        }


        [ImportingConstructor]
        public GeneralVisualExperienceSettingsViewModel(IThemeManager manager, EnvironmentGeneralOptions generalOptions)
        {
            _generalOptions = generalOptions;
            _manager = manager;

            _selectedTheme = Themes.FirstOrDefault(x => x.GetType() == _manager.Theme?.GetType());
            _useTitleCaseOnMenu = _generalOptions.UseTitleCaseOnMenu;
            _autoAdjustExperience = _generalOptions.AutoAdjustExperience;
            _useHardwareAcceleration = _generalOptions.UseHardwareAcceleration;
            _useRichVisualExperience = _generalOptions.UseRichVisualExperience;

            UpdateStatusText();
        }

        //public override bool Apply()
        //{
        //    _manager.Theme = SelectedTheme;
        //    _generalOptions.UseTitleCaseOnMenu = UseTitleCaseOnMenu;
        //    _generalOptions.UseHardwareAcceleration = UseHardwareAcceleration;
        //    _generalOptions.UseRichVisualExperience = UseRichVisualExperience;
        //    _generalOptions.AutoAdjustExperience = AutoAdjustExperience;
        //    _generalOptions.Save();
        //    return base.Apply();
        //}

        protected override bool SetData()
        {
            _manager.Theme = SelectedTheme;
            _generalOptions.UseTitleCaseOnMenu = UseTitleCaseOnMenu;
            _generalOptions.UseHardwareAcceleration = UseHardwareAcceleration;
            _generalOptions.UseRichVisualExperience = UseRichVisualExperience;
            _generalOptions.AutoAdjustExperience = AutoAdjustExperience;
            _generalOptions.StoreSettings();
            return true;
        }

        public override bool CanApply()
        {
            return SelectedTheme != null;
        }

        public override void Activate()
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