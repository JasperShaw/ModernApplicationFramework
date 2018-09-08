using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal class MafThreadpoolLowIoScheduler : TaskScheduler, IMafTaskScheduler
    {
        private readonly WaitCallback _executeTaskAndDequeueHelperWaitCallback;
        private readonly ParameterizedThreadStart _executeTaskHelperThreadStart;
        private readonly HashSet<Task> _queuedTasks = new HashSet<Task>();

        public bool IsUiThreadScheduler => false;

        public MafTaskRunContext SchedulerContext => MafTaskRunContext.BackgroundThreadLowIoPriority;

        public MafThreadpoolLowIoScheduler()
        {
            _executeTaskHelperThreadStart = ExecuteTaskHelper;
            _executeTaskAndDequeueHelperWaitCallback = ExecuteTaskAndDequeueHelper;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            lock (_queuedTasks)
            {
                return _queuedTasks.ToList();
            }
        }

        protected override void QueueTask(Task task)
        {
            if ((task.CreationOptions & TaskCreationOptions.LongRunning) == TaskCreationOptions.LongRunning)
            {
                new Thread(_executeTaskHelperThreadStart)
                {
                    IsBackground = true
                }.Start(task);
            }
            else
            {
                lock (_queuedTasks)
                {
                    _queuedTasks.Add(task);
                }

                ThreadPool.QueueUserWorkItem(_executeTaskAndDequeueHelperWaitCallback, task);
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (!Thread.CurrentThread.IsThreadPoolThread || !TryExecuteTask(task))
                return false;
            if (taskWasPreviouslyQueued)
                lock (_queuedTasks)
                {
                    _queuedTasks.Remove(task);
                }

            return true;
        }

        private void ExecuteTaskAndDequeueHelper(object state)
        {
            var task = (Task) state;
            bool flag;
            using (LowPriorityIo.Start())
            {
                flag = TryExecuteTask(task);
            }

            if (!flag)
                return;
            lock (_queuedTasks)
            {
                _queuedTasks.Remove(task);
            }
        }

        private void ExecuteTaskHelper(object state)
        {
            var task = (Task) state;
            using (LowPriorityIo.Start())
            {
                TryExecuteTask(task);
            }
        }

        private struct LowPriorityIo : IDisposable
        {
            private bool _shouldUnsetThreadPriority;

            private LowPriorityIo(bool shouldUnsetThreadPriority)
            {
                _shouldUnsetThreadPriority = shouldUnsetThreadPriority;
            }

            public void Dispose()
            {
                if (!_shouldUnsetThreadPriority)
                    return;
                _shouldUnsetThreadPriority = false;
                Kernel32.SetThreadPriority(Kernel32.GetCurrentThread(),
                    NativeMethods.ThreadPriority.THREAD_MODE_BACKGROUND_END);
            }

            internal static LowPriorityIo Start()
            {
                return new LowPriorityIo(Kernel32.SetThreadPriority(Kernel32.GetCurrentThread(),
                    NativeMethods.ThreadPriority.THREAD_MODE_BACKGROUND_BEGIN));
            }
        }
    }
}