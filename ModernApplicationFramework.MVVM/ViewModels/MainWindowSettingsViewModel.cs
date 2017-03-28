using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.ViewModels;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.MVVM.Properties;

namespace ModernApplicationFramework.MVVM.ViewModels
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainWindowSettingsViewModel : ViewModelBase, ISettingsPage
    {
        private readonly ThemeManager _manager;

        private readonly IDockingMainWindowViewModel _shell;

        private Theme _selectedTheme;


        [ImportingConstructor]
        public MainWindowSettingsViewModel(IDockingMainWindowViewModel shell, ThemeManager manager)
        {
            _shell = shell;
            _manager = manager;
        }

        public IEnumerable<Theme> Themes => _manager.Themes;

        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (value.Equals(_selectedTheme))
                    return;
                _selectedTheme = value;
                OnPropertyChanged();
            }
        }

        public int SortOrder => 0;
        public string Name => "General";
        public string Path => "Environment";

        public void Apply()
        {
            _manager.SaveTheme(SelectedTheme.Name);
            Settings.Default.Save();
        }

        public bool CanApply()
        {
            return SelectedTheme != null;
        }
    }
}