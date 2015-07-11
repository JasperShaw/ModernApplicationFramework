using System;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ModernApplicationFramework.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;
        private MenuHostControl _menuHostControlControl;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            MenuHostViewModel = new MenuHostViewModel(this);
        }

        public MenuHostViewModel MenuHostViewModel { get; }

        /// <summary>
        /// Contains the MenuHostControl
        /// Only the API is allowed to change this Property: Security reasons
        /// TODO: I hate this but it works think of a new way some day: Problem is that the MenuHostControl should know its ViewModel which it does not now at the moment. Currently we tunnel thourgh the whole API
        /// </summary>
        internal MenuHostControl MenuHostControl
        {
            get { return _menuHostControlControl; }
            set
            {
                if (Equals(value, _menuHostControlControl))
                    return;
                if (_menuHostControlControl != null)
                    throw new NotSupportedException("Can not be setted once initalized");
                _menuHostControlControl = value;
                MenuHostViewModel.MenuHostControl = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        public ICommand MinimizeCommand => new Command(Minimize, CanMinimize);

        public virtual void Minimize()
        {
            _mainWindow.WindowState = WindowState.Minimized;
        }

        public virtual bool CanMinimize()
        {
            return  _mainWindow.WindowState != WindowState.Minimized;
        }

        public ICommand CloseCommand => new Command(Close, CanClose);

        public virtual void Close()
        {
            _mainWindow.Close();
        }

        public virtual bool CanClose()
        {
            return true;
        }

        public ICommand TestCommand => new Command(OnTest);

        protected virtual void OnTest()
        {
            MessageBox.Show("Test");
        }

        #endregion
    }
}
