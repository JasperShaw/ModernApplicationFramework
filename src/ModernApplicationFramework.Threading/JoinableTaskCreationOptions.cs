using System;

namespace ModernApplicationFramework.Threading
{
    [Flags]
    [Serializable]
    public enum  JoinableTaskCreationOptions
    {
        /// <summary>
        /// Specifies that the default behavior should be used.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Specifies that a task will be a long-running operation. It provides a hint to the
        /// <see cref="JoinableTaskContext"/> that hang report should not be fired, when the main thread task is blocked on it.
        /// </summary>
        LongRunning = 0x01
    }
}
