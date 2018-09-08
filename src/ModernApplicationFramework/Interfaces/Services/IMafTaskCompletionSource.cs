using ModernApplicationFramework.Interfaces.Threading;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IMafTaskCompletionSource
    {
        /// <summary>
        /// Gets the task owned by this source.
        /// </summary>
        IMafTask Task { get; }

        /// <summary>
        /// Sets the task owned by this source to completed state with the result.
        /// </summary>
        /// <param name="result">The result to be set.</param>
        void SetResult(object result);

        /// <summary>
        /// Sets the task owned by this source to cancelled state, also cancelling the task.
        /// </summary>
        void SetCanceled();

        /// <summary>
        /// Sets the task owned by this source to the faulted state (with the given HRESULT code).
        /// </summary>
        /// <param name="hr">The error code to set in the faulted state.</param>
        void SetFaulted(int hr);

        /// <summary>
        /// Adds the specified task to the task completion sources dependent task list. Then if <see cref="IMafTask.Wait()"/> is called on <see cref="IMafTaskCompletionSource.Task"/>, the UI can be unblocked correctly.
        /// </summary>
        /// <param name="pTask">The p task.</param>
        void AddDependentTask(IMafTask pTask);
    }
}