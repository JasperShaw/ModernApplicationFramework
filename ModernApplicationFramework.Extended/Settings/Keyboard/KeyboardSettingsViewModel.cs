using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.Settings.Keyboard
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class KeyboardSettingsViewModel : AbstractSettingsPage, IStretchSettingsPanelPanel
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IDialogProvider _dialogProvider;
        private readonly IKeyGestureService _gestureService;
        private IEnumerable<CommandDefinition> _items;
        private string _searchFilter;
        private GestureCollection _availableGestureCommands;
        private CommandDefinition _selectedCommand;
        private CategoryGestureMapping _selectedGestureBinding;
        private CommandGestureCategory _selectedCategory;
        private string _gestureInput;
        public override uint SortOrder => 15;
        public override string Name => "Keyboard";
        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;


        public ICommand ShowBindings => new Command(ExecuteMethod);
        public ICommand RemoveSelectedBinding => new UICommand(ExecuteRemoveBinding, CanExecuteRemoveBinding);
        public ICommand AssignGesture => new UICommand(ExecuteAssignGesture, CanAssignGesture);
        
        public CommandDefinitionViewSource CollViewSource { get; set; }
        
        public IObservableCollection<CommandGestureCategory> Categories { get; }

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
                UpdateAvailableGestureBinding();
            }
        }

        public GestureCollection AvailableGestureBindings
        {
            get => _availableGestureCommands;
            set
            {
                if (Equals(value, _availableGestureCommands))
                    return;
                _availableGestureCommands = value;
                OnPropertyChanged();
            }
        }

        public CategoryGestureMapping SelectedGestureBinding
        {
            get => _selectedGestureBinding;
            set
            {
                if (Equals(value, _selectedGestureBinding))
                    return;
                _selectedGestureBinding = value; 
                OnPropertyChanged();
            }
        }

        public CommandGestureCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (Equals(value, _selectedCategory))
                    return;
                _selectedCategory = value;
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
            }
        }


        [ImportingConstructor]
        public KeyboardSettingsViewModel(ISettingsManager settingsManager,
            IDialogProvider dialogProvider, IKeyGestureService gestureService)
        {
            _settingsManager = settingsManager;
            _dialogProvider = dialogProvider;
            _gestureService = gestureService;
            AllCommands = gestureService.GetAllCommandDefinitions();
            Categories = new BindableCollection<CommandGestureCategory>(gestureService.GetAllCommandGestureCategories());
            SelectedCategory = CommandGestureCategories.GlobalGestureCategory;
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
        }


        private void ExecuteRemoveBinding()
        {
            SelectedCommand.Gestures.Remove(SelectedGestureBinding);
            UpdateAvailableGestureBinding();  
        }

        private bool CanExecuteRemoveBinding()
        {
            return SelectedGestureBinding != null;
        }


        private void UpdateAvailableGestureBinding()
        {
            AvailableGestureBindings = SelectedCommand.Gestures;
            SelectedGestureBinding = SelectedCommand.Gestures.Count > 0 ? SelectedCommand.Gestures[0] : null;
        }

        private bool CanAssignGesture()
        {
            return !string.IsNullOrEmpty(GestureInput);
        }

        private void ExecuteAssignGesture()
        {
            var gesture =
                (MultiKeyGesture) new MultiKeyGestureConverter().ConvertFrom(null, CultureInfo.CurrentCulture,
                    GestureInput);
            var category = SelectedCategory;        
            var categoryKeyGesture = new CategoryGestureMapping(category, gesture);
            SelectedCommand.Gestures.Add(categoryKeyGesture);
        }

        private void ExecuteMethod()
        {
            var b = _gestureService.GetAllBindings();

            var message = b.Aggregate(string.Empty, (current, value) => current + (value + "\r\n"));

            _dialogProvider.ShowMessage(message);


            var ic = IoC.Get<ICommandService>().GetCommandDefinition(typeof(MultiUndoCommandDefinition)) as CommandDefinition;   
            if (ic == null)
                return;
            
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
