using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Threading
{
    public class AsyncAutoResetEvent
    {
        private readonly Queue<WaiterCompletionSource> _signalAwaiters = new Queue<WaiterCompletionSource>();
        private readonly bool _allowInliningAwaiters;
        private readonly Action<object> _onCancellationRequestHandler;
        private bool _signaled;

        public AsyncAutoResetEvent()
          : this(false)
        {
        }

        public AsyncAutoResetEvent(bool allowInliningAwaiters)
        {
            _allowInliningAwaiters = allowInliningAwaiters;
            _onCancellationRequestHandler = OnCancellationRequest;
        }

        public Task WaitAsync()
        {
            return WaitAsync(CancellationToken.None);
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return ThreadingTools.TaskFromCanceled(cancellationToken);
            lock (_signalAwaiters)
            {
                if (_signaled)
                {
                    _signaled = false;
                    return TplExtensions.CompletedTask;
                }
                WaiterCompletionSource tcs = new WaiterCompletionSource(this, cancellationToken, _allowInliningAwaiters);
                if (cancellationToken.IsCancellationRequested)
                    tcs.TrySetCanceled<EmptyStruct>(cancellationToken);
                else
                    _signalAwaiters.Enqueue(tcs);
                return tcs.Task;
            }
        }

        public void Set()
        {
            WaiterCompletionSource completionSource = null;
            lock (_signalAwaiters)
            {
                if (_signalAwaiters.Count > 0)
                    completionSource = _signalAwaiters.Dequeue();
                else if (!_signaled)
                    _signaled = true;
            }
            if (completionSource == null)
                return;
            completionSource.Registration.Dispose();
            completionSource.TrySetResult(new EmptyStruct());
        }

        private void OnCancellationRequest(object state)
        {
            WaiterCompletionSource completionSource = (WaiterCompletionSource)state;
            bool flag;
            lock (_signalAwaiters)
                flag = _signalAwaiters.RemoveMidQueue(completionSource);
            if (!flag)
                return;
            completionSource.TrySetCanceled<EmptyStruct>(completionSource.CancellationToken);
        }

        private class WaiterCompletionSource : TaskCompletionSourceWithoutInlining<EmptyStruct>
        {
            public WaiterCompletionSource(AsyncAutoResetEvent owner, CancellationToken cancellationToken, bool allowInliningContinuations)
              : base(allowInliningContinuations)
            {
                CancellationToken = cancellationToken;
                Registration = cancellationToken.Register(owner._onCancellationRequestHandler, this);
            }

            public CancellationToken CancellationToken { get; }

            public CancellationTokenRegistration Registration { get; }
        }
    }
}
