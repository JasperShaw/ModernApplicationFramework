using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal sealed class MafRunningTasksManager
    {
        private static readonly Stack<MafTask> TasksBeingWaited = new Stack<MafTask>();
        private static int _waitOnUiThreadCount;
        private static readonly HashSet<Task> BlockingTasks = new HashSet<Task>();
        [ThreadStatic]
        private static Stack<MafTask> _runningTasks;

        public static MafTask GetCurrentTask()
        {
            if (_runningTasks == null || _runningTasks.Count == 0)
                return null;
            return _runningTasks.Peek();
        }

        public static void PushCurrentTask(MafTask task)
        {
            if (_runningTasks == null)
                _runningTasks = new Stack<MafTask>();
            _runningTasks.Push(task);
        }

        public static void PopCurrentTask()
        {
            if (_runningTasks == null || _runningTasks.Count == 0)
                return;
            _runningTasks.Pop();
        }

        public static void PromoteTaskIfBlocking(MafTask blockingTask, MafTask taskToUnblock)
        {
            Validate.IsNotNull(blockingTask, nameof(blockingTask));
            Validate.IsNotNull(taskToUnblock, nameof(taskToUnblock));
            bool flag = false;
            lock (BlockingTasks)
            {
                if (_waitOnUiThreadCount == 0)
                    return;
                if (IsBlockingTask((Task)taskToUnblock.InternalTask))
                {
                    flag = true;
                    BlockingTasks.UnionWith(blockingTask.GetAllDependentInternalTasks());
                }
            }
            if (!flag)
                return;
            foreach (IMafUiThreadBlockableTaskScheduler uiThreadScheduler in MafTaskSchedulerService.Instance.GetAllUiThreadSchedulers())
                uiThreadScheduler.EnsureTasksUnblocked();
        }

        public static MafTask GetCurrentTaskWaitedOnUiThread()
        {
            lock (TasksBeingWaited)
            {
                if (TasksBeingWaited.Count == 0)
                    return null;
                return TasksBeingWaited.Peek();
            }
        }

        private static void PushCurrentTaskWaitedOnUiThread(MafTask task)
        {
            lock (TasksBeingWaited)
                TasksBeingWaited.Push(task);
        }

        private static void PopCurrentTaskWaitedOnUiThread()
        {
            lock (TasksBeingWaited)
            {
                if (TasksBeingWaited.Count == 0)
                    return;
                TasksBeingWaited.Pop();
            }
        }

        public static void BeginTaskWaitOnUiThread(MafTask task)
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(BeginTaskWaitOnUiThread));
            PushCurrentTaskWaitedOnUiThread(task);
            lock (BlockingTasks)
            {
                ++_waitOnUiThreadCount;
                BlockingTasks.UnionWith(task.GetAllDependentInternalTasks());
            }
            foreach (IMafUiThreadBlockableTaskScheduler uiThreadScheduler in MafTaskSchedulerService.Instance.GetAllUiThreadSchedulers())
                uiThreadScheduler.EnsureTasksUnblocked();
        }

        public static void EndTaskWaitOnUiThread()
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(EndTaskWaitOnUiThread));
            PopCurrentTaskWaitedOnUiThread();
            lock (BlockingTasks)
            {
                --_waitOnUiThreadCount;
                if (_waitOnUiThreadCount != 0)
                    return;
                BlockingTasks.Clear();
            }
        }

        public static bool IsBlockingTask(Task task)
        {
            lock (BlockingTasks)
                return _waitOnUiThreadCount != 0 && BlockingTasks.Contains(task);
        }
    }
}