using System;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    ///     Specifies the options for task wait operations
    /// </summary>
    [Flags]
    public enum MafTaskWaitOptions
    {
        /// <summary>
        ///     The default behavior should be used.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The task must return from a wait immediately if the task is canceled.
        /// </summary>
        AbortOnTaskCancellation = 1
    }
}