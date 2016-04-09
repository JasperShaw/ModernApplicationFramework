﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Caliburn.Extensions;

namespace ModernApplicationFramework.Caliburn.Result
{
    /// <summary>
    ///     A couroutine that encapsulates an <see cref="System.Threading.Tasks.Task" />.
    /// </summary>
    public class TaskResult : IResult
    {
        private readonly Task _innerTask;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaskResult" /> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public TaskResult(Task task)
        {
            _innerTask = task;
        }

        /// <summary>
        ///     Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        ///     Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(CoroutineExecutionContext context)
        {
            if (_innerTask.IsCompleted)
                OnCompleted(_innerTask);
            else
                _innerTask.ContinueWith(OnCompleted,
                    SynchronizationContext.Current != null
                        ? TaskScheduler.FromCurrentSynchronizationContext()
                        : TaskScheduler.Current);
        }

        /// <summary>
        ///     Called when the asynchronous task has completed.
        /// </summary>
        /// <param name="task">The completed task.</param>
        protected virtual void OnCompleted(Task task)
        {
            Completed(this, new ResultCompletionEventArgs {WasCancelled = task.IsCanceled, Error = task.Exception});
        }
    }

    /// <summary>
    ///     A couroutine that encapsulates an <see cref="System.Threading.Tasks.Task&lt;TResult&gt;" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class TaskResult<TResult> : TaskResult, IResult<TResult>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TaskResult{TResult}" /> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public TaskResult(Task<TResult> task)
            : base(task) {}

        /// <summary>
        ///     Gets the result of the asynchronous operation.
        /// </summary>
        public TResult Result { get; private set; }

        /// <summary>
        ///     Called when the asynchronous task has completed.
        /// </summary>
        /// <param name="task">The completed task.</param>
        protected override void OnCompleted(Task task)
        {
            if (!task.IsFaulted && !task.IsCanceled)
                Result = ((Task<TResult>) task).Result;

            base.OnCompleted(task);
        }
    }
}