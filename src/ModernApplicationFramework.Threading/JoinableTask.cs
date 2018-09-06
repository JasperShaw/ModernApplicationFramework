using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    public class JoinableTask
    {
        private static readonly ThreadLocal<JoinableTask> CompletingTask = new ThreadLocal<JoinableTask>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly JoinableTaskFactory _owner;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly JoinableTaskCreationOptions _creationOptions;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ListOfOftenOne<JoinableTaskFactory> _nestingFactories;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ListOfOftenOne<JoinableTaskCollection> _collectionMembership;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Task _wrappedTask;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private WeakKeyDictionary<JoinableTask, int> _childOrJoinedJobs;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AsyncManualResetEvent _queueNeedProcessEvent;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private WeakReference<JoinableTask> _pendingEventSource;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _pendingEventCount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ExecutionQueue _mainThreadQueue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ExecutionQueue _threadPoolQueue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JoinableTaskFlags _state;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JoinableTaskSynchronizationContext _mainThreadJobSyncContext;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JoinableTaskSynchronizationContext _threadPoolJobSyncContext;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Delegate _initialDelegate;

        private WeakReference<JoinableTask> _weakSelf;

        private DependentSynchronousTask _dependingSynchronousTaskTracking;

        private Task QueueNeedProcessEvent
        {
            get
            {
                using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (_owner.Context.SyncContextLock)
                    {
                        if (_queueNeedProcessEvent == null)
                        {
                            // We pass in allowInliningWaiters: true,
                            // since we control all waiters and their continuations
                            // are benign, and it makes it more efficient.
                            _queueNeedProcessEvent = new AsyncManualResetEvent(allowInliningAwaiters: true);
                        }

                        return _queueNeedProcessEvent.WaitAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the async operation represented by this instance has completed.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (_owner.Context.SyncContextLock)
                    {
                        if (!IsCompleteRequested)
                        {
                            return false;
                        }

                        if (_mainThreadQueue != null && !_mainThreadQueue.IsCompleted)
                        {
                            return false;
                        }

                        if (_threadPoolQueue != null && !_threadPoolQueue.IsCompleted)
                        {
                            return false;
                        }

                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the asynchronous task that completes when the async operation completes.
        /// </summary>
        public Task Task
        {
            get
            {
                using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (_owner.Context.SyncContextLock)
                    {
                        Validate.IsNotNull(_wrappedTask, "wrappedTask");
                        return _wrappedTask;
                    }
                }
            }
        }

        internal ListOfOftenOne<JoinableTaskFactory> NestingFactories
        {
            get => _nestingFactories;
            set => _nestingFactories = value;
        }

        internal static JoinableTask TaskCompletingOnThisThread => CompletingTask.Value;

        internal JoinableTaskFactory Factory => _owner;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal SynchronizationContext ApplicableJobSyncContext
        {
            get
            {
                using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (_owner.Context.SyncContextLock)
                    {
                        if (Factory.Context.IsOnMainThread)
                        {
                            return _mainThreadJobSyncContext ?? (_mainThreadJobSyncContext =
                                       new JoinableTaskSynchronizationContext(this, true));
                        }

                        if (SynchronouslyBlockingThreadPool)
                        {
                            return _threadPoolJobSyncContext ?? (_threadPoolJobSyncContext =
                                       new JoinableTaskSynchronizationContext(this, false));
                        }

                        return null;
                    }
                }
            }
        }

        internal WeakReference<JoinableTask> WeakSelf => _weakSelf ?? (_weakSelf = new WeakReference<JoinableTask>(this));

        internal JoinableTaskFlags State => _state;

        internal JoinableTaskCreationOptions CreationOptions => _creationOptions;

        internal MethodInfo EntryMethodInfo => _initialDelegate?.GetMethodInfo();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal bool HasNonEmptyQueue
        {
            get
            {
                if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                    throw new Exception();
                return (_mainThreadQueue != null && _mainThreadQueue.Count > 0)
                       || (_threadPoolQueue != null && _threadPoolQueue.Count > 0);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IEnumerable<JoinableTask> ChildOrJoinedJobs
        {
            get
            {
                if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                    throw new Exception();
                if (_childOrJoinedJobs == null)
                {
                    return Enumerable.Empty<JoinableTask>();
                }

                return _childOrJoinedJobs.Select(p => p.Key).ToArray();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IEnumerable<JoinableTaskFactory.SingleExecuteProtector> MainThreadQueueContents
        {
            get
            {
                if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                    throw new Exception();
                if (_mainThreadQueue == null)
                {
                    return Enumerable.Empty<JoinableTaskFactory.SingleExecuteProtector>();
                }

                return _mainThreadQueue.ToArray();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IEnumerable<JoinableTaskFactory.SingleExecuteProtector> ThreadPoolQueueContents
        {
            get
            {
                if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                    throw new Exception();
                if (_threadPoolQueue == null)
                {
                    return Enumerable.Empty<JoinableTaskFactory.SingleExecuteProtector>();
                }

                return _threadPoolQueue.ToArray();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IEnumerable<JoinableTaskCollection> ContainingCollections
        {
            get
            {
                if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                    throw new Exception();
                return _collectionMembership.ToArray();
            }
        }

        internal bool HasMainThreadSynchronousTaskWaiting
        {
            get
            {
                using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (_owner.Context.SyncContextLock)
                    {
                        DependentSynchronousTask existingTaskTracking = _dependingSynchronousTaskTracking;
                        while (existingTaskTracking != null)
                        {
                            if ((existingTaskTracking.SynchronousTask.State & JoinableTaskFlags.SynchronouslyBlockingMainThread) == JoinableTaskFlags.SynchronouslyBlockingMainThread)
                            {
                                return true;
                            }

                            existingTaskTracking = existingTaskTracking.Next;
                        }

                        return false;
                    }
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool IsCompleteRequested
        {
            get => (_state & JoinableTaskFlags.CompleteRequested) != 0;

            set
            {
                if (!value)
                    throw new Exception();
                _state |= JoinableTaskFlags.CompleteRequested;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool SynchronouslyBlockingThreadPool => (_state & JoinableTaskFlags.StartedSynchronously) == JoinableTaskFlags.StartedSynchronously
                                                        && (_state & JoinableTaskFlags.StartedOnMainThread) != JoinableTaskFlags.StartedOnMainThread
                                                        && (_state & JoinableTaskFlags.CompleteRequested) != JoinableTaskFlags.CompleteRequested;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool SynchronouslyBlockingMainThread => (_state & JoinableTaskFlags.StartedSynchronously) == JoinableTaskFlags.StartedSynchronously
                                                        && (_state & JoinableTaskFlags.StartedOnMainThread) == JoinableTaskFlags.StartedOnMainThread
                                                        && (_state & JoinableTaskFlags.CompleteRequested) != JoinableTaskFlags.CompleteRequested;

        internal static bool AwaitShouldCaptureSyncContext => SynchronizationContext.Current is JoinableTaskSynchronizationContext;

        internal JoinableTask(JoinableTaskFactory owner, bool synchronouslyBlocking,
            JoinableTaskCreationOptions creationOptions, Delegate initialDelegate)
        {
            Validate.IsNotNull(owner, nameof(owner));

            _owner = owner;
            if (synchronouslyBlocking)
            {
                _state |= JoinableTaskFlags.StartedSynchronously | JoinableTaskFlags.CompletingSynchronously;
            }

            if (owner.Context.IsOnMainThread)
            {
                _state |= JoinableTaskFlags.StartedOnMainThread;
                if (synchronouslyBlocking)
                {
                    _state |= JoinableTaskFlags.SynchronouslyBlockingMainThread;
                }
            }

            _creationOptions = creationOptions;
            _owner.Context.OnJoinableTaskStarted(this);
            _initialDelegate = initialDelegate;
        }

        /// <summary>
        /// Synchronously blocks the calling thread until the operation has completed.
        /// If the caller is on the Main thread (or is executing within a JoinableTask that has access to the main thread)
        /// the caller's access to the Main thread propagates to this JoinableTask so that it may also access the main thread.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that will exit this method before the task is completed.</param>
        public void Join(CancellationToken cancellationToken = default)
        {
            _owner.Run(
                () => JoinAsync(cancellationToken),
                JoinableTaskCreationOptions.None,
                _initialDelegate);
        }

        /// <summary>
        /// Shares any access to the main thread the caller may have
        /// Joins any main thread affinity of the caller with the asynchronous operation to avoid deadlocks
        /// in the event that the main thread ultimately synchronously blocks waiting for the operation to complete.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token that will revert the Join and cause the returned task to complete
        /// before the async operation has completed.
        /// </param>
        /// <returns>A task that completes after the asynchronous operation completes and the join is reverted.</returns>
        public async Task JoinAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (AmbientJobJoinsThis())
            {
                await Task.WithCancellation(AwaitShouldCaptureSyncContext, cancellationToken).ConfigureAwait(AwaitShouldCaptureSyncContext);
            }
        }

        internal void Post(SendOrPostCallback d, object state, bool mainThreadAffinitized)
        {
            using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
            {
                JoinableTaskFactory.SingleExecuteProtector wrapper = null;
                List<AsyncManualResetEvent> eventsNeedNotify = null;
                bool postToFactory = false;

                bool isCompleteRequested;
                bool synchronouslyBlockingMainThread;
                lock (_owner.Context.SyncContextLock)
                {
                    isCompleteRequested = IsCompleteRequested;
                    synchronouslyBlockingMainThread = SynchronouslyBlockingMainThread;
                }

                if (isCompleteRequested)
                    postToFactory = true;
                else
                {
                    bool mainThreadQueueUpdated = false;
                    bool backgroundThreadQueueUpdated = false;
                    wrapper = JoinableTaskFactory.SingleExecuteProtector.Create(this, d, state);

                    if (ThreadingEventSource.Instance.IsEnabled())
                    {
                        ThreadingEventSource.Instance.PostExecutionStart(wrapper.GetHashCode(), mainThreadAffinitized);
                    }

                    if (mainThreadAffinitized && !synchronouslyBlockingMainThread)
                    {
                        wrapper.RaiseTransitioningEvents();
                    }

                    lock (_owner.Context.SyncContextLock)
                    {
                        if (mainThreadAffinitized)
                        {
                            if (_mainThreadQueue == null)
                                _mainThreadQueue = new ExecutionQueue(this);
                            _mainThreadQueue.TryEnqueue(wrapper);
                            mainThreadQueueUpdated = true;
                        }
                        else
                        {
                            if (SynchronouslyBlockingThreadPool)
                            {
                                if (_threadPoolQueue == null)
                                {
                                    _threadPoolQueue = new ExecutionQueue(this);
                                }

                                backgroundThreadQueueUpdated = _threadPoolQueue.TryEnqueue(wrapper);
                                if (!backgroundThreadQueueUpdated)
                                {
                                    ThreadPool.QueueUserWorkItem(JoinableTaskFactory.SingleExecuteProtector.ExecuteOnceWaitCallback, wrapper);
                                }
                            }
                            else
                            {
                                ThreadPool.QueueUserWorkItem(JoinableTaskFactory.SingleExecuteProtector.ExecuteOnceWaitCallback, wrapper);
                            }
                        }

                        if (mainThreadQueueUpdated || backgroundThreadQueueUpdated)
                        {
                            var tasksNeedNotify = GetDependingSynchronousTasks(mainThreadQueueUpdated);
                            if (tasksNeedNotify.Count > 0)
                            {
                                eventsNeedNotify = new List<AsyncManualResetEvent>(tasksNeedNotify.Count);
                                foreach (var taskToNotify in tasksNeedNotify)
                                {
                                    if (taskToNotify._pendingEventSource == null || taskToNotify == this)
                                    {
                                        taskToNotify._pendingEventSource = WeakSelf;
                                    }

                                    taskToNotify._pendingEventCount++;
                                    if (taskToNotify._queueNeedProcessEvent != null)
                                    {
                                        eventsNeedNotify.Add(taskToNotify._queueNeedProcessEvent);
                                    }
                                }
                            }
                        }
                    }
                }
                if (eventsNeedNotify != null)
                {
                    foreach (var queueEvent in eventsNeedNotify)
                    {
                        queueEvent.PulseAll();
                    }
                }
                if (postToFactory)
                {
                    Factory.Post(d, state, mainThreadAffinitized);
                }
                else if (mainThreadAffinitized)
                {
                    if (wrapper == null)
                        throw new Exception();
                    _owner.PostToUnderlyingSynchronizationContextOrThreadPool(wrapper);

                    foreach (var nestingFactory in _nestingFactories)
                    {
                        if (nestingFactory != _owner)
                        {
                            nestingFactory.PostToUnderlyingSynchronizationContextOrThreadPool(wrapper);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets an awaiter that is equivalent to calling <see cref="JoinAsync"/>.
        /// </summary>
        /// <returns>A task whose result is the result of the asynchronous operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public TaskAwaiter GetAwaiter()
        {
            return JoinAsync().GetAwaiter();
        }

        internal void SetWrappedTask(Task wrappedTask)
        {
            Validate.IsNotNull(wrappedTask, nameof(wrappedTask));

            using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
            {
                lock (_owner.Context.SyncContextLock)
                {
                    if (_wrappedTask != null)
                        throw new Exception();
                    _wrappedTask = wrappedTask;

                    if (wrappedTask.IsCompleted)
                    {
                        Complete();
                    }
                    else
                    {
                        _wrappedTask.ContinueWith(
                            (t, s) => ((JoinableTask)s).Complete(),
                            this,
                            CancellationToken.None,
                            TaskContinuationOptions.ExecuteSynchronously,
                            TaskScheduler.Default);
                    }
                }
            }
        }

        internal void Complete()
        {
            using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
            {
                AsyncManualResetEvent queueNeedProcessEvent = null;
                lock (_owner.Context.SyncContextLock)
                {
                    if (!IsCompleteRequested)
                    {
                        IsCompleteRequested = true;

                        _mainThreadQueue?.Complete();

                        _threadPoolQueue?.Complete();

                        OnQueueCompleted();
                        queueNeedProcessEvent = _queueNeedProcessEvent;

                        CleanupDependingSynchronousTask();
                    }
                }

                queueNeedProcessEvent?.PulseAll();
            }
        }

        internal void RemoveDependency(JoinableTask joinChild)
        {
            Validate.IsNotNull(joinChild, nameof(joinChild));

            using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
            {
                lock (_owner.Context.SyncContextLock)
                {
                    if (_childOrJoinedJobs != null && _childOrJoinedJobs.TryGetValue(joinChild, out int refCount))
                    {
                        if (refCount == 1)
                        {
                            _childOrJoinedJobs.Remove(joinChild);
                            RemoveDependingSynchronousTaskFromChild(joinChild);
                        }
                        else
                        {
                            _childOrJoinedJobs[joinChild] = --refCount;
                        }
                    }
                }
            }
        }

        internal void AddSelfAndDescendentOrJoinedJobs(HashSet<JoinableTask> joinables)
        {
            Validate.IsNotNull(joinables, nameof(joinables));

            if (!IsCompleted)
            {
                if (joinables.Add(this))
                {
                    if (_childOrJoinedJobs != null)
                    {
                        foreach (var item in _childOrJoinedJobs)
                        {
                            item.Key.AddSelfAndDescendentOrJoinedJobs(joinables);
                        }
                    }
                }
            }
        }

        internal void CompleteOnCurrentThread()
        {
            Validate.IsNotNull(_wrappedTask, "wrappedTask");
            JoinableTask priorCompletingTask = CompletingTask.Value;
            CompletingTask.Value = this;
            try
            {
                bool onMainThread = false;
                var additionalFlags = JoinableTaskFlags.CompletingSynchronously;
                if (_owner.Context.IsOnMainThread)
                {
                    additionalFlags |= JoinableTaskFlags.SynchronouslyBlockingMainThread;
                    onMainThread = true;
                }

                AddStateFlags(additionalFlags);

                if (!IsCompleteRequested)
                {
                    if (ThreadingEventSource.Instance.IsEnabled())
                    {
                        ThreadingEventSource.Instance.CompleteOnCurrentThreadStart(GetHashCode(), onMainThread);
                    }

                    using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
                    {
                        lock (_owner.Context.SyncContextLock)
                        {
                            _pendingEventCount = 0;
                            _pendingEventSource = AddDependingSynchronousTask(this, ref _pendingEventCount)?.WeakSelf;
                        }
                    }

                    if (onMainThread)
                    {
                        _owner.Context.OnSynchronousJoinableTaskToCompleteOnMainThread(this);
                    }

                    try
                    {
                        HashSet<JoinableTask> visited = null;
                        while (!IsCompleteRequested)
                        {
                            if (TryDequeueSelfOrDependencies(onMainThread, ref visited, out JoinableTaskFactory.SingleExecuteProtector work, out Task tryAgainAfter))
                            {
                                work.TryExecute();
                            }
                            else if (tryAgainAfter != null)
                            {
                                ThreadingEventSource.Instance.WaitSynchronouslyStart();
                                _owner.WaitSynchronously(tryAgainAfter);
                                ThreadingEventSource.Instance.WaitSynchronouslyStop();
                                if (!(tryAgainAfter.IsCompleted))
                                    throw new Exception();
                            }
                        }
                    }
                    finally
                    {
                        using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
                        {
                            lock (_owner.Context.SyncContextLock)
                            {
                                RemoveDependingSynchronousTask(this, true);
                            }
                        }
                    }

                    if (ThreadingEventSource.Instance.IsEnabled())
                    {
                        ThreadingEventSource.Instance.CompleteOnCurrentThreadStop(GetHashCode());
                    }
                }
                else
                {
                    if (onMainThread)
                    {
                        _owner.Context.OnSynchronousJoinableTaskToCompleteOnMainThread(this);
                    }
                }
                if (_threadPoolQueue?.Count > 0)
                {
                    while (_threadPoolQueue.TryDequeue(out JoinableTaskFactory.SingleExecuteProtector executor))
                    {
                        ThreadPool.QueueUserWorkItem(JoinableTaskFactory.SingleExecuteProtector.ExecuteOnceWaitCallback, executor);
                    }
                }

                if (!(Task.IsCompleted))
                    throw new Exception();
                Task.GetAwaiter().GetResult();
            }
            finally
            {
                CompletingTask.Value = priorCompletingTask;
            }
        }

        internal void OnQueueCompleted()
        {
            if (IsCompleted)
            {
                // Note this code may execute more than once, as multiple queue completion
                // notifications come in.
                _owner.Context.OnJoinableTaskCompleted(this);

                foreach (var collection in _collectionMembership)
                {
                    collection.Remove(this);
                }

                _mainThreadJobSyncContext?.OnCompleted();

                _threadPoolJobSyncContext?.OnCompleted();

                _nestingFactories = default;
                _initialDelegate = null;
                _state |= JoinableTaskFlags.CompleteFinalized;
            }
        }

        internal void OnAddedToCollection(JoinableTaskCollection collection)
        {
            Validate.IsNotNull(collection, nameof(collection));
            _collectionMembership.Add(collection);
        }

        internal void OnRemovedFromCollection(JoinableTaskCollection collection)
        {
            Validate.IsNotNull(collection, nameof(collection));
            _collectionMembership.Remove(collection);
        }

        private void AddStateFlags(JoinableTaskFlags flags)
        {
            // Try to avoid taking a lock if the flags are already set appropriately.
            if ((_state & flags) != flags)
            {
                using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (_owner.Context.SyncContextLock)
                    {
                        _state |= flags;
                    }
                }
            }
        }

        private bool TryDequeueSelfOrDependencies(bool onMainThread, ref HashSet<JoinableTask> visited, out JoinableTaskFactory.SingleExecuteProtector work, out Task tryAgainAfter)
        {
            using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
            {
                lock (_owner.Context.SyncContextLock)
                {
                    if (IsCompleted)
                    {
                        work = null;
                        tryAgainAfter = null;
                        return false;
                    }

                    if (_pendingEventCount > 0)
                    {
                        _pendingEventCount--;

                        if (_pendingEventSource != null)
                        {
                            if (_pendingEventSource.TryGetTarget(out JoinableTask pendingSource) && pendingSource.IsDependingSynchronousTask(this))
                            {
                                var queue = onMainThread ? pendingSource._mainThreadQueue : pendingSource._threadPoolQueue;
                                if (queue != null && !queue.IsCompleted && queue.TryDequeue(out work))
                                {
                                    if (queue.Count == 0)
                                    {
                                        _pendingEventSource = null;
                                    }

                                    tryAgainAfter = null;
                                    return true;
                                }
                            }

                            _pendingEventSource = null;
                        }

                        if (visited == null)
                        {
                            visited = new HashSet<JoinableTask>();
                        }
                        else
                        {
                            visited.Clear();
                        }

                        if (TryDequeueSelfOrDependencies(onMainThread, visited, out work))
                        {
                            tryAgainAfter = null;
                            return true;
                        }
                    }

                    _pendingEventCount = 0;

                    work = null;
                    tryAgainAfter = IsCompleteRequested ? null : QueueNeedProcessEvent;
                    return false;
                }
            }
        }

        private bool TryDequeueSelfOrDependencies(bool onMainThread, HashSet<JoinableTask> visited, out JoinableTaskFactory.SingleExecuteProtector work)
        {
            Validate.IsNotNull(visited, nameof(visited));

            work = null;
            if (visited.Add(this))
            {
                var queue = onMainThread ? _mainThreadQueue : _threadPoolQueue;
                if (queue != null && !queue.IsCompleted)
                {
                    queue.TryDequeue(out work);
                }

                if (work == null)
                {
                    if (_childOrJoinedJobs != null && !IsCompleted)
                    {
                        foreach (var item in _childOrJoinedJobs)
                        {
                            if (item.Key.TryDequeueSelfOrDependencies(onMainThread, visited, out work))
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return work != null;
        }

        internal JoinableTaskCollection.JoinRelease AddDependency(JoinableTask joinChild)
        {
            Validate.IsNotNull(joinChild, nameof(joinChild));
            if (this == joinChild)
            {
                // Joining oneself would be pointless.
                return default;
            }

            using (Factory.Context.NoMessagePumpSynchronizationContext.Apply())
            {
                List<AsyncManualResetEvent> eventsNeedNotify = null;
                lock (_owner.Context.SyncContextLock)
                {
                    if (_childOrJoinedJobs == null)
                    {
                        _childOrJoinedJobs = new WeakKeyDictionary<JoinableTask, int>(capacity: 3);
                    }

                    _childOrJoinedJobs.TryGetValue(joinChild, out int refCount);
                    _childOrJoinedJobs[joinChild] = ++refCount;
                    if (refCount == 1)
                    {
                        // This constitutes a significant change, so we should apply synchronous task tracking to the new child.
                        var tasksNeedNotify = AddDependingSynchronousTaskToChild(joinChild);
                        if (tasksNeedNotify.Count > 0)
                        {
                            eventsNeedNotify = new List<AsyncManualResetEvent>(tasksNeedNotify.Count);
                            foreach (var taskToNotify in tasksNeedNotify)
                            {
                                if (taskToNotify.SynchronousTask._pendingEventSource == null || taskToNotify.TaskHasPendingMessages == taskToNotify.SynchronousTask)
                                {
                                    taskToNotify.SynchronousTask._pendingEventSource = taskToNotify.TaskHasPendingMessages?.WeakSelf;
                                }

                                taskToNotify.SynchronousTask._pendingEventCount += taskToNotify.NewPendingMessagesCount;

                                var notifyEvent = taskToNotify.SynchronousTask._queueNeedProcessEvent;
                                if (notifyEvent != null)
                                {
                                    eventsNeedNotify.Add(notifyEvent);
                                }
                            }
                        }
                    }
                }

                // We explicitly do this outside our lock.
                if (eventsNeedNotify != null)
                {
                    foreach (var queueEvent in eventsNeedNotify)
                    {
                        queueEvent.PulseAll();
                    }
                }

                return new JoinableTaskCollection.JoinRelease(this, joinChild);
            }
        }

        private JoinableTaskCollection.JoinRelease AmbientJobJoinsThis()
        {
            if (!IsCompleted)
            {
                var ambientJob = _owner.Context.AmbientTask;
                if (ambientJob != null && ambientJob != this)
                {
                    return ambientJob.AddDependency(this);
                }
            }

            return default;
        }

        private int CountOfDependingSynchronousTasks()
        {
            int count = 0;
            DependentSynchronousTask existingTaskTracking = _dependingSynchronousTaskTracking;
            while (existingTaskTracking != null)
            {
                count++;
                existingTaskTracking = existingTaskTracking.Next;
            }

            return count;
        }

        private bool IsDependingSynchronousTask(JoinableTask syncTask)
        {
            DependentSynchronousTask existingTaskTracking = _dependingSynchronousTaskTracking;
            while (existingTaskTracking != null)
            {
                if (existingTaskTracking.SynchronousTask == syncTask)
                {
                    return true;
                }

                existingTaskTracking = existingTaskTracking.Next;
            }

            return false;
        }

        private List<JoinableTask> GetDependingSynchronousTasks(bool forMainThread)
        {
            if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                throw new Exception();

            var tasksNeedNotify = new List<JoinableTask>(CountOfDependingSynchronousTasks());
            DependentSynchronousTask existingTaskTracking = _dependingSynchronousTaskTracking;
            while (existingTaskTracking != null)
            {
                var syncTask = existingTaskTracking.SynchronousTask;
                bool syncTaskInOnMainThread = (syncTask._state & JoinableTaskFlags.SynchronouslyBlockingMainThread) == JoinableTaskFlags.SynchronouslyBlockingMainThread;
                if (forMainThread == syncTaskInOnMainThread)
                {
                    // Only synchronous tasks are in the list, so we don't need do further check for the CompletingSynchronously flag
                    tasksNeedNotify.Add(syncTask);
                }

                existingTaskTracking = existingTaskTracking.Next;
            }

            return tasksNeedNotify;
        }

        private List<PendingNotification> AddDependingSynchronousTaskToChild(JoinableTask child)
        {
            Validate.IsNotNull(child, nameof(child));
            if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                throw new Exception();

            var tasksNeedNotify = new List<PendingNotification>(CountOfDependingSynchronousTasks());
            DependentSynchronousTask existingTaskTracking = _dependingSynchronousTaskTracking;
            while (existingTaskTracking != null)
            {
                int totalEventNumber = 0;
                var eventTriggeringTask = child.AddDependingSynchronousTask(existingTaskTracking.SynchronousTask, ref totalEventNumber);
                if (eventTriggeringTask != null)
                {
                    tasksNeedNotify.Add(new PendingNotification(existingTaskTracking.SynchronousTask, eventTriggeringTask, totalEventNumber));
                }

                existingTaskTracking = existingTaskTracking.Next;
            }

            return tasksNeedNotify;
        }

        private void RemoveDependingSynchronousTaskFromChild(JoinableTask child)
        {
            Validate.IsNotNull(child, nameof(child));
            if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                throw new Exception();

            DependentSynchronousTask existingTaskTracking = _dependingSynchronousTaskTracking;
            while (existingTaskTracking != null)
            {
                child.RemoveDependingSynchronousTask(existingTaskTracking.SynchronousTask);
                existingTaskTracking = existingTaskTracking.Next;
            }
        }

        private int GetPendingEventCountForTask(JoinableTask task)
        {
            var queue = (task._state & JoinableTaskFlags.SynchronouslyBlockingMainThread) == JoinableTaskFlags.SynchronouslyBlockingMainThread
                ? _mainThreadQueue
                : _threadPoolQueue;
            return queue?.Count ?? 0;
        }

        private JoinableTask AddDependingSynchronousTask(JoinableTask task, ref int totalEventsPending)
        {
            Validate.IsNotNull(task, nameof(task));
            if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                throw new Exception();

            if (IsCompleted)
            {
                return null;
            }

            if (IsCompleteRequested)
            {
                // A completed task might still have pending items in the queue.
                int pendingCount = GetPendingEventCountForTask(task);
                if (pendingCount > 0)
                {
                    totalEventsPending += pendingCount;
                    return this;
                }

                return null;
            }

            DependentSynchronousTask existingTaskTracking = _dependingSynchronousTaskTracking;
            while (existingTaskTracking != null)
            {
                if (existingTaskTracking.SynchronousTask == task)
                {
                    existingTaskTracking.ReferenceCount++;
                    return null;
                }

                existingTaskTracking = existingTaskTracking.Next;
            }

            int pendingItemCount = GetPendingEventCountForTask(task);
            JoinableTask eventTriggeringTask = null;

            if (pendingItemCount > 0)
            {
                totalEventsPending += pendingItemCount;
                eventTriggeringTask = this;
            }

            // For a new synchronous task, we need apply it to our child tasks.
            DependentSynchronousTask newTaskTracking = new DependentSynchronousTask(task)
            {
                Next = _dependingSynchronousTaskTracking,
            };
            _dependingSynchronousTaskTracking = newTaskTracking;

            if (_childOrJoinedJobs != null)
            {
                foreach (var item in _childOrJoinedJobs)
                {
                    var childTiggeringTask = item.Key.AddDependingSynchronousTask(task, ref totalEventsPending);
                    if (eventTriggeringTask == null)
                    {
                        eventTriggeringTask = childTiggeringTask;
                    }
                }
            }

            return eventTriggeringTask;
        }

        private void CleanupDependingSynchronousTask()
        {
            if (_dependingSynchronousTaskTracking != null)
            {
                DependentSynchronousTask existingTaskTracking = _dependingSynchronousTaskTracking;
                _dependingSynchronousTaskTracking = null;

                if (_childOrJoinedJobs != null)
                {
                    var childrenTasks = _childOrJoinedJobs.Select(item => item.Key).ToList();
                    while (existingTaskTracking != null)
                    {
                        RemoveDependingSynchronousTaskFrom(childrenTasks, existingTaskTracking.SynchronousTask, false);
                        existingTaskTracking = existingTaskTracking.Next;
                    }
                }
            }
        }

        private void RemoveDependingSynchronousTask(JoinableTask task, bool force = false)
        {
            Validate.IsNotNull(task, nameof(task));
            if (!(Monitor.IsEntered(_owner.Context.SyncContextLock)))
                throw new Exception();

            if (task._dependingSynchronousTaskTracking != null)
            {
                RemoveDependingSynchronousTaskFrom(new[] { this }, task, force);
            }
        }

        private static void RemoveDependingSynchronousTaskFrom(IReadOnlyList<JoinableTask> tasks, JoinableTask syncTask, bool force)
        {
            Validate.IsNotNull(tasks, nameof(tasks));
            Validate.IsNotNull(syncTask, nameof(syncTask));

            HashSet<JoinableTask> reachableTasks = null;
            HashSet<JoinableTask> remainTasks = null;

            if (force)
            {
                reachableTasks = new HashSet<JoinableTask>();
            }

            foreach (var task in tasks)
            {
                task.RemoveDependingSynchronousTask(syncTask, reachableTasks, ref remainTasks);
            }

            if (!force && remainTasks != null && remainTasks.Count > 0)
            {
                reachableTasks = new HashSet<JoinableTask>();
                syncTask.ComputeSelfAndDescendentOrJoinedJobsAndRemainTasks(reachableTasks, remainTasks);
                HashSet<JoinableTask> remainPlaceHold = null;
                foreach (var remainTask in remainTasks)
                {
                    remainTask.RemoveDependingSynchronousTask(syncTask, reachableTasks, ref remainPlaceHold);
                }
            }
        }

        private void ComputeSelfAndDescendentOrJoinedJobsAndRemainTasks(HashSet<JoinableTask> reachableTasks, HashSet<JoinableTask> remainTasks)
        {
            Validate.IsNotNull(remainTasks, nameof(remainTasks));
            Validate.IsNotNull(reachableTasks, nameof(reachableTasks));
            if (!IsCompleted)
            {
                if (reachableTasks.Add(this))
                {
                    if (remainTasks.Remove(this) && reachableTasks.Count == 0)
                    {
                        // no remain task left, quit the loop earlier
                        return;
                    }

                    if (_childOrJoinedJobs != null)
                    {
                        foreach (var item in _childOrJoinedJobs)
                        {
                            item.Key.ComputeSelfAndDescendentOrJoinedJobsAndRemainTasks(reachableTasks, remainTasks);
                        }
                    }
                }
            }
        }

        private void RemoveDependingSynchronousTask(JoinableTask task, HashSet<JoinableTask> reachableTasks, ref HashSet<JoinableTask> remainingDependentTasks)
        {
            Validate.IsNotNull(task, nameof(task));

            DependentSynchronousTask previousTaskTracking = null;
            DependentSynchronousTask currentTaskTracking = _dependingSynchronousTaskTracking;
            bool removed = false;

            while (currentTaskTracking != null)
            {
                if (currentTaskTracking.SynchronousTask == task)
                {
                    if (--currentTaskTracking.ReferenceCount > 0)
                    {
                        if (reachableTasks != null)
                        {
                            if (!reachableTasks.Contains(this))
                            {
                                currentTaskTracking.ReferenceCount = 0;
                            }
                        }
                    }

                    if (currentTaskTracking.ReferenceCount == 0)
                    {
                        removed = true;
                        if (previousTaskTracking != null)
                        {
                            previousTaskTracking.Next = currentTaskTracking.Next;
                        }
                        else
                        {
                            _dependingSynchronousTaskTracking = currentTaskTracking.Next;
                        }
                    }

                    if (reachableTasks == null)
                    {
                        if (removed)
                        {
                            remainingDependentTasks?.Remove(this);
                        }
                        else
                        {
                            if (remainingDependentTasks == null)
                            {
                                remainingDependentTasks = new HashSet<JoinableTask>();
                            }

                            remainingDependentTasks.Add(this);
                        }
                    }

                    break;
                }

                previousTaskTracking = currentTaskTracking;
                currentTaskTracking = currentTaskTracking.Next;
            }

            if (removed && _childOrJoinedJobs != null)
            {
                foreach (var item in _childOrJoinedJobs)
                {
                    item.Key.RemoveDependingSynchronousTask(task, reachableTasks, ref remainingDependentTasks);
                }
            }
        }

        private struct PendingNotification
        {
            public PendingNotification(JoinableTask synchronousTask, JoinableTask taskHasPendingMessages, int newPendingMessagesCount)
            {
                Validate.IsNotNull(synchronousTask, nameof(synchronousTask));
                Validate.IsNotNull(taskHasPendingMessages, nameof(taskHasPendingMessages));

                SynchronousTask = synchronousTask;
                TaskHasPendingMessages = taskHasPendingMessages;
                NewPendingMessagesCount = newPendingMessagesCount;
            }

            public JoinableTask SynchronousTask { get; }

            public JoinableTask TaskHasPendingMessages { get; }

            public int NewPendingMessagesCount { get; }
        }


        [Flags]
        internal enum JoinableTaskFlags
        {
            None = 0x0,
            StartedSynchronously = 0x1,
            StartedOnMainThread = 0x2,
            CompleteRequested = 0x4,
            CompleteFinalized = 0x8,
            CompletingSynchronously = 0x10,
            SynchronouslyBlockingMainThread = 0x20
        }

        private class DependentSynchronousTask
        {
            public DependentSynchronousTask(JoinableTask task)
            {
                SynchronousTask = task;
                ReferenceCount = 1;
            }

            internal DependentSynchronousTask Next { get; set; }

            internal JoinableTask SynchronousTask { get; }

            internal int ReferenceCount { get; set; }
        }
    }
}