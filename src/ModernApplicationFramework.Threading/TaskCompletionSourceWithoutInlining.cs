using System.Threading.Tasks;

namespace ModernApplicationFramework.Threading
{
    internal class TaskCompletionSourceWithoutInlining<T> : TaskCompletionSource<T>
    {
        private readonly Task<T> _exposedTask;

        internal new Task<T> Task => base.Task.IsCompleted ? base.Task : _exposedTask;

        internal TaskCompletionSourceWithoutInlining(bool allowInliningContinuations, TaskCreationOptions options = TaskCreationOptions.None, object state = null)
            : base(state, AdjustFlags(options, allowInliningContinuations))
        {
            if (!allowInliningContinuations && !LightUps.IsRunContinuationsAsynchronouslySupported)
            {
                var innerTcs = new TaskCompletionSource<T>(state, options);
                base.Task.ApplyResultTo(innerTcs, false);
                _exposedTask = innerTcs.Task;
            }
            else
            {
                _exposedTask = base.Task;
            }
        }

        private static TaskCreationOptions AdjustFlags(TaskCreationOptions options, bool allowInliningContinuations)
        {
            return (!allowInliningContinuations && LightUps.IsRunContinuationsAsynchronouslySupported)
                ? (options | LightUps.RunContinuationsAsynchronously)
                : options;
        }
    }
}