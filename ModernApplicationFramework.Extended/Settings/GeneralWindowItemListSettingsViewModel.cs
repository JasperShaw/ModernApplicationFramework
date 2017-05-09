using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Settings.General;
using ModernApplicationFramework.Basics.SettingsDialog;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Settings
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralWindowItemListSettingsViewModel : ViewModelBase, ISettingsPage
    {
        private readonly EnvironmentGeneralOptions _generalOptions;
        private int _windowListItems;
        public uint SortOrder => 1;
        public string Name => GeneralSettingsResources.GeneralSettings_Name;
        public SettingsCategory Category => SettingsCategories.EnvironmentCategory;


        [ImportingConstructor]
        public GeneralWindowItemListSettingsViewModel(EnvironmentGeneralOptions generalOptions)
        {
            _generalOptions = generalOptions;
            WindowListItems = generalOptions.WindowListItems;
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

        public bool Apply()
        {
            var flag = EnvironmentGeneralOptions.MinFileListCount <= WindowListItems && WindowListItems <= EnvironmentGeneralOptions.MaxFileListCount;
            if (!flag)
                return false;
            _generalOptions.WindowListItems = WindowListItems;
            _generalOptions.Save();
            return true;
        }

        public bool CanApply()
        {
            return true;
        }

        public void Load()
        {

        }
    }
}
