namespace ModernApplicationFramework.Interfaces
{
    internal interface IMafUiThreadBlockableTaskScheduler : IMafTaskScheduler
    {
        void EnsureTasksUnblocked();
    }
}