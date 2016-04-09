using System;
using ModernApplicationFramework.Caliburn.Extensions;

namespace ModernApplicationFramework.Caliburn.Result
{
    /// <summary>
    ///     A simple result.
    /// </summary>
    public sealed class SimpleResult : IResult
    {
        private readonly Exception _error;
        private readonly bool _wasCancelled;

        private SimpleResult(bool wasCancelled, Exception error)
        {
            _wasCancelled = wasCancelled;
            _error = error;
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
            Completed(this, new ResultCompletionEventArgs {WasCancelled = _wasCancelled, Error = _error});
        }

        /// <summary>
        ///     A result that is always canceled.
        /// </summary>
        /// <returns>The result.</returns>
        public static IResult Cancelled()
        {
            return new SimpleResult(true, null);
        }

        /// <summary>
        ///     A result that is always failed.
        /// </summary>
        public static IResult Failed(Exception error)
        {
            return new SimpleResult(false, error);
        }

        /// <summary>
        ///     A result that is always succeeded.
        /// </summary>
        public static IResult Succeeded()
        {
            return new SimpleResult(false, null);
        }
    }
}