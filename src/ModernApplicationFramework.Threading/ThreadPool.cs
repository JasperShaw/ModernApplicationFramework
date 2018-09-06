using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Threading
{
    internal static class ThreadPool
    {
        internal static void QueueUserWorkItem(Action<object> action, object state)
        {
            Task.Factory.StartNew(action, state, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }
    }
}
