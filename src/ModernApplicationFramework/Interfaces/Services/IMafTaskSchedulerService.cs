using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IMafTaskSchedulerService
    {
        JoinableTaskContext GetAsyncTaskContext();
    }
}
