using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Threading;

namespace ModernApplicationFramework.Basics.Threading
{
    internal class MafTaskCompletionSource : IMafTaskCompletionSource
    {
        public object AsyncState { get; }

        public IMafTask Task => InternalTask;

        internal TaskCompletionSource<object> InternalCompletionSource { get; }
        internal MafTask InternalTask { get; }

        public MafTaskCompletionSource(object asyncState, MafTaskCreationOptions options)
        {
            InternalCompletionSource = new TaskCompletionSource<object>(asyncState, MafTask.GetTplOptions(options));
            InternalTask = new MafTask(InternalCompletionSource);
            AsyncState = asyncState;
        }

        public void AddDependentTask(IMafTask task)
        {
            InternalTask.TryAddDependentTask(task, false);
            MafTask.JoinAntecedentJoinableTasks(task);
        }

        public void SetCanceled()
        {
            InternalCompletionSource.SetCanceled();
            InternalTask.Cancel();
        }

        public void SetFaulted(int hr)
        {
            InternalCompletionSource.SetException(Marshal.GetExceptionForHR(hr));
        }

        public void SetResult(object result)
        {
            InternalCompletionSource.SetResult(result);
        }
    }
}