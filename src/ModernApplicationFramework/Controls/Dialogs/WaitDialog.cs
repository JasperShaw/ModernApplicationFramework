using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Dialogs
{
    //TODO: Restyle and improve
    /// <summary>
    /// A dialog showing a progress bar
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Controls.Windows.ModernChromeWindow" />
    public class WaitDialog : ModernChromeWindow
    {
        private readonly IWaitDialogCallback _callback;

        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof(string), typeof(WaitDialog),
                new FrameworkPropertyMetadata("Preparing..."));

        private readonly Dispatcher _dispatcher;

        static WaitDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitDialog),
                new FrameworkPropertyMetadata(typeof(WaitDialog)));
        }

        public WaitDialog(IWaitDialogCallback callback) : this()
        {
            _callback = callback;
        }

        public WaitDialog()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Height = 130;
            Width = 450;
        }

        public string MessageText
        {
            get => (string) GetValue(MessageTextProperty);
            set => SetValue(MessageTextProperty, value);
        }

        public bool ActionWasAborted { get; private set; }


        public bool? ShowDialog(Func<Task> func)
        {
            bool? result = true;

            var t = new Thread(async () =>
            {
                // start the submitted action
                try
                {
                    await func.Invoke();
                }
                catch
                {
                    
                }
                finally
                {
                    DoClose();
                }
            });
            _thread = t;
            t.Start();


            if (t.ThreadState != ThreadState.Stopped)
            {
                result = ShowDialog();
            }
            return result;
        }


        //public bool? ShowDialog(Action action)
        //{
        //    bool? result = true;
        //    // start a new thread to start the submitted action
        //    var t = new Thread(() =>
        //    {
        //        // start the submitted action
        //        try
        //        {
        //            action.Invoke();
        //        }
        //        finally
        //        {
        //            DoClose();
        //        }
        //    });
        //    _thread = t;
        //    t.Start();

        //    if (t.ThreadState != ThreadState.Stopped)
        //    {
        //        result = ShowDialog();
        //    }
        //    return result;
        //}

        private Thread _thread;

        private void DoClose()
        {
            _dispatcher.BeginInvoke(new ThreadStart(Close));
        }

        private new bool? ShowDialog()
        {
            Topmost = true;
            base.ShowDialog();
            return true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _callback?.OnCanceled();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_thread.IsAlive)
                ActionWasAborted = true;
            _thread?.Abort();
            DoClose();
            base.OnClosed(e);

        }
    }

    public interface IWaitDialog
    {
        void StartWaitDialog(string caption, string waitMessage, string progressText, string statusBarText, int delayToShowDialog, bool isCacelable, bool showMarqueeProgress);

        void StartWaitDialogWithPercentageProgress(string caption, string waitMessage, string progressText, string statusBarText, int delayToShowDialog, bool isCacelable, int totalSteps, int currentStep);

        void StartWaitDialogWithCallback(string caption, string waitMessage, string progressText, string statusBarText, bool isCancelable, int delayToShowDialog, bool showProgress, int totalSteps, int currentStep, IWaitDialogCallback callback);

        void EndWaitDialog(out bool canceled);

        void UpdateProgress(string waitMessage, string progressText, string statusBarText, int currentStep, int totalSteps, bool disableCancel, out bool canceled);

        void HasCanceled(out bool canceled);
    }

    public interface IWaitDialogCallback
    {
        void OnCanceled();
    }

    public static class WaitDialogHelper
    {
        //public static IWaitDialog CreateInstance(this IWaitDialogFactory factory)
        //{
        //    Validate.IsNotNull(factory, nameof(factory));
        //    factory.CreateInstance(out var waitDialog);
        //    return waitDialog;
        //}

        //public static Session StartWaitDialog(this IWaitDialogFactory dialogFactory, string waitCaption,
        //    WaitDialogProgressData initialProgress = null, TimeSpan delayToShowDialog = default(TimeSpan))
        //{
        //    Validate.IsNotNull(dialogFactory, nameof(dialogFactory));
        //    var instance = dialogFactory.CreateInstance();
        //    var cancellationTokenSource = new CancellationTokenSource();
        //    var progress = (IProgress<WaitDialogProgressData>)new ProgressAdapter(instance, cancellationTokenSource);
        //    var cancellationCallback = new CancellationCallback(cancellationTokenSource);
        //    instance.StartWaitDialogWithCallback(waitCaption,
        //        initialProgress?.WaitMessage, initialProgress?.ProgressText,
        //        initialProgress?.StatusBarText, initialProgress != null && initialProgress.IsCancelable,
        //        (int)delayToShowDialog.TotalSeconds, true, initialProgress?.TotalSteps ?? 0,
        //        initialProgress?.CurrentStep ?? 0, cancellationCallback);
        //    return new Session(instance, progress, cancellationTokenSource.Token);
        //}


        public static SessionEx CreateSession(WaitDialogProgressData initialProgress = null,
            TimeSpan delayToShowDialog = default(TimeSpan))
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = (IProgress<WaitDialogProgressData>)new ProgressAdapterEx(cancellationTokenSource);

            var cancellationCallback = new CancellationCallback(cancellationTokenSource);

            return new SessionEx(progress, cancellationTokenSource.Token, cancellationCallback);
        }

        public static bool EndWaitDialog(this IWaitDialog dialog)
        {
            Validate.IsNotNull(dialog, nameof(dialog));
            dialog.EndWaitDialog(out var canceled);
            return canceled;
        }



        public class SessionEx : IDisposable
        {
            private bool _disposed;

            public IProgress<WaitDialogProgressData> Progress { get; }

            public CancellationToken UserCancellationToken { get; }
            public IWaitDialogCallback Callback { get; }

            internal SessionEx(IProgress<WaitDialogProgressData> progress, CancellationToken token, IWaitDialogCallback callback)
            {
                Validate.IsNotNull(progress, nameof(progress));
                Progress = progress;
                UserCancellationToken = token;
                Callback = callback;
            }

            public void Dispose()
            {
                var session = this;
                if (session._disposed)
                    return;
                session._disposed = true;
            }
        }

        //public class Session : IDisposable
        //{
        //    private readonly IWaitDialog _dialog;
        //    private bool _disposed;

        //    public IProgress<WaitDialogProgressData> Progress { get; }

        //    public CancellationToken UserCancellationToken { get; }

        //    internal Session(IWaitDialog dialog, IProgress<WaitDialogProgressData> progress, CancellationToken token)
        //    {
        //        Validate.IsNotNull(dialog, nameof(dialog));
        //        Validate.IsNotNull(progress, nameof(progress));
        //        _dialog = dialog;
        //        Progress = progress;
        //        UserCancellationToken = token;
        //    }

        //    public void Dispose()
        //    {
        //        new JoinableTaskFactory(new JoinableTaskContext()).Run(InternalDispose);
        //    }

        //    private async Task InternalDispose()
        //    {
        //        Session session = this;
        //        if (session._disposed)
        //            return;
        //        session._disposed = true;
        //        await new JoinableTaskFactory(new JoinableTaskContext()).SwitchToMainThreadAsync();
        //        session._dialog.EndWaitDialog();
        //    }
        //}

        private class ProgressAdapterEx : IProgress<WaitDialogProgressData>
        {
            private readonly CancellationTokenSource _cancellationTokenSource;

            internal ProgressAdapterEx(CancellationTokenSource cancellationTokenSource)
            {
                _cancellationTokenSource = cancellationTokenSource;
            }

            public void Report(WaitDialogProgressData value)
            {
                if (value == null)
                    return;
                try
                {
                    //_dialog.UpdateProgress(value.WaitMessage, value.ProgressText, value.StatusBarText,
                    //    value.CurrentStep, value.TotalSteps, !value.IsCancelable, out var pfCanceled);

                    //if (!pfCanceled || _cancellationTokenSource == null)
                    //    return;
                    //_cancellationTokenSource.Cancel();
                }
                catch
                {
                    // ignored
                }
            }
        }




        private class CancellationCallback : IWaitDialogCallback
        {
            private readonly CancellationTokenSource _cancellationSource;

            internal CancellationCallback(CancellationTokenSource cancellationSource)
            {
                Validate.IsNotNull(cancellationSource, nameof(cancellationSource));
                _cancellationSource = cancellationSource;
            }

            public void OnCanceled()
            {
                _cancellationSource.Cancel();
            }
        }

        //private class ProgressAdapter : IProgress<WaitDialogProgressData>
        //{
        //    private readonly IWaitDialog _dialog;
        //    private readonly CancellationTokenSource _cancellationTokenSource;

        //    internal ProgressAdapter(IWaitDialog dialog, CancellationTokenSource cancellationTokenSource)
        //    {
        //        Validate.IsNotNull(dialog, nameof(dialog));
        //        _dialog = dialog;
        //        _cancellationTokenSource = cancellationTokenSource;
        //    }

        //    public void Report(WaitDialogProgressData value)
        //    {
        //        if (value == null)
        //            return;
        //        try
        //        {
        //            _dialog.UpdateProgress(value.WaitMessage, value.ProgressText, value.StatusBarText,
        //                value.CurrentStep, value.TotalSteps, !value.IsCancelable, out var pfCanceled);

        //            if (!pfCanceled || _cancellationTokenSource == null)
        //                return;
        //            _cancellationTokenSource.Cancel();
        //        }
        //        catch
        //        {
        //            // ignored
        //        }
        //    }
        //}

    }

    public class WaitDialogProgressData
    {
        public WaitDialogProgressData(string waitMessage, string progressText = null, string statusBarText = null, bool isCancelable = false)
            : this(waitMessage, progressText, statusBarText, isCancelable, 0, 0)
        {
        }

        public WaitDialogProgressData(string waitMessage, string progressText, string statusBarText, bool isCancelable, int currentStep, int totalSteps)
        {
            WaitMessage = waitMessage;
            ProgressText = progressText;
            StatusBarText = statusBarText;
            IsCancelable = isCancelable;
            CurrentStep = currentStep;
            TotalSteps = totalSteps;
        }

        public string WaitMessage { get; }

        public string ProgressText { get; }

        public string StatusBarText { get; }

        public int CurrentStep { get; }

        public int TotalSteps { get; }

        public bool IsCancelable { get; }

        public WaitDialogProgressData NextStep()
        {
            return new WaitDialogProgressData(WaitMessage, ProgressText, StatusBarText, IsCancelable, CurrentStep + 1, TotalSteps);
        }
    }

    public interface IWaitDialogFactory
    {
        void CreateInstance(out IWaitDialog waitDialog);
    }

    public class WaitDialogFactory : IWaitDialogFactory
    {
        public void CreateInstance(out IWaitDialog waitDialog)
        {
            waitDialog = new WaitDialogEx();
        }
    }


    public class WaitDialogEx : Window, IWaitDialog
    {
        private IWaitDialogCallback _callback;

        public void StartWaitDialog(string caption, string waitMessage, string progressText, string statusBarText,
            int delayToShowDialog, bool isCacelable, bool showMarqueeProgress)
        {
            
        }

        public void StartWaitDialogWithPercentageProgress(string caption, string waitMessage, string progressText,
            string statusBarText, int delayToShowDialog, bool isCacelable, int totalSteps, int currentStep)
        {

        }

        public void StartWaitDialogWithCallback(string caption, string waitMessage, string progressText, string statusBarText,
            bool isCancelable, int delayToShowDialog, bool showProgress, int totalSteps, int currentStep,
            IWaitDialogCallback callback)
        {
            Show();
            _callback = callback;
        }

        public void EndWaitDialog(out bool canceled)
        {    
            canceled = false;
            Close();
        }

        public void UpdateProgress(string waitMessage, string progressText, string statusBarText, int currentStep, int totalSteps,
            bool disableCancel, out bool canceled)
        {
            canceled = false;
        }

        public void HasCanceled(out bool canceled)
        {
            canceled = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _callback.OnCanceled();
        }
    }
}