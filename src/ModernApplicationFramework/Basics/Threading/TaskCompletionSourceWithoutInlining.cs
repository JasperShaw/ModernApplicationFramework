using System;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Basics.Threading
{
    internal class TaskCompletionSourceWithoutInlining<T> : TaskCompletionSource<T>
    {
        private readonly Task<T> _exposedTask;

        internal TaskCompletionSourceWithoutInlining(bool allowInliningContinuations, TaskCreationOptions options = TaskCreationOptions.None, object state = null)
            : base(state, AdjustFlags(options, allowInliningContinuations))
        {
            if (!allowInliningContinuations && !Enum.TryParse<TaskCreationOptions>("RunContinuationsAsynchronously", out _))
            {
                var tcs = new TaskCompletionSource<T>(state, options);
                base.Task.ApplyResultTo(tcs, false);
                _exposedTask = tcs.Task;
            }
            else
                _exposedTask = base.Task;
        }

        internal new Task<T> Task => !base.Task.IsCompleted ? _exposedTask : base.Task;

        private static TaskCreationOptions AdjustFlags(TaskCreationOptions options, bool allowInliningContinuations)
        {
            if (allowInliningContinuations || !Enum.TryParse<TaskCreationOptions>("RunContinuationsAsynchronously", out var enumVal))
                return options;

            return options | enumVal;
        }
    }
}
