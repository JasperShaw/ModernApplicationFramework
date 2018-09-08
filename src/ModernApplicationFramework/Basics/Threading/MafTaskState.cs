namespace ModernApplicationFramework.Basics.Threading
{
    public enum MafTaskState
    {
        Created,
        Scheduled,
        Running,
        Canceled,
        Faulted,
        Completed
    }
}