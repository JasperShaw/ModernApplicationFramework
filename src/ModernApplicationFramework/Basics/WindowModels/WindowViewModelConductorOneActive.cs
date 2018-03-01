using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.WindowModels
{
    /// <inheritdoc cref="IWindowViewModel" />
    /// <summary>
    ///     View model for a <see cref="Window" /> that is an instance of <see cref="Conductor{T}.Collection.OneActive" />
    ///     where T is <see langowrd="object" />
    /// </summary>
    /// <seealso cref="Conductor{T}.Collection.OneActive" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IWindowViewModel" />
    public class WindowViewModelConductorOneActive : Conductor<object>.Collection.OneActive, IWindowViewModel,
        IPartImportsSatisfiedNotification
    {
        protected bool WindowInitialized;
        private bool _isSimpleWindow;
        private bool _useSimpleMovement;
        private Window _window;

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

        public virtual bool CanMoveSimpleWindow()
        {
            return UseSimpleMovement;
        }

        public virtual void MoveSimpleWindow()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                _window?.DragMove();
        }

        public virtual void OnImportsSatisfied()
        {
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
            return _window.WindowState != WindowState.Minimized;
        }

        protected virtual void Close()
        {
            SystemCommands.CloseWindow(_window);
        }

        protected virtual void MaximizeResize()
        {
            if (_window.WindowState == WindowState.Maximized)
                SystemCommands.RestoreWindow(_window);
            else
                SystemCommands.MaximizeWindow(_window);
        }

        protected virtual void Minimize()
        {
            SystemCommands.MinimizeWindow(_window);
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

        protected override void OnViewAttached(object view, object context)
        {
            _window = view as Window;
            if (_window != null)
                _window.SourceInitialized += _mainWindow_SourceInitialized;
            base.OnViewAttached(view, context);
        }

        protected virtual void OnWindowClosing(CancelEventArgs e)
        {
            WindowClosing?.Invoke(this, e);
        }

        private async void _mainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await ((Command) SimpleMoveWindowCommand).Execute();
        }

        private void _mainWindow_SourceInitialized(object sender, EventArgs e)
        {
            WindowInitialized = true;
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
    }
}