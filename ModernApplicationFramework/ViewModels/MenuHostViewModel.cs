using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.ViewModels
{
    public class MenuHostViewModel : ViewModelBase
    {
        private IMainWindowViewModel _mainWindowViewModel;
        private Menu _menu;

        public MenuHostViewModel(MenuHostControl control)
        {
            MenuHostControl = control;
            MenuHostControl.MouseRightButtonDown += _control_MouseRightButtonDown;
        }

        public MenuHostControl MenuHostControl { get; }
     
        /// <summary>
        /// Tells if you can open the ToolbarHostContextMenu
        /// Default is true
        /// </summary>
        public bool CanOpenToolBarContextMenu { get; set; } = true;

        /// <summary>
        /// Contains the UseDockingHost shall not be changed after setted up
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

        /// <summary>
        /// Contains the Menu of the MenuHostControl
        /// </summary>
        public Menu Menu
        {
            get { return _menu; }
            set
            {
                if (Equals(value, _menu))
                    return;
                _menu = value;
                OnPropertyChanged();
            }
        }

        async private void _control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            await RightClickCommand.Execute();
        }

        #region Commands
        public Command RightClickCommand => new Command(ExecuteRightClick);

        async protected virtual void ExecuteRightClick()
        {
            if (CanOpenToolBarContextMenu)
                await MainWindowViewModel.ToolBarHostViewModel.OpenContextMenuCommand.Execute();
        }
        #endregion
    }
}