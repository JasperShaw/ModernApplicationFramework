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
	    


		[ImportingConstructor]
        public GeneralVisualExperienceSettingsViewModel(IThemeManager manager, EnvironmentGeneralOptions generalOptions)
		{
			_generalOptions = generalOptions;
            _manager = manager;
            SelectedTheme = Themes.FirstOrDefault(x => x.GetType() == _manager.Theme?.GetType());
			UseTitleCaseOnMenu = generalOptions.UseTitleCaseOnMenu;
		}

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


	    public uint SortOrder => uint.MinValue;
        public string Name => GeneralVisualExperienceSettingsResources.VisualExperienceSettings_Name;
        public SettingsCategory Category => SettingsCategories.EnvironmentCategory;

        public void Apply()
        {
            _manager.Theme = SelectedTheme;
	        _generalOptions.UseTitleCaseOnMenu = UseTitleCaseOnMenu;


			_generalOptions.Save();
        }

        public bool CanApply()
        {
            return SelectedTheme != null;
        }
    }
}