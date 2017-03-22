using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using Screen = Caliburn.Micro.Screen;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.Basics.Customize
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
            Toolbars = IoC.Get<IToolBarHostViewModel>().ToolbarDefinitions;
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
            var def = new ToolbarDefinition(new ToolBar(), customizeDialog.ToolbarName, int.MaxValue, true, Dock.Top, true);
            IoC.Get<IToolBarHostViewModel>().AddToolbarDefinition(def);
            SelectedToolbarDefinition = def;
            _control.ToolBarListBox.ScrollIntoView(def);
            _control.ToolBarListBox.Focus();
        }

        private void ExecuteDeleteSelectedToolbar()
        {
            if (!SelectedToolbarDefinition.IsCustom)
                return;
            var result = MessageBox.Show(
                $"Are you sure you want to delete the '{SelectedToolbarDefinition.Name}' toolbar?",
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

    internal interface IToolBarsPageView
    {
        CheckableListBox ToolBarListBox { get; }

        DropDownDialogButton ModifySelectionButton { get; }
    }
}
