using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Interfaces.Views;
using Screen = Caliburn.Micro.Screen;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IToolBarsPageViewModel))]
    internal sealed class ToolBarsPageViewModel : Screen, IToolBarsPageViewModel
    {
        private ToolbarDefinition _selectedToolbarDefinition;

        private IToolBarsPageView _control;

        public ObservableCollectionEx<ToolbarDefinition> Toolbars { get; }

        public ToolbarDefinition SelectedToolbarDefinition
        {
            get => _selectedToolbarDefinition;
            set
            {
                if (_selectedToolbarDefinition == value)
                    return;
                if (value == null)
                    return;
                _selectedToolbarDefinition = value;
                NotifyOfPropertyChange();
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view is IToolBarsPageView correctView)
                _control = correctView;
        }

        [ImportingConstructor]
        public ToolBarsPageViewModel()
        {
            DisplayName = "Toolbars";
            var t = IoC.Get<IToolBarHostViewModel>().TopLevelDefinitions as ObservableCollectionEx<CommandBarDefinitionBase>;
            if (t == null)
                return;
            t.CollectionChanged += T_CollectionChanged;

            Toolbars = new ObservableCollectionEx<ToolbarDefinition>();
            foreach (var definitionBase in t)
            {
                if (definitionBase is ToolbarDefinition toolbarDefinition)
                    Toolbars.Add(toolbarDefinition);
            }
        }



        private void T_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Toolbars.Clear();
            var t = IoC.Get<IToolBarHostViewModel>().TopLevelDefinitions;
            foreach (var definitionBase in t)
            {
                if (definitionBase is ToolbarDefinition toolbarDefinition)
                    Toolbars.Add(toolbarDefinition);
            }
        }

        public Command DropDownClickCommand => new Command(ExecuteDropDownClick);
        public Command DeleteSelectedToolbarCommand => new Command(ExecuteDeleteSelectedToolbar);
        public Command CreateNewToolbarCommand => new Command(ExecuteCreateNewToolbar);

        private void ExecuteCreateNewToolbar()
        {
            var windowManager = new WindowManager();
            var customizeDialog = new NewToolBarDialogViewModel();
            var result = windowManager.ShowDialog(customizeDialog);
            if (!result.HasValue || !result.Value)
                return;
            var def = new ToolbarDefinition(customizeDialog.ToolbarName, int.MaxValue, true, Dock.Top, true, true);
            IoC.Get<IToolBarHostViewModel>().AddToolbarDefinition(def);
            SelectedToolbarDefinition= def;
            _control.ToolBarListBox.ScrollIntoView(def);
            _control.ToolBarListBox.Focus();
        }

        private void ExecuteDeleteSelectedToolbar()
        {
            if (!SelectedToolbarDefinition.IsCustom)
                return;
            var result = MessageBox.Show(
                $"Are you sure you want to delete the '{SelectedToolbarDefinition.Text}' toolbar?",
                Application.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;
            IoC.Get<IToolBarHostViewModel>().RemoveToolbarDefinition(SelectedToolbarDefinition);
        }

        private void ExecuteDropDownClick()
        {
            var dropDownMenu = _control.ModifySelectionButton.DropDownMenu;
            dropDownMenu.DataContext = SelectedToolbarDefinition;
            dropDownMenu.IsOpen = true;
        }
    }   
}
