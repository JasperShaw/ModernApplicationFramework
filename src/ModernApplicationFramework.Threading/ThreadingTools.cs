using System;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    public static class ThreadingTools
    {
        public static SpecializedSyncContext Apply(this SynchronizationContext syncContext,
            bool checkForChangesOnRevert = true)
        {
            return SpecializedSyncContext.Apply(syncContext, checkForChangesOnRevert);
        }

        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            Validate.IsNotNull(task, nameof(task));
            if (!cancellationToken.CanBeCanceled || task.IsCompleted)
                return task;
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<T>(cancellationToken);
            return WithCancellationSlow(task, cancellationToken);
        }

        internal static Task WithCancellation(this Task task, bool continueOnCapturedContext, CancellationToken cancellationToken)
        {
            Validate.IsNotNull(task, nameof(task));
            if (!cancellationToken.CanBeCanceled || task.IsCompleted)
            {
                return task;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return TaskFromCanceled(cancellationToken);
            }

            return WithCancellationSlow(task, continueOnCapturedContext, cancellationToken);
        }

        internal static Task TaskFromCanceled(CancellationToken cancellationToken)
        {
            return TaskFromCanceled<EmptyStruct>(cancellationToken);
        }

        internal static bool TrySetCanceled<T>(this TaskCompletionSource<T> tcs, CancellationToken cancellationToken)
        {
            if (LightUps<T>.TrySetCanceled == null)
                return tcs.TrySetCanceled();
            return LightUps<T>.TrySetCanceled(tcs, cancellationToken);
        }

        private static async Task WithCancellationSlow(this Task task, bool continueOnCapturedContext, CancellationToken cancellationToken)
        {
            Validate.IsNotNull(task, nameof(task));
            if (!(cancellationToken.CanBeCanceled))
                throw new Exception();

            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(continueOnCapturedContext))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            await task.ConfigureAwait(continueOnCapturedContext);
        }

        private static async Task<T> WithCancellationSlow<T>(Task<T> task, CancellationToken cancellationToken)
        {
            Validate.IsNotNull(task, nameof(task));
            if (!cancellationToken.CanBeCanceled)
                throw new Exception();

            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    cancellationToken.ThrowIfCancellationRequested();
            }

            return await task.ConfigureAwait(false);
        }

        internal static Task<T> TaskFromCanceled<T>(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.TrySetCanceled(cancellationToken);
            return tcs.Task;
        }

        internal static void AttachCancellation<T>(this TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken, ICancellationNotification cancellationCallback = null)
        {
            Validate.IsNotNull(taskCompletionSource, nameof(taskCompletionSource));

            if (cancellationToken.CanBeCanceled)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    taskCompletionSource.TrySetCanceled(cancellationToken);
                }
                else
                {
                    var tuple = new CancelableTaskCompletionSource<T>(taskCompletionSource, cancellationToken, cancellationCallback);
                    tuple.CancellationTokenRegistration = cancellationToken.Register(
                        s =>
                        {
                            var t = (CancelableTaskCompletionSource<T>)s;
                            t.TaskCompletionSource.TrySetCanceled(t.CancellationToken);
                            t.CancellationCallback?.OnCanceled();
                        },
                        tuple, false);
                    taskCompletionSource.Task.ContinueWith(
                        (_, s) =>
                        {
                            var t = (CancelableTaskCompletionSource<T>)s;
                            t.CancellationTokenRegistration.Dispose();
                        },
                        tuple,
                        CancellationToken.None,
                        TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                }
            }
        }

        private class CancelableTaskCompletionSource<T>
        {
            internal CancellationToken CancellationToken { get; }

            internal TaskCompletionSource<T> TaskCompletionSource { get; }

            internal ICancellationNotification CancellationCallback { get; }

            internal CancellationTokenRegistration CancellationTokenRegistration { get; set; }

            internal CancelableTaskCompletionSource(TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken, ICancellationNotification cancellationCallback)
            {
                TaskCompletionSource = taskCompletionSource ?? throw new ArgumentNullException(nameof(taskCompletionSource));
                CancellationToken = cancellationToken;
                CancellationCallback = cancellationCallback;
            }
        }

        internal interface ICancellationNotification
        {
            void OnCanceled();
        }
    }
}
