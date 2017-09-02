using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.KeyBindingScheme;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;
using ModernApplicationFramework.Utilities.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.Settings.Keyboard
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class KeyboardSettingsViewModel : SettingsPage, IStretchSettingsPanelPanel
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IKeyGestureService _gestureService;
        private readonly IKeyBindingSchemeManager _schemeManager;
        private IEnumerable<CommandDefinition> _items;
        private string _searchFilter;
        private CommandDefinition _selectedCommand;
        private GestureScopeMapping _selectedGestureScopeBinding;
        private GestureScope _selectedScope;
        private string _gestureInput;
        private IEnumerable<CommandGestureScopeMapping> _duplicates;
        private int _selectedCommandIndex;
        private IEnumerable<SchemeDefinition> _schemes;
        private SchemeDefinition _selectedScheme;

        public override uint SortOrder => 15;
        public override string Name => "Keyboard";
        public override SettingsPageCategory Category => SettingsPageCategories.EnvironmentCategory;


        public ICommand ResetSchemeCommand => new Command(ExecuteResetScheme, CanExecuteResetScheme);
        public ICommand RemoveSelectedBinding => new UICommand(ExecuteRemoveBinding, CanExecuteRemoveBinding);
        public ICommand AssignGesture => new UICommand(ExecuteAssignGesture, CanAssignGesture);

        public CommandDefinitionViewSource CollViewSource { get; set; }

        public IObservableCollection<GestureScope> Scopes { get; }

        public int SelectedCommandIndex
        {
            get => _selectedCommandIndex;
            set
            {
                if(value == _selectedCommandIndex)
                    return;
                _selectedCommandIndex = value;
                OnPropertyChanged();
            }
        }

        public string SearchFilter
        {
            get => _searchFilter;
            set
            {
                if (_searchFilter == value)
                    return;
                _searchFilter = value;
                if (!string.IsNullOrEmpty(SearchFilter))
                    AddFilter();
                CollViewSource.View.Refresh(); // important to refresh your View
            }
        }

        public IEnumerable<CommandDefinition> AllCommands
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public CommandDefinition SelectedCommand
        {
            get => _selectedCommand;
            set
            {
                if (Equals(value, _selectedCommand))
                    return;
                _selectedCommand = value;
                OnPropertyChanged();
                if (value != null)
                    UpdateAvailableGestureBinding();
            }
        }

        public ObservableCollection<GestureScopeMapping> AvailableGestureBindings { get; set; }

        public GestureScopeMapping SelectedGestureScopeBinding
        {
            get => _selectedGestureScopeBinding;
            set
            {
                if (Equals(value, _selectedGestureScopeBinding))
                    return;
                _selectedGestureScopeBinding = value;
                OnPropertyChanged();
            }
        }

        public GestureScope SelectedScope
        {
            get => _selectedScope;
            set
            {
                if (Equals(value, _selectedScope))
                    return;
                _selectedScope = value;
                OnPropertyChanged();
            }
        }

        public string GestureInput
        {
            get => _gestureInput;
            set
            {
                if (Equals(value, _gestureInput))
                    return;
                _gestureInput = value;
                OnPropertyChanged();
                CurrentKeyGesture = (MultiKeyGesture)new MultiKeyGestureConverter().ConvertFrom(null, CultureInfo.CurrentCulture,
                    GestureInput);
                UpdateDuplicate();
            }
        }

        public IEnumerable<CommandGestureScopeMapping> Duplicates
        {
            get => _duplicates;
            set
            {
                if (Equals(_duplicates, value))
                    return;
                _duplicates = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<SchemeDefinition> Schemes
        {
            get => _schemes;
            set
            {
                if (Equals(_schemes, value))
                    return;
                _schemes = value;
                OnPropertyChanged();
            }
        }

        public SchemeDefinition SelectedScheme
        {
            get => _selectedScheme;
            set
            {
                if (Equals(_selectedScheme, value))
                    return;
                _selectedScheme = value;
                OnPropertyChanged();
            }
        }

        public MultiKeyGesture CurrentKeyGesture { get; set; }


        [ImportingConstructor]
        public KeyboardSettingsViewModel(ISettingsManager settingsManager,
            IKeyGestureService gestureService,
            IKeyBindingSchemeManager schemeManager)
        {
            _settingsManager = settingsManager;
            _gestureService = gestureService;
            _schemeManager = schemeManager;
            Schemes = _schemeManager.SchemeDefinitions;
            SelectedScheme = Schemes.FirstOrDefault();
            AllCommands = gestureService.GetAllCommandDefinitions();
            Scopes = new BindableCollection<GestureScope>(gestureService.GetAllCommandGestureCategories());
            SelectedScope = GestureScopes.GlobalGestureScope;
            AvailableGestureBindings = new ObservableCollection<GestureScopeMapping>();
            SetupCollectionViewSource();
        }

        protected override bool SetData()
        {
            return true;
        }

        public override bool CanApply()
        {
            return true;
        }

        public override void Activate()
        {
            SelectedCommandIndex = 0;
            GestureInput = string.Empty;
        }

        private void ExecuteResetScheme()
        {
            if (MessageBox.Show(KeyboardSettingsResources.KeyboardSettingsWarningApplyScheme, 
                    IoC.Get<IEnvironmentVarirables>().ApplicationName, 
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                return;
            _schemeManager.SetScheme(SelectedScheme);
            UpdateAvailableGestureBinding();
        }

        private bool CanExecuteResetScheme()
        {
            return SelectedScheme != null;
        }

        private void UpdateDuplicate()
        {
            Duplicates = _gestureService.FindKeyGestures(MultiKeyGesture.GetKeySequences(CurrentKeyGesture),
                FindKeyGestureOption.Containing);
        }


        private void ExecuteRemoveBinding()
        {
            SelectedCommand.Gestures.Remove(SelectedGestureScopeBinding);
            UpdateAvailableGestureBinding();
        }

        private bool CanExecuteRemoveBinding()
        {
            return SelectedGestureScopeBinding != null;
        }


        private void UpdateAvailableGestureBinding()
        {
            AvailableGestureBindings.Clear();
            foreach (var gesture in SelectedCommand.Gestures)
            {
                AvailableGestureBindings.Add(gesture);
            }

            SelectedGestureScopeBinding = SelectedCommand.Gestures.Count > 0 ? SelectedCommand.Gestures[0] : null;
        }

        private bool CanAssignGesture()
        {
            return !string.IsNullOrEmpty(GestureInput);
        }

        private void ExecuteAssignGesture()
        {
            if (Duplicates.Any())
            {
                var exactDuplicate = _gestureService
                    .FindKeyGestures(MultiKeyGesture.GetKeySequences(CurrentKeyGesture),
                        FindKeyGestureOption.ExactMatch)
                    .FirstOrDefault(x => Equals(x.GestureScopeMapping.Scope, SelectedScope));
                exactDuplicate?.CommandDefinition.Gestures.Remove(exactDuplicate.GestureScopeMapping);
            }

            var category = SelectedScope;
            var categoryKeyGesture = new GestureScopeMapping(category, CurrentKeyGesture);
            SelectedCommand.Gestures.Insert(0, categoryKeyGesture);
            UpdateAvailableGestureBinding();
            GestureInput = string.Empty;
            UpdateDuplicate();
        }

        private void AddFilter()
        {
            CollViewSource.Filter -= Filter;
            CollViewSource.Filter += Filter;
        }

        private void Filter(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as CommandDefinition;
            if (src == null)
                e.Accepted = false;
            else if (src.TrimmedCategoryCommandName != null &&
                     !Regex.IsMatch(src.TrimmedCategoryCommandName, SearchFilter,
                         RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace))
                e.Accepted = false;
        }

        private void SetupCollectionViewSource()
        {
            var sd = new SortDescription { PropertyName = nameof(CommandDefinition.TrimmedCategoryCommandName) };
            CollViewSource = new CommandDefinitionViewSource
            {
                Source = AllCommands,
                BoundPropertyName = nameof(CommandDefinition.TrimmedCategoryCommandName)
            };
            CollViewSource.SortDescriptions.Add(sd);
        }
    }
}
