using System.Threading;
using ModernApplicationFramework.Basics.Services.TaskSchedulerService;
using ModernApplicationFramework.Basics.Threading;

namespace ModernApplicationFramework.Interfaces.Threading
{
    public interface IMafTask
    {
        /// <summary>
        /// Gets the cancellation token used for this task.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets whether the task completed with an exception. If <see langword="true"/>, an exception occurred.
        /// </summary>
        bool IsFaulted { get; }

        /// <summary>
        /// Gets whether the task result is available. If true, the task result is available. If <see langowrd="false"/>, a <see cref="GetResult()"/> call is blocked until the task is completed.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets whether the task group is cancelled. If <see langword="true"/>, the task group is cancelled.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Gets the asynchronous state object that was given when the task was created.
        /// </summary>
        object AsyncState { get; }

        /// <summary>
        /// Gets or sets the description for the text that is displayed for component diagnostics.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Indicates that this IVsTask instance acts as a wrapper around the specified JoinableTask.
        /// </summary>
        /// <param name="joinableTask">The JoinableTask.</param>
        void AssociateJoinableTask(object joinableTask);

        /// <summary>
        /// Appends the provided action to this task to be run after the task is run to completion. The action is invoked on the context provided.
        /// </summary>
        /// <param name="context">Where to run this task. Values are from <see cref="MafTaskRunContext"/></param>
        /// <param name="pTaskBody">Action to be executed.</param>
        /// <returns></returns>
        IMafTask ContinueWith(uint context, IMafTaskBody pTaskBody);

        /// <summary>
        /// Appends the provided action (using the specified options) to this task to be run after the task is run to completion. The action is invoked on the context provided.
        /// </summary>
        /// <param name="context">Where to run this task. Values are from <see cref="MafTaskRunContext"/></param>
        /// <param name="options">Allows setting task continuation options. Values are from <see cref="MafTaskContinuationOptions"/></param>
        /// <param name="pTaskBody">Action to be executed.</param>
        /// <param name="pAsyncState">The asynchronous state of the task.</param>
        /// <returns></returns>
        IMafTask ContinueWithEx(uint context, uint options, IMafTaskBody pTaskBody, object pAsyncState);

        /// <summary>
        ///	Starts the task.
        /// </summary>
        void Start();

        /// <summary>
        /// Cancels the task group. An antecedent task and all of its children share the same cancellation token, so cancelling any of the tasks cancels the whole task group.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Waits for the task to complete (not including any continuations) and returns the result set by the task. If the task returns an error code or an exception, this method returns the same error code.
        /// </summary>
        /// <returns>The result set by the task.</returns>
        object GetResult();

        /// <summary>
        /// Aborts the task if the task has been cancelled. Use this method to return from a cancelled task.
        /// </summary>
        void AbortIfCanceled();

        /// <summary>
        /// Waits for the task to complete (not including any continuations). If the task returns an error code or an exception, this method returns the same error code.
        /// </summary>
        void Wait();

        /// <summary>
        /// Waits for the task to complete (not including any continuations). You can either specify a timeout (or INFINITE) or set the option to abort on task cancellation.
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout (in milliseconds) or INFINITE.</param>
        /// <param name="options">Values are of type <see cref="MafTaskWaitOptions"/> Set to <see cref="MafTaskWaitOptions.AbortOnTaskCancellation"/> to abort if a cancellation occurs.</param>
        /// <returns></returns>
        bool WaitEx(int millisecondsTimeout, uint options);
    }
}