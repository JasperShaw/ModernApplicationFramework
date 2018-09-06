using System;
using System.Threading;
using ModernApplicationFramework.Threading.NativeMethods;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    public class NoMessagePumpSyncContext : SynchronizationContext
    {
        /// <summary>
        /// Gets a shared instance of this class.
        /// </summary>
        public static SynchronizationContext Default { get; } = new NoMessagePumpSyncContext();

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ModernApplicationFramework.Threading.NoMessagePumpSyncContext" /> class.
        /// </summary>
        public NoMessagePumpSyncContext()
        {
            SetWaitNotificationRequired();
        }

        /// <inheritdoc />
        /// <summary>
        /// Synchronously blocks without a message pump.
        /// </summary>
        /// <param name="waitHandles">An array of type <see cref="T:System.IntPtr" /> that contains the native operating system handles.</param>
        /// <param name="waitAll">true to wait for all handles; false to wait for any handle.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
        /// <returns>
        /// The array index of the object that satisfied the wait.
        /// </returns>
        public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
        {
            Validate.IsNotNull(waitHandles, nameof(waitHandles));
            return Kernel32.WaitForMultipleObjects((uint)waitHandles.Length, waitHandles, waitAll, (uint)millisecondsTimeout);
        }
    }
}
