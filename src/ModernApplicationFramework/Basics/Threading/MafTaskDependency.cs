namespace ModernApplicationFramework.Basics.Threading
{
    internal enum MafTaskDependency
    {
        Continuation = 1,
        AttachedTask = 2,
        WaitForExecution = 3
    }
}