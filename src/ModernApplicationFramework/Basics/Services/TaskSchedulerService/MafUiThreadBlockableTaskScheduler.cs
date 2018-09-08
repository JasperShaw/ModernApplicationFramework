using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Threading;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal abstract class MafUiThreadBlockableTaskScheduler : TaskScheduler, IMafUiThreadBlockableTaskScheduler
    {
        private readonly Lazy<JoinableTaskFactory> _matchingPriorityJoinableTaskFactory;
        private readonly object _syncObject = new object();

        private readonly Dictionary<Task, TaskCompletionSource<object>> _taskActivationWaiters =
            new Dictionary<Task, TaskCompletionSource<object>>();

        private readonly ConcurrentQueue<Task> _taskQueue = new ConcurrentQueue<Task>();

        public bool IsUiThreadScheduler => true;

        public override int MaximumConcurrencyLevel => 1;

        public abstract MafTaskRunContext SchedulerContext { get; }

        protected MafUiThreadBlockableTaskScheduler()
        {
            _matchingPriorityJoinableTaskFactory =
                new Lazy<JoinableTaskFactory>(() => new TaskSchedulerJoinableTaskFactory(this));
        }

        public void EnsureTasksUnblocked()
        {
            lock (_syncObject)
            {
                var taskList = new List<Task>();
                while (!_taskQueue.IsEmpty)
                    if (_taskQueue.TryDequeue(out var result))
                    {
                        if (MafRunningTasksManager.IsBlockingTask(result))
                            PromoteTaskExecution(result);
                        taskList.Add(result);
                    }

                taskList.Reverse();
                foreach (var task in taskList)
                    _taskQueue.Enqueue(task);
            }
        }

        internal async Task<bool> TryExecuteTaskAsync(Task task)
        {
            var blockableTaskScheduler = this;
            if (task.IsCompleted)
                return false;
            await Default.SwitchTo(true);
            if (task.IsCompleted)
                return false;
            await blockableTaskScheduler.WaitForTaskToBeScheduledAsync(task);
            await blockableTaskScheduler._matchingPriorityJoinableTaskFactory.Value.SwitchToMainThreadAsync();
            return blockableTaskScheduler.TryExecuteTask(task);
        }

        protected bool DoOneTask(out int dequeuedTaskCount)
        {
            dequeuedTaskCount = 0;
            Task result = null;
            for (var flag = false; !flag; flag = TryExecuteTask(result))
            {
                if (!_taskQueue.TryDequeue(out result))
                    return false;
                ++dequeuedTaskCount;
            }

            return true;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _taskQueue;
        }

        protected abstract void OnTaskQueued(Task task);

        protected void PromoteTaskExecution(Task task)
        {
            UiThreadReentrancyScope.EnqueueActionAsync(() => TryExecuteTask(task)).Forget();
        }

        protected override void QueueTask(Task task)
        {
            TaskCompletionSource<object> completionSource1;
            lock (_syncObject)
            {
                if (_taskActivationWaiters.TryGetValue(task, out completionSource1))
                    _taskActivationWaiters.Remove(task);
                if (MafRunningTasksManager.IsBlockingTask(task))
                    PromoteTaskExecution(task);
                _taskQueue.Enqueue(task);
                OnTaskQueued(task);
            }

            if (completionSource1 == null)
                return;
            var factory = Task.Factory;
            var completionSource2 = completionSource1;
            var none = CancellationToken.None;
            var num = 0;
            var scheduler = Default;
            factory.StartNew(state => ((TaskCompletionSource<object>) state).TrySetResult(null), completionSource2,
                none, (TaskCreationOptions) num, scheduler).Forget();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (taskWasPreviouslyQueued && ThreadHelper.CheckAccess())
                return TryExecuteTask(task);
            return false;
        }

        private async Task WaitForTaskToBeScheduledAsync(Task task)
        {
            Validate.IsNotNull(task, nameof(task));
            if (task.Status >= TaskStatus.WaitingToRun)
                return;
            TaskCompletionSource<object> tcs;
            lock (_syncObject)
            {
                if (task.Status >= TaskStatus.WaitingToRun)
                    return;
                if (!_taskActivationWaiters.TryGetValue(task, out tcs))
                {
                    tcs = new TaskCompletionSource<object>();
                    _taskActivationWaiters.Add(task, tcs);
                }
            }

            if (task.Status >= TaskStatus.WaitingToRun)
                tcs.TrySetResult(null);
            task.ApplyResultTo(tcs);
            try
            {
                var task1 = await tcs.Task;
            }
            catch
            {
            }
            finally
            {
                lock (_syncObject)
                {
                    _taskActivationWaiters.Remove(task);
                }
            }
        }

        private class TaskSchedulerJoinableTaskFactory : JoinableTaskFactory
        {
            private readonly TaskScheduler _taskScheduler;

            internal TaskSchedulerJoinableTaskFactory(TaskScheduler taskScheduler)
                : base(MafTaskSchedulerService.Instance.JoinableTaskContext)
            {
                Validate.IsNotNull(taskScheduler, nameof(taskScheduler));
                _taskScheduler = taskScheduler;
            }

            protected override void PostToUnderlyingSynchronizationContext(SendOrPostCallback callback, object state)
            {
                Task.Factory.StartNew(callback.Invoke, state, CancellationToken.None, TaskCreationOptions.None,
                    _taskScheduler).Forget();
            }
        }
    }
}