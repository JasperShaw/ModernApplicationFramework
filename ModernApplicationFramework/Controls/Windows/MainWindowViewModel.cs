using System;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Controls.Windows
{
    /// <inheritdoc cref="IMainWindowViewModel" />
    /// <summary>
    /// View model for a main window
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Core.ViewModelBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IMainWindowViewModel" />
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        protected bool MainWindowInitialized;
        private readonly MainWindow _mainWindow;
        private bool _isSimpleWindow;
        private IMenuHostViewModel _menuHostViewModel;
        private IToolBarHostViewModel _toolBarHostViewModel;
        private bool _useSimpleMovement;
        private bool _useTitleBar;
        private bool _useMenu;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            UseTitleBar = true;
            UseMenu = true;
            _mainWindow = mainWindow;
            _mainWindow.SourceInitialized += _mainWindow_SourceInitialized;
        }

        private bool MenuHostViewModelSetted => MenuHostViewModel != null;

        private bool ToolbarHostViewModelSetted => ToolBarHostViewModel != null;


        /// <summary>
        ///     Contains the ViewModel of the MainWindows MenuHostControl
        ///     This can not be changed once it was setted with a value.
        /// </summary>
        public IMenuHostViewModel MenuHostViewModel
        {
            get => _menuHostViewModel;
            set
            {
                if (MenuHostViewModelSetted)
                    throw new InvalidOperationException("You can not change the MenuHostViewModel once it was seeted up");
                _menuHostViewModel = value;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Contains the ViewModel of the MainWindows ToolbarHostControl
        ///     This can not be changed once it was setted with a value
        /// </summary>
        public IToolBarHostViewModel ToolBarHostViewModel
        {
            get => _toolBarHostViewModel;
            set
            {
                if (ToolbarHostViewModelSetted)
                    throw new InvalidOperationException(
                        "You can not change the ToolBarHostViewModel once it was seeted up");
                _toolBarHostViewModel = value;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Contains information whether a TitleBar is displayed or not
        /// </summary>
        public bool UseTitleBar
        {
            get => _useTitleBar;
            set
            {
                if (Equals(value, _useTitleBar))
                    return;
                _useTitleBar = value;
                OnPropertyChanged();
                //NotifyOfPropertyChange(() => UseTitleBar);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Option to use a menu bar or not
        /// </summary>
        public bool UseMenu
        {
            get => _useMenu;
            set
            {
                if (Equals(value, _useMenu))
                    return;
                _useMenu = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     A SimpleWindow is a window which is not possible to resize my dragging the edges
        /// </summary>
        public bool IsSimpleWindow
        {
            get => _isSimpleWindow;
            set
            {
                if (Equals(value, _isSimpleWindow))
                    return;
                _isSimpleWindow = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Contains the Movement Technique for the MainWindow
        ///     SimpleMovemtn allows to move the Window by clicking/dragging anywhere on it
        /// </summary>
        public bool UseSimpleMovement
        {
            get => _useSimpleMovement;
            set
            {
                if (Equals(value, _useSimpleMovement))
                    return;
                _useSimpleMovement = value;
                OnUseSimpleMovementChanged();
                ((Command) SimpleMoveWindowCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     This Method initializes the MainWindow after it is initialized. Do not call from contstructor.
        /// </summary>
        protected virtual void InitializeMainWindow()
        {
            if (!MainWindowInitialized)
                throw new Exception("You can not run this Operation until the MainWindow is not initialized");
        }


        /// <summary>
        ///     Handles what happens after UseSimpleMovement was changed
        /// </summary>
        protected virtual void OnUseSimpleMovementChanged()
        {
            if (_mainWindow == null)
                return;
            if (_useSimpleMovement)
                _mainWindow.MouseDown += _mainWindow_MouseDown;
            else
                _mainWindow.MouseDown -= _mainWindow_MouseDown;
        }

        private async void _mainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await ((Command) SimpleMoveWindowCommand).Execute();
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
            SystemCommands.MinimizeWindow(_mainWindow);
        }

        protected virtual bool CanMinimize()
        {
            return _mainWindow.WindowState != WindowState.Minimized;
        }

        public ICommand MaximizeResizeCommand => new Command(MaximizeResize, CanMaximizeResize);

        protected virtual void MaximizeResize()
        {
            if (_mainWindow.WindowState == WindowState.Maximized)
                SystemCommands.RestoreWindow(_mainWindow);
            else
                SystemCommands.MaximizeWindow(_mainWindow);
        }

        protected virtual bool CanMaximizeResize()
        {
            return true;
        }

        public ICommand CloseCommand => new Command(Close, CanClose);

        protected virtual void Close()
        {
            SystemCommands.CloseWindow(_mainWindow);
        }

        protected virtual bool CanClose()
        {
            return true;
        }

        public ICommand SimpleMoveWindowCommand => new Command(MoveSimpleWindow, CanMoveSimpleWindow);

        public virtual void MoveSimpleWindow()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                _mainWindow?.DragMove();
        }

        public virtual bool CanMoveSimpleWindow()
        {
            return UseSimpleMovement;
        }

        #endregion
    }
}