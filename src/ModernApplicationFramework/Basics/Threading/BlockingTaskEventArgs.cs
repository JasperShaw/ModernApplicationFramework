using System;
using ModernApplicationFramework.Interfaces.Threading;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <inheritdoc />
    /// <summary>
    ///     The event arguments to be used with an event that passes in blocking task information. The parameters identify the
    ///     task that is being waited on the UI thread (blocked task) and the task that is blocking the wait (blocking task).
    /// </summary>
    /// <seealso cref="T:System.EventArgs" />
    public class BlockingTaskEventArgs : EventArgs
    {
        /// <summary>
        ///     Gets the task that is being waited on the UI thread and that needs to be unblocked.
        /// </summary>
        public IMafTask BlockedTask { get; }

        /// <summary>
        ///     Gets the task that is blocking a task being waited on the UI thread.
        /// </summary>
        public IMafTask BlockingTask { get; }

        public BlockingTaskEventArgs(IMafTask blockingTask, IMafTask blockedTask)
        {
            BlockedTask = blockedTask;
            BlockingTask = blockingTask;
        }
    }
}