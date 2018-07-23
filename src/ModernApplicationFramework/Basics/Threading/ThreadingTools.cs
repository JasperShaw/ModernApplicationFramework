using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Basics.Threading
{
    internal static class ThreadingTools
    {
        internal static Task<T> TaskFromCanceled<T>(CancellationToken cancellationToken)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            tcs.TrySetCanceled<T>(cancellationToken);
            return tcs.Task;
        }

        internal static bool TrySetCanceled<T>(this TaskCompletionSource<T> tcs, CancellationToken cancellationToken)
        {
            if (LightUps<T>.TrySetCanceled == null)
                return tcs.TrySetCanceled();
            return LightUps<T>.TrySetCanceled(tcs, cancellationToken);
        }

        internal static void ApplyResultTo<T>(this Task<T> task, TaskCompletionSource<T> tcs, bool inlineSubsequentCompletion)
        {
            if (task.IsCompleted)
            {
                ApplyCompletedTaskResultTo<T>(task, tcs);
            }
            else
            {
                var task1 = task;
                void Action(Task<T> t, object s) => ApplyCompletedTaskResultTo<T>(t, (TaskCompletionSource<T>) s);
                TaskCompletionSource<T> completionSource = tcs;
                CancellationToken none = CancellationToken.None;
                int num = inlineSubsequentCompletion ? 524288 : 0;
                TaskScheduler scheduler = TaskScheduler.Default;
                task1.ContinueWith(Action, completionSource, none, (TaskContinuationOptions)num, scheduler);
            }
        }

        private static void ApplyCompletedTaskResultTo<T>(Task<T> completedTask, TaskCompletionSource<T> taskCompletionSource)
        {
            if (completedTask.IsCanceled)
                taskCompletionSource.TrySetCanceled();
            else if (completedTask.IsFaulted)
                taskCompletionSource.TrySetException(completedTask.Exception.InnerExceptions);
            else
                taskCompletionSource.TrySetResult(completedTask.Result);
        }

        internal static void AttachCancellation<T>(this TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken, ICancellationNotification cancellationCallback = null)
        {
            if (!cancellationToken.CanBeCanceled)
                return;
            if (cancellationToken.IsCancellationRequested)
            {
                taskCompletionSource.TrySetCanceled<T>(cancellationToken);
            }
            else
            {
                CancelableTaskCompletionSource<T> completionSource1 = new CancelableTaskCompletionSource<T>(taskCompletionSource, cancellationToken, cancellationCallback);
                completionSource1.CancellationTokenRegistration = cancellationToken.Register(s =>
                {
                    CancelableTaskCompletionSource<T> completionSource = (CancelableTaskCompletionSource<T>)s;
                    completionSource.TaskCompletionSource.TrySetCanceled<T>(completionSource.CancellationToken);
                    completionSource.CancellationCallback?.OnCanceled();
                }, completionSource1, false);
                Task<T> task = taskCompletionSource.Task;
                CancelableTaskCompletionSource<T> completionSource3 = completionSource1;
                CancellationToken none = CancellationToken.None;
                int num = 524288;
                TaskScheduler scheduler = TaskScheduler.Default;
                task.ContinueWith((_, s) => ((CancelableTaskCompletionSource<T>)s).CancellationTokenRegistration.Dispose(), completionSource3, none, (TaskContinuationOptions)num, scheduler);
            }
        }

        internal interface ICancellationNotification
        {
            void OnCanceled();
        }

        private class CancelableTaskCompletionSource<T>
        {
            internal CancelableTaskCompletionSource(TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken, ICancellationNotification cancellationCallback)
            {
                TaskCompletionSource<T> completionSource = taskCompletionSource;
                TaskCompletionSource = completionSource ?? throw new ArgumentNullException(nameof(taskCompletionSource));
                CancellationToken = cancellationToken;
                CancellationCallback = cancellationCallback;
            }

            internal CancellationToken CancellationToken { get; }

            internal TaskCompletionSource<T> TaskCompletionSource { get; }

            internal ICancellationNotification CancellationCallback { get; }

            internal CancellationTokenRegistration CancellationTokenRegistration { get; set; }
        }
    }
}
