using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core
{
    public sealed class WpfSynchronizeInvoke : ISynchronizeInvoke
    {
        readonly Dispatcher _dispatcher;

        public bool InvokeRequired => !_dispatcher.CheckAccess();

        public WpfSynchronizeInvoke(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public object EndInvoke(IAsyncResult result)
        {
            if (!(result is AsyncResult r))
				throw new ArgumentException("result must be the return value of a WpfSynchronizeInvoke.BeginInvoke call!");
			r.Op.Wait();
			return r.Op.Result;
        }

        public object Invoke(Delegate method, object[] args)
        {
            object result = null;
            System.Exception exception = null;
            _dispatcher.Invoke(
                DispatcherPriority.Normal,
                (Action)delegate {
                    try
                    {
                        result = method.DynamicInvoke(args);
                    }
                    catch (TargetInvocationException ex)
                    {
                        exception = ex.InnerException;
                    }
                });
            // if an exception occurred, re-throw it on the calling thread
            if (exception != null)
                throw new TargetInvocationException(exception);
            return result;
        }

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            DispatcherOperation op;
            if (args == null || args.Length == 0)
                op = _dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
            else if (args.Length == 1)
                op = _dispatcher.BeginInvoke(DispatcherPriority.Normal, method, args[0]);
            else
                op = _dispatcher.BeginInvoke(DispatcherPriority.Normal, method, args[0], args.Splice(1));
            return new AsyncResult(op);
        }

        private sealed class AsyncResult : IAsyncResult
        {
            internal readonly DispatcherOperation Op;
            readonly object _lockObj = new object();
            ManualResetEvent _resetEvent;

            public AsyncResult(DispatcherOperation op)
            {
                Op = op;
            }

            public bool IsCompleted => Op.Status == DispatcherOperationStatus.Completed;
            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    lock (_lockObj)
                    {
                        if (_resetEvent == null)
                        {
                            Op.Completed += Op_Completed;
                            _resetEvent = new ManualResetEvent(false);
                            if (IsCompleted)
                                _resetEvent.Set();
                        }
                        return _resetEvent;
                    }
                }
            }

            void Op_Completed(object sender, EventArgs e)
            {
                lock (_lockObj)
                {
                    _resetEvent.Set();
                }
            }

            public object AsyncState => null;
            public bool CompletedSynchronously => false;
        }
    }
}
