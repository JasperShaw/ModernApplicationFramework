using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.ViewModels
{
    public class MenuHostViewModel : ViewModelBase
    {
        private MenuHostControl _control;
        private Menu _menu;

        public MenuHostViewModel(MainWindowViewModel mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public ICommand RightClickCommand => new Command(ExecuteRightClick);

        /// <summary>
        /// Tells if you can open the ToolbarHostContextMenu
        /// Default is true
        /// </summary>
        public bool CanOpenToolBarContextMenu { get; set; } = true;

        public Menu Menu
        {
            get { return _menu; }
            set
            {
                if (!Equals(value, _menu))
                {
                    _menu = value;
                    _mainWindow.MenuHostControl.Menu = value;
                    OnPropertyChanged();
                }
            }
        }

        public MenuHostControl MenuHostControl
        {
            get { return _control; }
            set
            {
                if (Equals(value, _control))
                    return;
                _control = value;
                if (value != null)
                    _control.MouseRightButtonDown += _control_MouseRightButtonDown;
                else
                    _control.MouseRightButtonDown -= _control_MouseRightButtonDown;
                OnPropertyChanged();
            }
        }

        private readonly MainWindowViewModel _mainWindow;

        public virtual void ExecuteRightClick()
        {
            if (CanOpenToolBarContextMenu)
                MessageBox.Show("Open");
        }

        private void _control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightClickCommand.Execute(null);
        }
    }
}
