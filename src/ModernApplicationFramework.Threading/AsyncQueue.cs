using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    public class AsyncQueue<T> : ThreadingTools.ICancellationNotification
    {
        private volatile TaskCompletionSource<object> _completedSource;
        private Queue<T> _queueElements;
        private Queue<TaskCompletionSource<T>> _dequeuingWaiters;
        private bool _completeSignaled;
        private bool _onCompletedInvoked;

        /// <summary>
        /// Gets a value indicating whether the queue is currently empty.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Gets the number of elements currently in the queue.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether the queue has completed.
        /// </summary>
        /// <remarks>
        /// This is arguably redundant with <see cref="Completion"/>.IsCompleted, but this property
        /// won't cause the lazy instantiation of the Task that <see cref="Completion"/> may if there
        /// is no other reason for the Task to exist.
        /// </remarks>
        public bool IsCompleted
        {
            get
            {
                lock (SyncRoot)
                {
                    return _completeSignaled && IsEmpty;
                }
            }
        }

        /// <summary>
        /// Gets a task that transitions to a completed state when <see cref="Complete"/> is called.
        /// </summary>
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
                            {
                                return TplExtensions.CompletedTask;
                            }

                            _completedSource = new TaskCompletionSource<object>();
                        }
                    }
                }

                return _completedSource.Task;
            }
        }

        /// <summary>
        /// Gets the synchronization object used by this queue.
        /// </summary>
        protected object SyncRoot => this; // save allocations by using this instead of a new object.

        /// <summary>
        /// Gets the initial capacity for the queue.
        /// </summary>
        protected virtual int InitialCapacity => 4;

        internal T[] ToArray()
        {
            lock (SyncRoot)
            {
                return _queueElements.ToArray();
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncQueue{T}"/> class.
        /// </summary>
        public AsyncQueue()
        {
        }

        /// <summary>
        /// Signals that no further elements will be enqueued.
        /// </summary>
        public void Complete()
        {
            lock (SyncRoot)
            {
                _completeSignaled = true;
            }

            CompleteIfNecessary();
        }

        /// <summary>
        /// Adds an element to the tail of the queue.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Enqueue(T value)
        {
            if (!TryEnqueue(value))
            {
                throw new InvalidOperationException("Invalid after complete");
            }
        }

        /// <summary>
        /// Adds an element to the tail of the queue if it has not yet completed.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns><c>true</c> if the value was added to the queue; <c>false</c> if the queue is already completed.</returns>
        public bool TryEnqueue(T value)
        {
            bool alreadyDispatched = false;
            lock (SyncRoot)
            {
                if (_completeSignaled)
                {
                    return false;
                }

                // Is a dequeuer waiting for this?
                while (_dequeuingWaiters?.Count > 0)
                {
                    TaskCompletionSource<T> waitingDequeuer = _dequeuingWaiters.Dequeue();
                    if (waitingDequeuer.TrySetResult(value))
                    {
                        alreadyDispatched = true;
                        break;
                    }
                }

                FreeCanceledDequeuers();

                if (!alreadyDispatched)
                {
                    if (_queueElements == null)
                    {
                        _queueElements = new Queue<T>(InitialCapacity);
                    }

                    _queueElements.Enqueue(value);
                }
            }

            OnEnqueued(value, alreadyDispatched);

            return true;
        }

        /// <summary>
        /// Gets the value at the head of the queue without removing it from the queue, if it is non-empty.
        /// </summary>
        /// <param name="value">Receives the value at the head of the queue; or the default value for the element type if the queue is empty.</param>
        /// <returns><c>true</c> if the queue was non-empty; <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Gets the value at the head of the queue without removing it from the queue.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
        public T Peek()
        {
            if (!TryPeek(out T value))
            {
                throw new InvalidOperationException("Queue empty");
            }

            return value;
        }

        /// <summary>
        /// Gets a task whose result is the element at the head of the queue.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token whose cancellation signals lost interest in the item.
        /// Cancelling this token does *not* guarantee that the task will be canceled
        /// before it is assigned a resulting element from the head of the queue.
        /// It is the responsibility of the caller to ensure after cancellation that
        /// either the task is canceled, or it has a result which the caller is responsible
        /// for then handling.
        /// </param>
        /// <returns>A task whose result is the head element.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
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
                {
                    return TplExtensions.CanceledTaskOfT<T>();
                }

                if (_queueElements?.Count > 0)
                {
                    result = _queueElements.Dequeue();
                }
                else
                {
                    if (_dequeuingWaiters == null)
                    {
                        _dequeuingWaiters = new Queue<TaskCompletionSource<T>>(2);
                    }
                    else
                    {
                        FreeCanceledDequeuers();
                    }

                    var waiterTcs = new TaskCompletionSourceWithoutInlining<T>(false);
                    waiterTcs.AttachCancellation(cancellationToken, this);
                    _dequeuingWaiters.Enqueue(waiterTcs);
                    return waiterTcs.Task;
                }
            }

            CompleteIfNecessary();
            return Task.FromResult(result);
        }

        /// <summary>
        /// Immediately dequeues the element from the head of the queue if one is available,
        /// otherwise returns without an element.
        /// </summary>
        /// <param name="value">Receives the element from the head of the queue; or <c>default(T)</c> if the queue is empty.</param>
        /// <returns><c>true</c> if an element was dequeued; <c>false</c> if the queue was empty.</returns>
        public bool TryDequeue(out T value)
        {
            bool result = TryDequeueInternal(null, out value);
            CompleteIfNecessary();
            return result;
        }

        /// <inheritdoc />
        void ThreadingTools.ICancellationNotification.OnCanceled() => FreeCanceledDequeuers();

        /// <summary>
        /// Immediately dequeues the element from the head of the queue if one is available
        /// that satisfies the specified check;
        /// otherwise returns without an element.
        /// </summary>
        /// <param name="valueCheck">The test on the head element that must succeed to dequeue.</param>
        /// <param name="value">Receives the element from the head of the queue; or <c>default(T)</c> if the queue is empty.</param>
        /// <returns><c>true</c> if an element was dequeued; <c>false</c> if the queue was empty.</returns>
        protected bool TryDequeue(Predicate<T> valueCheck, out T value)
        {
            Validate.IsNotNull(valueCheck, nameof(valueCheck));

            bool result = TryDequeueInternal(valueCheck, out value);
            CompleteIfNecessary();
            return result;
        }

        /// <summary>
        /// Invoked when a value is enqueued.
        /// </summary>
        /// <param name="value">The enqueued value.</param>
        /// <param name="alreadyDispatched">
        /// <c>true</c> if the item will skip the queue because a dequeuer was already waiting for an item;
        /// <c>false</c> if the item was actually added to the queue.
        /// </param>
        protected virtual void OnEnqueued(T value, bool alreadyDispatched)
        {
        }

        /// <summary>
        /// Invoked when a value is dequeued.
        /// </summary>
        /// <param name="value">The dequeued value.</param>
        protected virtual void OnDequeued(T value)
        {
        }

        /// <summary>
        /// Invoked when the queue is completed.
        /// </summary>
        protected virtual void OnCompleted()
        {
        }

        private bool TryDequeueInternal(Predicate<T> valueCheck, out T value)
        {
            bool dequeued;
            lock (SyncRoot)
            {
                if (_queueElements != null && _queueElements.Count > 0 && (valueCheck == null || valueCheck(_queueElements.Peek())))
                {
                    value = _queueElements.Dequeue();
                    dequeued = true;
                }
                else
                {
                    value = default;
                    dequeued = false;
                }
            }

            if (dequeued)
            {
                OnDequeued(value);
            }

            return dequeued;
        }

        private void CompleteIfNecessary()
        {
            if (Monitor.IsEntered(SyncRoot))
                throw new Exception();

            bool transitionTaskSource, invokeOnCompleted = false;
            lock (SyncRoot)
            {
                transitionTaskSource = _completeSignaled && (_queueElements == null || _queueElements.Count == 0);
                if (transitionTaskSource)
                {
                    invokeOnCompleted = !_onCompletedInvoked;
                    _onCompletedInvoked = true;
                    while (_dequeuingWaiters?.Count > 0)
                    {
                        _dequeuingWaiters.Dequeue().TrySetCanceled();
                    }
                }
            }

            if (transitionTaskSource)
            {
                _completedSource?.TrySetResult(null);
                if (invokeOnCompleted)
                {
                    OnCompleted();
                }
            }
        }

        private void FreeCanceledDequeuers()
        {
            lock (SyncRoot)
            {
                while (_dequeuingWaiters?.Count > 0 && _dequeuingWaiters.Peek().Task.IsCompleted)
                {
                    _dequeuingWaiters.Dequeue();
                }
            }
        }
    }
}
