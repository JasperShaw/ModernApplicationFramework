using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
        private readonly IDialogProvider _dialogProvider;

        [ImportingConstructor]
        public LanguageSettingsViewModel(ILanguageManager languageManager, IDialogProvider dialogProvider)
        {
            _languageManager = languageManager;
	        _dialogProvider = dialogProvider;
            Languages = languageManager.GetInstalledLanguages();
            SelectedLanguage = Languages.FirstOrDefault(x => x.Code.Equals(_languageManager.SavedLanguage.Code));
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
        public string Name => LanguageSettingsResources.LanguageSettings_Name;
        public SettingsCategory Category => SettingsCategories.EnvironmentCategory;

	    public bool Apply()
	    {
	        if (_languageManager.SavedLanguage.Code == SelectedLanguage.Code)
	            return true;
		    _languageManager.SaveLanguage(SelectedLanguage);
		    _dialogProvider.Warn(LanguageSettingsResources.LanguageChangedWarning);
	        return true;
	    }

	    public bool CanApply()
        {
            return SelectedLanguage != null;
        }

        public void Load()
        {
            
        }
    }
}