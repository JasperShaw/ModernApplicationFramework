using ModernApplicationFramework.Basics.Threading;

namespace ModernApplicationFramework.Interfaces
{
    internal interface IMafTaskScheduler
    {
        MafTaskRunContext SchedulerContext { get; }

        bool IsUiThreadScheduler { get; }
    }
}