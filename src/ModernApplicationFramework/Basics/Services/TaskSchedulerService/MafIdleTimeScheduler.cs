using System;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal sealed class MafIdleTimeScheduler : MafUiThreadBlockableTaskScheduler, IDisposable
    {
        private readonly uint _mainThreadId;
        private int _idleTriggered;

        public override MafTaskRunContext SchedulerContext => MafTaskRunContext.UiThreadIdlePriority;

        public MafIdleTimeScheduler()
        {
            _mainThreadId = Kernel32.GetCurrentThreadId();
        }

        public void Dispose()
        {
        }

        protected override void OnTaskQueued(Task task)
        {
            if (Interlocked.CompareExchange(ref _idleTriggered, 1, 0) != 0)
                return;
            User32.PostThreadMessage(_mainThreadId, 0U, IntPtr.Zero, IntPtr.Zero);
        }
    }
}