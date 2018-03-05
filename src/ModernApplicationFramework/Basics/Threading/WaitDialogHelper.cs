﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Threading
{
    public static class WaitDialogHelper
    {
        /// <summary>
        /// Creates the instance of an <see cref="IWaitDialog"/>.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>The instance of the <see cref="IWaitDialog"/></returns>
        public static IWaitDialog CreateInstance(this IWaitDialogFactory factory)
        {
            Validate.IsNotNull(factory, nameof(factory));
            factory.CreateInstance(out var waitDialog);
            return waitDialog;
        }

        /// <summary>
        /// Ends the wait dialog.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns>Returns <see langword="true"/> if the task was canceled by the user</returns>
        public static bool EndWaitDialog(this IWaitDialog dialog)
        {
            Validate.IsNotNull(dialog, nameof(dialog));
            dialog.EndWaitDialog(out var canceled);
            return canceled;
        }

        /// <summary>
        /// Starts the wait dialog and executes the task.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="asyncFunc">The asynchronous function.</param>
        /// <param name="waitCaption">The caption of the dialog.</param>
        /// <param name="initialProgress">The initial progress.</param>
        /// <param name="delayToShowDialog">The delay to show the dialog.</param>
        public static void StartWaitDialog(this IWaitDialogFactory factory,
            Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> asyncFunc, string waitCaption,
            WaitDialogProgressData initialProgress = null, TimeSpan delayToShowDialog = default)
        {
            var instance = factory.CreateInstance();
            var session = CreateSession(instance);
            instance.SetFunction(() => asyncFunc(session.Progress, session.UserCancellationToken));
            instance.StartWaitDialogWithCallback(
                waitCaption,
                initialProgress?.WaitMessage,
                initialProgress?.ProgressText,
                initialProgress?.StatusBarText,
                initialProgress != null && initialProgress.IsCancelable,
                (int) delayToShowDialog.TotalSeconds,
                true, initialProgress?.TotalSteps ?? 0,
                initialProgress?.CurrentStep ?? 0,
                session.Callback);
        }


        /// <summary>
        /// Starts the wait dialog and executes the task.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="asyncFunc">The asynchronous function.</param>
        /// <param name="waitCaption">The caption of the dialog.</param>
        /// <param name="initialProgress">The initial progress.</param>
        /// <param name="delayToShowDialog">The delay to show the dialog.</param>
        public static void StartWaitDialog(this IWaitDialogFactory factory,
            Func<IProgress<WaitDialogProgressData>, Task> asyncFunc, string waitCaption,
            WaitDialogProgressData initialProgress = null, TimeSpan delayToShowDialog = default(TimeSpan))
        {
            var instance = factory.CreateInstance();
            var session = CreateSession(instance);
            instance.SetFunction(() => asyncFunc(session.Progress));
            instance.StartWaitDialogWithCallback(
                waitCaption,
                initialProgress?.WaitMessage,
                initialProgress?.ProgressText,
                initialProgress?.StatusBarText,
                initialProgress != null && initialProgress.IsCancelable,
                (int) delayToShowDialog.TotalSeconds,
                true, initialProgress?.TotalSteps ?? 0,
                initialProgress?.CurrentStep ?? 0,
                session.Callback);
        }


        public static Session CreateSession(IWaitDialog waitDialog)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = (IProgress<WaitDialogProgressData>) new ProgressAdapter(waitDialog, cancellationTokenSource);
            var cancellationCallback = new CancellationCallback(cancellationTokenSource);
            return new Session(progress, cancellationTokenSource.Token, cancellationCallback);
        }

        public class Session
        {
            /// <summary>
            /// The callback instance.
            /// </summary>
            public IWaitDialogCallback Callback { get; }

            /// <summary>
            /// The progress report.
            /// </summary>
            public IProgress<WaitDialogProgressData> Progress { get; }

            /// <summary>
            /// The cancellation token.
            /// </summary>
            public CancellationToken UserCancellationToken { get; }

            internal Session(IProgress<WaitDialogProgressData> progress, CancellationToken token,
                IWaitDialogCallback callback)
            {
                Validate.IsNotNull(progress, nameof(progress));
                Progress = progress;
                UserCancellationToken = token;
                Callback = callback;
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

        private class ProgressAdapter : IProgress<WaitDialogProgressData>
        {
            private readonly CancellationTokenSource _cancellationTokenSource;
            private readonly IWaitDialog _dialog;

            internal ProgressAdapter(IWaitDialog dialog, CancellationTokenSource cancellationTokenSource)
            {
                Validate.IsNotNull(dialog, nameof(dialog));
                _dialog = dialog;
                _cancellationTokenSource = cancellationTokenSource;
            }

            public void Report(WaitDialogProgressData value)
            {
                if (value == null)
                    return;
                try
                {
                    _dialog.UpdateProgress(value.WaitMessage, value.ProgressText, value.StatusBarText,
                        value.CurrentStep, value.TotalSteps, !value.IsCancelable, out var pfCanceled);

                    if (!pfCanceled || _cancellationTokenSource == null)
                        return;
                    _cancellationTokenSource.Cancel();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}