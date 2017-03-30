using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls.Primitives;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Themes;

namespace ModernApplicationFramework.Extended.DockingMainWindow.ViewModels
{
    [Export(typeof(IDockingMainWindowViewModel))]
    public class DockingMainWindowViewModel : Conductor<IDockingHostViewModel>, IDockingMainWindowViewModel,
        IPartImportsSatisfiedNotification
    {
        protected bool MainWindowInitialized;

        private BitmapImage _activeIcon;
        private BitmapImage _icon;
        private bool _isSimpleWindow;


        private IMenuHostViewModel _menuHostViewModel;

        private BitmapImage _passiveIcon;

        private StatusBar _statusBar;

        private IToolBarHostViewModel _toolBarHostViewModel;
        private bool _useSimpleMovement;
        private bool _useStatusbar;
        private bool _useTitleBar;
        private bool _useMenu;


        public bool MenuHostViewModelSetted => MenuHostViewModel != null;
        public bool ToolbarHostViewModelSetted => ToolBarHostViewModel != null;

        /// <summary>
        ///     Contains the Current Icon of the MainWindow
        /// </summary>
        public BitmapImage Icon
        {
            get => _icon;
            set
            {
                if (Equals(value, _icon))
                    return;
                _icon = value;
                NotifyOfPropertyChange();
            }
        }

        public Window Window { get; private set; }
        public WindowState WindowState { get; set; }

        public DockingMainWindowViewModel()
        {
            UseStatusBar = true;
            UseTitleBar = true;
            UseMenu = true;
        }

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


        /// <summary>
        ///     Contains the StatusBar of the MainWindow
        ///     This can not be changed once it was setted with a value
        /// </summary>
        public StatusBar StatusBar
        {
            get { return _statusBar; }
            set
            {
                if (_statusBar == null)
                    _statusBar = value;
            }
        }

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

        /// <summary>
        ///     Contains information whether a StatusBar is displayed or not
        /// </summary>
        public bool UseStatusBar
        {
            get => _useStatusbar;
            set
            {
                if (Equals(value, _useStatusbar))
                    return;
                _useStatusbar = value;
                NotifyOfPropertyChange(() => UseStatusBar);
            }
        }

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

        /// <summary>
        ///     Contains the Active Icon for the MainWindow
        /// </summary>
        public BitmapImage ActiveIcon
        {
            get => _activeIcon;
            set
            {
                if (Equals(value, _activeIcon))
                    return;
                _activeIcon = value;
                NotifyOfPropertyChange(() => ActiveIcon);
                ApplyWindowIconChange();
            }
        }

        public ICommand ChangeWindowIconActiveCommand => new Command(ChangeWindowIconActive, CanChangeWindowIcon);

        public ICommand ChangeWindowIconPassiveCommand => new Command(ChangeWindowIconPassive, CanChangeWindowIcon);

        public ICommand CloseCommand => new Command(Close, CanClose);

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

        /// <summary>
        ///     Contains the Passive Icon for the MainWindow
        /// </summary>
        public BitmapImage PassiveIcon
        {
            get => _passiveIcon;
            set
            {
                if (Equals(value, _passiveIcon))
                    return;
                _passiveIcon = value;
                NotifyOfPropertyChange(() => PassiveIcon);
                ApplyWindowIconChange();
            }
        }

        public ICommand SimpleMoveWindowCommand => new Command(MoveSimpleWindow, CanMoveSimpleWindow);

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

        public void ChangeWindowIconActive()
        {
            Icon = ActiveIcon;
        }

        public void ChangeWindowIconPassive()
        {
            Icon = PassiveIcon;
        }

        protected virtual void ApplyWindowIconChange()
        {
            if (Window == null)
                return;
            Icon = Window.IsActive ? ActiveIcon : PassiveIcon;
        }

        protected virtual bool CanClose()
        {
            //TODO: This is part of the Editor Base
            //var items = DockingHost.Documents.OfType<StorableDocument>().Where(x => x.IsDirty);
            //var storableDocuments = items as IList<StorableDocument> ?? items.ToList();
            //if (!storableDocuments.Any())
            //    return true;

            //var saveList = storableDocuments.Select(item => new SaveDirtyDocumentItem(item.DisplayName)).ToList();

            //var result = SaveDirtyDocumentsDialog.Show(saveList);
            //switch (result)
            //{
            //    case MessageBoxResult.Yes:
            //        foreach (var item in storableDocuments)
            //            item.SaveFileCommand.Execute(null);
            //        return true;
            //    case MessageBoxResult.No:
            //        Application.Current.Shutdown(0);
            //        return true;
            //}
            //return false;
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
            var window = view as Window;
            if (window == null)
                return;
            Window = window;
            window.SourceInitialized += _mainWindow_SourceInitialized;
            window.Activated += _mainWindow_Activated;
            window.Deactivated += _mainWindow_Deactivated;

            _themeManager.Theme = !string.IsNullOrEmpty(_themeManager.StartUpTheme?.Name) ? _themeManager.StartUpTheme : new GenericTheme();  

            _commandKeyGestureService.BindKeyGesture((UIElement) view);
        }

        private async void _mainWindow_Activated(object sender, EventArgs e)
        {
            await ((Command) ChangeWindowIconActiveCommand).Execute();
        }

        private async void _mainWindow_Deactivated(object sender, EventArgs e)
        {
            await ((Command) ChangeWindowIconPassiveCommand).Execute();
        }

        private async void _mainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await ((Command) SimpleMoveWindowCommand).Execute();
        }

        private void _mainWindow_SourceInitialized(object sender, EventArgs e)
        {
            MainWindowInitialized = true;
        }

        private bool CanChangeWindowIcon()
        {
            return ActiveIcon != null && PassiveIcon != null;
        }

#pragma warning disable 649
        [Import] private IDockingHostViewModel _dockingHost;
        [Import] private IThemeManager _themeManager;
        [Import] private IKeyGestureHandler _commandKeyGestureService;
#pragma warning restore 649
    }
}