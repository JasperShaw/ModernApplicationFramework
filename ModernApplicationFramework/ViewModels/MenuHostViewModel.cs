using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.ViewModels
{
    public class MenuHostViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainWindowViewModel;
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
        /// Contains the MainWindowViewModel shall not be changed after setted up
        /// </summary>
        public MainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            internal set
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

        private void _control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightClickCommand.Execute(null);
        }

        #region Commands
        public ICommand RightClickCommand => new Command(ExecuteRightClick);

        protected virtual void ExecuteRightClick()
        {
            if (CanOpenToolBarContextMenu)
                MainWindowViewModel.ToolBarHostViewModel.OpenContextMenuCommand.Execute(null);
        }
        #endregion
    }
}