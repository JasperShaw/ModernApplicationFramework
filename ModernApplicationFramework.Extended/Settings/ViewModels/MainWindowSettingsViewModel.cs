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
    public class MainWindowSettingsViewModel : ViewModelBase, ISettingsPage
    {
        private readonly IThemeManager _manager;

        private Theme _selectedTheme;


        [ImportingConstructor]
        public MainWindowSettingsViewModel(IThemeManager manager)
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

        public uint SortOrder => uint.MinValue;
        public string Name => "General";
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