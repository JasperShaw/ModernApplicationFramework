using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Threading
{
    public class JoinableTask<T> : JoinableTask
    {
        /// <summary>
        /// Gets the asynchronous task that completes when the async operation completes.
        /// </summary>
        public new Task<T> Task => (Task<T>)base.Task;

        internal JoinableTask(JoinableTaskFactory owner, bool synchronouslyBlocking, JoinableTaskCreationOptions creationOptions, Delegate initialDelegate)
            : base(owner, synchronouslyBlocking, creationOptions, initialDelegate)
        {
        }

        /// <summary>
        /// Gets an awaiter that is equivalent to calling <see cref="JoinAsync"/>.
        /// </summary>
        /// <returns>A task whose result is the result of the asynchronous operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public new TaskAwaiter<T> GetAwaiter()
        {
            return JoinAsync().GetAwaiter();
        }

        /// <summary>
        /// Joins any main thread affinity of the caller with the asynchronous operation to avoid deadlocks
        /// in the event that the main thread ultimately synchronously blocks waiting for the operation to complete.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that will exit this method before the task is completed.</param>
        /// <returns>A task that completes after the asynchronous operation completes and the join is reverted, with the result of the operation.</returns>
        public new async Task<T> JoinAsync(CancellationToken cancellationToken = default)
        {
            await base.JoinAsync(cancellationToken).ConfigureAwait(AwaitShouldCaptureSyncContext);
            return await Task.ConfigureAwait(AwaitShouldCaptureSyncContext);
        }

        /// <summary>
        /// Synchronously blocks the calling thread until the operation has completed.
        /// If the calling thread is the Main thread, deadlocks are mitigated.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that will exit this method before the task is completed.</param>
        /// <returns>The result of the asynchronous operation.</returns>
        public new T Join(CancellationToken cancellationToken = default)
        {
            base.Join(cancellationToken);
            if (!Task.IsCompleted)
                throw new Exception();
            return Task.Result;
        }

        internal new T CompleteOnCurrentThread()
        {
            base.CompleteOnCurrentThread();
            return Task.GetAwaiter().GetResult();
        }
    }
}
