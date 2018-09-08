using System;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    ///     Specifies the task’s continuation options.
    /// </summary>
    [Flags]
    public enum MafTaskContinuationOptions
    {
        /// <summary>
        ///     Default = "Continue on any, no task options, run asynchronously" Specifies that the default behavior should be
        ///     used. Continuations, by default, are scheduled when the antecedent task completes, regardless of the task's final
        ///     state.
        /// </summary>
        None = 0,

        /// <summary>
        ///     A hint to the task library to schedule a task in as fair a manner as possible, meaning that tasks scheduled sooner
        ///     are more likely to be run sooner, and tasks scheduled later are more likely to be run later.
        /// </summary>
        PreferFairness = 1,

        /// <summary>
        ///     The task is a long-running, course-grained operation. It provides a hint to the task library that oversubscription
        ///     may be warranted.
        /// </summary>
        LongRunning = 2,

        /// <summary>
        ///     The task is attached to a parent in the task hierarchy. The parent task is not marked as completed until this child
        ///     task is completed as well.
        /// </summary>
        AttachedToParent = 4,

        /// <summary>
        ///     An <see cref="InvalidOperationException" /> is thrown if an attempt is made to attach a child task to the created
        ///     task.
        /// </summary>
        DenyChildAttach = 8,

        /// <summary>
        ///     In the case of continuation cancellation, prevents completion of the continuation until the antecedent has
        ///     completed.
        /// </summary>
        LazyCancelation = 32,

        /// <summary>
        ///     The continuation task should not be scheduled if its antecedent ran to completion. This option is not valid for
        ///     multi-task continuations.
        /// </summary>
        NotOnRanToCompletion = 65536,

        /// <summary>
        ///     The continuation task should not be scheduled if its antecedent threw an unhandled exception. This option is not
        ///     valid for multi-task continuations.
        /// </summary>
        NotOnFaulted = 131072,

        /// <summary>
        ///     The continuation task should not be scheduled if its antecedent was canceled. This option is not valid for
        ///     multi-task continuations.
        /// </summary>
        NotOnCanceled = 262144,

        /// <summary>
        ///     The continuation task should be scheduled only if its antecedent threw an unhandled exception. This option is not
        ///     valid for multi-task continuations.
        /// </summary>
        OnlyOnFaulted = NotOnCanceled | NotOnRanToCompletion,

        /// <summary>
        ///     The continuation task should be scheduled only if its antecedent ran to completion. This option is not valid for
        ///     multi-task continuations.
        /// </summary>
        OnlyOnRanToCompletion = NotOnCanceled | NotOnFaulted,

        /// <summary>
        ///     The continuation task should be executed synchronously. With this option specified, the continuation is run on the
        ///     same thread that causes the antecedent task to transition into its final state. If the antecedent is already
        ///     complete when the continuation is created, the continuation is run on the thread creating the continuation. Only
        ///     very short-running continuations should be executed synchronously
        /// </summary>
        ExecuteSynchronously = 524288,
        CancelWithParent = 536870912,

        /// <summary>
        ///     The task can be canceled independently of any other task.
        /// </summary>
        IndependentlyCanceled = 1073741824,

        /// <summary>
        ///     The continuation task cannot be canceled.
        /// </summary>
        NotCancelable = -2147483648,

        /// <summary>
        ///     The same as <see cref="NotOnFaulted" />.
        /// </summary>
        Default = NotOnFaulted
    }
}