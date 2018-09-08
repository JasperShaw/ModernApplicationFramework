using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services.TaskSchedulerService;
using ModernApplicationFramework.Basics.Services.WaitDialog;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Threading;
using ModernApplicationFramework.Threading;
using ModernApplicationFramework.Utilities;
using Action = System.Action;
using Timer = System.Timers.Timer;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    ///     Supplies helper methods for running tasks in managed code.
    /// </summary>
    public static class MafTaskHelper
    {
        private static readonly TimeSpan DefaultWaitDialogDelay = TimeSpan.FromSeconds(2.0);
        private static IMafTaskSchedulerService _cachedServiceInstance;

        /// <summary>
        ///     Gets the singleton service instance.
        /// </summary>
        public static IMafTaskSchedulerService ServiceInstance =>
            _cachedServiceInstance ?? (_cachedServiceInstance = new MafTaskSchedulerService());

        /// <summary>
        ///     Signals a <see cref="IMafTask" /> to cancel operations as soon as possible when the specified token is canceled.
        /// </summary>
        /// <param name="task">The task that may be canceled.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static void ApplyCancellationToken(this IMafTask task, CancellationToken cancellationToken)
        {
            Validate.IsNotNull(task, nameof(task));
            if (task.IsCompleted || !cancellationToken.CanBeCanceled)
                return;
            var vsTask = task;
            var registration = cancellationToken.Register(state => ((IMafTask) state).Cancel(), vsTask, false);
            task.ContinueWith(MafTaskRunContext.BackgroundThread, MafTaskContinuationOptions.ExecuteSynchronously,
                CreateTaskBody(() => registration.Dispose()), null);
        }

        /// <summary>
        ///     Wraps a <see cref="JoinableTask{T}" /> instance in an <see cref="IMafTask" />
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="joinableTask">The task to wrap.</param>
        /// <returns>An <see cref="IMafTask" /> instance.</returns>
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

        /// <summary>
        ///     Sets a continuation on the task passed in so that task completion source is set to correct state after the task is
        ///     completed, faulted or canceled.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="taskCompletionSource">TThe task completion source that is set after the task is completed</param>
        /// <param name="task">The task that is used to set the state of the task completion source.</param>
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

        /// <summary>
        ///     Creates a <see cref="IMafTask" /> that's run after the provided tasks have either finished running or been
        ///     cancelled. Overrides
        ///     <see cref="IMafTaskSchedulerService.ContinueWhenAllCompleted(MafTaskRunContext, uint,IMafTask[], IMafTaskBody)" />.
        /// </summary>
        /// <param name="service">The task scheduler service to use to create the task.</param>
        /// <param name="context">Where to run this task.</param>
        /// <param name="dependentTasks">An array of tasks to wait.</param>
        /// <param name="taskBody">Worker method for the task.</param>
        /// <returns>The task scheduler service that's creating the task</returns>
        public static IMafTask ContinueWhenAllCompleted(this IMafTaskSchedulerService service,
            MafTaskRunContext context, IMafTask[] dependentTasks, IMafTaskBody taskBody)
        {
            return service.ContinueWhenAllCompleted(context, (uint) dependentTasks.Length, dependentTasks, taskBody);
        }

        /// <summary>
        ///     Uses the specified options to create a task that's run after the given tasks are completed. Overrides
        ///     <see
        ///         cref="IMafTaskSchedulerService.ContinueWhenAllCompletedEx(MafTaskRunContext, uint, IMafTask[], MafTaskContinuationOptions, IMafTaskBody, object)" />
        /// </summary>
        /// <param name="service">The task scheduler service to use to create the task.</param>
        /// <param name="context">Where to run this task.</param>
        /// <param name="dependentTasks">An array of tasks to wait.</param>
        /// <param name="options">The continuation options set for the task.</param>
        /// <param name="taskBody">Worker method for the task.</param>
        /// <param name="asyncState">The asynchronous state for the task.</param>
        /// <returns>The task scheduler service that's creating the task.</returns>
        public static IMafTask ContinueWhenAllCompleted(this IMafTaskSchedulerService service,
            MafTaskRunContext context, IMafTask[] dependentTasks, MafTaskContinuationOptions options,
            IMafTaskBody taskBody, object asyncState)
        {
            return service.ContinueWhenAllCompletedEx(context, (uint) dependentTasks.Length, dependentTasks, options,
                taskBody, asyncState);
        }

        /// <summary>
        ///     Appends to this task the provided action, to be run after the task is run to completion. The action is invoked on
        ///     the provided context. Overrides <see cref="IMafTask.ContinueWith(uint, IMafTaskBody)" />.
        /// </summary>
        /// <param name="task">The task to which to append the action.</param>
        /// <param name="context">Where to run this task.</param>
        /// <param name="body">The action to be executed.</param>
        /// <returns>A new <see cref="IMafTask" /> instance that has the current task as its parent.</returns>
        public static IMafTask ContinueWith(this IMafTask task, MafTaskRunContext context, IMafTaskBody body)
        {
            return task.ContinueWith((uint) context, body);
        }

        /// <summary>
        ///     Uses the specified options to append to this task the provided action, to be run after the task is run to
        ///     completion. The action is invoked on the provided context. Overrides
        ///     <see cref="IMafTask.ContinueWithEx(uint, uint, IMafTaskBody, object)" />.
        /// </summary>
        /// <param name="task">The task to which to append the action.</param>
        /// <param name="context">Where to run this task.</param>
        /// <param name="options">Enables the setting of TPL Task continuation options.</param>
        /// <param name="taskBody">The action to be executed.</param>
        /// <param name="asyncState">The asynchronous state for the task.</param>
        /// <returns>A new <see cref="IMafTask" /> instance that has the current task as its parent.</returns>
        public static IMafTask ContinueWith(this IMafTask task, MafTaskRunContext context,
            MafTaskContinuationOptions options, IMafTaskBody taskBody, object asyncState)
        {
            return task.ContinueWithEx((uint) context, (uint) options, taskBody, asyncState);
        }

        /// <summary>
        ///     Creates a <see cref="IMafTask" /> task that will be executed in the specified context.
        /// </summary>
        /// <param name="scheduler">The scheduler service.</param>
        /// <param name="context">The context where the task will be executed.</param>
        /// <param name="action">An anonymous method to execute as the task body.</param>
        /// <returns>The task that was created.</returns>
        public static IMafTask CreateAndStartTask(IMafTaskSchedulerService scheduler, MafTaskRunContext context,
            Action action)
        {
            return CreateAndStartTask(scheduler, context, CreateTaskBody(action));
        }


        /// <summary>
        ///     Creates a <see cref="IMafTask" /> task that will be executed in the specified context.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="context">The context where the task will be executed.</param>
        /// <param name="action">
        ///     A CreateAndStartTask(IMafTaskSchedulerService, MafTaskRunContext, MafTaskBodyCallback) to execute
        ///     as the task body.
        /// </param>
        /// <returns>The task that was created.</returns>
        public static IMafTask CreateAndStartTask(IMafTaskSchedulerService scheduler, MafTaskRunContext context,
            MafTaskBodyCallback action)
        {
            return CreateAndStartTask(scheduler, context, CreateTaskBody(action));
        }

        /// <summary>
        ///     Creates a <see cref="IMafTask" /> task that's executed in the specified context.
        /// </summary>
        /// <param name="scheduler">The task scheduler service.</param>
        /// <param name="context">Where the task will be executed.</param>
        /// <param name="pTaskBody">The action to be executed.</param>
        /// <returns>The task that was created.</returns>
        public static IMafTask CreateAndStartTask(IMafTaskSchedulerService scheduler, MafTaskRunContext context,
            IMafTaskBody pTaskBody)
        {
            var task = scheduler.CreateTask(context, pTaskBody);
            task.Start();
            return task;
        }

        /// <summary>
        ///     Creates a <see cref="IMafTask" /> task that's executed with the specified context.
        /// </summary>
        /// <param name="scheduler">The task scheduler service.</param>
        /// <param name="context">Where the task will be executed.</param>
        /// <param name="options">Flags that control optional behavior for the creation and execution of tasks.</param>
        /// <param name="pTaskBody">The action to be executed.</param>
        /// <param name="pAsyncState">The asynchronous state for the task.</param>
        /// <returns>The task that was created.</returns>
        public static IMafTask CreateAndStartTaskEx(IMafTaskSchedulerService scheduler, MafTaskRunContext context,
            MafTaskCreationOptions options, IMafTaskBody pTaskBody, object pAsyncState)
        {
            var task = scheduler.CreateTask(context, options, pTaskBody, pAsyncState);
            task.Start();
            return task;
        }

        /// <summary>
        ///     Creates a task that's run on the given context.
        /// </summary>
        /// <param name="service">The task scheduler service to use to create the task.</param>
        /// <param name="context">Where to run this task.</param>
        /// <param name="taskBody">The action to be executed.</param>
        /// <returns>The task scheduler service that's creating the task to run.</returns>
        public static IMafTask CreateTask(this IMafTaskSchedulerService service, MafTaskRunContext context,
            IMafTaskBody taskBody)
        {
            return service.CreateTask(context, taskBody);
        }


        /// <summary>
        ///     Creates a task with the specified options that is run on the given context.
        /// </summary>
        /// <param name="service">The task scheduler service to use to create the task.</param>
        /// <param name="context">Where to run this task.</param>
        /// <param name="options">The creation options set for the task.</param>
        /// <param name="taskBody">The action to be executed.</param>
        /// <param name="asyncState">The asynchronous state for the task.</param>
        /// <returns>The task scheduler service that's creating the task to run.</returns>
        public static IMafTask CreateTask(this IMafTaskSchedulerService service, MafTaskRunContext context,
            MafTaskCreationOptions options, IMafTaskBody taskBody, object asyncState)
        {
            return service.CreateTaskEx(context, options, taskBody, asyncState);
        }


        /// <summary>
        ///     Creates a task body that can be consumed by the task scheduler service.
        /// </summary>
        /// <param name="action">Anonymous method to execute as the task body.</param>
        /// <returns>An implementation of <see cref="IMafTaskBody" />.</returns>
        public static IMafTaskBody CreateTaskBody(Action action)
        {
            return new MafManagedTaskBody((task, parents) =>
            {
                action();
                return null;
            });
        }

        /// <summary>
        ///     Creates a task body that can be consumed by the task scheduler service.
        /// </summary>
        /// <param name="action">Anonymous method to execute as the task body.</param>
        /// <returns>An implementation of <see cref="IMafTaskBody" /></returns>
        public static IMafTaskBody CreateTaskBody(MafTaskBodyCallback action)
        {
            return new MafManagedTaskBody(action);
        }

        /// <summary>
        ///     Creates a task body that can be consumed by the task scheduler service.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="action">Anonymous method to execute as the task body.</param>
        /// <returns>An implementation of <see cref="IMafTaskBody" />.</returns>
        public static IMafTaskBody CreateTaskBody<T>(Func<T, object> action)
        {
            return new MafManagedTaskBody((task, parents) => action(GetParentResult<T>(parents)));
        }

        /// <summary>
        ///     Creates a task body that can be consumed by the task scheduler service.
        /// </summary>
        /// <param name="action">An anonymous method to execute as the task body.</param>
        /// <returns>An implementation of <see cref="IMafTaskBody" />.</returns>
        public static IMafTaskBody CreateTaskBody(Func<object> action)
        {
            return new MafManagedTaskBody((task, parents) => action());
        }

        /// <summary>
        ///     Creates a task-completion source instance with the specified options.
        /// </summary>
        /// <param name="service">The task scheduler service to use to create the completion source.</param>
        /// <param name="options">Task creation options for the task controlled by the completion source.</param>
        /// <param name="asyncState">The asynchronous state that will be stored by the task controlled by the completion source.</param>
        /// <returns>The task scheduler service that is creating the task completion source.</returns>
        public static IMafTaskCompletionSource CreateTaskCompletionSource(this IMafTaskSchedulerService service,
            MafTaskCreationOptions options, object asyncState)
        {
            return service.CreateTaskCompletionSourceEx(options, asyncState);
        }

        /// <summary>
        ///     Returns a task that delays execution of the subsequent task by a given period of time.
        /// </summary>
        /// <param name="scheduler">The task scheduler service.</param>
        /// <param name="delay">The amount of time to delay the subsequent task.</param>
        /// <returns>The delaying task.</returns>
        public static IMafTask Delay(IMafTaskSchedulerService scheduler, TimeSpan delay)
        {
            return Delay(scheduler, delay.TotalMilliseconds);
        }

        /// <summary>
        ///     Retrieves a task that delays execution of the subsequent task by a given period of time.
        /// </summary>
        /// <param name="scheduler">The task scheduler service.</param>
        /// <param name="delayMilliseconds">The number of milliseconds to delay the subsequent task.</param>
        /// <returns>The delaying task.</returns>
        public static IMafTask Delay(IMafTaskSchedulerService scheduler, double delayMilliseconds)
        {
            Validate.IsNotNull(scheduler, nameof(scheduler));
            var completer = scheduler.CreateTaskCompletionSource();
            var timer = new Timer(delayMilliseconds);
            timer.Elapsed += (sender, e) =>
            {
                timer.Stop();
                completer.SetResult(null);
            };
            timer.Start();
            return completer.Task;
        }

        /// <summary>
        ///     Internal use only. Gets the task to be used for scheduling continuations.
        /// </summary>
        /// <param name="task">The task to be used for scheduling continuations.</param>
        /// <returns>An awaitable object for the <see cref="IMafTask" /> instance.</returns>
        public static MafTaskAwaiter GetAwaiter(this IMafTask task)
        {
            return new MafTaskAwaiter(task);
        }

        /// <summary>
        ///     Internal use only. Gets the awaiter instance that contains the task that will be used to schedule continuations.
        ///     Adds await support for an awaiter instance that can be returned from a call to ResumeWith(IMafTask,
        ///     MafTaskRunContext).
        /// </summary>
        /// <param name="awaiter">Awaiter that contains the task that will be used to schedule continuations.</param>
        /// <returns>The same instance of the awaiter on which this method was called.</returns>
        public static MafTaskAwaiter GetAwaiter(this MafTaskAwaiter awaiter)
        {
            return awaiter;
        }

        public static AwaitExtensions.TaskSchedulerAwaiter GetAwaiter(this MafTaskRunContext context)
        {
            return GetTaskScheduler(context).GetAwaiter();
        }

        public static TaskScheduler GetTaskScheduler(MafTaskRunContext context)
        {
            var serviceInstance = ServiceInstance;
            return (TaskScheduler) serviceInstance.GetTaskScheduler(context);
        }

        /// <summary>
        ///     Transforms a task parallel library (TPL) task from an asynchronous function into an <see cref="IMafTask" />.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="scheduler">The task scheduler used to create the <see cref="IMafTaskCompletionSource" />.</param>
        /// <param name="asyncFunction">
        ///     The asynchronous function that takes an <see cref="IMafTaskCompletionSource" /> and returns
        ///     a TPL task.
        /// </param>
        /// <returns>
        ///     An <see cref="IMafTask" /> that completes only when the TPL task that was returned from asyncFunction
        ///     completes.
        /// </returns>
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

        /// <summary>
        ///     Determines whether the specified context represents UI thread work.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     <see langword="true" />  if the context represents work on the UI thread.
        /// </returns>
        public static bool IsUiThreadContext(this MafTaskRunContext context)
        {
            switch (context)
            {
                case MafTaskRunContext.UiThreadSend:
                case MafTaskRunContext.UiThreadBackgroundPriority:
                case MafTaskRunContext.UiThreadIdlePriority:
                case MafTaskRunContext.UiThreadNormalPriority:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Extension method for task awaiter to support awaits with a specific context.
        /// </summary>
        /// <param name="task">he task that will be used to schedule continuations.</param>
        /// <param name="context">The context under which the continuation would be scheduled.</param>
        /// <returns>The awaitable object.</returns>
        public static MafTaskAwaiter ResumeWith(this IMafTask task, MafTaskRunContext context)
        {
            return new MafTaskAwaiter(task, context);
        }


        /// <summary>
        ///     Blocks the calling (UI) thread till a cancelable, async operation completes. An optionally cancelable wait dialog
        ///     is displayed if the operation takes too long.
        /// </summary>
        /// <param name="joinableTaskFactory">The factory to use for creating the task.</param>
        /// <param name="waitCaption">The title of the wait dialog, when and if it appears.</param>
        /// <param name="asyncMethod">
        ///     The operation which would otherwise be async but is blocking the UI thread in this scenario.
        ///     A means of updating the wait dialog's progress display, and a CancellationToken signaling user cancellation are
        ///     provided.
        /// </param>
        /// <param name="delayToShowDialog">
        ///     The time to wait for <see cref="asyncMethod" /> to complete before displaying the wait
        ///     dialog. The default (null) is 2 seconds.
        /// </param>
        public static void Run(this JoinableTaskFactory joinableTaskFactory, string waitCaption,
            Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> asyncMethod,
            TimeSpan? delayToShowDialog = null)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNullAndNotEmpty(waitCaption, nameof(waitCaption));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            ThreadHelper.ThrowIfNotOnUIThread(nameof(Run));
            var factory = IoC.Get<IWaitDialogFactory>();
            var initialProgress = new WaitDialogProgressData(null, null, null, true);
            var twd = factory.StartWaitDialog(waitCaption, initialProgress,
                delayToShowDialog ?? DefaultWaitDialogDelay);
            try
            {
                joinableTaskFactory.Run(() => asyncMethod(twd.Progress, twd.UserCancellationToken));
            }
            finally
            {
                twd?.Dispose();
            }
        }

        /// <summary>
        ///     Blocks the calling (UI) thread till an async operation completes. An optionally cancelable wait dialog is displayed
        ///     if the operation takes too long.
        /// </summary>
        /// <param name="joinableTaskFactory">The factory to use for creating the task.</param>
        /// <param name="waitCaption">The title of the wait dialog, when and if it appears.</param>
        /// <param name="asyncMethod">
        ///     The operation which would otherwise be async but is blocking the UI thread in this scenario.
        ///     A means of updating the wait dialog's progress display is provided
        /// </param>
        /// <param name="delayToShowDialog">
        ///     The time to wait for <see cref="asyncMethod" /> to complete before displaying the wait
        ///     dialog. The default (null) is 2 seconds.
        /// </param>
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


        /// <summary>
        ///     Blocks the calling (UI) thread till an async operation completes. Cancellation is offered to the user.
        /// </summary>
        /// <param name="joinableTaskFactory">The factory to use for creating the task.</param>
        /// <param name="waitCaption">The title of the wait dialog, when and if it appears.</param>
        /// <param name="waitMessage">The message inside the wait dialog, when and if it appears.</param>
        /// <param name="asyncMethod">The operation which would otherwise be async but is blocking the UI thread in this scenario.</param>
        /// <param name="delayToShowDialog">
        ///     The time to wait for <see cref="asyncMethod" /> to complete before displaying the wait
        ///     dialog. The default (null) is 2 seconds.
        /// </param>
        public static void Run(this JoinableTaskFactory joinableTaskFactory, string waitCaption, string waitMessage,
            Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> asyncMethod,
            TimeSpan? delayToShowDialog = null)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNullAndNotEmpty(waitCaption, nameof(waitCaption));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            ThreadHelper.ThrowIfNotOnUIThread(nameof(Run));
            var factory = IoC.Get<IWaitDialogFactory>();
            var initialProgress = new WaitDialogProgressData(waitMessage, null, null, true);
            var twd = factory.StartWaitDialog(waitCaption, initialProgress,
                delayToShowDialog ?? DefaultWaitDialogDelay);
            try
            {
                joinableTaskFactory.Run(() => asyncMethod(twd.Progress, twd.UserCancellationToken));
            }
            finally
            {
                twd?.Dispose();
            }
        }

        /// <summary>
        ///     Blocks the calling (UI) thread till an async operation completes. No cancellation is offered to the user.
        /// </summary>
        /// <param name="joinableTaskFactory">The factory to use for creating the task.</param>
        /// <param name="waitCaption">The title of the wait dialog, when and if it appears.</param>
        /// <param name="waitMessage">The message inside the wait dialog, when and if it appears.</param>
        /// <param name="asyncMethod">The operation which would otherwise be async but is blocking the UI thread in this scenario.</param>
        /// <param name="delayToShowDialog">
        ///     The time to wait for <see cref="asyncMethod" /> to complete before displaying the wait
        ///     dialog. The default (null) is 2 seconds.
        /// </param>
        public static void Run(this JoinableTaskFactory joinableTaskFactory, string waitCaption, string waitMessage,
            Func<IProgress<WaitDialogProgressData>, Task> asyncMethod, TimeSpan? delayToShowDialog = null)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNullAndNotEmpty(waitCaption, nameof(waitCaption));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            ThreadHelper.ThrowIfNotOnUIThread(nameof(Run));
            var factory = IoC.Get<IWaitDialogFactory>();
            var initialProgress = new WaitDialogProgressData(waitMessage);
            var twd = factory.StartWaitDialog(waitCaption, initialProgress,
                delayToShowDialog ?? DefaultWaitDialogDelay);
            try
            {
                joinableTaskFactory.Run(() => asyncMethod(twd.Progress));
            }
            finally
            {
                twd?.Dispose();
            }
        }

        /// <summary>
        ///     Wraps the invocation of an async method so that it may execute asynchronously, but may potentially be synchronously
        ///     completed (waited on) in the future.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="joinableTaskFactory">The factory to use for creating the task.</param>
        /// <param name="priority">
        ///     The priority with which to schedule any work on the UI thread, when and if
        ///     <see cref="JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken)" /> is called.
        /// </param>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <returns>
        ///     An object that tracks the completion of the async operation, and allows for later synchronous blocking of the
        ///     main thread for completion if necessary.
        /// </returns>
        public static JoinableTask<T> RunAsync<T>(this JoinableTaskFactory joinableTaskFactory,
            MafTaskRunContext priority, Func<Task<T>> asyncMethod)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            return joinableTaskFactory.WithPriority(priority).RunAsync(asyncMethod);
        }

        /// <summary>
        ///     Wraps the invocation of an async method so that it may execute asynchronously, but may potentially be synchronously
        ///     completed (waited on) in the future.
        /// </summary>
        /// <param name="joinableTaskFactory">The factory to use for creating the task.</param>
        /// <param name="priority">
        ///     The priority with which to schedule any work on the UI thread, when and if
        ///     <see cref="JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken)" /> is called.
        /// </param>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <returns>
        ///     An object that tracks the completion of the async operation, and allows for later synchronous blocking of the
        ///     main thread for completion if necessary.
        /// </returns>
        public static JoinableTask RunAsync(this JoinableTaskFactory joinableTaskFactory, MafTaskRunContext priority,
            Func<Task> asyncMethod)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            return joinableTaskFactory.WithPriority(priority).RunAsync(asyncMethod);
        }

        /// <summary>
        ///     Creates an <see cref="IMafTask" /> to track a cancelable async operation.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the async operation.</typeparam>
        /// <param name="joinableTaskFactory">The factory to use for creating the task.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <returns>
        ///     An object that tracks the completion of the async operation, and allows for later synchronous blocking of the
        ///     main thread for completion if necessary.
        /// </returns>
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


        /// <summary>
        ///     Schedules a delegate for background execution on the UI thread without inheriting any claim to the UI thread from
        ///     its caller.
        /// </summary>
        /// <param name="joinableTaskFactory">The factory to use for creating the task.</param>
        /// <param name="asyncMethod">TThe async delegate to invoke on the UI thread sometime in the future.</param>
        /// <param name="priority">The priority to use when switching to the UI thread or resuming after a yielding await.</param>
        /// <returns></returns>
        public static JoinableTask StartOnIdle(this JoinableTaskFactory joinableTaskFactory, Func<Task> asyncMethod,
            MafTaskRunContext priority = MafTaskRunContext.UiThreadBackgroundPriority)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));
            using (joinableTaskFactory.Context.SuppressRelevance())
            {
                return joinableTaskFactory.RunAsync(priority, async () =>
                {
                    await Task.Yield();
                    await joinableTaskFactory.SwitchToMainThreadAsync();
                    await asyncMethod();
                });
            }
        }

        /// <summary>
        ///     Schedules a delegate for background execution on the UI thread without inheriting any claim to the UI thread from
        ///     its caller.
        /// </summary>
        /// <param name="joinableTaskFactory">The factory to use for creating the task.</param>
        /// <param name="asyncMethod">The delegate to invoke on the UI thread sometime in the future.</param>
        /// <param name="priority">The priority to use when switching to the UI thread or resuming after a yielding await.</param>
        /// <returns>The <see cref="JoinableTask" /> that represents the on-idle operation.</returns>
        public static JoinableTask StartOnIdle(this JoinableTaskFactory joinableTaskFactory, Action action,
            MafTaskRunContext priority = MafTaskRunContext.UiThreadBackgroundPriority)
        {
            Validate.IsNotNull(action, nameof(action));
            return joinableTaskFactory.StartOnIdle(() =>
            {
                action();
                return TplExtensions.CompletedTask;
            }, priority);
        }

        public static JoinableTaskFactory WithPriority(this JoinableTaskFactory joinableTaskFactory,
            MafTaskRunContext priority)
        {
            Validate.IsNotNull(joinableTaskFactory, nameof(joinableTaskFactory));
            if (!priority.IsUiThreadJoinableTaskSafeContext())
                throw new ArgumentOutOfRangeException(nameof(priority));
            return new SchedulerModifyingJoinableTaskFactoryWrapper(joinableTaskFactory, priority);
        }

        /// <summary>
        ///     Yields the current operation on the thread. The rest of the asynchronous method will be scheduled as a
        ///     continuation.
        /// </summary>
        /// <param name="scheduler">The instance of the task scheduler service.</param>
        /// <param name="context">The context to use for scheduling the rest of the asynchronous method.</param>
        /// <param name="taskCompletionSource">If a task completion source is passed in, the task created is added as a dependency.</param>
        /// <returns>An awaiter implementation to use with the <see langword="await" /> keyword.</returns>
        public static YieldAwaiter Yield(this IMafTaskSchedulerService scheduler,
            MafTaskRunContext context = MafTaskRunContext.CurrentContext,
            IMafTaskCompletionSource taskCompletionSource = null)
        {
            Validate.IsNotNull(scheduler, nameof(scheduler));
            return new YieldAwaiter(scheduler, taskCompletionSource, context);
        }

        /// <summary>
        ///     Yields the current operation on the thread, so that the rest of the async method is scheduled as a continuation.
        /// </summary>
        /// <param name="context">The context to use for scheduling the rest of the async method.</param>
        /// <param name="taskCompletionSource">If not <see langword="null" />, the task created is added as a dependency.</param>
        /// <returns>An awaiter implementation to use with the <see langword="await" /> keyword.</returns>
        public static YieldAwaiter Yield(MafTaskRunContext context = MafTaskRunContext.CurrentContext,
            IMafTaskCompletionSource taskCompletionSource = null)
        {
            return new YieldAwaiter(ServiceInstance, taskCompletionSource, context);
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

        private static T GetParentResult<T>(IMafTask[] parentTasks)
        {
            var singleParent = GetSingleParent(parentTasks);
            if (singleParent == null)
                return default;
            var result = singleParent.GetResult();
            try
            {
                return (T) result;
            }
            catch (InvalidCastException)
            {
                return default;
            }
        }

        private static IMafTask GetSingleParent(IMafTask[] parentTasks)
        {
            if (parentTasks == null)
                return null;
            if (parentTasks.Length == 0)
                return null;
            return parentTasks[0];
        }

        private static bool IsUiThreadJoinableTaskSafeContext(this MafTaskRunContext context)
        {
            switch (context)
            {
                case MafTaskRunContext.UiThreadBackgroundPriority:
                case MafTaskRunContext.UiThreadIdlePriority:
                case MafTaskRunContext.UiThreadNormalPriority:
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
}