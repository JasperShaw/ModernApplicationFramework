using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Basics.SettingsDialog;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.Settings.General
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralMainWindowSettingsViewModel : ViewModelBase, ISettingsPage
    {
        private readonly EnvironmentGeneralOptions _generalOptions;
        private bool _showStatusBar;
        private bool _closeAffectsOnlyActive;
        private bool _autoHideAffectsOnlyActive;
        public uint SortOrder => 3;
        public string Name => GeneralSettingsResources.GeneralSettings_Name;
        public SettingsCategory Category => SettingsCategories.EnvironmentCategory;

        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set
            {
                if (_showStatusBar == value)
                    return;
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
                _autoHideAffectsOnlyActive = value;
                OnPropertyChanged();
            }
        }

        [ImportingConstructor]
        public GeneralMainWindowSettingsViewModel(EnvironmentGeneralOptions generalOptions)
        {
            _generalOptions = generalOptions;
            ShowStatusBar = generalOptions.ShowStatusBar;
            AutoHideAffectsOnlyActive = generalOptions.DockedWinAuto;
            CloseAffectsOnlyActive = generalOptions.DockedWinClose;
        }

        public bool Apply()
        {
            _generalOptions.ShowStatusBar = ShowStatusBar;
            _generalOptions.DockedWinAuto = AutoHideAffectsOnlyActive;
            _generalOptions.DockedWinClose = CloseAffectsOnlyActive;
            _generalOptions.Save();
            return true;
        }

        public bool CanApply()
        {
            return true;
        }

        public void Activate()
        {

        }
    }
}
