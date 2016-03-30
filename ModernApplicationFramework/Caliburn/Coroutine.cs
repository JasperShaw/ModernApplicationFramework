﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Logger;
using ModernApplicationFramework.Caliburn.Result;

namespace ModernApplicationFramework.Caliburn
{
    /// <summary>
    /// Manages coroutine execution.
    /// </summary>
    public static class Coroutine
    {
        /// <summary>
        /// Creates the parent enumerator.
        /// </summary>
        public static Func<IEnumerator<IResult>, IResult> CreateParentEnumerator = inner => new SequentialResult(inner);

        static readonly ILog Log = LogManager.GetLog(typeof (Coroutine));

        /// <summary>
        /// Called upon completion of a coroutine.
        /// </summary>
        public static event EventHandler<ResultCompletionEventArgs> Completed = (s, e) =>
        {
            if (e.Error != null)
            {
                Log.Error(e.Error);
            }
            else if (e.WasCancelled)
            {
                Log.Info("Coroutine execution cancelled.");
            }
            else
            {
                Log.Info("Coroutine execution completed.");
            }
        };

        /// <summary>
        /// Executes a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        /// <param name="context">The context to execute the coroutine within.</param>
        /// /// <param name="callback">The completion callback for the coroutine.</param>
        public static void BeginExecute(IEnumerator<IResult> coroutine, CoroutineExecutionContext context = null,
            EventHandler<ResultCompletionEventArgs> callback = null)
        {
            Log.Info("Executing coroutine.");

            var enumerator = CreateParentEnumerator(coroutine);
            IoC.BuildUp(enumerator);

            if (callback != null)
            {
                ExecuteOnCompleted(enumerator, callback);
            }

            ExecuteOnCompleted(enumerator, Completed);
            enumerator.Execute(context ?? new CoroutineExecutionContext());
        }

        /// <summary>
        /// Executes a coroutine asynchronous.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        /// <param name="context">The context to execute the coroutine within.</param>
        /// <returns>A task that represents the asynchronous coroutine.</returns>
        public static Task ExecuteAsync(IEnumerator<IResult> coroutine, CoroutineExecutionContext context = null)
        {
            var taskSource = new TaskCompletionSource<object>();

            BeginExecute(coroutine, context, (s, e) =>
            {
                if (e.Error != null)
                    taskSource.SetException(e.Error);
                else if (e.WasCancelled)
                    taskSource.SetCanceled();
                else
                    taskSource.SetResult(null);
            });

            return taskSource.Task;
        }

        static void ExecuteOnCompleted(IResult result, EventHandler<ResultCompletionEventArgs> handler)
        {
            EventHandler<ResultCompletionEventArgs> onCompledted = null;
            onCompledted = (s, e) =>
            {
                result.Completed -= onCompledted;
                handler(s, e);
            };
            result.Completed += onCompledted;
        }
    }
}