﻿using System;
using ModernApplicationFramework.Caliburn.Extensions;

namespace ModernApplicationFramework.Caliburn.Result
{
    /// <summary>
    /// Base class for all <see cref="IResult"/> decorators.
    /// </summary>
    public abstract class ResultDecoratorBase : IResult
    {
        readonly IResult _innerResult;
        CoroutineExecutionContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultDecoratorBase"/> class.
        /// </summary>
        /// <param name="result">The result to decorate.</param>
        protected ResultDecoratorBase(IResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            _innerResult = result;
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
            _context = context;

            try
            {
                _innerResult.Completed += InnerResultCompleted;
                IoC.BuildUp(_innerResult);
                _innerResult.Execute(_context);
            }
            catch (Exception ex)
            {
                InnerResultCompleted(_innerResult, new ResultCompletionEventArgs {Error = ex});
            }
        }

        /// <summary>
        /// Raises the <see cref="Completed" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ResultCompletionEventArgs"/> instance containing the event data.</param>
        protected void OnCompleted(ResultCompletionEventArgs args)
        {
            Completed(this, args);
        }

        /// <summary>
        /// Called when the execution of the decorated result has completed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="innerResult">The decorated result.</param>
        /// <param name="args">The <see cref="ResultCompletionEventArgs"/> instance containing the event data.</param>
        protected abstract void OnInnerResultCompleted(CoroutineExecutionContext context, IResult innerResult,
            ResultCompletionEventArgs args);

        void InnerResultCompleted(object sender, ResultCompletionEventArgs args)
        {
            _innerResult.Completed -= InnerResultCompleted;
            OnInnerResultCompleted(_context, _innerResult, args);
            _context = null;
        }
    }
}