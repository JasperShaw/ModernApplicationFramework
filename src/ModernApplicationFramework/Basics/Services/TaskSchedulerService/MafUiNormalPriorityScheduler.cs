using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal sealed class MafUiNormalPriorityScheduler : MafUiThreadBlockableTaskScheduler
    {
        public override MafTaskRunContext SchedulerContext => MafTaskRunContext.UIThreadNormalPriority;

        protected override void OnTaskQueued(Task task)
        {
            ThreadHelper.Generic.BeginInvoke(() =>
            {
                int dequeuedTaskCount = 0;
                DoOneTask(out dequeuedTaskCount);
            });
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (ThreadHelper.CheckAccess())
                return TryExecuteTask(task);
            return false;
        }
    }
}