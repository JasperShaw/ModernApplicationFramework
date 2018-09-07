namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal interface IMafUiThreadBlockableTaskScheduler : IMafTaskScheduler
    {
        void EnsureTasksUnblocked();
    }
}