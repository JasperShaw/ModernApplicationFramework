using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        protected bool MainWindowInitialized;
        private readonly MainWindow _mainWindow;
        private BitmapImage _activeIcon;
        private BitmapImage _icon;
        private bool _isSimpleWindow;
        private MenuHostViewModel _menuHostViewModel;
        private BitmapImage _passiveIcon;
        private StatusBar _statusBar;
        private ToolBarHostViewModel _toolBarHostViewModel;
        private bool _useStatusbar;
        private bool _useTitleBar;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            UseStatusBar = true;
            UseTitleBar = true;
            _mainWindow = mainWindow;
            _mainWindow.SourceInitialized += _mainWindow_SourceInitialized;
            _mainWindow.Activated += _mainWindow_Activated;
            _mainWindow.Deactivated += _mainWindow_Deactivated;
        }

        public bool MenuHostViewModelSetted => MenuHostViewModel != null;
        public bool ToolbarHostViewModelSetted => ToolBarHostViewModel != null;

        /// <summary>
        /// Contains the Active Icon for the MainWindow
        /// </summary>
        public BitmapImage ActiveIcon
        {
            get { return _activeIcon; }
            set
            {
                if (Equals(value, _activeIcon))
                    return;
                _activeIcon = value;
                OnPropertyChanged();
                ApplyMainWindowIconChange();
            }
        }

        /// <summary>
        /// Contains the Current Icon of the MainWindow
        /// </summary>
        public BitmapImage Icon { get { return _icon; }
            set
            {
                if (Equals(value, _icon))
                    return;
                _icon = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A SimpleWindow is a window which is not possible to resize my dragging the edges
        /// </summary>
        public bool IsSimpleWindow
        {
            get { return _isSimpleWindow; }
            set
            {
                if (Equals(value, _isSimpleWindow))
                    return;
                _isSimpleWindow = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains the ViewModel of the MainWindows MenuHostControl
        /// This can not be changed once it was setted with a value.
        /// </summary>
        public MenuHostViewModel MenuHostViewModel {
            get { return _menuHostViewModel; }
            internal set
            {
                if (MenuHostViewModelSetted)
                    throw new InvalidOperationException("You can not change the MenuHostViewModel once it was seeted up");
                _menuHostViewModel = value;
            }
        }

        /// <summary>
        /// Contains the Passive Icon for the MainWindow
        /// </summary>
        public BitmapImage PassiveIcon
        {
            get { return _passiveIcon; }
            set
            {
                if (Equals(value, _passiveIcon))
                    return;
                _passiveIcon = value;
                OnPropertyChanged();
                ApplyMainWindowIconChange();
            }
        }

        /// <summary>
        /// Contains the StatusBar of the MainWindow
        /// This can not be changed once it was setted with a value
        /// </summary>
        public StatusBar StatusBar
        {
            get { return _statusBar; }
            internal set { if (_statusBar == null) _statusBar = value; }
        }

        /// <summary>
        /// Contains the ViewModel of the MainWindows ToolbarHostControl
        /// This can not be changed once it was setted with a value
        /// </summary>
        public ToolBarHostViewModel ToolBarHostViewModel
        {
            get { return _toolBarHostViewModel; }
            internal set
            {
                if (ToolbarHostViewModelSetted)
                    throw new InvalidOperationException("You can not change the ToolBarHostViewModel once it was seeted up");
                _toolBarHostViewModel = value;
            }
        }

        /// <summary>
        /// Contains information whether a StatusBar is displayed or not
        /// </summary>
        public bool UseStatusBar
        {
            get { return _useStatusbar; }
            set
            {
                if (Equals(value, _useStatusbar))
                    return;
                _useStatusbar = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains information whether a TitleBar is displayed or not
        /// </summary>
        public bool UseTitleBar
        {
            get { return _useTitleBar; }
            set
            {
                if (Equals(value, _useTitleBar))
                    return;
                _useTitleBar = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Makes sure the just changed Active or Passive Icons are applied to the View
        /// </summary>
        protected virtual void ApplyMainWindowIconChange()
        {
            if (_mainWindow == null)
                return;
            Icon = _mainWindow.IsActive ? ActiveIcon : PassiveIcon;
        }

        /// <summary>
        /// This Method initializes the MainWindow after it is initialized. Do not call from contstructor.
        /// </summary>
        protected virtual void InitializeMainWindow()
        {
            if (!MainWindowInitialized)
                throw new Exception("You can not run this Operation until the MainWindow is not initialized");
        }

        private void _mainWindow_Activated(object sender, EventArgs e)
        {
            ChangeWindowIconActiveCommand.Execute(null);
        }

        private void _mainWindow_Deactivated(object sender, EventArgs e)
        {
            ChangeWindowIconPassiveCommand.Execute(null);
        }

        private void _mainWindow_SourceInitialized(object sender, EventArgs e)
        {
            MainWindowInitialized = true;
            InitializeMainWindow();
        }

        #region Commands

        public ICommand MinimizeCommand => new Command(Minimize, CanMinimize);

        protected virtual void Minimize()
        {
            _mainWindow.WindowState = WindowState.Minimized;
        }

        protected virtual bool CanMinimize()
        {
            return  _mainWindow.WindowState != WindowState.Minimized;
        }

        public ICommand MaximizeResizeCommand => new Command(MaximizeResize, CanMaximizeResize);

        protected virtual void MaximizeResize()
        {
            _mainWindow.WindowState = _mainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        protected virtual bool CanMaximizeResize()
        {
            return true;
        }

        public ICommand CloseCommand => new Command(Close, CanClose);

        protected virtual void Close()
        {
            _mainWindow.Close();
        }

        protected virtual bool CanClose()
        {
            return true;
        }

        public ICommand ChangeWindowIconActiveCommand => new Command(ChangeWindowIconActive, CanChangeWindowIcon);

        private bool CanChangeWindowIcon()
        {
            return ActiveIcon != null && PassiveIcon != null;
        }

        public void ChangeWindowIconActive()
        {
            Icon = ActiveIcon;
        }

        public ICommand ChangeWindowIconPassiveCommand => new Command(ChangeWindowIconPassive, CanChangeWindowIcon);


        public void ChangeWindowIconPassive()
        {
            Icon = PassiveIcon;
        }


        public ICommand TestCommand => new Command(OnTest);

        protected virtual void OnTest()
        {
            var m = new Menu();
            m.Items.Add(new MenuItem { Header = "Testing" });
            MenuHostViewModel.Menu = m;
            //MessageBox.Show("Test");
        }

        #endregion
    }
}
