using ModernApplicationFramework.Interfaces.Threading;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    ///     This interface and its method are used to encapsulate a body of work that is going to be executed in a task.
    ///     Instead of using this interface, you should use the helper classes found in the <see cref="MafTaskHelper" />
    ///     namespace for managed or <see cref="MafTaskHelper" /> include files for native code to create instances of
    ///     <see cref="IMafTaskBody" /> from anonymous methods.
    /// </summary>
    public interface IMafTaskBody
    {
        /// <summary>
        ///     Performs the task work.
        /// </summary>
        /// <param name="pTask">The task.</param>
        /// <param name="dwCount">The number of parent tasks.</param>
        /// <param name="pParentTasks">The parent tasks.</param>
        /// <param name="pResult">The result.</param>
        void DoWork(IMafTask pTask, uint dwCount, IMafTask[] pParentTasks, out object pResult);
    }
}