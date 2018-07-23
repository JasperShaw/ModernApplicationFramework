using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Native.Standard;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Threading
{
    public class AsyncQueue<T> : ThreadingTools.ICancellationNotification
    {
        private volatile TaskCompletionSource<object> _completedSource;
        private Queue<T> _queueElements;
        private Queue<TaskCompletionSource<T>> _dequeuingWaiters;
        private bool _completeSignaled;
        private bool _onCompletedInvoked;

        public static readonly Task CompletedTask = Task.FromResult(new EmptyStruct());

        public bool IsEmpty => Count == 0;

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return _queueElements?.Count ?? 0;
                }
            }
        }

        public bool IsCompleted
        {
            get
            {
                lock (SyncRoot)
                    return _completeSignaled && IsEmpty;
            }
        }

        public Task Completion
        {
            get
            {
                if (_completedSource == null)
                {
                    lock (SyncRoot)
                    {
                        if (_completedSource == null)
                        {
                            if (IsCompleted)
                                return CompletedTask;
                            _completedSource = new TaskCompletionSource<object>();
                        }
                    }
                }
                return _completedSource.Task;
            }
        }

        protected object SyncRoot => this;

        protected virtual int InitialCapacity => 4;

        public void Complete()
        {
            lock (SyncRoot)
                _completeSignaled = true;
            CompleteIfNecessary();
        }

        public void Enqueue(T value)
        {
            if (TryEnqueue(value))
                return;
            Verify.FailOperation("Invalid after complete");
        }

        public bool TryEnqueue(T value)
        {
            bool alreadyDispatched = false;
            lock (SyncRoot)
            {
                if (_completeSignaled)
                    return false;

                var flag = false;
                do
                {
                    if ((_dequeuingWaiters != null ? _dequeuingWaiters.Count > 0 ? 1 : 0 : 0) == 0)
                    {
                        flag = true;
                        break;
                    }
                } while (!_dequeuingWaiters.Dequeue().TrySetResult(value));

                if (!flag)
                alreadyDispatched = true;
                FreeCanceledDequeuers();
                if (!alreadyDispatched)
                {
                    if (_queueElements == null)
                        _queueElements = new Queue<T>(InitialCapacity);
                    _queueElements.Enqueue(value);
                }
            }
            OnEnqueued(value, alreadyDispatched);
            return true;
        }

        public bool TryPeek(out T value)
        {
            lock (SyncRoot)
            {
                if (_queueElements != null && _queueElements.Count > 0)
                {
                    value = _queueElements.Peek();
                    return true;
                }
                value = default;
                return false;
            }
        }

        public T Peek()
        {
            if (!TryPeek(out var obj))
                Verify.FailOperation("Empty");
            return obj;
        }

        private void FreeCanceledDequeuers()
        {
            lock(SyncRoot)
            {
                while (true)
                {
                    if ((_dequeuingWaiters != null ? _dequeuingWaiters.Count > 0 ? 1 : 0 : 0) != 0 && _dequeuingWaiters.Peek().Task.IsCompleted)
                        _dequeuingWaiters.Dequeue();
                    else
                        break;
                }
            }
        }

        public Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ThreadingTools.TaskFromCanceled<T>(cancellationToken);
            }
            T result;
            lock (SyncRoot)
            {
                if (IsCompleted)
                    return ThreadingTools.TaskFromCanceled<T>(new CancellationToken(true));
                if ((_queueElements != null ? _queueElements.Count > 0 ? 1 : 0 : 0) != 0)
                {
                    result = _queueElements.Dequeue();
                }
                else
                {
                    if (_dequeuingWaiters == null)
                        _dequeuingWaiters = new Queue<TaskCompletionSource<T>>(2);
                    else
                        FreeCanceledDequeuers();
                    TaskCompletionSourceWithoutInlining<T> taskCompletionSource = new TaskCompletionSourceWithoutInlining<T>(false);
                    taskCompletionSource.AttachCancellation(cancellationToken, this);
                    _dequeuingWaiters.Enqueue(taskCompletionSource);
                    return taskCompletionSource.Task;
                }
            }
            CompleteIfNecessary();
            return Task.FromResult(result);
        }

        public bool TryDequeue(out T value)
        {
            int num = TryDequeueInternal(null, out value) ? 1 : 0;
            CompleteIfNecessary();
            return num != 0;
        }

        public void OnCanceled()
        {
            FreeCanceledDequeuers();
        }

        internal T[] ToArray()
        {
            lock (SyncRoot)
                return _queueElements.ToArray();
        }

        protected bool TryDequeue(Predicate<T> valueCheck, out T value)
        {
            Validate.IsNotNull(valueCheck, nameof(valueCheck));
            int num = TryDequeueInternal(valueCheck, out value) ? 1 : 0;
            CompleteIfNecessary();
            return num != 0;
        }

        protected virtual void OnEnqueued(T value, bool alreadyDispatched)
        {
        }

        protected virtual void OnDequeued(T value)
        {
        }

        protected virtual void OnCompleted()
        {
        }

        private bool TryDequeueInternal(Predicate<T> valueCheck, out T value)
        {
            bool flag;
            lock (SyncRoot)
            {
                if (_queueElements != null && _queueElements.Count > 0 && (valueCheck == null || valueCheck(_queueElements.Peek())))
                {
                    value = _queueElements.Dequeue();
                    flag = true;
                }
                else
                {
                    value = default;
                    flag = false;
                }
            }
            if (flag)
                OnDequeued(value);
            return flag;
        }

        private void CompleteIfNecessary()
        {
            bool flag1 = false;
            bool flag2;
            lock (SyncRoot)
            {
                flag2 = _completeSignaled && (_queueElements == null || _queueElements.Count == 0);
                if (flag2)
                {
                    flag1 = !_onCompletedInvoked;
                    _onCompletedInvoked = true;
                    while (true)
                    {
                        if ((_dequeuingWaiters != null ? _dequeuingWaiters.Count > 0 ? 1 : 0 : 0) != 0)
                            _dequeuingWaiters.Dequeue().TrySetCanceled();
                        else
                            break;
                    }
                }
            }
            if (!flag2)
                return;
            _completedSource?.TrySetResult(null);
            if (!flag1)
                return;
            OnCompleted();
        }
    }
}

