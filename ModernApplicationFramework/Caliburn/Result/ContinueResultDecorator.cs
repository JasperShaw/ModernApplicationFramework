﻿using System;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Logger;

namespace ModernApplicationFramework.Caliburn.Result
{
    /// <summary>
    /// A result decorator which executes a coroutine when the wrapped result was cancelled.
    /// </summary>
    public class ContinueResultDecorator : ResultDecoratorBase
    {
        static readonly ILog Log = LogManager.GetLog(typeof (ContinueResultDecorator));
        readonly Func<IResult> _coroutine;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinueResultDecorator"/> class.
        /// </summary>
        /// <param name="result">The result to decorate.</param>
        /// <param name="coroutine">The coroutine to execute when <paramref name="result"/> was canceled.</param>
        public ContinueResultDecorator(IResult result, Func<IResult> coroutine)
            : base(result)
        {
            if (coroutine == null)
                throw new ArgumentNullException(nameof(coroutine));

            _coroutine = coroutine;
        }

        /// <summary>
        /// Called when the execution of the decorated result has completed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="innerResult">The decorated result.</param>
        /// <param name="args">The <see cref="ResultCompletionEventArgs" /> instance containing the event data.</param>
        protected override void OnInnerResultCompleted(CoroutineExecutionContext context, IResult innerResult,
            ResultCompletionEventArgs args)
        {
            if (args.Error != null || !args.WasCancelled)
            {
                OnCompleted(new ResultCompletionEventArgs {Error = args.Error});
            }
            else
            {
                Log.Info($"Executing coroutine because {innerResult.GetType().Name} was cancelled.");
                Continue(context);
            }
        }

        private void Continue(CoroutineExecutionContext context)
        {
            IResult continueResult;
            try
            {
                continueResult = _coroutine();
            }
            catch (Exception ex)
            {
                OnCompleted(new ResultCompletionEventArgs {Error = ex});
                return;
            }

            try
            {
                continueResult.Completed += ContinueCompleted;
                IoC.BuildUp(continueResult);
                continueResult.Execute(context);
            }
            catch (Exception ex)
            {
                ContinueCompleted(continueResult, new ResultCompletionEventArgs {Error = ex});
            }
        }

        void ContinueCompleted(object sender, ResultCompletionEventArgs args)
        {
            ((IResult) sender).Completed -= ContinueCompleted;
            OnCompleted(new ResultCompletionEventArgs {Error = args.Error, WasCancelled = (args.Error == null)});
        }
    }
}