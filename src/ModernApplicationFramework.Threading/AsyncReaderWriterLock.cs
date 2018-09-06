using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    /// <summary>
    /// A non-blocking lock that allows concurrent access, exclusive access, or concurrent with upgradeability to exclusive access.
    /// </summary>
    /// <remarks>
    /// We have to use a custom awaitable rather than simply returning Task{LockReleaser} because
    /// we have to set CallContext data in the context of the person receiving the lock,
    /// which requires that we get to execute code at the start of the continuation (whether we yield or not).
    /// </remarks>
    /// <devnotes>
    /// Considering this class to be a state machine, the states are:
    /// <![CDATA[
    ///    -------------
    ///    |           | <-----> READERS
    ///    |    IDLE   | <-----> UPGRADEABLE READER + READERS -----> UPGRADED WRITER --\
    ///    |  NO LOCKS |                             ^                                 |
    ///    |           |                             |--- RE-ENTER CONCURRENCY PREP <--/
    ///    |           | <-----> WRITER
    ///    -------------
    /// ]]>
    /// </devnotes>
    public partial class AsyncReaderWriterLock : IDisposable
    {
        private static readonly SynchronizationContext DefaultSynchronizationContext = new SynchronizationContext();
        private readonly AsyncLocal<Awaiter> _topAwaiter = new AsyncLocal<Awaiter>();
        private readonly HashSet<Awaiter> _issuedReadLocks = new HashSet<Awaiter>();
        private readonly HashSet<Awaiter> _issuedUpgradeableReadLocks = new HashSet<Awaiter>();
        private readonly HashSet<Awaiter> _issuedWriteLocks = new HashSet<Awaiter>();
        private readonly Queue<Awaiter> _waitingReaders = new Queue<Awaiter>();
        private readonly Queue<Awaiter> _waitingUpgradeableReaders = new Queue<Awaiter>();
        private readonly Queue<Awaiter> _waitingWriters = new Queue<Awaiter>();
        private readonly TaskCompletionSource<object> _completionSource = new TaskCompletionSource<object>();
        private readonly Queue<Func<Task>> _beforeWriteReleasedCallbacks = new Queue<Func<Task>>();
        private volatile Awaiter _reenterConcurrencyPrepRunning;
        private bool _completeInvoked;
        private readonly EventsHelper _etw;

        /// <summary>
        /// Gets a value indicating whether any kind of lock is held by the caller and can
        /// be immediately used given the caller's context.
        /// </summary>
        public bool IsAnyLockHeld => IsReadLockHeld || IsUpgradeableReadLockHeld || IsWriteLockHeld;

        /// <summary>
        /// Gets a value indicating whether any kind of lock is held by the caller without regard
        /// to the lock compatibility of the caller's context.
        /// </summary>
        public bool IsAnyPassiveLockHeld => IsPassiveReadLockHeld || IsPassiveUpgradeableReadLockHeld || IsPassiveWriteLockHeld;

        /// <summary>
        /// Gets a value indicating whether the caller holds a read lock.
        /// </summary>
        /// <remarks>
        /// This property returns <c>false</c> if any other lock type is held, unless
        /// within that alternate lock type this lock is also nested.
        /// </remarks>
        public bool IsReadLockHeld => IsLockHeld(LockKind.Read);

        /// <summary>
        /// Gets a value indicating whether a read lock is held by the caller without regard
        /// to the lock compatibility of the caller's context.
        /// </summary>
        /// <remarks>
        /// This property returns <c>false</c> if any other lock type is held, unless
        /// within that alternate lock type this lock is also nested.
        /// </remarks>
        public bool IsPassiveReadLockHeld => IsLockHeld(LockKind.Read, checkSyncContextCompatibility: false, allowNonLockSupportingContext: true);

        /// <summary>
        /// Gets a value indicating whether the caller holds an upgradeable read lock.
        /// </summary>
        /// <remarks>
        /// This property returns <c>false</c> if any other lock type is held, unless
        /// within that alternate lock type this lock is also nested.
        /// </remarks>
        public bool IsUpgradeableReadLockHeld => IsLockHeld(LockKind.UpgradeableRead);

        /// <summary>
        /// Gets a value indicating whether an upgradeable read lock is held by the caller without regard
        /// to the lock compatibility of the caller's context.
        /// </summary>
        /// <remarks>
        /// This property returns <c>false</c> if any other lock type is held, unless
        /// within that alternate lock type this lock is also nested.
        /// </remarks>
        public bool IsPassiveUpgradeableReadLockHeld => IsLockHeld(LockKind.UpgradeableRead, checkSyncContextCompatibility: false, allowNonLockSupportingContext: true);

        /// <summary>
        /// Gets a value indicating whether the caller holds a write lock.
        /// </summary>
        /// <remarks>
        /// This property returns <c>false</c> if any other lock type is held, unless
        /// within that alternate lock type this lock is also nested.
        /// </remarks>
        public bool IsWriteLockHeld => IsLockHeld(LockKind.Write);

        /// <summary>
        /// Gets a value indicating whether a write lock is held by the caller without regard
        /// to the lock compatibility of the caller's context.
        /// </summary>
        /// <remarks>
        /// This property returns <c>false</c> if any other lock type is held, unless
        /// within that alternate lock type this lock is also nested.
        /// </remarks>
        public bool IsPassiveWriteLockHeld => IsLockHeld(LockKind.Write, checkSyncContextCompatibility: false, allowNonLockSupportingContext: true);

        /// <summary>
        /// Gets a task whose completion signals that this lock will no longer issue locks.
        /// </summary>
        /// <remarks>
        /// This task only transitions to a complete state after a call to <see cref="Complete"/>.
        /// </remarks>
        public Task Completion => _completionSource.Task;

        /// <summary>
        /// Gets the object used to synchronize access to this instance's fields.
        /// </summary>
        protected object SyncObject { get; } = new object();

        /// <summary>
        /// Gets the lock held by the caller's execution context.
        /// </summary>
        protected LockHandle AmbientLock => new LockHandle(GetFirstActiveSelfOrAncestor(_topAwaiter.Value));

        /// <summary>
        /// Gets or sets a value indicating whether additional resources should be spent to collect
        /// information that would be useful in diagnosing deadlocks, etc.
        /// </summary>
        protected bool CaptureDiagnostics { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current thread is allowed to
        /// hold an active lock.
        /// </summary>
        /// <remarks>
        /// The default implementation of this property in builds of this
        /// assembly that target the .NET Framework is to return <c>true</c>
        /// when the calling thread is an MTA thread.
        /// On builds that target the portable profile, this property always
        /// returns <c>true</c> and should be overridden return <c>false</c>
        /// on threads that may compromise the integrity of the lock.
        /// </remarks>
        protected virtual bool CanCurrentThreadHoldActiveLock => Thread.CurrentThread.GetApartmentState() != ApartmentState.STA;

        /// <summary>
        /// Gets a value indicating whether the current SynchronizationContext is one that is not supported
        /// by this lock.
        /// </summary>
        protected virtual bool IsUnsupportedSynchronizationContext
        {
            get
            {
                var ctxt = SynchronizationContext.Current;
                var supported = ctxt == null || ctxt is NonConcurrentSynchronizationContext;
                return !supported;
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ModernApplicationFramework.Threading.AsyncReaderWriterLock" /> class.
        /// </summary>
        public AsyncReaderWriterLock()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncReaderWriterLock"/> class.
        /// </summary>
        /// <param name="captureDiagnostics">
        /// <c>true</c> to spend additional resources capturing diagnostic details that can be used
        /// to analyze deadlocks or other issues.</param>
        public AsyncReaderWriterLock(bool captureDiagnostics)
        {
            _etw = new EventsHelper(this);
            CaptureDiagnostics = captureDiagnostics;
        }

        /// <summary>
        /// Obtains a read lock, asynchronously awaiting for the lock if it is not immediately available.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token whose cancellation indicates lost interest in obtaining the lock.
        /// A canceled token does not release a lock that has already been issued.  But if the lock isn't immediately available,
        /// a canceled token will cause the code that is waiting for the lock to resume with an <see cref="OperationCanceledException"/>.
        /// </param>
        /// <returns>An awaitable object whose result is the lock releaser.</returns>
        public Awaitable ReadLockAsync(CancellationToken cancellationToken = default)
        {
            return new Awaitable(this, LockKind.Read, LockFlags.None, cancellationToken);
        }

        /// <summary>
        /// Obtains an upgradeable read lock, asynchronously awaiting for the lock if it is not immediately available.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token whose cancellation indicates lost interest in obtaining the lock.
        /// A canceled token does not release a lock that has already been issued.  But if the lock isn't immediately available,
        /// a canceled token will cause the code that is waiting for the lock to resume with an <see cref="OperationCanceledException"/>.
        /// </param>
        /// <returns>An awaitable object whose result is the lock releaser.</returns>
        public Awaitable UpgradeableReadLockAsync(CancellationToken cancellationToken = default)
        {
            return new Awaitable(this, LockKind.UpgradeableRead, LockFlags.None, cancellationToken);
        }

        /// <summary>
        /// Obtains a read lock, asynchronously awaiting for the lock if it is not immediately available.
        /// </summary>
        /// <param name="options">Modifications to normal lock behavior.</param>
        /// <param name="cancellationToken">
        /// A token whose cancellation indicates lost interest in obtaining the lock.
        /// A canceled token does not release a lock that has already been issued.  But if the lock isn't immediately available,
        /// a canceled token will cause the code that is waiting for the lock to resume with an <see cref="OperationCanceledException"/>.
        /// </param>
        /// <returns>An awaitable object whose result is the lock releaser.</returns>
        public Awaitable UpgradeableReadLockAsync(LockFlags options, CancellationToken cancellationToken = default)
        {
            return new Awaitable(this, LockKind.UpgradeableRead, options, cancellationToken);
        }

        /// <summary>
        /// Obtains a write lock, asynchronously awaiting for the lock if it is not immediately available.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token whose cancellation indicates lost interest in obtaining the lock.
        /// A canceled token does not release a lock that has already been issued.  But if the lock isn't immediately available,
        /// a canceled token will cause the code that is waiting for the lock to resume with an <see cref="OperationCanceledException"/>.
        /// </param>
        /// <returns>An awaitable object whose result is the lock releaser.</returns>
        public Awaitable WriteLockAsync(CancellationToken cancellationToken = default)
        {
            return new Awaitable(this, LockKind.Write, LockFlags.None, cancellationToken);
        }

        /// <summary>
        /// Obtains a write lock, asynchronously awaiting for the lock if it is not immediately available.
        /// </summary>
        /// <param name="options">Modifications to normal lock behavior.</param>
        /// <param name="cancellationToken">
        /// A token whose cancellation indicates lost interest in obtaining the lock.
        /// A canceled token does not release a lock that has already been issued.  But if the lock isn't immediately available,
        /// a canceled token will cause the code that is waiting for the lock to resume with an <see cref="OperationCanceledException"/>.
        /// </param>
        /// <returns>An awaitable object whose result is the lock releaser.</returns>
        public Awaitable WriteLockAsync(LockFlags options, CancellationToken cancellationToken = default)
        {
            return new Awaitable(this, LockKind.Write, options, cancellationToken);
        }

        /// <summary>
        /// Prevents use or visibility of the caller's lock(s) until the returned value is disposed.
        /// </summary>
        /// <returns>The value to dispose to restore lock visibility.</returns>
        /// <remarks>
        /// This can be used by a write lock holder that is about to fork execution to avoid
        /// two threads simultaneously believing they hold the exclusive write lock.
        /// The lock should be hidden just before kicking off the work and can be restored immediately
        /// after kicking off the work.
        /// </remarks>
        public Suppression HideLocks()
        {
            return new Suppression(this);
        }

        /// <summary>
        /// Causes new top-level lock requests to be rejected and the <see cref="Completion"/> task to transition
        /// to a completed state after any issued locks have been released.
        /// </summary>
        public void Complete()
        {
            lock (SyncObject)
            {
                _completeInvoked = true;
                CompleteIfAppropriate();
            }
        }

        /// <summary>
        /// Registers a callback to be invoked when the write lock held by the caller is
        /// about to be ultimately released (outermost write lock).
        /// </summary>
        /// <param name="action">
        /// The asynchronous delegate to invoke.
        /// Access to the write lock is provided throughout the asynchronous invocation.
        /// </param>
        /// <remarks>
        /// This supports some scenarios VC++ has where change event handlers need to inspect changes,
        /// or follow up with other changes to respond to earlier changes, at the conclusion of the lock.
        /// This method is safe to call from within a previously registered callback, in which case the
        /// registered callback will run when previously registered callbacks have completed execution.
        /// If the write lock is released to an upgradeable read lock, these callbacks are fired synchronously
        /// with respect to the writer who is releasing the lock.  Otherwise, the callbacks are invoked
        /// asynchronously with respect to the releasing thread.
        /// </remarks>
        public void OnBeforeWriteLockReleased(Func<Task> action)
        {
            Validate.IsNotNull(action, nameof(action));

            lock (SyncObject)
            {
                if (!IsWriteLockHeld)
                {
                    throw new InvalidOperationException();
                }

                _beforeWriteReleasedCallbacks.Enqueue(action);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes managed and unmanaged resources held by this instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> if <see cref="Dispose()"/> was called; <c>false</c> if the object is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Checks whether the aggregated flags from all locks in the lock stack satisfy the specified flag(s).
        /// </summary>
        /// <param name="flags">The flag(s) that must be specified for a <c>true</c> result.</param>
        /// <param name="handle">The head of the lock stack to consider.</param>
        /// <returns><c>true</c> if all the specified flags are found somewhere in the lock stack; <c>false</c> otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags")]
        protected bool LockStackContains(LockFlags flags, LockHandle handle)
        {
            var aggregateFlags = LockFlags.None;
            var awaiter = handle.Awaiter;
            if (awaiter != null)
            {
                lock (SyncObject)
                {
                    while (awaiter != null)
                    {
                        if (IsLockActive(awaiter, true, true))
                        {
                            aggregateFlags |= awaiter.Options;
                            if ((aggregateFlags & flags) == flags)
                            {
                                return true;
                            }
                        }

                        awaiter = awaiter.NestingLock;
                    }
                }
            }

            return (aggregateFlags & flags) == flags;
        }

        /// <summary>
        /// Returns the aggregate of the lock flags for all nested locks.
        /// </summary>
        /// <remarks>
        /// This is not redundant with <see cref="LockStackContains(LockFlags, LockHandle)"/> because that returns fast
        /// once the presence of certain flag(s) is determined, whereas this will aggregate all flags,
        /// some of which may be defined by derived types.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected LockFlags GetAggregateLockFlags()
        {
            var aggregateFlags = LockFlags.None;
            var awaiter = _topAwaiter.Value;
            if (awaiter != null)
            {
                lock (SyncObject)
                {
                    while (awaiter != null)
                    {
                        if (IsLockActive(awaiter, true, true))
                        {
                            aggregateFlags |= awaiter.Options;
                        }

                        awaiter = awaiter.NestingLock;
                    }
                }
            }

            return aggregateFlags;
        }

        /// <summary>
        /// Fired when any lock is being released.
        /// </summary>
        /// <param name="exclusiveLockRelease"><c>true</c> if the last write lock that the caller holds is being released; <c>false</c> otherwise.</param>
        /// <param name="releasingLock">The lock being released.</param>
        /// <returns>A task whose completion signals the conclusion of the asynchronous operation.</returns>
        protected virtual Task OnBeforeLockReleasedAsync(bool exclusiveLockRelease, LockHandle releasingLock)
        {
            if (exclusiveLockRelease)
            {
                return OnBeforeExclusiveLockReleasedAsync();
            }

            return TplExtensions.CompletedTask;
        }

        /// <summary>
        /// Fired when the last write lock is about to be released.
        /// </summary>
        /// <returns>A task whose completion signals the conclusion of the asynchronous operation.</returns>
        protected virtual Task OnBeforeExclusiveLockReleasedAsync()
        {
            lock (SyncObject)
            {
                if (!(_issuedWriteLocks.Count == 1 || GetType() != typeof(AsyncReaderWriterLock)))
                    throw new Exception();

                if (_beforeWriteReleasedCallbacks.Count > 0)
                {
                    return InvokeBeforeWriteLockReleaseHandlersAsync();
                }

                return TplExtensions.CompletedTask;
            }
        }

        /// <summary>
        /// Get the task scheduler to execute the continuation when the lock is acquired.
        ///  AsyncReaderWriterLock uses a special <see cref="SynchronizationContext"/> to handle execusive locks, and will ignore task scheduler provided, so this is only used in a read lock scenario.
        /// This method is called within the execution context to wait the read lock, so it can pick up <see cref="TaskScheduler"/> based on the current execution context.
        /// Note: the task scheduler is only used, when the lock is issued later.  If the lock is issued immediately when <see cref="CanCurrentThreadHoldActiveLock"/> returns true, it will be ignored.
        /// </summary>
        /// <returns>A task scheduler to schedule the continutation task when a lock is issued.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual TaskScheduler GetTaskSchedulerForReadLockRequest()
        {
            return TaskScheduler.Default;
        }

        private void ThrowIfUnsupportedThreadOrSyncContext()
        {
            if (!CanCurrentThreadHoldActiveLock)
            {
                throw new InvalidOperationException("Caller not allowed");
            }

            if (IsUnsupportedSynchronizationContext)
            {
                throw new InvalidOperationException("Context not allowed");
            }
        }

        private bool IsLockSupportingContext(Awaiter awaiter = null)
        {
            if (!CanCurrentThreadHoldActiveLock || IsUnsupportedSynchronizationContext)
            {
                return false;
            }

            awaiter = awaiter ?? _topAwaiter.Value;
            if (IsLockHeld(LockKind.Write, awaiter, allowNonLockSupportingContext: true, checkSyncContextCompatibility: false) ||
                IsLockHeld(LockKind.UpgradeableRead, awaiter, allowNonLockSupportingContext: true, checkSyncContextCompatibility: false))
            {
                if (!(SynchronizationContext.Current is NonConcurrentSynchronizationContext))
                {
                    // Upgradeable read and write locks *must* have the NonConcurrentSynchronizationContext applied.
                    return false;
                }
            }

            return true;
        }

        private void CompleteIfAppropriate()
        {
            if (!Monitor.IsEntered(SyncObject))
                throw new Exception();

            if (_completeInvoked &&
                !_completionSource.Task.IsCompleted &&
                _reenterConcurrencyPrepRunning == null &&
                _issuedReadLocks.Count == 0 && _issuedUpgradeableReadLocks.Count == 0 && _issuedWriteLocks.Count == 0 &&
                _waitingReaders.Count == 0 && _waitingUpgradeableReaders.Count == 0 && _waitingWriters.Count == 0)
            {
                Task.Run(delegate { _completionSource.TrySetResult(null); });
            }
        }

        private void AggregateLockStackKinds(Awaiter awaiter, out bool read, out bool upgradeableRead, out bool write)
        {
            read = false;
            upgradeableRead = false;
            write = false;

            if (awaiter != null)
            {
                lock (SyncObject)
                {
                    while (awaiter != null)
                    {
                        switch (awaiter.Kind)
                        {
                            case LockKind.Read:
                                read |= _issuedReadLocks.Contains(awaiter);
                                break;
                            case LockKind.UpgradeableRead:
                                upgradeableRead |= _issuedUpgradeableReadLocks.Contains(awaiter);
                                write |= IsStickyWriteUpgradedLock(awaiter);
                                break;
                            case LockKind.Write:
                                write |= _issuedWriteLocks.Contains(awaiter);
                                break;
                        }

                        if (read && upgradeableRead && write)
                        {
                            return;
                        }

                        awaiter = awaiter.NestingLock;
                    }
                }
            }
        }

        private bool AllHeldLocksAreByThisStack(Awaiter awaiter)
        {
            if (!(awaiter == null || !IsLockHeld(LockKind.Write, awaiter)))
                throw new Exception();
            lock (SyncObject)
            {
                if (awaiter != null)
                {
                    var locksMatched = 0;
                    while (awaiter != null)
                    {
                        if (GetActiveLockSet(awaiter.Kind).Contains(awaiter))
                        {
                            locksMatched++;
                        }

                        awaiter = awaiter.NestingLock;
                    }

                    return locksMatched == _issuedReadLocks.Count + _issuedUpgradeableReadLocks.Count + _issuedWriteLocks.Count;
                }

                return _issuedReadLocks.Count == 0 && _issuedUpgradeableReadLocks.Count == 0 && _issuedWriteLocks.Count == 0;
            }
        }

        private bool LockStackContains(LockKind kind, Awaiter awaiter)
        {
            if (awaiter != null)
            {
                lock (SyncObject)
                {
                    var lockSet = GetActiveLockSet(kind);
                    while (awaiter != null)
                    {
                        if (awaiter.Kind == kind && lockSet.Contains(awaiter))
                        {
                            return true;
                        }

                        if (kind == LockKind.Write && IsStickyWriteUpgradedLock(awaiter))
                        {
                            return true;
                        }

                        awaiter = awaiter.NestingLock;
                    }
                }
            }

            return false;
        }

        private bool IsStickyWriteUpgradedLock(Awaiter awaiter)
        {
            if (awaiter.Kind == LockKind.UpgradeableRead && (awaiter.Options & LockFlags.StickyWrite) == LockFlags.StickyWrite)
            {
                lock (SyncObject)
                {
                    return _issuedWriteLocks.Contains(awaiter);
                }
            }

            return false;
        }

        private bool IsLockHeld(LockKind kind, Awaiter awaiter = null, bool checkSyncContextCompatibility = true, bool allowNonLockSupportingContext = false)
        {
            if (allowNonLockSupportingContext || IsLockSupportingContext(awaiter))
            {
                lock (SyncObject)
                {
                    awaiter = awaiter ?? _topAwaiter.Value;
                    if (checkSyncContextCompatibility)
                    {
                        CheckSynchronizationContextAppropriateForLock(awaiter);
                    }

                    return LockStackContains(kind, awaiter);
                }
            }

            return false;
        }

        private bool IsLockActive(Awaiter awaiter, bool considerStaActive, bool checkSyncContextCompatibility = false)
        {
            Validate.IsNotNull(awaiter, nameof(awaiter));

            if (considerStaActive || IsLockSupportingContext(awaiter))
            {
                lock (SyncObject)
                {
                    var activeLock = GetActiveLockSet(awaiter.Kind).Contains(awaiter);
                    if (checkSyncContextCompatibility && activeLock)
                    {
                        CheckSynchronizationContextAppropriateForLock(awaiter);
                    }

                    return activeLock;
                }
            }

            return false;
        }

        private void CheckSynchronizationContextAppropriateForLock(Awaiter awaiter)
        {
        }


        private bool TryIssueLock(Awaiter awaiter, bool previouslyQueued)
        {
            lock (SyncObject)
            {
                if (_completeInvoked && !previouslyQueued)
                {
                    if (awaiter.NestingLock == null)
                    {
                        awaiter.SetFault(new InvalidOperationException());
                        return false;
                    }
                }

                var issued = false;
                if (_reenterConcurrencyPrepRunning == null)
                {
                    if (_issuedWriteLocks.Count == 0 && _issuedUpgradeableReadLocks.Count == 0 && _issuedReadLocks.Count == 0)
                    {
                        issued = true;
                    }
                    else
                    {
                        AggregateLockStackKinds(awaiter, out var hasRead, out var hasUpgradeableRead, out var hasWrite);
                        switch (awaiter.Kind)
                        {
                            case LockKind.Read:
                                if (_issuedWriteLocks.Count == 0 && _waitingWriters.Count == 0)
                                {
                                    issued = true;
                                }
                                else if (hasWrite)
                                {
                                    if (CanCurrentThreadHoldActiveLock && !(SynchronizationContext.Current is NonConcurrentSynchronizationContext))
                                    {
                                        throw new InvalidOperationException("Dangerous Read Lock Request From Write Lock Fork");
                                    }

                                    issued = true;
                                }
                                else if (hasRead || hasUpgradeableRead)
                                {
                                    issued = true;
                                }

                                break;
                            case LockKind.UpgradeableRead:
                                if (hasUpgradeableRead || hasWrite)
                                {
                                    issued = true;
                                }
                                else if (hasRead)
                                {
                                    throw new InvalidOperationException();
                                }
                                else if (_issuedUpgradeableReadLocks.Count == 0 && _issuedWriteLocks.Count == 0)
                                {
                                    issued = true;
                                }

                                break;
                            case LockKind.Write:
                                if (hasWrite)
                                {
                                    issued = true;
                                }
                                else if (hasRead && !hasUpgradeableRead)
                                {
                                    throw new InvalidOperationException();
                                }
                                else if (AllHeldLocksAreByThisStack(awaiter.NestingLock))
                                {
                                    issued = true;

                                    var stickyWriteAwaiter = FindRootUpgradeableReadWithStickyWrite(awaiter);
                                    if (stickyWriteAwaiter != null)
                                    {
                                        _issuedWriteLocks.Add(stickyWriteAwaiter);
                                    }
                                }

                                break;
                            default:
                                throw new Exception();
                        }
                    }
                }

                if (issued)
                {
                    GetActiveLockSet(awaiter.Kind).Add(awaiter);
                    _etw.Issued(awaiter);
                }

                if (!issued)
                {
                    _etw.WaitStart(awaiter);
                    Debugger.NotifyOfCrossThreadDependency();
                }

                return issued;
            }
        }

        private Awaiter FindRootUpgradeableReadWithStickyWrite(Awaiter headAwaiter)
        {
            if (headAwaiter == null)
            {
                return null;
            }

            var lowerMatch = FindRootUpgradeableReadWithStickyWrite(headAwaiter.NestingLock);
            if (lowerMatch != null)
            {
                return lowerMatch;
            }

            if (headAwaiter.Kind == LockKind.UpgradeableRead && (headAwaiter.Options & LockFlags.StickyWrite) == LockFlags.StickyWrite)
            {
                lock (SyncObject)
                {
                    if (_issuedUpgradeableReadLocks.Contains(headAwaiter))
                    {
                        return headAwaiter;
                    }
                }
            }

            return null;
        }

        private HashSet<Awaiter> GetActiveLockSet(LockKind kind)
        {
            switch (kind)
            {
                case LockKind.Read:
                    return _issuedReadLocks;
                case LockKind.UpgradeableRead:
                    return _issuedUpgradeableReadLocks;
                case LockKind.Write:
                    return _issuedWriteLocks;
                default:
                    throw new Exception();
            }
        }

        private Queue<Awaiter> GetLockQueue(LockKind kind)
        {
            switch (kind)
            {
                case LockKind.Read:
                    return _waitingReaders;
                case LockKind.UpgradeableRead:
                    return _waitingUpgradeableReaders;
                case LockKind.Write:
                    return _waitingWriters;
                default:
                    throw new Exception();
            }
        }

        private Awaiter GetFirstActiveSelfOrAncestor(Awaiter awaiter)
        {
            while (awaiter != null)
            {
                if (IsLockActive(awaiter, true))
                {
                    break;
                }

                awaiter = awaiter.NestingLock;
            }

            return awaiter;
        }

        private void IssueAndExecute(Awaiter awaiter)
        {
            EventsHelper.WaitStop(awaiter);
            if (!TryIssueLock(awaiter, true))
                throw new Exception();
            if (!ExecuteOrHandleCancellation(awaiter, false))
                throw new Exception();
        }

        /// <summary>
        /// Invoked after an exclusive lock is released but before anyone has a chance to enter the lock.
        /// </summary>
        /// <remarks>
        /// This method is called while holding a private lock in order to block future lock consumers till this method is finished.
        /// </remarks>
        protected virtual Task OnExclusiveLockReleasedAsync()
        {
            return TplExtensions.CompletedTask;
        }

        /// <summary>
        /// Invoked when a top-level upgradeable read lock is released, leaving no remaining (write) lock.
        /// </summary>
        protected virtual void OnUpgradeableReadLockReleased()
        {
        }

        /// <summary>
        /// Invoked when the lock detects an internal error or illegal usage pattern that
        /// indicates a serious flaw that should be immediately reported to the application
        /// and/or bring down the process to avoid hangs or data corruption.
        /// </summary>
        /// <param name="ex">The exception that captures the details of the failure.</param>
        /// <returns>An exception that may be returned by some implementations of tis method for he caller to rethrow.</returns>
        protected virtual Exception OnCriticalFailure(Exception ex)
        {
            Validate.IsNotNull(ex, nameof(ex));
            Environment.FailFast(ex.ToString(), ex);
            throw new Exception();
        }

        /// <summary>
        /// Invoked when the lock detects an internal error or illegal usage pattern that
        /// indicates a serious flaw that should be immediately reported to the application
        /// and/or bring down the process to avoid hangs or data corruption.
        /// </summary>
        /// <param name="message">The message to use for the exception.</param>
        /// <returns>An exception that may be returned by some implementations of tis method for he caller to rethrow.</returns>
        protected Exception OnCriticalFailure(string message)
        {
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                throw OnCriticalFailure(ex);
            }
        }

        private static bool HasAnyNestedLocks(Awaiter lck, HashSet<Awaiter> lockCollection)
        {
            Validate.IsNotNull(lck, nameof(lck));
            Validate.IsNotNull(lockCollection, nameof(lockCollection));

            if (lockCollection.Count > 0)
            {
                foreach (var nestedCandidate in lockCollection)
                {
                    if (nestedCandidate == lck)
                    {
                        continue;
                    }

                    for (var a = nestedCandidate.NestingLock; a != null; a = a.NestingLock)
                    {
                        if (a == lck)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private Task ReleaseAsync(Awaiter awaiter, bool lockConsumerCanceled = false)
        {
            var illegalConcurrentLock = _reenterConcurrencyPrepRunning;
            if (illegalConcurrentLock != null)
            {
                try
                {
                    throw new Exception(string.Format(CultureInfo.CurrentCulture,
                        "Illegal concurrent use of exclusive lock. Exclusive lock: {0}, Nested lock that outlived parent: {1}",
                        illegalConcurrentLock, awaiter));
                }
                catch (Exception ex)
                {
                    throw OnCriticalFailure(ex);
                }
            }

            if (!IsLockActive(awaiter, true))
            {
                return TplExtensions.CompletedTask;
            }

            Task reenterConcurrentOutsideCode = null;
            Task synchronousCallbackExecution = null;
            var synchronousRequired = false;
            Awaiter remainingAwaiter;
            var topAwaiterAtStart = _topAwaiter.Value;

            lock (SyncObject)
            {
                var upgradedStickyWrite = awaiter.Kind == LockKind.UpgradeableRead
                    && (awaiter.Options & LockFlags.StickyWrite) == LockFlags.StickyWrite
                    && _issuedWriteLocks.Contains(awaiter);

                var writeLocksBefore = _issuedWriteLocks.Count;
                var upgradeableReadLocksBefore = _issuedUpgradeableReadLocks.Count;
                var writeLocksAfter = writeLocksBefore - (awaiter.Kind == LockKind.Write || upgradedStickyWrite ? 1 : 0);
                var upgradeableReadLocksAfter = upgradeableReadLocksBefore - (awaiter.Kind == LockKind.UpgradeableRead ? 1 : 0);
                var finalExclusiveLockRelease = writeLocksBefore > 0 && writeLocksAfter == 0;

                var callbackExecution = TplExtensions.CompletedTask;
                if (!lockConsumerCanceled)
                {
                    callbackExecution = OnBeforeLockReleasedAsync(finalExclusiveLockRelease, new LockHandle(awaiter)) ?? TplExtensions.CompletedTask;
                    synchronousRequired = finalExclusiveLockRelease && upgradeableReadLocksAfter > 0;
                    if (synchronousRequired)
                    {
                        synchronousCallbackExecution = callbackExecution;
                    }
                }

                if (!lockConsumerCanceled)
                {
                    if (writeLocksAfter == 0)
                    {
                        var fireWriteLockReleased = writeLocksBefore > 0;
                        var fireUpgradeableReadLockReleased = upgradeableReadLocksBefore > 0 && upgradeableReadLocksAfter == 0;
                        if (fireWriteLockReleased || fireUpgradeableReadLockReleased)
                        {
                            if (fireWriteLockReleased)
                            {
                                reenterConcurrentOutsideCode = DowngradeLockAsync(awaiter, upgradedStickyWrite, fireUpgradeableReadLockReleased, callbackExecution);
                            }
                            else if (fireUpgradeableReadLockReleased)
                            {
                                OnUpgradeableReadLockReleased();
                            }
                        }
                    }
                }

                if (reenterConcurrentOutsideCode == null)
                {
                    OnReleaseReenterConcurrencyComplete(awaiter, upgradedStickyWrite, false);
                }

                remainingAwaiter = GetFirstActiveSelfOrAncestor(topAwaiterAtStart);
            }

            if (remainingAwaiter == null)
            {
                _topAwaiter.Value = remainingAwaiter;
            }

            if (synchronousRequired || true)
            {
                if (reenterConcurrentOutsideCode != null && synchronousCallbackExecution != null && !synchronousCallbackExecution.IsCompleted)
                {
                    return Task.WhenAll(reenterConcurrentOutsideCode, synchronousCallbackExecution);
                }

                return reenterConcurrentOutsideCode ?? synchronousCallbackExecution ?? TplExtensions.CompletedTask;
            }

            return TplExtensions.CompletedTask;
        }

        private async Task DowngradeLockAsync(Awaiter awaiter, bool upgradedStickyWrite, bool fireUpgradeableReadLockReleased, Task beginAfterPrerequisite)
        {
            Validate.IsNotNull(awaiter, nameof(awaiter));
            Validate.IsNotNull(beginAfterPrerequisite, nameof(beginAfterPrerequisite));

            Exception prereqException = null;
            try
            {
                if (SynchronizationContext.Current is NonConcurrentSynchronizationContext)
                {
                    await beginAfterPrerequisite;
                }
                else
                {
                    await beginAfterPrerequisite.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                prereqException = ex;
            }

            Task onExclusiveLockReleasedTask;
            lock (SyncObject)
            {
                if (_issuedReadLocks.Count > 0)
                {
                    if (HasAnyNestedLocks(awaiter))
                    {
                        try
                        {
                            throw new InvalidOperationException("Write lock out-lived by a nested read lock, which is not allowed.");
                        }
                        catch (InvalidOperationException ex)
                        {
                            OnCriticalFailure(ex);
                        }
                    }
                }

                _reenterConcurrencyPrepRunning = awaiter;
                onExclusiveLockReleasedTask = OnExclusiveLockReleasedAsync();
            }

            Exception onExclusiveLockReleasedTaskException = null;
            try
            {
                await onExclusiveLockReleasedTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                onExclusiveLockReleasedTaskException = ex;
            }

            if (fireUpgradeableReadLockReleased)
            {
                OnUpgradeableReadLockReleased();
            }

            lock (SyncObject)
            {
                _reenterConcurrencyPrepRunning = null;
                OnReleaseReenterConcurrencyComplete(awaiter, upgradedStickyWrite, true);
            }

            if (prereqException != null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(prereqException).Throw();
            }

            if (onExclusiveLockReleasedTaskException != null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(onExclusiveLockReleasedTaskException).Throw();
            }
        }

        private bool HasAnyNestedLocks(Awaiter lck)
        {
            Validate.IsNotNull(lck, nameof(lck));
            if (!Monitor.IsEntered(SyncObject))
                throw new Exception();

            return HasAnyNestedLocks(lck, _issuedReadLocks)
                   || HasAnyNestedLocks(lck, _issuedUpgradeableReadLocks)
                   || HasAnyNestedLocks(lck, _issuedWriteLocks);
        }

        private void OnReleaseReenterConcurrencyComplete(Awaiter awaiter, bool upgradedStickyWrite, bool searchAllWaiters)
        {
            Validate.IsNotNull(awaiter, nameof(awaiter));

            lock (SyncObject)
            {
                if (!GetActiveLockSet(awaiter.Kind).Remove(awaiter))
                    throw new Exception();
                if (upgradedStickyWrite)
                {
                    if (awaiter.Kind != LockKind.UpgradeableRead)
                        throw new Exception();
                    if (!_issuedWriteLocks.Remove(awaiter))
                        throw new Exception();
                }

                CompleteIfAppropriate();
                TryInvokeLockConsumer(searchAllWaiters);
            }
        }

        private bool TryInvokeLockConsumer(bool searchAllWaiters)
        {
            return TryInvokeOneWriterIfAppropriate(searchAllWaiters)
                   || TryInvokeOneUpgradeableReaderIfAppropriate(searchAllWaiters)
                   || TryInvokeAllReadersIfAppropriate(searchAllWaiters);
        }

        private async Task InvokeBeforeWriteLockReleaseHandlersAsync()
        {
            if (!Monitor.IsEntered(SyncObject))
                throw new Exception();
            if (!(_beforeWriteReleasedCallbacks.Count > 0))
                throw new Exception();

            using (var releaser = await new Awaitable(this, LockKind.Write, LockFlags.None, CancellationToken.None, false))
            {
                await Task.Yield();
                List<Exception> exceptions = null;
                while (TryDequeueBeforeWriteReleasedCallback(out var callback))
                {
                    try
                    {
                        await callback();
                    }
                    catch (Exception ex)
                    {
                        if (exceptions == null)
                        {
                            exceptions = new List<Exception>();
                        }

                        exceptions.Add(ex);
                    }
                }

                await releaser.ReleaseAsync().ConfigureAwait(false);

                if (exceptions != null)
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        private bool TryDequeueBeforeWriteReleasedCallback(out Func<Task> callback)
        {
            lock (SyncObject)
            {
                if (_beforeWriteReleasedCallbacks.Count > 0)
                {
                    callback = _beforeWriteReleasedCallbacks.Dequeue();
                    return true;
                }

                callback = null;
                return false;
            }
        }

        private void ApplyLockToCallContext(Awaiter topAwaiter)
        {
            var awaiter = GetFirstActiveSelfOrAncestor(topAwaiter);
            _topAwaiter.Value = awaiter;
        }

        private bool TryInvokeAllReadersIfAppropriate(bool searchAllWaiters)
        {
            var invoked = false;
            if (_issuedWriteLocks.Count == 0 && _waitingWriters.Count == 0)
            {
                while (_waitingReaders.Count > 0)
                {
                    var pendingReader = _waitingReaders.Dequeue();
                    if (pendingReader.Kind != LockKind.Read)
                        throw new Exception();
                    IssueAndExecute(pendingReader);
                    invoked = true;
                }
            }
            else if (searchAllWaiters)
            {
                if (TryInvokeAnyWaitersInQueue(_waitingReaders, false))
                {
                    return true;
                }
            }

            return invoked;
        }

        private bool TryInvokeOneUpgradeableReaderIfAppropriate(bool searchAllWaiters)
        {
            if (_issuedUpgradeableReadLocks.Count == 0 && _issuedWriteLocks.Count == 0)
            {
                if (_waitingUpgradeableReaders.Count > 0)
                {
                    var pendingUpgradeableReader = _waitingUpgradeableReaders.Dequeue();
                    if (pendingUpgradeableReader.Kind != LockKind.UpgradeableRead)
                        throw new Exception();
                    IssueAndExecute(pendingUpgradeableReader);
                    return true;
                }
            }
            else if (searchAllWaiters)
            {
                if (TryInvokeAnyWaitersInQueue(_waitingUpgradeableReaders, true))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryInvokeOneWriterIfAppropriate(bool searchAllWaiters)
        {
            if (_issuedReadLocks.Count == 0 && _issuedUpgradeableReadLocks.Count == 0 && _issuedWriteLocks.Count == 0)
            {
                if (_waitingWriters.Count > 0)
                {
                    var pendingWriter = _waitingWriters.Dequeue();
                    if (pendingWriter.Kind != LockKind.Write)
                        throw new Exception();
                    IssueAndExecute(pendingWriter);
                    return true;
                }
            }
            else if (_issuedUpgradeableReadLocks.Count > 0 || searchAllWaiters)
            {
                if (TryInvokeAnyWaitersInQueue(_waitingWriters, true))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryInvokeAnyWaitersInQueue(Queue<Awaiter> waiters, bool breakOnFirstIssue)
        {
            Validate.IsNotNull(waiters, nameof(waiters));

            var invoked = false;
            bool invokedThisLoop;
            do
            {
                invokedThisLoop = false;
                foreach (var lockWaiter in waiters)
                {
                    if (TryIssueLock(lockWaiter, true))
                    {
                        if (!ExecuteOrHandleCancellation(lockWaiter, true))
                            throw new Exception();
                        invoked = true;
                        invokedThisLoop = true;
                        if (breakOnFirstIssue)
                        {
                            return true;
                        }
                        EventsHelper.WaitStop(lockWaiter);
                        break;
                    }
                }
            } while (invokedThisLoop);

            return invoked;
        }

        private void PendAwaiter(Awaiter awaiter)
        {
            lock (SyncObject)
            {
                if (TryIssueLock(awaiter, true))
                {
                    if (!ExecuteOrHandleCancellation(awaiter, false))
                        throw new Exception();
                }
                else
                {
                    var queue = GetLockQueue(awaiter.Kind);
                    queue.Enqueue(awaiter);
                }
            }
        }

        private bool ExecuteOrHandleCancellation(Awaiter awaiter, bool stillInQueue)
        {
            Validate.IsNotNull(awaiter, nameof(awaiter));

            lock (SyncObject)
            {
                if (stillInQueue)
                {
                    var queue = GetLockQueue(awaiter.Kind);
                    if (!queue.RemoveMidQueue(awaiter))
                    {
                        if (!awaiter.CancellationToken.IsCancellationRequested)
                            throw new Exception();
                        return false;
                    }
                }

                return awaiter.TryScheduleContinuationExecution();
            }
        }

        /// <summary>
        /// An awaitable that is returned from asynchronous lock requests.
        /// </summary>
        public struct Awaitable
        {
            private readonly Awaiter _awaiter;

            internal Awaitable(AsyncReaderWriterLock lck, LockKind kind, LockFlags options, CancellationToken cancellationToken, bool checkSyncContextCompatibility = true)
            {
                if (checkSyncContextCompatibility)
                {
                    lck.CheckSynchronizationContextAppropriateForLock(lck._topAwaiter.Value);
                }

                _awaiter = new Awaiter(lck, kind, options, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    lck.TryIssueLock(_awaiter, false);
                }
            }

            /// <summary>
            /// Gets the awaiter value.
            /// </summary>
            public Awaiter GetAwaiter()
            {
                if (_awaiter == null)
                {
                    throw new InvalidOperationException();
                }

                return _awaiter;
            }
        }

        /// <summary>
        /// A value whose disposal releases a held lock.
        /// </summary>
        [DebuggerDisplay("{_awaiter._kind}")]
        public struct Releaser : IDisposable
        {
            private readonly Awaiter _awaiter;

            internal Releaser(Awaiter awaiter)
            {
                _awaiter = awaiter;
            }

            /// <summary>
            /// Releases the lock.
            /// </summary>
            public void Dispose()
            {
                if (_awaiter != null)
                {
                    var nonConcurrentSyncContext = SynchronizationContext.Current as NonConcurrentSynchronizationContext;
                    if (!_awaiter.IsReleased)
                    {
                        using (nonConcurrentSyncContext?.LoanBackAnyHeldResource(_awaiter.OwningLock) ?? default)
                        {
                            var releaseTask = _awaiter.ReleaseAsync();
                            using (NoMessagePumpSyncContext.Default.Apply())
                            {
                                try
                                {
                                    while (!releaseTask.Wait(1000))
                                    {
                                    }
                                }
                                catch (AggregateException)
                                {
                                    releaseTask.GetAwaiter().GetResult();
                                }
                            }
                        }
                    }

                    if (nonConcurrentSyncContext != null && !_awaiter.OwningLock.AmbientLock.IsValid)
                    {
                        nonConcurrentSyncContext.EarlyExitSynchronizationContext();
                    }
                }
            }

            /// <summary>
            /// Asynchronously releases the lock.  Dispose should still be called after this.
            /// </summary>
            /// <returns>
            /// A task that should complete before the releasing thread accesses any resource protected by
            /// a lock wrapping the lock being released.
            /// </returns>
            public Task ReleaseAsync()
            {
                if (_awaiter != null)
                {
                    var nonConcurrentSyncContext = SynchronizationContext.Current as NonConcurrentSynchronizationContext;
                    using (nonConcurrentSyncContext?.LoanBackAnyHeldResource(_awaiter.OwningLock) ?? default)
                    {
                        return _awaiter.ReleaseAsync();
                    }
                }

                return TplExtensions.CompletedTask;
            }
        }

        /// <summary>
        /// A value whose disposal restores visibility of any locks held by the caller.
        /// </summary>
        public struct Suppression : IDisposable
        {
            private readonly AsyncReaderWriterLock _lck;

            private readonly Awaiter _awaiter;

            internal Suppression(AsyncReaderWriterLock lck)
            {
                _lck = lck;
                _awaiter = _lck._topAwaiter.Value;
                if (_awaiter != null)
                {
                    _lck._topAwaiter.Value = null;
                }
            }

            /// <summary>
            /// Restores visibility of hidden locks.
            /// </summary>
            public void Dispose()
            {
                _lck?.ApplyLockToCallContext(_awaiter);
            }
        }

        public class Awaiter : ICriticalNotifyCompletion
        {
            private static readonly Action<object> CancellationResponseAction = CancellationResponder;
            private CancellationToken _cancellationToken;
            private CancellationTokenRegistration _cancellationRegistration;
            private Exception _fault;
            private Action _continuation;
            private Action _continuationAfterLockIssued;
            private TaskScheduler _continuationTaskScheduler;
            private Task _releaseAsyncTask;
            private SynchronizationContext _synchronizationContext;

            /// <summary>
            /// Gets a value indicating whether the lock has been issued.
            /// </summary>
            public bool IsCompleted
            {
                get
                {
                    if (_fault != null)
                    {
                        return true;
                    }

                    // If lock has already been issued, we have to switch to the right context, and ignore the CancellationToken.
                    if (OwningLock.IsLockActive(this, true))
                    {
                        return OwningLock.IsLockSupportingContext(this);
                    }

                    return _cancellationToken.IsCancellationRequested;
                }
            }

            internal AsyncReaderWriterLock OwningLock { get; }

            internal StackTrace RequestingStackTrace { get; }

            internal Delegate LockRequestingContinuation => _continuation ?? _continuationAfterLockIssued;

            internal Awaiter NestingLock { get; }

            internal object Data { get; set; }

            internal CancellationToken CancellationToken => _cancellationToken;

            internal LockKind Kind { get; }

            internal LockFlags Options { get; }

            internal bool IsReleased => _releaseAsyncTask != null && _releaseAsyncTask.Status == TaskStatus.RanToCompletion;

            private bool LockIssued => OwningLock.IsLockActive(this, false);

            internal Awaiter(AsyncReaderWriterLock lck, LockKind kind, LockFlags options,
                CancellationToken cancellationToken)
            {
                Validate.IsNotNull(lck, nameof(lck));
                OwningLock = lck;
                Kind = kind;
                Options = options;
                _cancellationToken = cancellationToken;
                NestingLock = lck.GetFirstActiveSelfOrAncestor(lck._topAwaiter.Value);
                RequestingStackTrace = lck.CaptureDiagnostics ? new StackTrace(2, true) : null;
            }

            public void OnCompleted(Action continuation) => OnCompleted(continuation, true);

            public void UnsafeOnCompleted(Action continuation) =>
                OnCompleted(continuation, false);

            public Releaser GetResult()
            {
                try
                {
                    _cancellationRegistration.Dispose();

                    if (!LockIssued && _continuation == null &&
                        !_cancellationToken.IsCancellationRequested)
                    {
                        using (var synchronousBlock = new ManualResetEventSlim())
                        {
                            OnCompleted(synchronousBlock.Set);
                            synchronousBlock.Wait(_cancellationToken);
                        }
                    }

                    if (_fault != null)
                    {
                        throw _fault;
                    }

                    if (LockIssued)
                    {
                        OwningLock.ThrowIfUnsupportedThreadOrSyncContext();
                        if ((Kind & (LockKind.UpgradeableRead | LockKind.Write)) != 0)
                        {
                            if (!(SynchronizationContext.Current is NonConcurrentSynchronizationContext))
                                throw new Exception();
                        }

                        OwningLock.ApplyLockToCallContext(this);

                        return new Releaser(this);
                    }

                    if (_cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    OwningLock.ThrowIfUnsupportedThreadOrSyncContext();
                    throw new Exception();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    ReleaseAsync(true);
                    throw;
                }
            }

            internal Task ReleaseAsync(bool lockConsumerCanceled = false)
            {
                if (_releaseAsyncTask == null)
                {
                    try
                    {
                        _continuationAfterLockIssued = null;
                        _releaseAsyncTask = OwningLock.ReleaseAsync(this, lockConsumerCanceled);
                    }
                    catch (Exception ex)
                    {
                        var tcs = new TaskCompletionSource<object>();
                        tcs.SetException(ex);
                        _releaseAsyncTask = tcs.Task;
                    }
                }

                return _releaseAsyncTask;
            }

            internal bool TryScheduleContinuationExecution()
            {
                var continuation = Interlocked.Exchange(ref _continuation, null);

                if (continuation != null)
                {
                    _continuationAfterLockIssued = continuation;

                    var synchronizationContext = GetEffectiveSynchronizationContext();
                    if (_continuationTaskScheduler != null &&
                        synchronizationContext == DefaultSynchronizationContext)
                    {
                        Task.Factory.StartNew(continuation, CancellationToken.None, TaskCreationOptions.PreferFairness,
                            _continuationTaskScheduler);
                    }
                    else
                    {
                        synchronizationContext.Post(state => ((Action) state)(), continuation);
                    }

                    return true;
                }

                return false;
            }

            internal void SetFault(Exception ex)
            {
                _fault = ex;
            }

            private SynchronizationContext GetEffectiveSynchronizationContext()
            {
                if (_synchronizationContext == null)
                {
                    SynchronizationContext synchronizationContext = null;

                    var awaiter = NestingLock;
                    while (awaiter != null)
                    {
                        if (OwningLock.IsLockActive(awaiter, true))
                        {
                            synchronizationContext = awaiter.GetEffectiveSynchronizationContext();
                            break;
                        }

                        awaiter = awaiter.NestingLock;
                    }

                    if (synchronizationContext == null)
                    {
                        synchronizationContext = Kind == LockKind.Read ? DefaultSynchronizationContext : new NonConcurrentSynchronizationContext();
                    }

                    Interlocked.CompareExchange(ref _synchronizationContext, synchronizationContext, null);
                }

                return _synchronizationContext;
            }

            private static void CancellationResponder(object state)
            {
                var awaiter = (Awaiter) state;

                if (awaiter.OwningLock.ExecuteOrHandleCancellation(awaiter, true))
                {
                    if (awaiter.Kind == LockKind.Write)
                    {
                        lock (awaiter.OwningLock.SyncObject)
                        {
                            awaiter.OwningLock.TryInvokeLockConsumer(false);
                        }
                    }
                }

                awaiter._cancellationRegistration.Dispose();
            }

            private void OnCompleted(Action continuation, bool flowExecutionContext)
            {
                if (LockIssued)
                {
                    throw new InvalidOperationException();
                }

                if (Interlocked.CompareExchange(ref _continuation, continuation, null) != null)
                {
                    throw new NotSupportedException("Multiple continuations are not supported.");
                }

                if (Kind == LockKind.Read)
                {
                    _continuationTaskScheduler = OwningLock.GetTaskSchedulerForReadLockRequest();
                }

                _cancellationRegistration = _cancellationToken.Register(CancellationResponseAction, this,
                    false);
                OwningLock.PendAwaiter(this);

                if (_cancellationToken.IsCancellationRequested &&
                    _cancellationRegistration == default)
                {
                    CancellationResponder(this);
                }
            }
        }

        internal class EventsHelper
        {
            private readonly AsyncReaderWriterLock _lck;

            internal EventsHelper(AsyncReaderWriterLock lck)
            {
                Validate.IsNotNull(lck, "lck");
                _lck = lck;
            }

            public void Issued(Awaiter lckAwaiter)
            {
                if (ThreadingEventSource.Instance.IsEnabled())
                {
                    ThreadingEventSource.Instance.ReaderWriterLockIssued(lckAwaiter.GetHashCode(), lckAwaiter.Kind,
                        _lck._issuedUpgradeableReadLocks.Count, _lck._issuedReadLocks.Count);
                }
            }

            public void WaitStart(Awaiter lckAwaiter)
            {
                if (ThreadingEventSource.Instance.IsEnabled())
                {
                    ThreadingEventSource.Instance.WaitReaderWriterLockStart(lckAwaiter.GetHashCode(), lckAwaiter.Kind,
                        _lck._issuedWriteLocks.Count, _lck._issuedUpgradeableReadLocks.Count,
                        _lck._issuedReadLocks.Count);
                }
            }

            public static void WaitStop(Awaiter lckAwaiter)
            {
                if (ThreadingEventSource.Instance.IsEnabled())
                {
                    ThreadingEventSource.Instance.WaitReaderWriterLockStop(lckAwaiter.GetHashCode(), lckAwaiter.Kind);
                }
            }
        }

        internal enum LockKind
        {
            Read,
            UpgradeableRead,
            Write
        }

        /// <summary>
        /// Flags that modify default lock behavior.
        /// </summary>
        [Flags]
        public enum LockFlags
        {
            /// <summary>
            /// The default behavior applies.
            /// </summary>
            None = 0x0,

            /// <summary>
            /// Causes an upgradeable reader to remain in an upgraded-write state once upgraded,
            /// even after the nested write lock has been released.
            /// </summary>
            /// <remarks>
            /// This is useful when you have a batch of possible write operations to apply, which
            /// may or may not actually apply in the end, but if any of them change anything,
            /// all of their changes should be seen atomically (within a single write lock).
            /// This approach is preferable to simply acquiring a write lock around the batch of
            /// potential changes because it doesn't defeat concurrent readers until it knows there
            /// is a change to actually make.
            /// </remarks>
            StickyWrite = 0x1,
        }

        /// <summary>
        /// A "public" representation of a specific lock.
        /// </summary>
        protected struct LockHandle
        {
            internal LockHandle(Awaiter awaiter)
            {
                Awaiter = awaiter;
            }

            /// <summary>
            /// Gets a value indicating whether this handle is to a lock which was actually acquired.
            /// </summary>
            public bool IsValid => Awaiter != null;

            /// <summary>
            /// Gets a value indicating whether this lock is still active.
            /// </summary>
            public bool IsActive => Awaiter.OwningLock.IsLockActive(Awaiter, true);

            /// <summary>
            /// Gets a value indicating whether this lock represents a read lock.
            /// </summary>
            public bool IsReadLock => IsValid && Awaiter.Kind == LockKind.Read;

            /// <summary>
            /// Gets a value indicating whether this lock represents an upgradeable read lock.
            /// </summary>
            public bool IsUpgradeableReadLock => IsValid && Awaiter.Kind == LockKind.UpgradeableRead;

            /// <summary>
            /// Gets a value indicating whether this lock represents a write lock.
            /// </summary>
            public bool IsWriteLock => IsValid && Awaiter.Kind == LockKind.Write;

            /// <summary>
            /// Gets a value indicating whether this lock is an active read lock or is nested by one.
            /// </summary>
            public bool HasReadLock => IsValid && Awaiter.OwningLock.IsLockHeld(LockKind.Read, Awaiter, false, true);

            /// <summary>
            /// Gets a value indicating whether this lock is an active upgradeable read lock or is nested by one.
            /// </summary>
            public bool HasUpgradeableReadLock => IsValid && Awaiter.OwningLock.IsLockHeld(LockKind.UpgradeableRead, Awaiter, false, true);

            /// <summary>
            /// Gets a value indicating whether this lock is an active write lock or is nested by one.
            /// </summary>
            public bool HasWriteLock => IsValid && Awaiter.OwningLock.IsLockHeld(LockKind.Write, Awaiter, false, true);

            /// <summary>
            /// Gets the flags that were passed into this lock.
            /// </summary>
            public LockFlags Flags => IsValid ? Awaiter.Options : LockFlags.None;

            /// <summary>
            /// Gets or sets some object associated to this specific lock.
            /// </summary>
            public object Data
            {
                get => IsValid ? Awaiter.Data : null;

                set
                {
                    if (!IsValid)
                        throw new InvalidOperationException("Invalid Lock");
                    Awaiter.Data = value;
                }
            }

            /// <summary>
            /// Gets the lock within which this lock was acquired.
            /// </summary>
            public LockHandle NestingLock => IsValid ? new LockHandle(Awaiter.NestingLock) : default;

            internal Awaiter Awaiter { get; }
        }

        internal sealed class NonConcurrentSynchronizationContext : SynchronizationContext, IDisposable
        {
            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
            private int? _semaphoreHoldingManagedThreadId;

            private bool IsCurrentThreadHoldingSemaphore
            {
                get
                {
                    var semaphoreHoldingManagedThreadId = _semaphoreHoldingManagedThreadId;
                    return semaphoreHoldingManagedThreadId.HasValue
                           && semaphoreHoldingManagedThreadId.Value == Environment.CurrentManagedThreadId;
                }
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException();
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                Validate.IsNotNull(d, nameof(d));

                if (ThreadingEventSource.Instance.IsEnabled())
                {
                    ThreadingEventSource.Instance.PostExecutionStart(d.GetHashCode(), false);
                }

                ThreadPool.QueueUserWorkItem(
                    s =>
                    {
                        var tuple = (Tuple<NonConcurrentSynchronizationContext, SendOrPostCallback, object>) s;
                        tuple.Item1.PostHelper(tuple.Item2, tuple.Item3);
                    },
                    Tuple.Create(this, d, state));
            }

            public void Dispose()
            {
                _semaphore.Dispose();
            }

            internal LoanBack LoanBackAnyHeldResource(AsyncReaderWriterLock asyncLock)
            {
                return _semaphore.CurrentCount == 0 && IsCurrentThreadHoldingSemaphore
                    ? new LoanBack(this, asyncLock)
                    : default;
            }

            internal void EarlyExitSynchronizationContext()
            {
                if (IsCurrentThreadHoldingSemaphore)
                {
                    _semaphoreHoldingManagedThreadId = null;
                    _semaphore.Release();
                }

                if (Current == this)
                {
                    SetSynchronizationContext(null);
                }
            }

            private async void PostHelper(SendOrPostCallback d, object state)
            {
                var delegateInvoked = false;
                try
                {
                    await _semaphore.WaitAsync().ConfigureAwait(false);
                    _semaphoreHoldingManagedThreadId = Environment.CurrentManagedThreadId;
                    try
                    {
                        SetSynchronizationContext(this);
                        if (ThreadingEventSource.Instance.IsEnabled())
                        {
                            ThreadingEventSource.Instance.PostExecutionStop(d.GetHashCode());
                        }

                        delegateInvoked = true;
                        d(state);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (IsCurrentThreadHoldingSemaphore)
                        {
                            _semaphoreHoldingManagedThreadId = null;
                            _semaphore.Release();
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    if (!delegateInvoked)
                    {
                        SetSynchronizationContext(null);
                        try
                        {
                            d(state);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            internal struct LoanBack : IDisposable
            {
                private readonly NonConcurrentSynchronizationContext _syncContext;
                private readonly AsyncReaderWriterLock _asyncLock;

                internal LoanBack(NonConcurrentSynchronizationContext syncContext, AsyncReaderWriterLock asyncLock)
                {
                    Validate.IsNotNull(syncContext, nameof(syncContext));
                    Validate.IsNotNull(asyncLock, nameof(asyncLock));
                    _syncContext = syncContext;
                    _asyncLock = asyncLock;
                    _syncContext._semaphoreHoldingManagedThreadId = null;
                    _syncContext._semaphore.Release();
                }

                public void Dispose()
                {
                    if (_syncContext != null)
                    {
                        if (Monitor.IsEntered(_asyncLock.SyncObject))
                            throw new Exception("Should not wait on the Semaphore, when we hold the syncObject. This causes deadlocks");
                        _syncContext._semaphore.Wait();
                        _syncContext._semaphoreHoldingManagedThreadId = Environment.CurrentManagedThreadId;
                    }
                }
            }
        }
    }
}
