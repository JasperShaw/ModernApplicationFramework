using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.SettingsBase;
using ModernApplicationFramework.Interfaces.Settings;
using ModernApplicationFramework.Settings.SettingsDialog;

namespace ModernApplicationFramework.Extended.Settings.General
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralMainWindowSettingsViewModel : AbstractSettingsPage
    {
        private readonly EnvironmentGeneralOptions _generalOptions;
        private bool _showStatusBar;
        private bool _closeAffectsOnlyActive;
        private bool _autoHideAffectsOnlyActive;
        public override uint SortOrder => 3;
        public override string Name => GeneralSettingsResources.GeneralSettings_Name;
        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;

        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set
            {
                if (_showStatusBar == value)
                    return;
                DirtyObjectManager.SetData(_showStatusBar, value);
                _showStatusBar = value;
                OnPropertyChanged();
            }
        }

        public bool CloseAffectsOnlyActive
        {
            get => _closeAffectsOnlyActive;
            set
            {
                if (_closeAffectsOnlyActive == value)
                    return;
                DirtyObjectManager.SetData(_closeAffectsOnlyActive, value);
                _closeAffectsOnlyActive = value;
                OnPropertyChanged();
            }
        }

        public bool AutoHideAffectsOnlyActive
        {
            get => _autoHideAffectsOnlyActive;
            set
            {
                if (_autoHideAffectsOnlyActive == value)
                    return;
                DirtyObjectManager.SetData(_autoHideAffectsOnlyActive, value);
                _autoHideAffectsOnlyActive = value;
                OnPropertyChanged();
            }
        }

        [ImportingConstructor]
        public GeneralMainWindowSettingsViewModel(EnvironmentGeneralOptions generalOptions)
        {
            _generalOptions = generalOptions;
            _showStatusBar = generalOptions.ShowStatusBar;
            _autoHideAffectsOnlyActive = generalOptions.DockedWinAuto;
            _closeAffectsOnlyActive = generalOptions.DockedWinClose;
        }

        protected override bool SetData()
        {
            _generalOptions.ShowStatusBar = ShowStatusBar;
            _generalOptions.DockedWinAuto = AutoHideAffectsOnlyActive;
            _generalOptions.DockedWinClose = CloseAffectsOnlyActive;
            _generalOptions.StoreSettings();
            return true;
        }

        public override bool CanApply()
        {
            return true;
        }

        public override void Activate()
        {

        }
    }
}
