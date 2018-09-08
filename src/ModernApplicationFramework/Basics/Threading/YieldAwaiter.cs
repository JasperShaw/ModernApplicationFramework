using System;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Threading
{
    public struct YieldAwaiter : INotifyCompletion
    {
        private readonly MafTaskRunContext _contextToUse;
        private readonly IMafTaskCompletionSource _ownerTaskCompletionSource;
        private readonly IMafTaskSchedulerService _taskScheduler;

        public bool IsCompleted => false;

        internal YieldAwaiter(IMafTaskSchedulerService scheduler, IMafTaskCompletionSource taskCompletionSource,
            MafTaskRunContext context)
        {
            _taskScheduler = scheduler;
            _ownerTaskCompletionSource = taskCompletionSource;
            _contextToUse = context;
        }

        public YieldAwaiter GetAwaiter()
        {
            return this;
        }

        public object GetResult()
        {
            return null;
        }

        public void OnCompleted(Action continuation)
        {
            var andStartTask = MafTaskHelper.CreateAndStartTask(_taskScheduler, _contextToUse,
                MafTaskHelper.CreateTaskBody(continuation));
            _ownerTaskCompletionSource?.AddDependentTask(andStartTask);
        }
    }
}