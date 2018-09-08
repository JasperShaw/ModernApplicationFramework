using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal sealed class MafUiNormalPriorityScheduler : MafUiThreadBlockableTaskScheduler
    {
        public override MafTaskRunContext SchedulerContext => MafTaskRunContext.UiThreadNormalPriority;

        protected override void OnTaskQueued(Task task)
        {
            ThreadHelper.Generic.BeginInvoke(() => { DoOneTask(out _); });
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return ThreadHelper.CheckAccess() && TryExecuteTask(task);
        }
    }
}