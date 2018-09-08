using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    public static class UiThreadReentrancyScope
    {
        private static readonly object LockObj = new object();
        private static readonly Queue<PendingRequest> Queue = new Queue<PendingRequest>();
        private static TaskCompletionSource<object> _queueHasElement = new TaskCompletionSource<object>();

        private static Task RequestWaiter
        {
            get
            {
                lock (LockObj)
                {
                    return _queueHasElement.Task;
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static async Task EnqueueActionAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (ThreadHelper.CheckAccess())
                await TaskScheduler.Default.SwitchTo();
            var pendingRequest = new PendingRequest(new InvokableAction(action), false);
            lock (LockObj)
            {
                Queue.Enqueue(pendingRequest);
                _queueHasElement.TrySetResult(null);
            }
        }

        public static bool WaitOnTaskComplete(Task task, CancellationToken cancel, int ms)
        {
            if (!ThreadHelper.CheckAccess())
                return task.Wait(ms, cancel);
            return WaitOnTaskCompleteInternal(task, cancel, ms);
        }

        internal static async Task<bool> TryExecuteActionAsyncInternal(InvokableBase action, int timeout = -1)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            ThreadHelper.ThrowIfOnUIThread(nameof(TryExecuteActionAsyncInternal));
            var pr = new PendingRequest(action, true);
            lock (LockObj)
            {
                Queue.Enqueue(pr);
                _queueHasElement.TrySetResult(null);
            }

            if (timeout != -1)
            {
                var delayCancellation = new CancellationTokenSource();
                await Task.WhenAny(pr.Waiter, Task.Delay(timeout, delayCancellation.Token));
                delayCancellation.Cancel();
            }
            else
            {
                var num = await pr.Waiter ? 1 : 0;
            }

            return await Dequeue(pr);
        }

        private static void ClearQueue()
        {
            lock (LockObj)
            {
                if (Queue.Count == 0)
                    return;
                List<PendingRequest> pendingRequestList = null;
                while (Queue.Count > 0)
                {
                    var pendingRequest = Queue.Dequeue();
                    if (!pendingRequest.AllowCleanup)
                    {
                        if (pendingRequestList == null)
                            pendingRequestList = new List<PendingRequest>();
                        pendingRequestList.Add(pendingRequest);
                    }
                    else
                    {
                        pendingRequest.SkipExecution().Forget();
                    }
                }

                if (pendingRequestList != null)
                {
                    pendingRequestList.Reverse();
                    foreach (var pendingRequest in pendingRequestList)
                        Queue.Enqueue(pendingRequest);
                }

                if (Queue.Count != 0)
                    return;
                _queueHasElement.TrySetResult(null);
                _queueHasElement = new TaskCompletionSource<object>();
            }
        }

        private static Task<bool> Dequeue(PendingRequest pr)
        {
            Task<bool> task;
            lock (LockObj)
            {
                task = pr.SkipExecution();
                while (Queue.Count > 0 && Queue.Peek().Revoked)
                    Queue.Dequeue();
                if (Queue.Count == 0)
                {
                    _queueHasElement.TrySetResult(null);
                    _queueHasElement = new TaskCompletionSource<object>();
                }
            }

            return task;
        }

        private static bool ExecuteOne()
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(ExecuteOne));
            TaskCompletionSource<bool> completeEvent = null;
            InvokableBase action = null;
            lock (LockObj)
            {
                PendingRequest pendingRequest = null;
                while (Queue.Count > 0 && pendingRequest == null)
                {
                    pendingRequest = Queue.Dequeue();
                    if (pendingRequest.Revoked)
                        pendingRequest = null;
                }

                pendingRequest?.InitiateExecute(out completeEvent, out action);
                if (Queue.Count == 0)
                {
                    _queueHasElement.TrySetResult(null);
                    _queueHasElement = new TaskCompletionSource<object>();
                }
            }

            if (completeEvent == null)
                return false;
            var num = action.Invoke();
            if (num >= 0)
                completeEvent.TrySetResult(true);
            else
                completeEvent.TrySetException(Marshal.GetExceptionForHR(num));
            return true;
        }

        private static void Flush()
        {
            do
            {
                ;
            } while (ExecuteOne());
        }

        private static bool WaitOnTaskCompleteInternal(Task task, CancellationToken cancel, int ms)
        {
            ThreadHelper.ThrowIfNotOnUIThread(nameof(WaitOnTaskCompleteInternal));
            var tasks = new[]
            {
                task,
                null
            };
            bool flag2;
            while (!task.IsCompleted)
            {
                tasks[1] = RequestWaiter;
                var stopwatch = Stopwatch.StartNew();
                switch (Task.WaitAny(tasks, ms, cancel))
                {
                    case 0:
                        task.Wait(ms, cancel);
                        break;
                    case 1:
                        Flush();
                        break;
                    default:
                        break;
                }

                stopwatch.Stop();
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                if (ms != -1)
                {
                    if (elapsedMilliseconds < 0L || elapsedMilliseconds >= ms)
                    {
                        flag2 = false;
                        goto label_11;
                    }

                    ms -= (int) elapsedMilliseconds;
                }
            }

            task.GetAwaiter().GetResult();
            flag2 = true;
            label_11:
            ClearQueue();
            return flag2;
        }

        internal class PendingRequest
        {
            internal bool AllowCleanup { get; }

            internal bool Revoked => InvokeAction == null;

            internal Task<bool> Waiter => WorkCompleteEvent.Task;

            private InvokableBase InvokeAction { get; set; }

            private bool Started { get; set; }
            private TaskCompletionSource<bool> WorkCompleteEvent { get; }

            internal PendingRequest(InvokableBase action, bool guaranteeExecution)
            {
                InvokeAction = action;
                Started = false;
                WorkCompleteEvent = new TaskCompletionSource<bool>();
                AllowCleanup = !guaranteeExecution;
            }

            internal void InitiateExecute(out TaskCompletionSource<bool> completeEvent, out InvokableBase action)
            {
                if (!Revoked)
                {
                    completeEvent = WorkCompleteEvent;
                    action = InvokeAction;
                    Started = true;
                }
                else
                {
                    completeEvent = null;
                    action = null;
                }

                InvokeAction = null;
            }

            internal Task<bool> SkipExecution()
            {
                if (!Started)
                {
                    WorkCompleteEvent.TrySetResult(false);
                    InvokeAction = null;
                }

                return Waiter;
            }
        }
    }
}