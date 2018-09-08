using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    public static class AwaitExtensions
    {
        public static TaskSchedulerAwaiter GetAwaiter(this TaskScheduler scheduler)
        {
            Validate.IsNotNull(scheduler, nameof(scheduler));
            return new TaskSchedulerAwaiter(scheduler);
        }

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

        public static TaskSchedulerAwaitable SwitchTo(this TaskScheduler scheduler, bool alwaysYield = false)
        {
            Validate.IsNotNull(scheduler, nameof(scheduler));
            return new TaskSchedulerAwaitable(scheduler, alwaysYield);
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

        public struct TaskSchedulerAwaitable
        {
            private readonly TaskScheduler _taskScheduler;
            private readonly bool _alwaysYield;

            public TaskSchedulerAwaitable(TaskScheduler taskScheduler, bool alwaysYield = false)
            {
                Validate.IsNotNull(taskScheduler, nameof(taskScheduler));
                _taskScheduler = taskScheduler;
                _alwaysYield = alwaysYield;
            }

            [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
            public TaskSchedulerAwaiter GetAwaiter()
            {
                return new TaskSchedulerAwaiter(_taskScheduler, _alwaysYield);
            }
        }

        public struct TaskSchedulerAwaiter : ICriticalNotifyCompletion
        {
            private readonly TaskScheduler _scheduler;
            private readonly bool _alwaysYield;

            public TaskSchedulerAwaiter(TaskScheduler scheduler, bool alwaysYield = false)
            {
                _scheduler = scheduler;
                _alwaysYield = alwaysYield;
            }

            public bool IsCompleted
            {
                get
                {
                    if (_alwaysYield)
                        return false;
                    if (_scheduler == TaskScheduler.Default & Thread.CurrentThread.IsThreadPoolThread)
                        return true;
                    if (_scheduler == TaskScheduler.Current)
                        return TaskScheduler.Current != TaskScheduler.Default;
                    return false;
                }
            }

            public void OnCompleted(Action continuation)
            {
                if (_scheduler == TaskScheduler.Default)
                    System.Threading.ThreadPool.QueueUserWorkItem(state => ((Action)state)(), continuation);
                else
                    Task.Factory.StartNew(continuation, CancellationToken.None, TaskCreationOptions.None, _scheduler);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                if (_scheduler == TaskScheduler.Default)
                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(state => ((Action)state)(), continuation);
                else
                    Task.Factory.StartNew(continuation, CancellationToken.None, TaskCreationOptions.None, _scheduler);
            }

            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            public void GetResult()
            {
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
