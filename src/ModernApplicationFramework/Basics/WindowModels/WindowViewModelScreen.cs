using System;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.WindowModels
{
    /// <inheritdoc cref="IWindowViewModel" />
    /// <summary>
    /// View model for a <see cref="Window"/> that is an instance of <see cref="Screen"/>
    /// </summary>
    /// <seealso cref="Screen" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IWindowViewModel" />
    public class WindowViewModelScreen : Screen, IWindowViewModel
    {
        protected bool WindowInitialized;
        private Window _window;
        private bool _isSimpleWindow;
        private bool _useSimpleMovement;

        protected override void OnViewAttached(object view, object context)
        {
            _window = view as Window;
            if (_window != null)
                _window.SourceInitialized += _mainWindow_SourceInitialized;
            base.OnViewAttached(view, context);
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
                NotifyOfPropertyChange();
            }
        }

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

        /// <summary>
        ///     Handles what happens after UseSimpleMovement was changed
        /// </summary>
        protected virtual void OnUseSimpleMovementChanged()
        {
            if (_window == null)
                return;
            if (_useSimpleMovement)
                _window.MouseDown += _mainWindow_MouseDown;
            else
                _window.MouseDown -= _mainWindow_MouseDown;
        }

        private async void _mainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await ((Command) SimpleMoveWindowCommand).Execute();
        }

        private void _mainWindow_SourceInitialized(object sender, EventArgs e)
        {
            WindowInitialized = true;
        }

        #region Commands

        public ICommand MinimizeCommand => new Command(Minimize, CanMinimize);

        protected virtual void Minimize()
        {
            SystemCommands.MinimizeWindow(_window);
        }

        protected virtual bool CanMinimize()
        {
            return _window.WindowState != WindowState.Minimized;
        }

        public ICommand MaximizeResizeCommand => new Command(MaximizeResize, CanMaximizeResize);

        protected virtual void MaximizeResize()
        {
            if (_window.WindowState == WindowState.Maximized)
                SystemCommands.RestoreWindow(_window);
            else
                SystemCommands.MaximizeWindow(_window);
        }

        protected virtual bool CanMaximizeResize()
        {
            return true;
        }

        public ICommand CloseCommand => new Command(Close, CanClose);

        protected virtual void Close()
        {
            SystemCommands.CloseWindow(_window);
        }

        protected virtual bool CanClose()
        {
            return true;
        }

        public ICommand SimpleMoveWindowCommand => new Command(MoveSimpleWindow, CanMoveSimpleWindow);

        public virtual void MoveSimpleWindow()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                _window?.DragMove();
        }

        public virtual bool CanMoveSimpleWindow()
        {
            return UseSimpleMovement;
        }

        #endregion
    }
}