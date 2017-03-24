using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using Screen = Caliburn.Micro.Screen;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IToolBarsPageViewModel))]
    internal sealed class ToolBarsPageViewModel : Screen, IToolBarsPageViewModel
    {
        private ToolbarDefinitionOld _selectedToolbarDefinitionOld;

        private IToolBarsPageView _control;

        public ObservableCollectionEx<ToolbarDefinitionOld> Toolbars { get; }

        public ToolbarDefinitionOld SelectedToolbarDefinitionOld
        {
            get => _selectedToolbarDefinitionOld;
            set
            {
                if (_selectedToolbarDefinitionOld == value)
                    return;
                if (value == null)
                    return;
                _selectedToolbarDefinitionOld = value;
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
            var def = new ToolbarDefinitionOld(new ToolBar(), customizeDialog.ToolbarName, int.MaxValue, true, Dock.Top, true);
            IoC.Get<IToolBarHostViewModel>().AddToolbarDefinition(def);
            SelectedToolbarDefinitionOld = def;
            _control.ToolBarListBox.ScrollIntoView(def);
            _control.ToolBarListBox.Focus();
        }

        private void ExecuteDeleteSelectedToolbar()
        {
            if (!SelectedToolbarDefinitionOld.IsCustom)
                return;
            var result = MessageBox.Show(
                $"Are you sure you want to delete the '{SelectedToolbarDefinitionOld.Name}' toolbar?",
                Application.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;
            IoC.Get<IToolBarHostViewModel>().RemoveToolbarDefinition(SelectedToolbarDefinitionOld);
        }

        private void ExecuteDropDownClick()
        {
            var dropDownMenu = _control.ModifySelectionButton.DropDownMenu;
            dropDownMenu.DataContext = SelectedToolbarDefinitionOld;
            dropDownMenu.IsOpen = true;
        }
    }

    internal interface IToolBarsPageView
    {
        CheckableListBox ToolBarListBox { get; }

        DropDownDialogButton ModifySelectionButton { get; }
    }
}
