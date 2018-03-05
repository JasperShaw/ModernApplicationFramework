using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    /// Supplies helper methods for running tasks in managed code.
    /// </summary>
    public static class MafTaskHelper
    {
        internal static async Task<TaskResult> Run<T>(string title, WaitDialogProgressData progressData, Func<T, IProgress<WaitDialogProgressData>, CancellationToken, Task> a, T param,
            bool cancelable, TimeSpan delayToShowDialog = default)
        {
            var f = IoC.Get<IWaitDialogFactory>();
            f.CreateInstance(out var window);

            var waitSeconds = (int)delayToShowDialog.TotalSeconds;

            var session = WaitDialogHelper.CreateSession(window);
            window.StartWaitDialogWithCallback(title, progressData.WaitMessage, progressData.ProgressText,
                progressData.StatusBarText, cancelable, waitSeconds, true, progressData.TotalSteps, progressData.CurrentStep,
                session.Callback);

            await Task.Run(async () => await a(param, session.Progress, session.UserCancellationToken));

            window.EndWaitDialog(out var canceled);
            return canceled ? TaskResult.Canceled : TaskResult.Completed;
        }

        public static async Task<TaskResult> Run(string title, string message, Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> task,
            TimeSpan? delayToShowDialog = null)
        {
            var data = new WaitDialogProgressData(message);
            return await Run(title, data, task, true, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }

        public static async Task<TaskResult> Run(string title, string message, Func<IProgress<WaitDialogProgressData>, Task> task,
            TimeSpan? delayToShowDialog = null)
        {
            var data = new WaitDialogProgressData(message);
            return await Run(title, data, (progress, cancellation) => task(progress), false, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }

        public static async Task<TaskResult> Run(string title, string message, Func<CancellationToken, Task> task,
            TimeSpan? delayToShowDialog = null)
        {
            var data = new WaitDialogProgressData(message);
            return await Run(title, data, (progress, cancellation) => task(cancellation), true, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }

        public static async Task<TaskResult> Run(string title, string message, Func<Task> task,
            TimeSpan? delayToShowDialog = null)
        {
            var data = new WaitDialogProgressData(message);
            return await Run(title, data, (progress, cancellation) => task(), true, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }


        internal static async Task<TaskResult> Run(string title, WaitDialogProgressData progressData, Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> task,
            bool cancelable, TimeSpan delayToShowDialog = default)
        {
            var f = IoC.Get<IWaitDialogFactory>();
            f.CreateInstance(out var window);

            var waitSeconds = (int)delayToShowDialog.TotalSeconds;

            var session = WaitDialogHelper.CreateSession(window);
            window.StartWaitDialogWithCallback(title, progressData.WaitMessage, progressData.ProgressText,
                progressData.StatusBarText, cancelable, waitSeconds, true, 0, 0,
                session.Callback);

            await Task.Run(async () => await task(session.Progress, session.UserCancellationToken));

            window.EndWaitDialog(out var canceled);
            return canceled ? TaskResult.Canceled : TaskResult.Completed;
        }

        public static async Task<TaskResult> Run<T>(string title, string message, Func<T, IProgress<WaitDialogProgressData>, CancellationToken, Task> task, 
            T param, TimeSpan? delayToShowDialog = null)
        {
            var data = new WaitDialogProgressData(message);
            return await Run(title, data, task, param, true, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }

        public static async Task<TaskResult> Run<T>(string title, string message, Func<T, Task> task, T param, TimeSpan? delayToShowDialog = null)
        {
            var data = new WaitDialogProgressData(message);
            return await Run(title, data, (p1, progress, cancellation) => task(p1), param, false, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }

        public static async Task<TaskResult> Run<T>(string title, string message, Func<T, IProgress<WaitDialogProgressData>, Task> task, T param,
            TimeSpan? delayToShowDialog = null)
        {
            var data = new WaitDialogProgressData(message);
            return await Run(title, data, (p1, progress, cancellation) => task(p1, progress), param, false, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }

        public static async Task<TaskResult> Run<T>(string title, string message, Func<T, CancellationToken, Task> task, T param,
            TimeSpan? delayToShowDialog = null)
        {
            var data = new WaitDialogProgressData(message);
            return await Run(title, data, (p1, progress, cancellation) => task(p1, cancellation), param, true, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }
    }
}
