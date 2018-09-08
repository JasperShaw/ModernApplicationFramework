namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    ///     Specifies how the task is run.
    /// </summary>
    public enum MafTaskRunContext
    {
        /// <summary>
        ///     Runs the task on the background thread pool with normal priority.
        /// </summary>
        BackgroundThread,

        /// <summary>
        ///     Runs the task with the highest priority
        /// </summary>
        UiThreadSend,

        /// <summary>
        ///     Runs the task on the UI thread using background priority (that is, below user input).
        /// </summary>
        UiThreadBackgroundPriority,

        /// <summary>
        ///     Runs the task on the UI thread when the application is idle.
        /// </summary>
        UiThreadIdlePriority,

        /// <summary>
        ///     Runs the task on the current context (that is, the UI thread or the background thread).
        /// </summary>
        CurrentContext,

        /// <summary>
        ///     Runs the task on the background thread pool and sets the background mode on the thread while the task is running.
        ///     This is useful for I/O-heavy background tasks that are not time critical.
        /// </summary>
        BackgroundThreadLowIoPriority,

        /// <summary>
        ///     Runs the task on UI thread using Dispatcher with Normal priority.
        /// </summary>
        UiThreadNormalPriority
    }
}