using System;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Interfaces.Threading;

namespace ModernApplicationFramework.Basics.Threading
{
    public class MafTaskAwaiter : INotifyCompletion
    {
        private readonly MafTaskRunContext _contextToUse;
        private readonly IMafTask _taskToWaitOn;

        public bool IsCompleted => _taskToWaitOn.IsCompleted;

        internal MafTaskAwaiter(IMafTask task)
            : this(task, MafTaskRunContext.CurrentContext)
        {
        }

        internal MafTaskAwaiter(IMafTask task, MafTaskRunContext context)
        {
            _taskToWaitOn = task;
            _contextToUse = context;
        }

        public object GetResult()
        {
            return _taskToWaitOn.GetResult();
        }

        public void OnCompleted(Action continuation)
        {
            _taskToWaitOn.ContinueWithEx((uint) _contextToUse, 1073741824, MafTaskHelper.CreateTaskBody(continuation),
                null);
        }
    }
}