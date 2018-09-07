using System;
using System.Windows.Threading;
using ModernApplicationFramework.Threading.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading.WaitDialog
{
    internal class WaitDialogService : DisposableObject, IWaitDialogService
    {
        private readonly Action _onCancelAction;
        private bool _isDialogAcquired;
        private Dispatcher _dispatcher;
        private WaitDialogWindow _window;
        private string _hostAppName;
        private WaitDialogDataSource _dataSource;


        public WaitDialogService(ICancelHandler cancelHandler)
        {
            _onCancelAction = cancelHandler.OnCancel;
        }

        public void Initialize(DialogInitializationArguments args)
        {
            if (_dispatcher != null)
                return;
            _dispatcher = BackgroundDispatcher.GetBackgroundDispatcher("WaitDialog-" + args.AppProcessId);
            _dispatcher.Invoke(() =>
            {
                _hostAppName = args.AppName;
                _dataSource = new WaitDialogDataSource();
                _window = new WaitDialogWindow(args.AppMainWindowHandle, args.AppProcessId);
                _window.Cancelled += OnDialogCancelled;
                _window.DataContext = _dataSource;
            });
        }

        public void CloseDialog()
        {
            ThrowIfDisposed();
            _dispatcher.Invoke(() =>
            {
                if (!_isDialogAcquired)
                    return;
                _window?.HideDialog();
                _isDialogAcquired = false;
            });
        }

        public void ShowDialog(DialogShowArguments args)
        {
            ThrowIfDisposed();
            _dispatcher?.Invoke(() =>
            {
                if (_dataSource == null)
                    return;
                _isDialogAcquired = true;
                _dataSource.Caption = string.IsNullOrEmpty(args.Caption) ? _hostAppName : args.Caption;
                _dataSource.WaitMessage = string.IsNullOrEmpty(args.WaitMessage) ? _hostAppName : args.WaitMessage;
                _dataSource.ProgressMessage = args.ProgressMessage;
                _dataSource.IsCancellable = args.IsCancellable;
                _dataSource.IsProgressVisible = args.IsProgressVisible;
                _dataSource.ShowMarqueeProgress = args.ShowMarqueeProgress;
                _window.TryShowDialog(args.ActiveWindowHandle, args.RootWindowCaption);
            });
        }

        public void UpdateDialog(DialogUpdateArguments args)
        {
            ThrowIfDisposed();
            _dispatcher.Invoke(() =>
            {
                if (!_isDialogAcquired || _dataSource == null)
                    return;
                if (!string.IsNullOrEmpty(args.WaitMessage))
                    _dataSource.WaitMessage = args.WaitMessage;
                _dataSource.ProgressMessage = args.ProgressMessage;
                _dataSource.IsCancellable = args.IsCancellable;
                if (!_dataSource.IsProgressVisible || _dataSource.ShowMarqueeProgress)
                    return;
                _dataSource.CurrentStep = args.CurrentStepCount;
                _dataSource.TotalSteps = args.TotalStepCount;
            });
        }

        protected override void DisposeManagedResources()
        {
            _isDialogAcquired = false;
            _dispatcher?.InvokeShutdown();
            _dispatcher = null;
            _window = null;
            base.DisposeManagedResources();
        }

        private void OnDialogCancelled(object sender, EventArgs e)
        {
            var action = _onCancelAction;
            action?.Invoke();
        }
    }

    public interface IWaitDialogService
    {
        void CloseDialog();
        void Initialize(DialogInitializationArguments args);
        void ShowDialog(DialogShowArguments args);
        void UpdateDialog(DialogUpdateArguments args);
    }

    public interface ICancelHandler
    {
        void OnCancel();
    }
}
