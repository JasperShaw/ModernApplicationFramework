using System;
using System.Runtime.InteropServices;
using System.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Threading.WaitDialog;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Services.WaitDialog
{
    internal sealed class WaitDialogServiceWrapper : DisposableObject, IWaitDialog
    {
        private readonly object _syncObject = new object();
        private WaitDialog _instance;
        private IStatusBarDataModelService _statusBarService;
        private bool _resetStatusBarOnClose;
        private bool _isUiSuppressed;
        private bool _isDialogStarted;
        private CancellationTokenSource _statusUpdateTaskCancellation;

        public void EndWaitDialog(out bool canceled)
        {
            canceled = false;
            canceled = CloseDialogHelper();
        }

        public void HasCanceled(out bool canceled)
        {
            var instance = _instance;
            canceled = instance != null && instance.IsCancelled;
        }

        public void StartWaitDialog(string caption, string waitMessage, string progressText, string statusBarText,
            int delayToShowDialog, bool isCancelable, bool showMarqueeProgress)
        {
            StartWaitDialogHelper(caption, waitMessage, progressText, delayToShowDialog, null, statusBarText,
                isCancelable, showMarqueeProgress);
        }

        public void StartWaitDialogWithCallback(string caption, string waitMessage, string progressText,
            string statusBarText,
            bool isCancelable, int delayToShowDialog, bool showProgress, int totalSteps, int currentStep,
            IWaitDialogCallback callback)
        {
            StartWaitDialogHelper(caption, waitMessage, progressText, delayToShowDialog, null, statusBarText,
                isCancelable, showProgress, currentStep, totalSteps, callback);
        }

        public void StartWaitDialogWithPercentageProgress(string caption, string waitMessage, string progressText,
            string statusBarText, int delayToShowDialog, bool isCancelable, int totalSteps, int currentStep)
        {
            StartWaitDialogHelper(caption, waitMessage, progressText, delayToShowDialog, null, statusBarText,
                isCancelable, isCancelable, currentStep, totalSteps);
        }

        public void UpdateProgress(string waitMessage, string progressText, string statusBarText, int currentStep,
            int totalSteps,
            bool disableCancel, out bool canceled)
        {
            var flag = UpdateDialogHelper(waitMessage, progressText, statusBarText, !disableCancel, currentStep, totalSteps);
            if (UnsafeHelpers.IsOptionalOutParamSet(out canceled))
                return;
            canceled = flag;
        }

        protected override void DisposeNativeResources()
        {
            if (ThreadHelper.CheckAccess())
                HideDialogAndResetStatusBar();
            base.DisposeNativeResources();
        }

        private void HideDialogAndResetStatusBar()
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(HideDialogAndResetStatusBar));
            if (!_isUiSuppressed)
                _instance?.CloseDialog();
            if (_resetStatusBarOnClose && _statusBarService != null)
            {
                _statusUpdateTaskCancellation.Cancel();
                _statusUpdateTaskCancellation = null;
                _statusBarService.SetText(null);
            }

            _resetStatusBarOnClose = false;
        }

        private bool CloseDialogHelper()
        {
            if (!ThreadHelper.CheckAccess())
            {
                var flag = false;
                ThreadHelper.Generic.Invoke(() => flag = CloseDialogHelper());
                return false;
            }
            ThreadHelper.ThrowIfNotOnUIThread(nameof(CloseDialogHelper));
            lock (_syncObject)
            {
                if (!_isDialogStarted)
                    Marshal.ThrowExceptionForHR(-2147418113);
                var flag = false;
                if (_instance != null)
                    flag = _instance.IsCancelled;
                HideDialogAndResetStatusBar();
                ReleaseDialogInstance();
                _isDialogStarted = false;
                return flag;
            }
        }

        private void ReleaseDialogInstance()
        {
            lock (_syncObject)
            {
                if (_instance == null)
                    return;
                WaitDialog.ReleaseInstance(_instance);
                _instance = null;
            }
        }

        private void StartWaitDialogHelper(string caption, string waitMessage, string progressText,
            int delayToShowDialog, object varStatusBmpAnim, string statusBarText, bool isCancellable,
            bool isProgressVisible, int currentStepCount = 0, int totalStepCount = -1,
            IWaitDialogCallback callback = null)
        {
            if (!ThreadHelper.CheckAccess())
            {
                ThreadHelper.Generic.Invoke(() =>
                    {
                        StartWaitDialogHelper(caption, waitMessage, progressText, delayToShowDialog, varStatusBmpAnim,
                            statusBarText, isCancellable, isProgressVisible, currentStepCount, totalStepCount,
                            callback);
                    });
            }
            else
            {
                ThreadHelper.ThrowIfNotOnUIThread(nameof(StartWaitDialogHelper));
                lock (_syncObject)
                {
                    if (_instance == null)
                        _instance = WaitDialog.AcquireInstance();
                    if (_instance == null)
                        _isDialogStarted = true;
                    else
                    {
                        _statusBarService = IoC.Get<IStatusBarDataModelService>();
                        _statusUpdateTaskCancellation = new CancellationTokenSource();
                        if (_statusBarService != null)
                        {
                            if (statusBarText != null)
                            {
                                _statusBarService.SetText(statusBarText);
                                _resetStatusBarOnClose = true;
                            }
                        }

                        _isUiSuppressed = IsUiSupressed();
                        if (!_isUiSuppressed)
                        {
                            try
                            {
                                var args = new DialogShowArguments(caption, waitMessage, progressText, isCancellable,
                                    isProgressVisible, currentStepCount, totalStepCount);
                                _instance.Show(TimeSpan.FromSeconds(delayToShowDialog), args, callback);
                                _isDialogStarted = true;
                            }
                            catch
                            {
                                WaitDialog.ReleaseInstance(_instance);
                                _instance = null;
                                throw;
                            }
                        }
                        else
                            _isDialogStarted = true;
                    }
                }
            }
        }

        private static bool IsUiSupressed()
        {
            return false;
        }

        private bool UpdateDialogHelper(string waitMessage, string progressText, string statusBarText,
            bool isCancellable, int currentStepCount = 0, int totalStepCount = 0)
        {
            lock (_syncObject)
            {
                if (_instance == null)
                    return false;
                if (_instance != null && _instance.IsCancelled)
                    return true;
                if (_statusBarService != null && statusBarText != null)
                {
                    _resetStatusBarOnClose = true;
                    if (ThreadHelper.CheckAccess())
                        _statusBarService.SetText(statusBarText);
                    else
                        Execute.OnUIThreadAsync(() => _statusBarService?.SetText(statusBarText));
                }

                if (!_isUiSuppressed)
                    _instance.UpdateDialog(new DialogUpdateArguments(waitMessage, progressText, isCancellable,
                        currentStepCount, totalStepCount));
                return false;
            }    
        }
    }
}
