using System.Collections.ObjectModel;
using System.Windows.Input;
using ModernApplicationFramework.Caliburn.Collections;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.ViewModels
{
    public class MenuHostViewModel : ViewModelBase, IMenuHostViewModel
    {
        private IMainWindowViewModel _mainWindowViewModel;

        public MenuHostViewModel(MenuHostControl control)
        {
            MenuHostControl = control;
            MenuHostControl.MouseRightButtonDown += _control_MouseRightButtonDown;
            Items = new BindableCollection<MenuItem>();
        }

        /// <summary>
        ///     Tells if you can open the ToolbarHostContextMenu
        ///     Default is true
        /// </summary>
        public bool CanOpenToolBarContextMenu { get; set; } = true;

        public void CreateMenu(IMenuCreator creator)
        {
            creator.CreateMenu(this);
        }

        /// <summary>
        ///     Contains the Items of the MenuHostControl
        /// </summary>
        public ObservableCollection<MenuItem> Items { get; set; }

        /// <summary>
        ///     Contains the UseDockingHost shall not be changed after setted up
        /// </summary>
        public IMainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            set
            {
                if (_mainWindowViewModel == null)
                    _mainWindowViewModel = value;
            }
        }

        public MenuHostControl MenuHostControl { get; }

        private async void _control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            await RightClickCommand.Execute();
        }

        #region Commands

        public Command RightClickCommand => new Command(ExecuteRightClick);

        protected virtual async void ExecuteRightClick()
        {
            if (CanOpenToolBarContextMenu)
                await MainWindowViewModel.ToolBarHostViewModel.OpenContextMenuCommand.Execute();
        }

        #endregion
    }
}