using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.Extended.Commands;
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
        private CategoryKeyGesture _selectedGestureBinding;
        public override uint SortOrder => 15;
        public override string Name => "Keyboard";
        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;


        public ICommand ShowBindings => new Command(ExecuteMethod);

        

        public CommandDefinitionViewSource CollViewSource { get; set; }

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

        public CategoryKeyGesture SelectedGestureBinding
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


        [ImportingConstructor]
        public KeyboardSettingsViewModel(ISettingsManager settingsManager,
            IDialogProvider dialogProvider, IKeyGestureService gestureService)
        {
            _settingsManager = settingsManager;
            _dialogProvider = dialogProvider;
            _gestureService = gestureService;
            AllCommands = gestureService.GetAllCommandCommandDefinitions();
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


        private void UpdateAvailableGestureBinding()
        {
            AvailableGestureBindings = SelectedCommand.Gestures;

            if (SelectedCommand.Gestures.Count > 0)
                SelectedGestureBinding = SelectedCommand.Gestures[0];


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
