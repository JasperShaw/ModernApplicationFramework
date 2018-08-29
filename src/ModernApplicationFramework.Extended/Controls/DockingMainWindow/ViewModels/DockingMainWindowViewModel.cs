using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Controls.DockingMainWindow.ViewModels
{
    [Export(typeof(IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : Conductor<IDockingHostViewModel>, IDockingMainWindowViewModel,
        IPartImportsSatisfiedNotification, ICanHaveInputBindings
    {
        protected bool MainWindowInitialized;

        private bool _isSimpleWindow;
        private IMenuHostViewModel _menuHostViewModel;

        private IToolBarHostViewModel _toolBarHostViewModel;
        private bool _useSimpleMovement;
        private bool _useTitleBar;
        private bool _useMenu;


        public bool MenuHostViewModelSetted => MenuHostViewModel != null;
        public bool ToolbarHostViewModelSetted => ToolBarHostViewModel != null;
        public bool InfoBarHostSetted => InfoBarHost != null;

        public Window Window { get; private set; }
        public WindowState WindowState { get; set; }

        public DockingMainWindowViewModel()
        {
            UseTitleBar = true;
            UseMenu = true;
        }

        /// <inheritdoc />
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
                    throw new InvalidOperationException(
                        "You can not change the MenuHostViewModel once it was seeted up");
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

        public IInfoBarHost InfoBarHost
        {
            get => _infoBarHost;
            set
            {
                if (InfoBarHostSetted)
                    throw new InvalidOperationException(
                        "You can not change the InfoBarHost once it was seeted up");
                _infoBarHost = value;
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
                NotifyOfPropertyChange();
            }
        }

        public bool UseMenu
        {
            get => _useMenu;
            set
            {
                if (value == _useMenu) return;
                _useMenu = value;
                NotifyOfPropertyChange();
            }
        }

        public IDockingHostViewModel DockingHost => _dockingHost;

        public event EventHandler<CancelEventArgs> WindowClosing;
        public ICommand CloseCommand => new Command(InternalClose, CanClose);

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
                NotifyOfPropertyChange();
            }
        }

        public ICommand MaximizeResizeCommand => new Command(MaximizeResize, CanMaximizeResize);

        public ICommand MinimizeCommand => new Command(Minimize, CanMinimize);

        public ICommand SimpleMoveWindowCommand => new Command(MoveSimpleWindow, CanMoveSimpleWindow);

        /// <inheritdoc />
        /// <summary>
        ///     Contains the Movement Technique for the MainWindow
        ///     SimpleMovement allows to move the Window by clicking/dragging anywhere on it
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

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            ActivateItem(_dockingHost);
        }

        public virtual bool CanMoveSimpleWindow()
        {
            return UseSimpleMovement;
        }

        public virtual void MoveSimpleWindow()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                Window?.DragMove();
        }

        protected virtual bool CanClose()
        {
            return true;
        }

        protected virtual bool CanMaximizeResize()
        {
            return true;
        }

        protected virtual bool CanMinimize()
        {
            return Window?.WindowState != WindowState.Minimized;
        }

        protected virtual void Close()
        {
            SystemCommands.CloseWindow(Window);
        }

        protected virtual void MaximizeResize()
        {
            if (Window.WindowState == WindowState.Maximized)
                SystemCommands.RestoreWindow(Window);
            else
                SystemCommands.MaximizeWindow(Window);
        }

        protected virtual void Minimize()
        {
            SystemCommands.MinimizeWindow(Window);
        }


        /// <summary>
        ///     Handles what happens after UseSimpleMovement was changed
        /// </summary>
        protected virtual void OnUseSimpleMovementChanged()
        {
            if (Window == null)
                return;
            if (_useSimpleMovement)
                Window.MouseDown += _mainWindow_MouseDown;
            else
                Window.MouseDown -= _mainWindow_MouseDown;
        }

        protected override void OnViewLoaded(object view)
        {
            _themeManager.Theme = !string.IsNullOrEmpty(_themeManager.StartUpTheme?.Name)
                ? _themeManager.StartUpTheme
                : IoC.GetAll<Theme>().First();
          
            if (!(view is Window window))
                return;
            Window = window;
            window.SourceInitialized += _mainWindow_SourceInitialized;
            if (BindableElement == null)
                return;
            //Do not use [Import] as this causes a cycle and fails to compose
            IoC.Get<IKeyGestureService>().AddModel(this);
        }

        private async void _mainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await ((Command) SimpleMoveWindowCommand).Execute();
        }

        private void _mainWindow_SourceInitialized(object sender, EventArgs e)
        {
            MainWindowInitialized = true;
        }

        private void InternalClose()
        {
            if (WindowClosing != null)
            {
                var eventargs = new CancelEventArgs();
                OnWindowClosing(eventargs);
                if (eventargs.Cancel)
                    return;
            }
            Close();
        }

#pragma warning disable 649
        [Import] private IDockingHostViewModel _dockingHost;
        [Import] private IThemeManager _themeManager;
        private IInfoBarHost _infoBarHost;
#pragma warning restore 649

        public IEnumerable<GestureScope> GestureScopes => new[]
            {ModernApplicationFramework.Input.Command.GestureScopes.GlobalGestureScope};

        public UIElement BindableElement => Window;

        protected virtual void OnWindowClosing(CancelEventArgs e)
        {
            WindowClosing?.Invoke(this, e);
        }
    }
}