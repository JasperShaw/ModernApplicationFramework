using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    public class MafUiThreadScheduler : TaskScheduler, IMafTaskScheduler
    {
        public MafTaskRunContext SchedulerContext => MafTaskRunContext.UIThreadSend;

        public bool IsUIThreadScheduler => true;

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotSupportedException();
        }

        protected override void QueueTask(Task task)
        {
            ExecuteTaskInRPCPriority(task, () => TryExecuteTask(task));
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (ThreadHelper.CheckAccess())
                return TryExecuteTask(task);
            return false;
        }

        internal static async void ExecuteTaskInRPCPriority(Task task, Action executeAction)
        {
            await ThreadHelper.Generic.InvokeAsync(executeAction, () =>
            {
                if (MafRunningTasksManager.IsBlockingTask(task))
                    MafRunningTasksManager.GetCurrentTaskWaitedOnUiThread()?.AbortWait();
                return false;
            });
        }
    }
}