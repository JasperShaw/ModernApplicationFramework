using ModernApplicationFramework.Interfaces.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    /// <summary>
    ///     Used by the asynchronous task library helper as the method type for a task body (what the task executes). Users of
    ///     the library can either pass in this delegate type directly or use one of the wrapper methods that wraps simpler
    ///     anonymous functions to this delegate type.
    /// </summary>
    /// <param name="task">
    ///     The task that is executing, that is, the asynchronous task to which the task body belongs. This can
    ///     be used to check if task cancellation was requested.
    /// </param>
    /// <param name="parentTasks">
    ///     An array that contains the dependent tasks that had to be completed before your task.
    ///     Normally this is either empty if the task was a new task, or it contains a single task if the task was a
    ///     continuation of another task.
    /// </param>
    /// <returns>Returns <see cref="object" /></returns>
    public delegate object MafTaskBodyCallback(IMafTask task, IMafTask[] parentTasks);
}