using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Threading
{
    internal class WaitDialog : DisposableObject
    {
        private static WaitDialog _sharedInstance;
        private static bool _isInstanceAcquired;
        private static int _currentInstanceId;
        private IWaitDialogCallback _cancellationCallback;
        private bool _isDialogActive;
        private DialogShowArguments _dialogArguments;
        private CancellationTokenSource _queueCancellationTokenSource;
        private DialogInitializationArguments _initializationArguments;

        private readonly WaitDialogService _service;

        public bool IsCancelled { get; private set; }

        public WaitDialog()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var cancelHandler = new CancelHandler(this);
            _service = new WaitDialogService(cancelHandler);
        }

        public static void ReleaseInstance(WaitDialog instance)
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(ReleaseInstance));
            if (_sharedInstance == null || _sharedInstance != instance)
                return;
            _sharedInstance.CloseDialog();
            _isInstanceAcquired = false;
        }

        public static WaitDialog AcquireInstance()
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(AcquireInstance));
            if (_isInstanceAcquired)
                return null;
            if (_sharedInstance == null)
                _sharedInstance = new WaitDialog();
            _isInstanceAcquired = true;
            ++_currentInstanceId;
            return _sharedInstance;
        }

        public async void UpdateDialog(DialogUpdateArguments dialogUpdateArguments)
        {
            if (!_isDialogActive)
                _dialogArguments?.Merge(dialogUpdateArguments);
            else
                await ThreadHelper.Generic.InvokeAsync(() => _service.UpdateDialog(dialogUpdateArguments));
        }


        public async void CloseDialog()
        {
            if (_isDialogActive)
                await ThreadHelper.Generic.InvokeAsync(() => _service.CloseDialog());
        }

        public void Show(TimeSpan delayToShowDialog, DialogShowArguments args, IWaitDialogCallback callback)
        {
            Validate.IsNotNull(args, nameof(args));
            ThreadHelper.ThrowIfNotOnUIThread(nameof(Show));
            EnsureInitialized();
            var activeWindow = User32.GetActiveWindow();
            var text = User32.GetWindowText(GetRootOwnerWindow(activeWindow));
            args.SetActiveWindowArgs(text, activeWindow);
            IsCancelled = false;
            _cancellationCallback = callback;
            MafTaskHelper.RunAsync(ShowDialogInternalAsync(delayToShowDialog, args));
        }

        private async Task ShowDialogInternalAsync(TimeSpan delayToShowDialog, DialogShowArguments showArguments)
        {
            var instanceId = _currentInstanceId;
            _dialogArguments = showArguments;
            await Task.Delay(delayToShowDialog).ConfigureAwait(false);
            if (instanceId != _currentInstanceId || !_isInstanceAcquired)
                return;
            _isDialogActive = true;
            await ThreadHelper.Generic.InvokeAsync(() => _service.ShowDialog(showArguments));
        }

        protected override void DisposeNativeResources()
        {
            _queueCancellationTokenSource?.Cancel();
            base.DisposeNativeResources();
        }

        private void EnsureInitialized()
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(EnsureInitialized));
            if (_initializationArguments == null)
            {
                _initializationArguments = new DialogInitializationArguments
                {
                    AppMainWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle,
                    AppProcessId = Process.GetCurrentProcess().Id
                };
                var shell = IoC.Get<IMafUIShell>();
                shell.GetAppName(out var appName);
                _initializationArguments.AppName = appName;
            }
            _queueCancellationTokenSource = new CancellationTokenSource();
            _service.Initialize(_initializationArguments);
        }


        private void OnDialogCancellation()
        {
            IsCancelled = true;
            Task.Run(() => _cancellationCallback?.OnCanceled());
        }



        private class CancelHandler : ICancelHandler
        {
            private readonly WaitDialog _owner;

            public CancelHandler(WaitDialog owner)
            {
                _owner = owner;
            }

            public void OnCancel()
            {
                _owner?.OnDialogCancellation();
            }
        }

        private static IntPtr GetRootOwnerWindow(IntPtr handle)
        {
            while (true)
            {
                var window = User32.GetWindow(handle, 4);
                if (!(window == IntPtr.Zero))
                    handle = window;
                else
                    break;
            }
            return handle;
        }
    }

    internal interface ICancelHandler
    {
        void OnCancel();
    }
}
