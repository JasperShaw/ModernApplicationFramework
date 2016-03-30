using System;
using ModernApplicationFramework.Caliburn.Extensions;
using Action = System.Action;

namespace ModernApplicationFramework.Caliburn.Result
{
    /// <summary>
    /// A result that executes an <see cref="System.Action"/>.
    /// </summary>
    public class DelegateResult : IResult
    {
        readonly Action _toExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateResult"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public DelegateResult(Action action)
        {
            _toExecute = action;
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(CoroutineExecutionContext context)
        {
            var eventArgs = new ResultCompletionEventArgs();

            try
            {
                _toExecute();
            }
            catch (Exception ex)
            {
                eventArgs.Error = ex;
            }

            Completed(this, eventArgs);
        }
    }

    /// <summary>
    /// A result that executes a <see cref="System.Func&lt;TResult&gt;"/>
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class DelegateResult<TResult> : IResult<TResult>
    {
        readonly Func<TResult> _toExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateResult&lt;TResult&gt;"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public DelegateResult(Func<TResult> action)
        {
            _toExecute = action;
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(CoroutineExecutionContext context)
        {
            var eventArgs = new ResultCompletionEventArgs();

            try
            {
                Result = _toExecute();
            }
            catch (Exception ex)
            {
                eventArgs.Error = ex;
            }

            Completed(this, eventArgs);
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public TResult Result { get; private set; }
    }
}