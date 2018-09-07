using ModernApplicationFramework.Basics.Services.TaskSchedulerService;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IMafTaskSchedulerService
    {
        JoinableTaskContext GetAsyncTaskContext();

        object GetTaskScheduler(MafTaskRunContext context);

        IMafTaskCompletionSource CreateTaskCompletionSource();
    }
}
