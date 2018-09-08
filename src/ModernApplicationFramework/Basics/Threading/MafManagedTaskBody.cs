using ModernApplicationFramework.Basics.Services.TaskSchedulerService;
using ModernApplicationFramework.Interfaces.Threading;

namespace ModernApplicationFramework.Basics.Threading
{
    internal sealed class MafManagedTaskBody : IMafTaskBody
    {
        private readonly MafTaskBodyCallback _taskBody;

        public MafManagedTaskBody(MafTaskBodyCallback action)
        {
            _taskBody = action;
        }

        public void DoWork(IMafTask pTask, uint dwCount, IMafTask[] pParentTasks, out object pResult)
        {
            pResult = _taskBody(pTask, pParentTasks);
        }
    }
}