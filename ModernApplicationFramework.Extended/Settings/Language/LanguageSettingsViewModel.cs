using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.SettingsBase;
using ModernApplicationFramework.Core.Localization;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Settings;
using ModernApplicationFramework.Settings.SettingsDialog;

namespace ModernApplicationFramework.Extended.Settings.Language
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LanguageSettingsViewModel : AbstractSettingsPage
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
            _selectedLanguage = Languages.FirstOrDefault(x => x.Code.Equals(_languageManager.SavedLanguage.Code));
        }


	    public IEnumerable<LanguageInfo> Languages { get; }

        public LanguageInfo SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage == value)
                    return;
                DirtyObjectManager.SetData(_selectedLanguage, value);
                _selectedLanguage = value;
                OnPropertyChanged();            
            }
        }


        public override uint SortOrder => 7;
        public override string Name => LanguageSettingsResources.LanguageSettings_Name;
        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;

        protected override bool SetData()
        {
            _languageManager.SaveLanguage(SelectedLanguage);
            _dialogProvider.Warn(LanguageSettingsResources.LanguageChangedWarning);
            return true;
        }

        public override bool CanApply()
        {
            return SelectedLanguage != null;
        }

        public override void Activate()
        {
            
        }
    }
}