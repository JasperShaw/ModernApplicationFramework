using ModernApplicationFramework.Basics.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal interface IMafTaskScheduler
    {
        MafTaskRunContext SchedulerContext { get; }

        bool IsUIThreadScheduler { get; }
    }
}