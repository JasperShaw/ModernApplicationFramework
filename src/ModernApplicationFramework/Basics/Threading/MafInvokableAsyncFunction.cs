using System.Threading.Tasks;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Threading
{
    /// <summary>
    ///     Used by the asynchronous task library helper to take an <see cref="IMafTaskCompletionSource" /> and return a Task
    ///     Parallel Library (TPL) task.
    /// </summary>
    /// <typeparam name="T">The type of the result produced by this task.</typeparam>
    /// <param name="tcs">The task completion source.</param>
    /// <returns>The task.</returns>
    public delegate Task<T> MafInvokableAsyncFunction<T>(IMafTaskCompletionSource tcs);
}