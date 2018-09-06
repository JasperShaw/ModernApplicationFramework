using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Threading;
using ModernApplicationFramework.Threading.WaitDialog;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Services.WaitDialog
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

        private readonly AsyncLazy<IWaitDialogService> _service;
        private readonly CancelHandler _cancelHandler;
        private AsyncQueue<Func<IWaitDialogService, Task>> _operationQueue;

        public bool IsCancelled { get; private set; }

        public WaitDialog()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cancelHandler = new CancelHandler(this);
            _service = new AsyncLazy<IWaitDialogService>(GetServiceAsync, ThreadHelper.JoinableTaskFactory);
        }

        private async Task<IWaitDialogService> GetServiceAsync()
        {
            try
            {
                var provider = await WaitDialogServiceProvider.CreateServiceProvider(_cancelHandler);

                await Execute.OnUIThreadAsync(() => provider.Service?.Initialize(_initializationArguments));

                return provider.Service;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void ReleaseInstance(WaitDialog instance)
        {
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


        public void UpdateDialog(DialogUpdateArguments dialogUpdateArguments)
        {
            if (!_isDialogActive)
                _dialogArguments?.Merge(dialogUpdateArguments);
            else
                _operationQueue.TryEnqueue(service => Task.Run(() => service.UpdateDialog(dialogUpdateArguments)));
        }


        public void CloseDialog()
        {
            if (_isDialogActive)
                _operationQueue.TryEnqueue(service => Task.Run(() => service.CloseDialog()));
            _isDialogActive = false;
            _cancellationCallback = null;
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
            ShowDialogInternalAsync(delayToShowDialog, args).Forget();
        }

        private async Task ShowDialogInternalAsync(TimeSpan delayToShowDialog, DialogShowArguments showArguments)
        {
            var instanceId = _currentInstanceId;
            _dialogArguments = showArguments;
            await Task.Delay(delayToShowDialog).ConfigureAwait(false);
            if (instanceId != _currentInstanceId || !_isInstanceAcquired)
                return;
            _isDialogActive = true;
            _operationQueue.TryEnqueue(async service => await Task.Run(() => service.ShowDialog(showArguments)));
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
            _operationQueue = new AsyncQueue<Func<IWaitDialogService, Task>>();
            _queueCancellationTokenSource = new CancellationTokenSource();
            StartProcessMessageQueue(); 
        }

        private void StartProcessMessageQueue()
        {
            var localAsyncQueue = _operationQueue;
            var cancellationToken = _queueCancellationTokenSource.Token;
            Task.Run(async () =>
            {
                while (!IsDisposed && !cancellationToken.IsCancellationRequested)
                {
                    var operation = await localAsyncQueue.DequeueAsync(cancellationToken);
                    try
                    {
                        var service = await GetChannelAsync();
                        cancellationToken.ThrowIfCancellationRequested();
                        if (service != null)
                            await operation(service);
                    }
                    catch(Exception e)
                    {
                    }
                }
            }).Forget();
        }

        private async Task<IWaitDialogService> GetChannelAsync()
        {
            return await _service.GetValueAsync();
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
}
