using System.ComponentModel.Composition;
using System.Globalization;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;

namespace ModernApplicationFramework.Extended.Settings.General
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralWindowItemListSettingsViewModel : SettingsPage
    {
        private readonly EnvironmentGeneralOptions _generalOptions;
        private readonly StorableEnvironmentGeneralOptions _storableOptions;
        private string _windowListItems;
        private string _mruListItems;
        public override uint SortOrder => 1;
        public override string Name => GeneralSettingsResources.GeneralSettings_Name;
        public override SettingsPageCategory Category => SettingsPageCategories.EnvironmentCategory;


        [ImportingConstructor]
        public GeneralWindowItemListSettingsViewModel(EnvironmentGeneralOptions generalOptions,
            StorableEnvironmentGeneralOptions storableOptions)
        {
            _generalOptions = generalOptions;
            _storableOptions = storableOptions;
            WindowListItems = generalOptions.WindowListItems.ToString();
            MruListItems = generalOptions.MRUListItems.ToString();
        }

        public string WindowListItems
        {
            get => _windowListItems;
            set
            {
                if (value == _windowListItems)
                    return;
                DirtyObjectManager.SetData(_windowListItems, value);
                _windowListItems = value;
                OnPropertyChanged();
            }
        }

        public string MruListItems
        {
            get => _mruListItems;
            set
            {
                if (value == _mruListItems)
                    return;
                DirtyObjectManager.SetData(_mruListItems, value);
                _mruListItems = value;
                OnPropertyChanged();
            }
        }

        protected override bool SetData()
        {
            if (!PreValidate(out int number))
            {
                ShowErrorMessage();
                return false;
            }
            _generalOptions.WindowListItems = number;
            _generalOptions.MRUListItems = number;
            _storableOptions.StoreSettings();
            return true;
        }

        private bool PreValidate(out int number)
        {
            if (!int.TryParse(WindowListItems, NumberStyles.Integer, CultureInfo.CurrentCulture, out number))
                return false;
            if (!int.TryParse(MruListItems, NumberStyles.Integer, CultureInfo.CurrentCulture, out number))
                return false;
            return EnvironmentGeneralOptions.MinFileListCount <= number && number <= EnvironmentGeneralOptions.MaxFileListCount;
        }

        private void ShowErrorMessage()
        {
            IoC.Get<IDialogProvider>().Warn(string.Format(CultureInfo.CurrentCulture,
                GeneralSettingsResources.Error_ItemsListCountNotMatching, EnvironmentGeneralOptions.MinFileListCount,
                EnvironmentGeneralOptions.MaxFileListCount));
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
