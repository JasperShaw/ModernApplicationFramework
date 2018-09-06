using System;
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
