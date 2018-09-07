using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    public static class TplExtensions
    {
        public static readonly Task CompletedTask = Task.FromResult(default(EmptyStruct));

        internal static Task<T> CanceledTaskOfT<T>() => CanceledTaskOfTCache<T>.CanceledTask;

        private static void ApplyCompletedTaskResultTo<T>(Task<T> completedTask, TaskCompletionSource<T> taskCompletionSource)
        {
            Validate.IsNotNull(completedTask, nameof(completedTask));
            if (!completedTask.IsCompleted)
                throw new Exception();
            Validate.IsNotNull(taskCompletionSource, nameof(taskCompletionSource));

            if (completedTask.IsCanceled)
                taskCompletionSource.TrySetCanceled();
            else if (completedTask.IsFaulted)
                taskCompletionSource.TrySetException(completedTask.Exception.InnerExceptions);
            else
                taskCompletionSource.TrySetResult(completedTask.Result);
        }

        private static void ApplyCompletedTaskResultTo<T>(Task completedTask, TaskCompletionSource<T> taskCompletionSource, T valueOnRanToCompletion)
        {
            Validate.IsNotNull(completedTask, nameof(completedTask));
            if (!(completedTask.IsCompleted))
                throw new Exception();
            Validate.IsNotNull(taskCompletionSource, nameof(taskCompletionSource));
            if (completedTask.IsCanceled)
                taskCompletionSource.TrySetCanceled();
            else if (completedTask.IsFaulted)
                taskCompletionSource.TrySetException(completedTask.Exception.InnerExceptions);
            else
                taskCompletionSource.TrySetResult(valueOnRanToCompletion);
        }

        internal static void ApplyResultTo<T>(this Task<T> task, TaskCompletionSource<T> tcs, bool inlineSubsequentCompletion)
        {
            Validate.IsNotNull(task, nameof(task));
            Validate.IsNotNull(tcs, nameof(tcs));

            if (task.IsCompleted)
                ApplyCompletedTaskResultTo(task, tcs);
            else
                task.ContinueWith(
                    (t, s) => ApplyCompletedTaskResultTo(t, (TaskCompletionSource<T>) s),
                    tcs,
                    CancellationToken.None,
                    inlineSubsequentCompletion
                        ? TaskContinuationOptions.ExecuteSynchronously
                        : TaskContinuationOptions.None,
                    TaskScheduler.Default);
        }

        public static void ApplyResultTo<T>(this Task task, TaskCompletionSource<T> tcs)
        {
            Validate.IsNotNull(task, nameof(task));
            Validate.IsNotNull(tcs, nameof(tcs));
            if (task.IsCompleted)
            {
                ApplyCompletedTaskResultTo(task, tcs, default);
            }
            else
            {
                task.ContinueWith(
                    (t, s) => ApplyCompletedTaskResultTo(t, (TaskCompletionSource<T>)s, default),
                    tcs,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }
        }

        public static void Forget(this Task task)
        {
        }

        private static class CanceledTaskOfTCache<T>
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            internal static readonly Task<T> CanceledTask = ThreadingTools.TaskFromCanceled<T>(new CancellationToken(true));
        }
    }
}
