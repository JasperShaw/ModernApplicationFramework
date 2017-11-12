using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    /// Supplies helper methods for running tasks in managed code.
    /// </summary>
    public static class MafTaskHelper
    {
        /// <summary>
        /// Runs a cancelable task asynchronously and shows a waiting dialog
        /// </summary>
        /// <param name="waitCaption">Caption for the wait dialog.</param>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="delayToShowDialog">The delay in seconds to show the wait dialog.</param>
        public static void Run(string waitCaption,
            Func<IProgress<WaitDialogProgressData>, CancellationToken, Task> asyncMethod,
            TimeSpan? delayToShowDialog = null)
        {
            var factory = IoC.Get<IWaitDialogFactory>();
            var initialProgress = new WaitDialogProgressData(null, null, null, true);
            factory.StartWaitDialog(asyncMethod, waitCaption, initialProgress, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Runs a task asynchronously and shows a waiting dialog
        /// </summary>
        /// <param name="waitCaption">Caption for the wait dialog.</param>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="delayToShowDialog">The delay in seconds to show the wait dialog.</param>
        public static void Run(string waitCaption, Func<IProgress<WaitDialogProgressData>, Task> asyncMethod,
            TimeSpan? delayToShowDialog = null)
        {
            var factory = IoC.Get<IWaitDialogFactory>();
            var initialProgress = new WaitDialogProgressData(null);
            factory.StartWaitDialog(asyncMethod, waitCaption, initialProgress, delayToShowDialog ?? TimeSpan.FromSeconds(2));
        }
    }
}
