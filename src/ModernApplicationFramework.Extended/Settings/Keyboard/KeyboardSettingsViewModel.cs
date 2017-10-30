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
        private readonly IKeyBindingManager _keyBindingManager;
        private IEnumerable<CommandGestureScopeMapping> _duplicates;
        private string _gestureInput;
        private IEnumerable<CommandDefinition> _items;
        private IEnumerable<SchemeDefinition> _schemes;
        private string _searchFilter;
        private CommandDefinition _selectedCommand;
        private int _selectedCommandIndex;
        private GestureScopeMapping _selectedGestureScopeBinding;
        private SchemeDefinition _selectedScheme;
        private GestureScope _selectedScope;

        private bool _isChanged;


        [ImportingConstructor]
        public KeyboardSettingsViewModel(ISettingsManager settingsManager,
            IKeyBindingManager keyBindingManager)
        {
            _keyBindingManager = keyBindingManager;
            Schemes = _keyBindingManager.SchemeManager.SchemeDefinitions;
            _selectedScheme = _keyBindingManager.SchemeManager.CurrentScheme;
            AllCommands = keyBindingManager.KeyGestureService.GetAllCommandDefinitions();
            Scopes = new BindableCollection<GestureScope>(keyBindingManager.KeyGestureService.GetAllCommandGestureScopes());
            SelectedScope = GestureScopes.GlobalGestureScope;
            AvailableGestureBindings = new ObservableCollection<GestureScopeMapping>();
            SetupCollectionViewSource();
        }

        public override uint SortOrder => 15;
        public override string Name => "Keyboard";
        public override SettingsPageCategory Category => SettingsPageCategories.EnvironmentCategory;


        public ICommand ResetSchemeCommand => new Command(ExecuteResetScheme, CanExecuteResetScheme);
        public ICommand RemoveSelectedBinding => new UICommand(ExecuteRemoveBinding, CanExecuteRemoveBinding);
        public ICommand AssignGesture => new UICommand(ExecuteAssignGesture, CanAssignGesture);

        public CommandDefinitionViewSource CollViewSource { get; set; }

        public IObservableCollection<GestureScope> Scopes { get; }

        protected bool IsChanged
        {
            get => _isChanged;
            set
            {
                if (value == _isChanged)
                    return;
                DirtyObjectManager.SetData(_isChanged, value);
                _isChanged = value;
                OnPropertyChanged();
            }
        }

        public int SelectedCommandIndex
        {
            get => _selectedCommandIndex;
            set
            {
                if (value == _selectedCommandIndex)
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
                CurrentKeyGesture = (MultiKeyGesture) new MultiKeyGestureConverter().ConvertFrom(null,
                    CultureInfo.CurrentCulture,
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
                DirtyObjectManager.SetData(_selectedScheme, value);
                _selectedScheme = value;
                OnPropertyChanged();
            }
        }

        public MultiKeyGesture CurrentKeyGesture { get; set; }

        protected override bool SetData()
        {
            _keyBindingManager.BuildCurrent();
            _keyBindingManager.SetKeyScheme(_selectedScheme);
            _keyBindingManager.BuildCurrent();
            _keyBindingManager.SaveCurrent();
            _isChanged = false;
            return true;
        }

        protected override void RestoreData()
        {
            _keyBindingManager.ApplyKeyBindingsFromSettings();
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

        public override void Cancel()
        {
            _keyBindingManager.ApplyKeyBindingsFromSettings();
        }

        private void ExecuteResetScheme()
        {
            if (MessageBox.Show(KeyboardSettingsResources.KeyboardSettingsWarningApplyScheme,
                    IoC.Get<IEnvironmentVariables>().ApplicationName,
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                return;
            _keyBindingManager.ResetToKeyScheme(SelectedScheme);
            _keyBindingManager.BuildCurrent();
            _keyBindingManager.SaveCurrent();        
            UpdateAvailableGestureBinding();
        }

        private bool CanExecuteResetScheme()
        {
            return SelectedScheme != null;
        }

        private void UpdateDuplicate()
        {
            Duplicates = _keyBindingManager.KeyGestureService.FindKeyGestures(MultiKeyGesture.GetKeySequences(CurrentKeyGesture),
                FindKeyGestureOption.Containing);
        }


        private void ExecuteRemoveBinding()
        {
            SelectedCommand.Gestures.Remove(SelectedGestureScopeBinding);
            UpdateAvailableGestureBinding();
            IsChanged = true;
        }

        private bool CanExecuteRemoveBinding()
        {
            return SelectedGestureScopeBinding != null;
        }


        private void UpdateAvailableGestureBinding()
        {
            AvailableGestureBindings.Clear();
            foreach (var gesture in SelectedCommand.Gestures)
                AvailableGestureBindings.Add(gesture);

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
                var exactDuplicate = _keyBindingManager.KeyGestureService
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
            IsChanged = true;
        }

        private void AddFilter()
        {
            CollViewSource.Filter -= Filter;
            CollViewSource.Filter += Filter;
        }

        private void Filter(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            if (!(e.Item is CommandDefinition src))
                e.Accepted = false;
            else if (src.TrimmedCategoryCommandName != null &&
                     !Regex.IsMatch(src.TrimmedCategoryCommandName, SearchFilter,
                         RegexOptions.IgnoreCase | RegexOptions.CultureInvariant |
                         RegexOptions.IgnorePatternWhitespace))
                e.Accepted = false;
        }

        private void SetupCollectionViewSource()
        {
            var sd = new SortDescription {PropertyName = nameof(CommandDefinition.TrimmedCategoryCommandName)};
            CollViewSource = new CommandDefinitionViewSource
            {
                Source = AllCommands,
                BoundPropertyName = nameof(CommandDefinition.TrimmedCategoryCommandName)
            };
            CollViewSource.SortDescriptions.Add(sd);
        }
    }
}