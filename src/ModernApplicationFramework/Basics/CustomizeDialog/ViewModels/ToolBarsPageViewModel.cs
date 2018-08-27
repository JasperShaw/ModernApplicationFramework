using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Interfaces.Views;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    /// <inheritdoc cref="IToolBarsPageViewModel" />
    /// <summary>
    /// Data view model implementing <see cref="T:ModernApplicationFramework.Interfaces.ViewModels.IToolBarsPageViewModel" />
    /// </summary>
    /// <seealso cref="T:Caliburn.Micro.Screen" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IToolBarsPageViewModel" />
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IToolBarsPageViewModel))]
    [Export(typeof(ICustomizeDialogScreen))]
    internal sealed class ToolBarsPageViewModel : Screen, IToolBarsPageViewModel
    {
        private ToolBarDataSource _selectedToolBarDataSource;

        private IToolBarsPageView _control;

        public ObservableCollection<CommandBarDataSource> Toolbars { get; }

        public Command DropDownClickCommand => new Command(ExecuteDropDownClick);
        public Command DeleteSelectedToolbarCommand => new Command(ExecuteDeleteSelectedToolbar);
        public Command CreateNewToolbarCommand => new Command(ExecuteCreateNewToolbar);

        public ToolBarDataSource SelectedToolBar
        {
            get => _selectedToolBarDataSource;
            set
            {
                if (_selectedToolBarDataSource == value)
                    return;
                if (value == null)
                    return;
                _selectedToolBarDataSource = value;
                NotifyOfPropertyChange();
            }
        }

        public uint SortOrder => 0;

        [ImportingConstructor]
        public ToolBarsPageViewModel()
        {
            DisplayName = Customize_Resources.CustomizeDialog_Toolbars;
            Toolbars = IoC.Get<IToolBarHostViewModel>().TopLevelDefinitions;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view is IToolBarsPageView correctView)
                _control = correctView;
        }

        private void ExecuteCreateNewToolbar()
        {
            var windowManager = new WindowManager();
            var customizeDialog = new NewToolBarDialogViewModel();
            var result = windowManager.ShowDialog(customizeDialog);
            if (!result.HasValue || !result.Value)
                return;
            var def = new ToolBarDataSource(Guid.Empty, customizeDialog.ToolbarName, 0, int.MaxValue, true, Dock.Top);
            ((IToolBarHostViewModelInternal)IoC.Get<IToolBarHostViewModel>())?.AddToolbarDefinition(def);
            SelectedToolBar = def;
            _control.ToolBarListBox.ScrollIntoView(def);
            _control.ToolBarListBox.Focus();
        }

        private void ExecuteDeleteSelectedToolbar()
        {
            if (!SelectedToolBar.IsCustom)
                return;
            var result = MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Customize_Resources.Prompt_ToolbarDeleteConfirmation, SelectedToolBar.Text),
                Application.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;
            ((IToolBarHostViewModelInternal) IoC.Get<IToolBarHostViewModel>())?.RemoveToolbarDefinition(SelectedToolBar);
        }

        private void ExecuteDropDownClick()
        {
            var dropDownMenu = _control.ModifySelectionButton.DropDownMenu;
            dropDownMenu.DataContext = SelectedToolBar;
            dropDownMenu.IsOpen = true;
        }
    }
}