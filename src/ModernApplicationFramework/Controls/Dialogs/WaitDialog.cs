using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using Action = System.Action;

namespace ModernApplicationFramework.Controls.Dialogs
{
    internal class WaitDialog : ModernChromeWindow, IWaitDialog
    {
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            "Caption", typeof(string), typeof(WaitDialog), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(
            "CurrentStep", typeof(double), typeof(WaitDialog), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty IsCancelableProperty = DependencyProperty.Register(
            "IsCancelable", typeof(bool), typeof(WaitDialog), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty ProgressTextProperty = DependencyProperty.Register(
            "ProgressText", typeof(string), typeof(WaitDialog), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ShowMarqueeProgressProperty = DependencyProperty.Register(
            "ShowMarqueeProgress", typeof(bool), typeof(WaitDialog), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty ShowProgressProperty = DependencyProperty.Register(
            "ShowProgress", typeof(bool), typeof(WaitDialog), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty TotalStepsProperty = DependencyProperty.Register(
            "TotalSteps", typeof(double), typeof(WaitDialog), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty WaitMessageProperty = DependencyProperty.Register(
            "WaitMessage", typeof(string), typeof(WaitDialog), new PropertyMetadata(default(string)));

        private readonly IStatusBarDataModelService _statusBar;

        private readonly TimerCallback _timerCallback = TimerCallbackLogic;
        private bool _canceled;
        private bool _executionDone;

        private bool _isClosing;
        private string _statusBarText;
        private Thread _thread;
        private Timer _timer;

        public ICommand CancelCommand => new Command(Cancel, () => IsCancelable);

        public IWaitDialogCallback CancellationCallback { get; private set; }

        public string Caption
        {
            get => (string) GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public double CurrentStep
        {
            get => (double) GetValue(CurrentStepProperty);
            set => SetValue(CurrentStepProperty, value);
        }

        public int DelayToShow { get; set; }

        public bool IsCancelable
        {
            get => (bool) GetValue(IsCancelableProperty);
            set => SetValue(IsCancelableProperty, value);
        }

        public string ProgressText
        {
            get => (string) GetValue(ProgressTextProperty);
            set => SetValue(ProgressTextProperty, value);
        }

        public bool ShowMarqueeProgress
        {
            get => (bool) GetValue(ShowMarqueeProgressProperty);
            set => SetValue(ShowMarqueeProgressProperty, value);
        }

        public bool ShowProgress
        {
            get => (bool) GetValue(ShowProgressProperty);
            set => SetValue(ShowProgressProperty, value);
        }

        public string StatusBarText
        {
            get => _statusBarText;
            set
            {
                _statusBarText = value;
                _statusBar?.SetText(value);
            }
        }

        public double TotalSteps
        {
            get => (double) GetValue(TotalStepsProperty);
            set
            {
                SetValue(TotalStepsProperty, value);
                ShowMarqueeProgress = value == 0;
            }
        }

        public string WaitMessage
        {
            get => (string) GetValue(WaitMessageProperty);
            set => SetValue(WaitMessageProperty, value);
        }

        private Action Action { get; set; }

        private Func<Task> Function { get; set; }

        static WaitDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitDialog),
                new FrameworkPropertyMetadata(typeof(WaitDialog)));
        }

        public WaitDialog()
        {
            _statusBar = IoC.Get<IStatusBarDataModelService>();
            Width = 465;
        }

        public void EndWaitDialog(out bool canceled)
        {
            canceled = _canceled;
            InternalClose();
        }

        public void HasCanceled(out bool canceled)
        {
            canceled = _canceled;
        }

        public void SetAction(Action action)
        {
            Action = action;
        }

        public void SetFunction(Func<Task> func)
        {
            Function = func;
        }

        public void StartWaitDialog(string caption, string waitMessage, string progressText, string statusBarText,
            int delayToShowDialog, bool isCancelable, bool showMarqueeProgress)
        {
            Caption = caption;
            WaitMessage = waitMessage;
            ProgressText = progressText;
            StatusBarText = statusBarText;
            IsCancelable = isCancelable;
            DelayToShow = delayToShowDialog;
            ShowProgress = true;
            TotalSteps = 0;
            Execute();
            InternalShow();
        }

        public void StartWaitDialogWithCallback(string caption, string waitMessage, string progressText,
            string statusBarText,
            bool isCancelable, int delayToShowDialog, bool showProgress, int totalSteps, int currentStep,
            IWaitDialogCallback callback)
        {
            Caption = caption;
            WaitMessage = waitMessage;
            ProgressText = progressText;
            StatusBarText = statusBarText;
            IsCancelable = isCancelable;
            DelayToShow = delayToShowDialog;
            ShowProgress = showProgress;
            TotalSteps = totalSteps;
            CurrentStep = currentStep;
            CancellationCallback = callback;
            Execute();
            InternalShow();
        }

        public void StartWaitDialogWithPercentageProgress(string caption, string waitMessage, string progressText,
            string statusBarText, int delayToShowDialog, bool isCancelable, int totalSteps, int currentStep)
        {
            Caption = caption;
            WaitMessage = waitMessage;
            ProgressText = progressText;
            StatusBarText = statusBarText;
            IsCancelable = isCancelable;
            DelayToShow = delayToShowDialog;
            ShowProgress = true;
            TotalSteps = totalSteps;
            CurrentStep = currentStep;
            Execute();
            InternalShow();
        }

        public void UpdateProgress(string waitMessage, string progressText, string statusBarText, int currentStep,
            int totalSteps,
            bool disableCancel, out bool canceled)
        {
            Caliburn.Micro.Execute.OnUIThreadAsync(() =>
            {
                WaitMessage = waitMessage;
                ProgressText = progressText;
                StatusBarText = statusBarText;
                CurrentStep = currentStep;
                TotalSteps = totalSteps;
                IsCancelable = !disableCancel;
            });

            canceled = _canceled;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                if (IsCancelable)
                    CancelCommand.Execute(null);
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        private static void TimerCallbackLogic(object state)
        {
            var waitDialog = (WaitDialog) state;
            waitDialog._timer.Dispose();

            if (waitDialog._executionDone)
                return;

            if (waitDialog.Action == null && waitDialog.Function == null)
                waitDialog.Dispatcher.Invoke(DispatcherPriority.Loaded, new ThreadStart(waitDialog.Show));
            else
                waitDialog.Dispatcher.Invoke(DispatcherPriority.Loaded, new ThreadStart(() => waitDialog.ShowDialog()));
        }

        private void Cancel()
        {
            CancellationCallback?.OnCanceled();
            _canceled = true;
            InternalClose();
        }

        private void Execute()
        {
            Thread thread;
            if (Function != null)
                thread = ExecuteFunction();
            else if (Action != null)
                thread = ExecuteAction();
            else
                thread = null;
            _thread = thread;
            thread?.Start();
        }

        private Thread ExecuteAction()
        {
            var t = new Thread(() =>
            {
                try
                {
                    Action();
                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    _executionDone = true;
                    InternalClose();
                }
            });
            return t;
        }

        private Thread ExecuteFunction()
        {
            var t = new Thread(async () =>
            {
                try
                {
                    await Function();
                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    _executionDone = true;
                    InternalClose();
                }
            });
            return t;
        }

        private void InternalClose()
        {
            if (_isClosing)
                return;
            _isClosing = true;
            if (_thread != null && _thread.IsAlive)
                _thread.Interrupt();
            CancellationCallback = null;
            Function = null;
            Action = null;
            _statusBar.SetReadyText();
            if (Dispatcher.CheckAccess())
                Close();
            else
                Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(Close));
        }

        private void InternalShow()
        {
            _timer = new Timer(_timerCallback, this, DelayToShow * 1000, -1);
        }
    }
}