using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    public static class AwaitExtensions
    {
        public static ExecuteContinuationSynchronouslyAwaitable ConfigureAwaitRunInline(this Task antecedent)
        {
            Validate.IsNotNull(antecedent, nameof(antecedent));
            return new ExecuteContinuationSynchronouslyAwaitable(antecedent);
        }

        public static ExecuteContinuationSynchronouslyAwaitable<T> ConfigureAwaitRunInline<T>(this Task<T> antecedent)
        {
            Validate.IsNotNull(antecedent, nameof(antecedent));
            return new ExecuteContinuationSynchronouslyAwaitable<T>(antecedent);
        }

        public struct ExecuteContinuationSynchronouslyAwaitable
        {
            private readonly Task _antecedent;

            public ExecuteContinuationSynchronouslyAwaitable(Task antecedent)
            {
                Validate.IsNotNull(antecedent, nameof(antecedent));
                _antecedent = antecedent;
            }

            public ExecuteContinuationSynchronouslyAwaiter GetAwaiter() => new ExecuteContinuationSynchronouslyAwaiter(_antecedent);
        }

        public struct ExecuteContinuationSynchronouslyAwaitable<T>
        {
            private readonly Task<T> _antecedent;

            public ExecuteContinuationSynchronouslyAwaitable(Task<T> antecedent)
            {
                Validate.IsNotNull(antecedent, nameof(antecedent));
                _antecedent = antecedent;
            }

            public ExecuteContinuationSynchronouslyAwaiter<T> GetAwaiter() => new ExecuteContinuationSynchronouslyAwaiter<T>(_antecedent);
        }

        public struct ExecuteContinuationSynchronouslyAwaiter : INotifyCompletion
        {
            private readonly Task _antecedent;

            public bool IsCompleted => _antecedent.IsCompleted;

            public ExecuteContinuationSynchronouslyAwaiter(Task antecedent)
            {
                Validate.IsNotNull(antecedent, nameof(antecedent));
                _antecedent = antecedent;
            }

            public void GetResult() => _antecedent.GetAwaiter().GetResult();

            public void OnCompleted(Action continuation)
            {
                Validate.IsNotNull(continuation, nameof(continuation));
                _antecedent.ContinueWith(
                    (_, s) => ((Action) s)(),
                    continuation,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }
        }

        public struct ExecuteContinuationSynchronouslyAwaiter<T> : INotifyCompletion
        {
            private readonly Task<T> _antecedent;

            public bool IsCompleted => _antecedent.IsCompleted;

            public ExecuteContinuationSynchronouslyAwaiter(Task<T> antecedent)
            {
                Validate.IsNotNull(antecedent, nameof(antecedent));
                _antecedent = antecedent;
            }

            public T GetResult() => _antecedent.GetAwaiter().GetResult();

            public void OnCompleted(Action continuation)
            {
                Validate.IsNotNull(continuation, nameof(continuation));
                _antecedent.ContinueWith(
                    (_, s) => ((Action)s)(),
                    continuation,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }
        }
    }
}
