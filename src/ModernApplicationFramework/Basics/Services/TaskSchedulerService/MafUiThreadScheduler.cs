using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    public class MafUiThreadScheduler : TaskScheduler, IMafTaskScheduler
    {
        public bool IsUiThreadScheduler => true;
        public MafTaskRunContext SchedulerContext => MafTaskRunContext.UiThreadSend;

        internal static async void ExecuteTaskInRpcPriority(Task task, Action executeAction)
        {
            await ThreadHelper.Generic.InvokeAsync(executeAction, () =>
            {
                if (MafRunningTasksManager.IsBlockingTask(task))
                    MafRunningTasksManager.GetCurrentTaskWaitedOnUiThread()?.AbortWait();
                return false;
            });
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotSupportedException();
        }

        protected override void QueueTask(Task task)
        {
            ExecuteTaskInRpcPriority(task, () => TryExecuteTask(task));
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (ThreadHelper.CheckAccess())
                return TryExecuteTask(task);
            return false;
        }
    }
}