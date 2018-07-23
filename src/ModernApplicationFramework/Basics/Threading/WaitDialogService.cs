using System;
using System.Windows.Threading;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Utilities; 

namespace ModernApplicationFramework.Basics.Threading
{
    internal class WaitDialogService : DisposableObject
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
            _dispatcher.Invoke(() =>
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

    internal class DialogInitializationArguments
    {
        public string AppName { get; set; }

        public IntPtr AppMainWindowHandle { get; set; }

        public int AppProcessId { get; set; }
    }

    internal class DialogUpdateArguments
    {
        public string WaitMessage { get; set; }

        public string ProgressMessage { get; set; }

        public bool IsCancellable { get; set; }

        public int CurrentStepCount { get; set; }

        public int TotalStepCount { get; set; }

        public DialogUpdateArguments()
        {
        }

        public DialogUpdateArguments(string waitMessage, string progressMessage, bool isCancellable)
            : this(waitMessage, progressMessage, isCancellable, 0, 0)
        {
        }

        public DialogUpdateArguments(string waitMessage, string progressMessage, bool isCancellable, int currentStepCount, int totalStepCount)
        {
            WaitMessage = waitMessage;
            ProgressMessage = progressMessage;
            IsCancellable = isCancellable;
            CurrentStepCount = currentStepCount;
            TotalStepCount = totalStepCount;
        }

        public void Merge(DialogUpdateArguments argsToMerge)
        {
            if (!string.IsNullOrEmpty(argsToMerge.WaitMessage))
                WaitMessage = argsToMerge.WaitMessage;
            ProgressMessage = argsToMerge.ProgressMessage ?? ProgressMessage;
            IsCancellable = argsToMerge.IsCancellable;
            CurrentStepCount = argsToMerge.CurrentStepCount;
            TotalStepCount = argsToMerge.TotalStepCount;
        }
    }

    internal class DialogShowArguments : DialogUpdateArguments
    {
        public string Caption { get; set; }

        public bool IsProgressVisible { get; set; }

        public bool ShowMarqueeProgress { get; set; }

        public string RootWindowCaption { get; set; }

        public IntPtr ActiveWindowHandle { get; set; }

        public DialogShowArguments()
        {
        }

        public DialogShowArguments(string caption, string waitMessage, string progressMessage, bool isCancellable, bool showProgress, int currentStepCount, int totalStepCount)
            : base(waitMessage, progressMessage, isCancellable, currentStepCount, totalStepCount)
        {
            IsProgressVisible = showProgress;
            ShowMarqueeProgress = totalStepCount < 0;
            Caption = caption;
        }

        public void SetActiveWindowArgs(string rootWindowCaption, IntPtr activeWindowHandle)
        {
            RootWindowCaption = rootWindowCaption;
            ActiveWindowHandle = activeWindowHandle;
        }
    }
}
