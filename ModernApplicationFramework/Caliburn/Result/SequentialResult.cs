using System;
using System.Collections.Generic;
using ModernApplicationFramework.Caliburn.Extensions;

namespace ModernApplicationFramework.Caliburn.Result
{
    /// <summary>
    ///     An implementation of <see cref="IResult" /> that enables sequential execution of multiple results.
    /// </summary>
    public class SequentialResult : IResult
    {
        private readonly IEnumerator<IResult> _enumerator;
        private CoroutineExecutionContext _context;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SequentialResult" /> class.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        public SequentialResult(IEnumerator<IResult> enumerator)
        {
            _enumerator = enumerator;
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
            _context = context;
            ChildCompleted(null, new ResultCompletionEventArgs());
        }

        private void ChildCompleted(object sender, ResultCompletionEventArgs args)
        {
            var previous = sender as IResult;
            if (previous != null)
            {
                previous.Completed -= ChildCompleted;
            }

            if (args.Error != null || args.WasCancelled)
            {
                OnComplete(args.Error, args.WasCancelled);
                return;
            }

            bool moveNextSucceeded;
            try
            {
                moveNextSucceeded = _enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                OnComplete(ex, false);
                return;
            }

            if (moveNextSucceeded)
            {
                try
                {
                    var next = _enumerator.Current;
                    IoC.BuildUp(next);
                    next.Completed += ChildCompleted;
                    next.Execute(_context);
                }
                catch (Exception ex)
                {
                    OnComplete(ex, false);
                }
            }
            else
            {
                OnComplete(null, false);
            }
        }

        private void OnComplete(Exception error, bool wasCancelled)
        {
            _enumerator.Dispose();
            Completed(this, new ResultCompletionEventArgs {Error = error, WasCancelled = wasCancelled});
        }
    }
}