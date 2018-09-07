using System;
using System.Runtime.Serialization;
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

        public override MafTaskRunContext SchedulerContext => MafTaskRunContext.UIThreadIdlePriority;

        public MafIdleTimeScheduler()
        {
            _mainThreadId = Kernel32.GetCurrentThreadId();
        }

        protected override void OnTaskQueued(Task task)
        {
            if (Interlocked.CompareExchange(ref _idleTriggered, 1, 0) != 0)
                return;
            User32.PostThreadMessage(_mainThreadId, 0U, IntPtr.Zero, IntPtr.Zero);
        }

        public void Dispose()
        {
        } 
    }

    [Serializable]
    public class TaskSchedulingException : Exception
    {
        public const int VsETaskschedulerfail = -2147213304;

        public TaskSchedulingException()
            : this("Task scheduling could not be completed in the requested context.")
        {
        }

        public TaskSchedulingException(string message)
            : base(message)
        {
            HResult = -2147213304;
        }

        protected TaskSchedulingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            HResult = -2147213304;
        }
    }
}