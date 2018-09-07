using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services.TaskSchedulerService;
using ModernApplicationFramework.Basics.Services.WaitDialog;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Threading;
using ModernApplicationFramework.Utilities;
using Action = System.Action;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    ///     Supplies helper methods for running tasks in managed code.
    /// </summary>
    public static class MafTaskHelper
    {
        private static IMafTaskSchedulerService _cachedServiceInstance;

        private static readonly TimeSpan DefaultWaitDialogDelay = TimeSpan.FromSeconds(2.0);

        public static IMafTaskSchedulerService ServiceInstance =>
            _cachedServiceInstance ?? (_cachedServiceInstance = new MafTaskSchedulerService());

        public static IMafTask AsMafTask<T>(this JoinableTask<T> joinableTask)
        {
            Validate.IsNotNull(joinableTask, nameof(joinableTask));
            var completionSource = ServiceInstance.CreateTaskCompletionSource();
            completionSource.CompleteAfterTask(joinableTask.Task);
            completionSource.Task.AssociateJoinableTask(joinableTask);
            var promoted = false;
            var taskEvents = (IMafTaskEvents) completionSource.Task;
            var blockingCancellationSource = new CancellationTokenSource();

            void UnblockDelegate(object sender, EventArgs e)
            {
                blockingCancellationSource.Cancel();
                blockingCancellationSource = new CancellationTokenSource();
                if (!(sender is IMafTaskEvents vsTaskEvents)) return;
                vsTaskEvents.OnBlockingWaitEnd -= UnblockDelegate;
            }

            void MarkedAsBlocking(object sender, BlockingTaskEventArgs e)
            {
                if (promoted) return;
                promoted = true;
                var action = (Action) (() =>
                {
                    if (e.BlockedTask is IMafTaskEvents blockedTask) blockedTask.OnBlockingWaitEnd += UnblockDelegate;
                    try
                    {
                        joinableTask.Join(blockingCancellationSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch
                    {
                    }
                });
                if (ThreadHelper.CheckAccess())
                    action();
                else
                    UiThreadReentrancyScope.EnqueueActionAsync(action).Forget();
            }

            taskEvents.OnMarkedAsBlocking += MarkedAsBlocking;
            completionSource.Task.ContinueWith(MafTaskRunContext.BackgroundThread, CreateTaskBody(() =>
            {
                taskEvents.OnMarkedAsBlocking -= MarkedAsBlocking;
                taskEvents.OnBlockingWaitEnd -= UnblockDelegate;
            }));
            return completionSource.Task;
        }

        public static void CompleteAfterTask<T>(this IMafTaskCompletionSource taskCompletionSource, Task<T> task)
        {
            Validate.IsNotNull(task, nameof(task));
            Validate.IsNotNull(taskCompletionSource, nameof(taskCompletionSource));
            if (CopyTaskResultIfCompleted(task, taskCompletionSource))
                return;
            task.ContinueWith(
                (_, source) =>
                    CopyTaskResultIfCompleted(task, (IMafTaskCompletionSource) source), taskCompletionSource,
                CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default).Forget();
        }

        public static IMafTask ContinueWith(this IMafTask task, MafTaskRunContext context, IMafTaskBody body)
        {
            return task.ContinueWith((uint) context, body);
        }

        public static IMafTaskBody CreateTaskBody(Action action)
        {
            return new MafManagedTaskBody((task, parents) =>
            {
                action();
                return null;
            });
        }

        public static TaskScheduler GetTaskScheduler(MafTaskRunContext context)
        {
            var serviceInstance = ServiceInstance;
            return (TaskScheduler) serviceInstance.GetTaskScheduler(context);
        }

        public static IMafTask InvokeAsync<T>(this IMafTaskSchedulerService scheduler,
            MafInvokableAsyncFunction<T> asyncFunction)
        {
            Validate.IsNotNull(scheduler, nameof(scheduler));
            Validate.IsNotNull(asyncFunction, nameof(asyncFunction));
            var completionSource = scheduler.CreateTaskCompletionSource();
            var task = asyncFunction(completionSource);
            completionSource.CompleteAfterTask(task);
            return completionSource.Task;
        }





        public static void Run(this JoinableTaskFactory joinableTaskFactory, string waitCaption, 
            Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> asyncMethod, TimeSpan? delayToShowDialog = null)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNullAndNotEmpty(waitCaption, nameof(waitCaption));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            ThreadHelper.ThrowIfNotOnUIThread(nameof(Run));
            var factory = IoC.Get<IWaitDialogFactory>();
            var initialProgress = new WaitDialogProgressData(null, null, null, true);
            var twd = factory.StartWaitDialog(waitCaption, initialProgress, delayToShowDialog ?? DefaultWaitDialogDelay);
            try
            {
                joinableTaskFactory.Run(() => asyncMethod(twd.Progress, twd.UserCancellationToken));
            }
            finally
            {
                twd?.Dispose();
            }
        }

        public static void Run(this JoinableTaskFactory joinableTaskFactory, string waitCaption, 
            Func<IProgress<WaitDialogProgressData>, Task> asyncMethod, TimeSpan? delayToShowDialog = null)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNullAndNotEmpty(waitCaption, nameof(waitCaption));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            ThreadHelper.ThrowIfNotOnUIThread(nameof(Run));
            var factory = IoC.Get<IWaitDialogFactory>();
            var twd = factory.StartWaitDialog(waitCaption, null, delayToShowDialog ?? DefaultWaitDialogDelay);
            try
            {
                joinableTaskFactory.Run(() => asyncMethod(twd.Progress));
            }
            finally
            {
                twd?.Dispose();
            }
        }


        public static void Run(this JoinableTaskFactory joinableTaskFactory, string waitCaption, string waitMessage, 
            Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> asyncMethod, TimeSpan? delayToShowDialog = null)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNullAndNotEmpty(waitCaption, nameof(waitCaption));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            ThreadHelper.ThrowIfNotOnUIThread(nameof(Run));
            var factory = IoC.Get<IWaitDialogFactory>();
            var initialProgress = new WaitDialogProgressData(waitMessage, null, null, true);
            var twd = factory.StartWaitDialog(waitCaption, initialProgress, delayToShowDialog ?? DefaultWaitDialogDelay);
            try
            {
                joinableTaskFactory.Run(() => asyncMethod(twd.Progress, twd.UserCancellationToken));
            }
            finally
            {
                twd?.Dispose();
            }
        }

        public static void Run(this JoinableTaskFactory joinableTaskFactory, string waitCaption, string waitMessage, 
            Func<IProgress<WaitDialogProgressData>, Task> asyncMethod, TimeSpan? delayToShowDialog = null)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNullAndNotEmpty(waitCaption, nameof(waitCaption));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            ThreadHelper.ThrowIfNotOnUIThread(nameof(Run));
            var factory = IoC.Get<IWaitDialogFactory>();
            var initialProgress = new WaitDialogProgressData(waitMessage);
            var twd = factory.StartWaitDialog(waitCaption, initialProgress, delayToShowDialog ?? DefaultWaitDialogDelay);
            try
            {
                joinableTaskFactory.Run(() => asyncMethod(twd.Progress));
            }
            finally
            {
                twd?.Dispose();
            }
        }

        public static JoinableTask StartOnIdle(this JoinableTaskFactory joinableTaskFactory, Func<Task> asyncMethod, MafTaskRunContext priority = MafTaskRunContext.UIThreadBackgroundPriority)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            using (joinableTaskFactory.Context.SuppressRelevance())
                return joinableTaskFactory.RunAsync(priority, async () =>
                {
                    await Task.Yield();
                    await joinableTaskFactory.SwitchToMainThreadAsync();
                    await asyncMethod();
                });
        }

        public static JoinableTask StartOnIdle(this JoinableTaskFactory joinableTaskFactory, Action action, MafTaskRunContext priority = MafTaskRunContext.UIThreadBackgroundPriority)
        {
            Validate.IsNotNull(action, nameof(action));
            return joinableTaskFactory.StartOnIdle(() =>
            {
                action();
                return TplExtensions.CompletedTask;
            }, priority);
        }

        public static JoinableTask<T> RunAsync<T>(this JoinableTaskFactory joinableTaskFactory,
            MafTaskRunContext priority, Func<Task<T>> asyncMethod)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            return joinableTaskFactory.WithPriority(priority).RunAsync(asyncMethod);
        }

        public static JoinableTask RunAsync(this JoinableTaskFactory joinableTaskFactory, MafTaskRunContext priority,
            Func<Task> asyncMethod)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            return joinableTaskFactory.WithPriority(priority).RunAsync(asyncMethod);
        }

       public static IMafTask RunAsyncAsMafTask<T>(this JoinableTaskFactory joinableTaskFactory,
            MafTaskRunContext priority, Func<CancellationToken, Task<T>> asyncMethod)
        {
            var cts = new CancellationTokenSource();
            var joinableTask = joinableTaskFactory.RunAsync(priority, () => asyncMethod(cts.Token));
            var vsTask = joinableTask.AsMafTask();
            if (!joinableTask.IsCompleted)
            {
                var cancellationToken = vsTask.CancellationToken;
                var tokenRegistration =
                    cancellationToken.Register(state => ((CancellationTokenSource) state).Cancel(), cts);
                var task = joinableTask.Task;
                var none = CancellationToken.None;
                var scheduler = TaskScheduler.Default;
                task.ContinueWith((_, state) => ((CancellationTokenRegistration) state).Dispose(), tokenRegistration,
                    none, TaskContinuationOptions.ExecuteSynchronously, scheduler).Forget();
            }

            return vsTask;
        }

        public static JoinableTaskFactory WithPriority(this JoinableTaskFactory joinableTaskFactory,
            MafTaskRunContext priority)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            if (!priority.IsUiThreadJoinableTaskSafeContext())
                throw new ArgumentOutOfRangeException(nameof(priority));
            return new SchedulerModifyingJoinableTaskFactoryWrapper(joinableTaskFactory, priority);
        }

        internal static async Task<TaskResult> Run<T>(string title, WaitDialogProgressData progressData,
            Func<T, IProgress<WaitDialogProgressData>, CancellationToken, Task> a, T param,
            bool cancelable, TimeSpan delayToShowDialog = default)
        {
            var f = IoC.Get<IWaitDialogFactory>();
            f.CreateInstance(out var window);

            var waitSeconds = (int) delayToShowDialog.TotalSeconds;

            var session = WaitDialogHelper.CreateSession(window);
            window.StartWaitDialogWithCallback(title, progressData.WaitMessage, progressData.ProgressText,
                progressData.StatusBarText, cancelable, waitSeconds, true, progressData.TotalSteps,
                progressData.CurrentStep,
                session.Callback);

            await Task.Run(async () => await a(param, session.Progress, session.UserCancellationToken));

            window.EndWaitDialog(out var canceled);
            return canceled ? TaskResult.Canceled : TaskResult.Completed;
        }


        internal static async Task<TaskResult> Run(string title, WaitDialogProgressData progressData,
            Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> task,
            bool cancelable, TimeSpan delayToShowDialog = default)
        {
            var f = IoC.Get<IWaitDialogFactory>();
            f.CreateInstance(out var window);

            var waitSeconds = (int) delayToShowDialog.TotalSeconds;

            var session = WaitDialogHelper.CreateSession(window);
            window.StartWaitDialogWithCallback(title, progressData.WaitMessage, progressData.ProgressText,
                progressData.StatusBarText, cancelable, waitSeconds, true, progressData.TotalSteps,
                progressData.CurrentStep,
                session.Callback);

            await Task.Run(async () => await task(session.Progress, session.UserCancellationToken));

            window.EndWaitDialog(out var canceled);
            return canceled ? TaskResult.Canceled : TaskResult.Completed;
        }

        private static bool CopyTaskResultIfCompleted<T>(Task<T> task, IMafTaskCompletionSource taskCompletionSource)
        {
            if (task.IsCanceled)
            {
                taskCompletionSource.SetCanceled();
            }
            else if (task.IsFaulted)
            {
                taskCompletionSource.SetFaulted(Marshal.GetHRForException(task.Exception));
            }
            else
            {
                if (!task.IsCompleted)
                    return false;
                taskCompletionSource.SetResult(task.Result);
            }

            return true;
        }

        private static bool IsUiThreadJoinableTaskSafeContext(this MafTaskRunContext context)
        {
            switch (context)
            {
                case MafTaskRunContext.UIThreadBackgroundPriority:
                case MafTaskRunContext.UIThreadIdlePriority:
                case MafTaskRunContext.UIThreadNormalPriority:
                    return true;
                default:
                    return false;
            }
        }

        private class SchedulerModifyingJoinableTaskFactoryWrapper : DelegatingJoinableTaskFactory
        {
            private readonly TaskScheduler _scheduler;

            internal SchedulerModifyingJoinableTaskFactoryWrapper(JoinableTaskFactory innerFactory,
                MafTaskRunContext context)
                : base(innerFactory)
            {
                _scheduler = GetTaskScheduler(context);
            }

            protected override void PostToUnderlyingSynchronizationContext(SendOrPostCallback callback, object state)
            {
                Task.Factory.StartNew(callback.Invoke, state, CancellationToken.None, TaskCreationOptions.HideScheduler,
                    _scheduler).Forget();
            }
        }
    }

    public enum MafTaskRunContext
    {
        BackgroundThread,
        UIThreadSend,
        UIThreadBackgroundPriority,
        UIThreadIdlePriority,
        CurrentContext,
        BackgroundThreadLowIOPriority,
        UIThreadNormalPriority
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct EmptyStruct
    {
        internal static EmptyStruct Instance => new EmptyStruct();
    }
}