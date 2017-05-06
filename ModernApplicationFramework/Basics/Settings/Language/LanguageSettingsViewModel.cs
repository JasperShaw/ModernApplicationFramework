using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Basics.SettingsDialog;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Core.Localization;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Settings.Language
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LanguageSettingsViewModel : ViewModelBase, ISettingsPage
    {
        private LanguageInfo _selectedLanguage;

        private readonly ILanguageManager _languageManager;

        [ImportingConstructor]
        public LanguageSettingsViewModel(ILanguageManager languageManager)
        {
            _languageManager = languageManager;
            Languages = languageManager.GetInstalledLanguages();

            SelectedLanguage = Languages.FirstOrDefault(x => x.Code.Equals(languageManager.GetSavedLanguage().Code));
        }


	    public IEnumerable<LanguageInfo> Languages { get; }

        public LanguageInfo SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage == value)
                    return;
                _selectedLanguage = value;
                OnPropertyChanged();            
            }
        }


        public uint SortOrder => 7;
        public string Name => "Language";
        public SettingsCategory Category => SettingsCategories.EnvironmentCategory;

        public void Apply()
        {
            if (_languageManager.CurrentLanguage.Code == SelectedLanguage.Code)
                return;
            _languageManager.SaveLanguage(SelectedLanguage);
            MessageBox.Show("Test", "", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        public bool CanApply()
        {
            return SelectedLanguage != null;
        }
    }
}