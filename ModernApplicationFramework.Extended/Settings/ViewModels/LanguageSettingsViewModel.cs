using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.SettingsDialog;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Settings.ViewModels
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LanguageSettingsViewModel : ViewModelBase, ISettingsPage
    {
        private readonly IThemeManager _manager;

        private Theme _selectedTheme;


        [ImportingConstructor]
        public LanguageSettingsViewModel(IThemeManager manager)
        {
            _manager = manager;
            SelectedTheme = Themes.FirstOrDefault(x => x.GetType() == _manager.Theme?.GetType());
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

        public int SortOrder => 1;
        public string Name => "Language";
        public SettingsCategory Category => SettingsCategories.EnvironmentCategory;

        public void Apply()
        {
            _manager.Theme = SelectedTheme;
        }

        public bool CanApply()
        {
            return SelectedTheme != null;
        }
    }
}