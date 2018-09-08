using System;
using ModernApplicationFramework.Basics.Threading;

namespace ModernApplicationFramework.Interfaces.Threading
{
    public interface IMafTaskEvents
    {
        /// <summary>
        /// Raised when a blocking wait call is made to an <see cref="IMafTask"/> instance on the main thread of the application.
        /// </summary>
        event EventHandler OnBlockingWaitBegin;

        /// <summary>
        /// Raised when a blocking wait call to <see cref="IMafTask"/> is finished on the main thread of the application.
        /// </summary>
        event EventHandler OnBlockingWaitEnd;

        /// <summary>
        /// Raised when this task is marked as a blocking task for a wait on the main thread of the application.
        /// </summary>
        event EventHandler<BlockingTaskEventArgs> OnMarkedAsBlocking;
    }
}