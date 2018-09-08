using ModernApplicationFramework.Basics.Services.TaskSchedulerService;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Interfaces.Threading;
using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IMafTaskSchedulerService
    {
        JoinableTaskContext GetAsyncTaskContext();

        object GetTaskScheduler(MafTaskRunContext context);

        IMafTaskCompletionSource CreateTaskCompletionSource();

        IMafTaskCompletionSource CreateTaskCompletionSourceEx(MafTaskCreationOptions options, object AsyncState);

        IMafTask CreateTask(MafTaskRunContext context, IMafTaskBody pTaskBody);

        IMafTask CreateTaskEx(MafTaskRunContext context, MafTaskCreationOptions options, IMafTaskBody pTaskBody, object pAsyncState);

        IMafTask ContinueWhenAllCompleted(MafTaskRunContext context, uint dwTasks, IMafTask[] pDependentTasks, IMafTaskBody pTaskBody);

        IMafTask ContinueWhenAllCompletedEx(MafTaskRunContext context, uint dwTasks, IMafTask[] pDependentTasks, MafTaskContinuationOptions options, IMafTaskBody pTaskBody, object pAsyncState);
    }
}
