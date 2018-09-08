using System;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    ///     Specifies the options for creating a task.
    /// </summary>
    [Flags]
    public enum MafTaskCreationOptions
    {
        None = 0,

        /// <summary>
        ///     A hint to a <see cref="TaskScheduler" /> to schedule a task in as fair a manner as possible, meaning that tasks
        ///     scheduled sooner will be more likely to be run sooner, and tasks scheduled later will be more likely to be run
        ///     later.
        /// </summary>
        PreferFairness = 1,

        /// <summary>
        ///     The task will be a long-running, coarse-grained operation. It provides a hint to the <see cref="TaskScheduler" />
        ///     that oversubscription may be warranted. For background tasks, this member causes the task to run its own thread
        ///     instead of the thread pool.
        /// </summary>
        LongRunning = 2,

        /// <summary>
        ///     Creates the task as attached to the currently-running task. The parent task is not marked as completed until this
        ///     child task is completed as well.
        /// </summary>
        AttachedToParent = 4,

        /// <summary>
        ///     A child task cannot be attached to the task.
        /// </summary>
        DenyChildAttach = 8,
        CancelWithParent = 536870912,

        /// <summary>
        ///     The task cannot be canceled. Users will get an exception if they try to cancel the task.
        /// </summary>
        NotCancelable = -2147483648
    }
}