using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;
using WaitCallback = System.Action<object>;

namespace ModernApplicationFramework.Threading
{
    public class JoinableTaskFactory
    {
        private readonly SynchronizationContext _mainThreadJobSyncContext;
        private TimeSpan _hangDetectionTimeout = TimeSpan.FromSeconds(6);


        /// <summary>
        /// Gets the joinable task context to which this factory belongs.
        /// </summary>
        public JoinableTaskContext Context { get; }

        internal SynchronizationContext ApplicableJobSyncContext => Context.IsOnMainThread ? _mainThreadJobSyncContext : null;

        internal JoinableTaskCollection Collection { get; }

        /// <summary>
        /// Gets or sets the timeout after which no activity while synchronously blocking
        /// suggests a hang has occurred.
        /// </summary>
        protected TimeSpan HangDetectionTimeout
        {
            get => _hangDetectionTimeout;

            set
            {
                if (value <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _hangDetectionTimeout = value;
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="SynchronizationContext"/> that controls the main thread in the host.
        /// </summary>
        protected SynchronizationContext UnderlyingSynchronizationContext => Context.UnderlyingSynchronizationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinableTaskFactory" /> class.
        /// </summary>
        /// <param name="owner">The context for the tasks created by this factory.</param>
        public JoinableTaskFactory(JoinableTaskContext owner)
            : this(owner, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinableTaskFactory" /> class
        /// that adds all generated jobs to the specified collection.
        /// </summary>
        /// <param name="collection">The collection that all tasks created by this factory will belong to till they complete.</param>
        public JoinableTaskFactory(JoinableTaskCollection collection)
            : this(collection.Context, collection)
        {
        }

        internal JoinableTaskFactory(JoinableTaskContext owner, JoinableTaskCollection collection)
        {
            Validate.IsNotNull(owner, nameof(owner));
            if (!(collection == null || collection.Context == owner))
                throw new Exception();

            Context = owner;
            Collection = collection;
            _mainThreadJobSyncContext = new JoinableTaskSynchronizationContext(this);
        }

        /// <summary>
        /// Gets an awaitable whose continuations execute on the synchronization context that this instance was initialized with,
        /// in such a way as to mitigate both deadlocks and reentrancy.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token whose cancellation will immediately schedule the continuation
        /// on a threadpool thread.
        /// </param>
        /// <returns>An awaitable.</returns>
        /// <remarks>
        /// <example>
        /// <code>
        /// private async Task SomeOperationAsync() {
        ///     // on the caller's thread.
        ///     await DoAsync();
        ///
        ///     // Now switch to a threadpool thread explicitly.
        ///     await TaskScheduler.Default;
        ///
        ///     // Now switch to the Main thread to talk to some STA object.
        ///     await this.JobContext.SwitchToMainThreadAsync();
        ///     STAService.DoSomething();
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public MainThreadAwaitable SwitchToMainThreadAsync(CancellationToken cancellationToken = default)
        {
            return new MainThreadAwaitable(this, Context.AmbientTask, cancellationToken);
        }

        /// <summary>
        /// Gets an awaitable whose continuations execute on the synchronization context that this instance was initialized with,
        /// in such a way as to mitigate both deadlocks and reentrancy.
        /// </summary>
        /// <param name="alwaysYield">A value indicating whether the caller should yield even if
        /// already executing on the main thread.</param>
        /// <param name="cancellationToken">
        /// A token whose cancellation will immediately schedule the continuation
        /// on a threadpool thread.
        /// </param>
        /// <returns>An awaitable.</returns>
        /// <remarks>
        /// <example>
        /// <code>
        /// private async Task SomeOperationAsync()
        /// {
        ///     // This first part can be on the caller's thread, whatever that is.
        ///     DoSomething();
        ///
        ///     // Now switch to the Main thread to talk to some STA object.
        ///     // Supposing it is also important to *not* do this step on our caller's callstack,
        ///     // be sure we yield even if we're on the UI thread.
        ///     await this.JoinableTaskFactory.SwitchToMainThreadAsync(alwaysYield: true);
        ///     STAService.DoSomething();
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public MainThreadAwaitable SwitchToMainThreadAsync(bool alwaysYield, CancellationToken cancellationToken = default)
        {
            return new MainThreadAwaitable(this, Context.AmbientTask, cancellationToken, alwaysYield);
        }

        internal SingleExecuteProtector RequestSwitchToMainThread(Action callback)
        {
            Validate.IsNotNull(callback, nameof(callback));
            var ambientJob = Context.AmbientTask;
            SingleExecuteProtector wrapper = null;
            if (ambientJob == null || (Collection != null && !Collection.Contains(ambientJob)))
            {
                var transient = RunAsync(
                    delegate
                    {
                        ambientJob = Context.AmbientTask;
                        wrapper = SingleExecuteProtector.Create(ambientJob, callback);
                        ambientJob.Post(SingleExecuteProtector.ExecuteOnce, wrapper, true);
                        return TplExtensions.CompletedTask;
                    },
                    false,
                    JoinableTaskCreationOptions.None,
                    callback);

                if (transient.Task.IsFaulted)
                {
                    // rethrow the exception.
                    transient.Task.GetAwaiter().GetResult();
                }
            }
            else
            {
                wrapper = SingleExecuteProtector.Create(ambientJob, callback);
                ambientJob.Post(SingleExecuteProtector.ExecuteOnce, wrapper, true);
            }

            Validate.IsNotNull(wrapper, "wrapper");
            return wrapper;
        }

        /// <summary>
        /// Posts a message to the specified underlying SynchronizationContext for processing when the main thread
        /// is freely available.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        /// <param name="state">State to pass to the callback.</param>
        protected internal virtual void PostToUnderlyingSynchronizationContext(SendOrPostCallback callback, object state)
        {
            Validate.IsNotNull(callback, nameof(callback));
            Validate.IsNotNull(UnderlyingSynchronizationContext, nameof(UnderlyingSynchronizationContext));

            UnderlyingSynchronizationContext.Post(callback, state);
        }

        /// <summary>
        /// Raised when a joinable task has requested a transition to the main thread.
        /// </summary>
        /// <param name="joinableTask">The task requesting the transition to the main thread.</param>
        /// <remarks>
        /// This event may be raised on any thread, including the main thread.
        /// </remarks>
        protected internal virtual void OnTransitioningToMainThread(JoinableTask joinableTask)
        {
            Validate.IsNotNull(joinableTask, nameof(joinableTask));
        }

        /// <summary>
        /// Raised whenever a joinable task has completed a transition to the main thread.
        /// </summary>
        /// <param name="joinableTask">The task whose request to transition to the main thread has completed.</param>
        /// <param name="canceled">A value indicating whether the transition was cancelled before it was fulfilled.</param>
        /// <remarks>
        /// This event is usually raised on the main thread, but can be on another thread when <paramref name="canceled"/> is <c>true</c>.
        /// </remarks>
        protected internal virtual void OnTransitionedToMainThread(JoinableTask joinableTask, bool canceled)
        {
            Validate.IsNotNull(joinableTask, nameof(joinableTask));
        }

        internal void PostToUnderlyingSynchronizationContextOrThreadPool(SingleExecuteProtector callback)
        {
            Validate.IsNotNull(callback, nameof(callback));

            if (UnderlyingSynchronizationContext != null)
            {
                this.PostToUnderlyingSynchronizationContext(SingleExecuteProtector.ExecuteOnce, callback);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(SingleExecuteProtector.ExecuteOnceWaitCallback, callback);
            }
        }

        /// <summary>
        /// Synchronously blocks the calling thread for the completion of the specified task.
        /// If running on the main thread, any applicable message pump is suppressed
        /// while the thread sleeps.
        /// </summary>
        /// <param name="task">The task whose completion is being waited on.</param>
        /// <remarks>
        /// Implementations should take care that exceptions from faulted or canceled tasks
        /// not be thrown back to the caller.
        /// </remarks>
        protected internal virtual void WaitSynchronously(Task task)
        {
            if (Context.IsOnMainThread)
            {
                // Suppress any reentrancy by causing this synchronously blocking wait
                // to not pump any messages at all.
                using (Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    WaitSynchronouslyCore(task);
                }
            }
            else
            {
                WaitSynchronouslyCore(task);
            }
        }

        /// <summary>
        /// Synchronously blocks the calling thread for the completion of the specified task.
        /// </summary>
        /// <param name="task">The task whose completion is being waited on.</param>
        /// <remarks>
        /// Implementations should take care that exceptions from faulted or canceled tasks
        /// not be thrown back to the caller.
        /// </remarks>
        protected virtual void WaitSynchronouslyCore(Task task)
        {
            Validate.IsNotNull(task, nameof(task));
            var hangTimeoutsCount = 0; // useful for debugging dump files to see how many times we looped.
            var hangNotificationCount = 0;
            var hangId = Guid.Empty;
            Stopwatch stopWatch = null;
            try
            {
                while (!task.Wait(HangDetectionTimeout))
                {
                    if (hangTimeoutsCount == 0)
                    {
                        stopWatch = Stopwatch.StartNew();
                    }

                    hangTimeoutsCount++;
                    var hangDuration = TimeSpan.FromMilliseconds(HangDetectionTimeout.TotalMilliseconds * hangTimeoutsCount);
                    if (hangId == Guid.Empty)
                    {
                        hangId = Guid.NewGuid();
                    }

                    if (!IsWaitingOnLongRunningTask())
                    {
                        hangNotificationCount++;
                        Context.OnHangDetected(hangDuration, hangNotificationCount, hangId);
                    }
                }

                if (hangNotificationCount > 0)
                {
                    // We detect a false alarm. The stop watch was started after the first timeout, so we add intial timeout to the total delay.
                    Context.OnFalseHangDetected(
                        stopWatch.Elapsed + HangDetectionTimeout,
                        hangId);
                }
            }
            catch (AggregateException)
            {
                // Swallow exceptions thrown by Task.Wait().
                // Our caller just wants to know when the Task completes,
                // whether successfully or not.
            }
        }

        /// <summary>
        /// Check whether the current joinableTask is waiting on a long running task.
        /// </summary>
        /// <returns>Return true if the current synchronous task on the thread is waiting on a long running task.</returns>
        protected bool IsWaitingOnLongRunningTask()
        {
            var currentBlockingTask = JoinableTask.TaskCompletingOnThisThread;
            if (currentBlockingTask != null)
            {
                if ((currentBlockingTask.CreationOptions & JoinableTaskCreationOptions.LongRunning) == JoinableTaskCreationOptions.LongRunning)
                {
                    return true;
                }

                using (Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    var allJoinedJobs = new HashSet<JoinableTask>();
                    lock (Context.SyncContextLock)
                    {
                        currentBlockingTask.AddSelfAndDescendentOrJoinedJobs(allJoinedJobs);
                        return allJoinedJobs.Any(t => (t.CreationOptions & JoinableTaskCreationOptions.LongRunning) == JoinableTaskCreationOptions.LongRunning);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Runs the specified asynchronous method to completion while synchronously blocking the calling thread.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        /// <remarks>
        /// <para>Any exception thrown by the delegate is rethrown in its original type to the caller of this method.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// <example>
        /// <code>
        /// // On threadpool or Main thread, this method will block
        /// // the calling thread until all async operations in the
        /// // delegate complete.
        /// joinableTaskFactory.Run(async delegate {
        ///     // still on the threadpool or Main thread as before.
        ///     await OperationAsync();
        ///     // still on the threadpool or Main thread as before.
        ///     await Task.Run(async delegate {
        ///          // Now we're on a threadpool thread.
        ///          await Task.Yield();
        ///          // still on a threadpool thread.
        ///     });
        ///     // Now back on the Main thread (or threadpool thread if that's where we started).
        /// });
        /// </code>
        /// </example>
        /// </remarks>
        public void Run(Func<Task> asyncMethod)
        {
            Run(asyncMethod, JoinableTaskCreationOptions.None, null);
        }

        /// <summary>
        /// Runs the specified asynchronous method to completion while synchronously blocking the calling thread.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        /// <param name="creationOptions">The <see cref="JoinableTaskCreationOptions"/> used to customize the task's behavior.</param>
        public void Run(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions)
        {
            Run(asyncMethod, creationOptions, null);
        }

        /// <summary>
        /// Runs the specified asynchronous method to completion while synchronously blocking the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        /// <returns>The result of the Task returned by <paramref name="asyncMethod"/>.</returns>
        /// <remarks>
        /// <para>Any exception thrown by the delegate is rethrown in its original type to the caller of this method.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// <para>See the <see cref="Run(Func{Task})" /> overload documentation for an example.</para>
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public T Run<T>(Func<Task<T>> asyncMethod)
        {
            return Run(asyncMethod, JoinableTaskCreationOptions.None);
        }

        /// <summary>
        /// Runs the specified asynchronous method to completion while synchronously blocking the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        /// <param name="creationOptions">The <see cref="JoinableTaskCreationOptions"/> used to customize the task's behavior.</param>
        /// <returns>The result of the Task returned by <paramref name="asyncMethod"/>.</returns>
        /// <remarks>
        /// <para>Any exception thrown by the delegate is rethrown in its original type to the caller of this method.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public T Run<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions)
        {
            VerifyNoNonConcurrentSyncContext();
            var joinable = RunAsync(asyncMethod, true, creationOptions);
            return joinable.CompleteOnCurrentThread();
        }

        /// <summary>
        /// Invokes an async delegate on the caller's thread, and yields back to the caller when the async method yields.
        /// The async delegate is invoked in such a way as to mitigate deadlocks in the event that the async method
        /// requires the main thread while the main thread is blocked waiting for the async method's completion.
        /// </summary>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <returns>An object that tracks the completion of the async operation, and allows for later synchronous blocking of the main thread for completion if necessary.</returns>
        /// <remarks>
        /// <para>Exceptions thrown by the delegate are captured by the returned <see cref="JoinableTask" />.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        public JoinableTask RunAsync(Func<Task> asyncMethod)
        {
            return RunAsync(asyncMethod, false, JoinableTaskCreationOptions.None);
        }

        /// <summary>
        /// Invokes an async delegate on the caller's thread, and yields back to the caller when the async method yields.
        /// The async delegate is invoked in such a way as to mitigate deadlocks in the event that the async method
        /// requires the main thread while the main thread is blocked waiting for the async method's completion.
        /// </summary>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <returns>An object that tracks the completion of the async operation, and allows for later synchronous blocking of the main thread for completion if necessary.</returns>
        /// <param name="creationOptions">The <see cref="JoinableTaskCreationOptions"/> used to customize the task's behavior.</param>
        /// <remarks>
        /// <para>Exceptions thrown by the delegate are captured by the returned <see cref="JoinableTask" />.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        public JoinableTask RunAsync(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions)
        {
            return RunAsync(asyncMethod, false, creationOptions);
        }

        internal void Run(Func<Task> asyncMethod, JoinableTaskCreationOptions creationOptions, Delegate entrypointOverride)
        {
            VerifyNoNonConcurrentSyncContext();
            var joinable = RunAsync(asyncMethod, true, creationOptions, entrypointOverride);
            joinable.CompleteOnCurrentThread();
        }

        private JoinableTask RunAsync(Func<Task> asyncMethod, bool synchronouslyBlocking, JoinableTaskCreationOptions creationOptions, Delegate entrypointOverride = null)
        {
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));

            var job = new JoinableTask(this, synchronouslyBlocking, creationOptions, entrypointOverride ?? asyncMethod);
            ExecuteJob<EmptyStruct>(asyncMethod, job);
            return job;
        }

        /// <summary>
        /// Invokes an async delegate on the caller's thread, and yields back to the caller when the async method yields.
        /// The async delegate is invoked in such a way as to mitigate deadlocks in the event that the async method
        /// requires the main thread while the main thread is blocked waiting for the async method's completion.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <returns>
        /// An object that tracks the completion of the async operation, and allows for later synchronous blocking of the main thread for completion if necessary.
        /// </returns>
        /// <remarks>
        /// <para>Exceptions thrown by the delegate are captured by the returned <see cref="JoinableTask" />.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        public JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod)
        {
            return RunAsync(asyncMethod, false, JoinableTaskCreationOptions.None);
        }

        /// <summary>
        /// Invokes an async delegate on the caller's thread, and yields back to the caller when the async method yields.
        /// The async delegate is invoked in such a way as to mitigate deadlocks in the event that the async method
        /// requires the main thread while the main thread is blocked waiting for the async method's completion.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the asynchronous operation.</typeparam>
        /// <param name="asyncMethod">The method that, when executed, will begin the async operation.</param>
        /// <param name="creationOptions">The <see cref="JoinableTaskCreationOptions"/> used to customize the task's behavior.</param>
        /// <returns>
        /// An object that tracks the completion of the async operation, and allows for later synchronous blocking of the main thread for completion if necessary.
        /// </returns>
        /// <remarks>
        /// <para>Exceptions thrown by the delegate are captured by the returned <see cref="JoinableTask" />.</para>
        /// <para>When the delegate resumes from a yielding await, the default behavior is to resume in its original context
        /// as an ordinary async method execution would. For example, if the caller was on the main thread, execution
        /// resumes after an await on the main thread; but if it started on a threadpool thread it resumes on a threadpool thread.</para>
        /// </remarks>
        public JoinableTask RunAsync<T>(Func<Task<T>> asyncMethod, JoinableTaskCreationOptions creationOptions)
        {
            return RunAsync(asyncMethod, false, creationOptions);
        }

        internal void Post(SendOrPostCallback callback, object state, bool mainThreadAffinitized)
        {
            Validate.IsNotNull(callback, nameof(callback));

            if (mainThreadAffinitized)
            {
                var transient = RunAsync(delegate
                {
                    Context.AmbientTask.Post(callback, state, true);
                    return TplExtensions.CompletedTask;
                });

                if (transient.Task.IsFaulted)
                {
                    // rethrow the exception.
                    transient.Task.GetAwaiter().GetResult();
                }
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(callback), state);
            }
        }

        /// <summary>
        /// Adds the specified joinable task to the applicable collection.
        /// </summary>
        protected void Add(JoinableTask joinable)
        {
            Validate.IsNotNull(joinable, nameof(joinable));
            Collection?.Add(joinable);
        }

        private static void VerifyNoNonConcurrentSyncContext()
        {
            // Don't use Verify.Operation here to avoid loading a string resource in success cases.
            if (SynchronizationContext.Current is AsyncReaderWriterLock.NonConcurrentSynchronizationContext)
            {
                throw new InvalidOperationException();
            }
        }

        private JoinableTask<T> RunAsync<T>(Func<Task<T>> asyncMethod, bool synchronouslyBlocking, JoinableTaskCreationOptions creationOptions)
        {
            Validate.IsNotNull(asyncMethod, nameof(asyncMethod));

            var job = new JoinableTask<T>(this, synchronouslyBlocking, creationOptions, asyncMethod);
            ExecuteJob<T>(asyncMethod, job);
            return job;
        }

        private void ExecuteJob<T>(Func<Task> asyncMethod, JoinableTask job)
        {
            using (new RunFramework(this, job))
            {
                Task asyncMethodResult;
                try
                {
                    asyncMethodResult = asyncMethod();
                }
                catch (Exception ex)
                {
                    var tcs = new TaskCompletionSource<T>();
                    tcs.SetException(ex);
                    asyncMethodResult = tcs.Task;
                }

                job.SetWrappedTask(asyncMethodResult);
            }
        }

        public struct MainThreadAwaitable
        {
            private readonly JoinableTaskFactory _jobFactory;

            private readonly JoinableTask _job;

            private readonly CancellationToken _cancellationToken;

            private readonly bool _alwaysYield;

            /// <summary>
            /// Initializes a new instance of the <see cref="MainThreadAwaitable"/> struct.
            /// </summary>
            internal MainThreadAwaitable(JoinableTaskFactory jobFactory, JoinableTask job, CancellationToken cancellationToken, bool alwaysYield = false)
            {
                Validate.IsNotNull(jobFactory, nameof(jobFactory));

                _jobFactory = jobFactory;
                _job = job;
                _cancellationToken = cancellationToken;
                _alwaysYield = alwaysYield;
            }

            /// <summary>
            /// Gets the awaiter.
            /// </summary>
            public MainThreadAwaiter GetAwaiter()
            {
                return new MainThreadAwaiter(_jobFactory, _job, _cancellationToken, _alwaysYield);
            }
        }

        public struct MainThreadAwaiter : ICriticalNotifyCompletion
        {
            private static readonly Action<object> SafeCancellationAction = state => ThreadPool.QueueUserWorkItem(SingleExecuteProtector.ExecuteOnceWaitCallback, state);
            private static readonly Action<object> UnsafeCancellationAction = SafeCancellationAction;
            private readonly JoinableTaskFactory _jobFactory;

            private readonly CancellationToken _cancellationToken;

            private readonly bool _alwaysYield;

            private readonly JoinableTask _job;

            private readonly StrongBox<CancellationTokenRegistration?> _cancellationRegistrationPtr;

            internal MainThreadAwaiter(JoinableTaskFactory jobFactory, JoinableTask job, CancellationToken cancellationToken, bool alwaysYield)
            {
                _jobFactory = jobFactory;
                _job = job;
                _cancellationToken = cancellationToken;
                _alwaysYield = alwaysYield;

                _cancellationRegistrationPtr = cancellationToken.CanBeCanceled
                    ? new StrongBox<CancellationTokenRegistration?>()
                    : null;
            }

            /// <summary>
            /// Gets a value indicating whether the caller is already on the Main thread.
            /// </summary>
            public bool IsCompleted
            {
                get
                {
                    if (_alwaysYield)
                    {
                        return false;
                    }

                    return _jobFactory == null
                        || _jobFactory.Context.IsOnMainThread
                        || _jobFactory.Context.UnderlyingSynchronizationContext == null;
                }
            }

            /// <summary>
            /// Schedules a continuation for execution on the Main thread
            /// without capturing the ExecutionContext.
            /// </summary>
            /// <param name="continuation">The action to invoke when the operation completes.</param>
            public void UnsafeOnCompleted(Action continuation)
            {
                OnCompleted(continuation, false);
            }

            /// <summary>
            /// Schedules a continuation for execution on the Main thread.
            /// </summary>
            /// <param name="continuation">The action to invoke when the operation completes.</param>
            public void OnCompleted(Action continuation)
            {
                OnCompleted(continuation, true);
            }

            /// <summary>
            /// Called on the Main thread to prepare it to execute the continuation.
            /// </summary>
            public void GetResult()
            {
                if (_jobFactory == null)
                    throw new Exception();
                if (!(_jobFactory.Context.IsOnMainThread || _jobFactory.Context.UnderlyingSynchronizationContext == null || _cancellationToken.IsCancellationRequested))
                {
                    throw new JoinableTaskContextException("Switch to main failed");
                }

                // Release memory associated with the cancellation request.
                if (_cancellationRegistrationPtr != null)
                {
                    var registration = default(CancellationTokenRegistration);
                    using (_jobFactory.Context.NoMessagePumpSynchronizationContext.Apply())
                    {
                        lock (_cancellationRegistrationPtr)
                        {
                            if (_cancellationRegistrationPtr.Value.HasValue)
                            {
                                registration = _cancellationRegistrationPtr.Value.Value;
                            }

                            // The reason we set this is to effectively null the struct that
                            // the strong box points to. Dispose does not seem to do this. If we
                            // have two copies of MainThreadAwaiter pointing to the same strongbox,
                            // then if one copy executes but the other does not, we could end
                            // up holding onto the memory pointed to through this pointer. By
                            // resetting the value here we make sure it gets cleaned.
                            //
                            // In addition, assigning default(CancellationTokenRegistration) to a field that
                            // stores a Nullable<CancellationTokenRegistration> effectively gives it a HasValue status,
                            // which will let OnCompleted know it lost the interest on the cancellation. That is an
                            // important hint for OnCompleted() in order NOT to leak the cancellation registration.
                            _cancellationRegistrationPtr.Value = default(CancellationTokenRegistration);
                        }
                    }

                    // Intentionally deferring disposal till we exit the lock to avoid executing outside code within the lock.
                    registration.Dispose();
                }

                // Only throw a cancellation exception if we didn't end up completing what the caller asked us to do (arrive at the main thread).
                if (!_jobFactory.Context.IsOnMainThread)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                }

                // If this method is called in a continuation after an actual yield, then SingleExecuteProtector.TryExecute
                // should have already applied the appropriate SynchronizationContext to avoid deadlocks.
                // However if no yield occurred then no TryExecute would have been invoked, so to avoid deadlocks in those
                // cases, we apply the synchronization context here.
                // We don't have an opportunity to revert the sync context change, but it turns out we don't need to because
                // this method should only be called from async methods, which automatically revert any execution context
                // changes they apply (including SynchronizationContext) when they complete, thanks to the way .NET 4.5 works.
                var syncContext = _job != null ? _job.ApplicableJobSyncContext : _jobFactory.ApplicableJobSyncContext;
                syncContext.Apply();
            }

            private void OnCompleted(Action continuation, bool flowExecutionContext)
            {
                if (_jobFactory == null)
                    throw new Exception();
                try
                {
                    // In the event of a cancellation request, it becomes a race as to whether the threadpool
                    // or the main thread will execute the continuation first. So we must wrap the continuation
                    // in a SingleExecuteProtector so that it can't be executed twice by accident.
                    // Success case of the main thread.
                    var wrapper = _jobFactory.RequestSwitchToMainThread(continuation);

                    // Cancellation case of a threadpool thread.
                    if (_cancellationRegistrationPtr != null)
                    {
                        // Store the cancellation token registration in the struct pointer. This way,
                        // if the awaiter has been copied (since it's a struct), each copy of the awaiter
                        // points to the same registration. Without this we can have a memory leak.
                        var registration = _cancellationToken.Register(
                            flowExecutionContext ? SafeCancellationAction : UnsafeCancellationAction,
                            wrapper,
                            false);

                        // Needs a lock to avoid a race condition between this method and GetResult().
                        // This method is called on a background thread. After "this.jobFactory.RequestSwitchToMainThread()" returns,
                        // the continuation is scheduled and GetResult() will be called whenever it is ready on main thread.
                        // We have observed sometimes GetResult() was called right after "this.jobFactory.RequestSwitchToMainThread()"
                        // and before "this.cancellationToken.Register()". If that happens, that means we lose the interest on the cancellation
                        // and should not register the cancellation here. Without protecting that, "this.cancellationRegistrationPtr" will be leaked.
                        var disposeThisRegistration = false;
                        using (_jobFactory.Context.NoMessagePumpSynchronizationContext.Apply())
                        {
                            lock (_cancellationRegistrationPtr)
                            {
                                if (!_cancellationRegistrationPtr.Value.HasValue)
                                {
                                    _cancellationRegistrationPtr.Value = registration;
                                }
                                else
                                {
                                    disposeThisRegistration = true;
                                }
                            }
                        }

                        if (disposeThisRegistration)
                        {
                            registration.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // This is bad. It would cause a hang without a trace as to why, since we if can't
                    // schedule the continuation, stuff would just never happen.
                    // Crash now, so that a Watson report would capture the original error.
                    Environment.FailFast("Failed to schedule time on the UI thread. A continuation would never execute.", ex);
                }
            }
        }

        private struct RunFramework : IDisposable
        {
            private readonly JoinableTaskFactory _factory;
            private readonly SpecializedSyncContext _syncContextRevert;
            private readonly JoinableTask _joinable;
            private readonly JoinableTask _previousJoinable;

            internal RunFramework(JoinableTaskFactory factory, JoinableTask joinable)
            {
                Validate.IsNotNull(factory, nameof(factory));
                Validate.IsNotNull(joinable, nameof(joinable));

                _factory = factory;
                _joinable = joinable;
                _factory.Add(joinable);
                _previousJoinable = _factory.Context.AmbientTask;
                _factory.Context.AmbientTask = joinable;
                _syncContextRevert = _joinable.ApplicableJobSyncContext.Apply();

                // Join the ambient parent job, so the parent can dequeue this job's work.
                if (_previousJoinable != null && !_previousJoinable.IsCompleted)
                {
                    _previousJoinable.AddDependency(joinable);

                    // By definition we inherit the nesting factories of our immediate nesting task.
                    var nestingFactories = _previousJoinable.NestingFactories;

                    // And we may add our immediate nesting parent's factory to the list of
                    // ancestors if it isn't already in the list.
                    if (_previousJoinable.Factory != _factory)
                    {
                        if (!nestingFactories.Contains(_previousJoinable.Factory))
                        {
                            nestingFactories.Add(_previousJoinable.Factory);
                        }
                    }

                    _joinable.NestingFactories = nestingFactories;
                }
            }

            /// <summary>
            /// Reverts the execution context to its previous state before this struct was created.
            /// </summary>
            public void Dispose()
            {
                _syncContextRevert.Dispose();
                _factory.Context.AmbientTask = _previousJoinable;
            }
        }

        internal class SingleExecuteProtector
        {
            internal static readonly SendOrPostCallback ExecuteOnce = state => ((SingleExecuteProtector)state).TryExecute();
            internal static readonly WaitCallback ExecuteOnceWaitCallback = state => ((SingleExecuteProtector)state).TryExecute();
            private JoinableTask _job;

            private bool _raiseTransitionComplete;
            private object _invokeDelegate;
            private object _state;
            private ListOfOftenOne<ExecutionQueue> _executingCallbacks;

            internal bool HasBeenExecuted => _invokeDelegate == null;

            internal string DelegateLabel => WalkAsyncReturnStackFrames().First();

            private SingleExecuteProtector(JoinableTask job)
            {
                Validate.IsNotNull(job, nameof(job));
                _job = job;
            }

            internal void AddExecutingCallback(ExecutionQueue callbackReceiver)
            {
                if (!HasBeenExecuted)
                {
                    _executingCallbacks.Add(callbackReceiver);
                }
            }

            internal void RemoveExecutingCallback(ExecutionQueue callbackReceiver)
            {
                _executingCallbacks.Remove(callbackReceiver);
            }

            internal IEnumerable<string> WalkAsyncReturnStackFrames()
            {
                // This instance might be a wrapper of another instance of "SingleExecuteProtector".
                // If that is true, we need to follow the chain to find the inner instance of "SingleExecuteProtector".
                var singleExecuteProtector = this;
                while (singleExecuteProtector._state is SingleExecuteProtector)
                {
                    singleExecuteProtector = (SingleExecuteProtector)singleExecuteProtector._state;
                }

                var invokeDelegate = singleExecuteProtector._invokeDelegate as Delegate;

                // We are in favor of "state" when "invokeDelegate" is a static method and "state" is the actual delegate.
                var actualDelegate = (singleExecuteProtector._state is Delegate stateDelegate && stateDelegate.Target != null) ? stateDelegate : invokeDelegate;
                if (actualDelegate == null)
                {
                    yield return "<COMPLETED>";
                    yield break;
                }

                foreach (var frame in actualDelegate.GetAsyncReturnStackFrames())
                {
                    yield return frame;
                }
            }

            internal void RaiseTransitioningEvents()
            {
                if (_raiseTransitionComplete)
                    throw new Exception();
                _raiseTransitionComplete = true;
                _job.Factory.OnTransitioningToMainThread(_job);
            }

            internal static SingleExecuteProtector Create(JoinableTask job, Action action)
            {
                return new SingleExecuteProtector(job)
                {
                    _invokeDelegate = action,
                };
            }

            internal static SingleExecuteProtector Create(JoinableTask job, SendOrPostCallback callback, object state)
            {
                Validate.IsNotNull(job, nameof(job));
                if (callback == ExecuteOnce && state is SingleExecuteProtector existing && job == existing._job)
                    return existing;

                return new SingleExecuteProtector(job)
                {
                    _invokeDelegate = callback,
                    _state = state,
                };
            }

            internal bool TryExecute()
            {
                var invokeDelegate = Interlocked.Exchange(ref _invokeDelegate, null);
                if (invokeDelegate != null)
                {
                    OnExecuting();
                    var syncContext = _job != null ? _job.ApplicableJobSyncContext : _job.Factory.ApplicableJobSyncContext;
                    using (syncContext.Apply(checkForChangesOnRevert: false))
                    {
                        if (invokeDelegate is Action action)
                        {
                            action();
                        }
                        else
                        {
                            var callback = (SendOrPostCallback)invokeDelegate;
                            callback(_state);
                        }

                        // Release the rest of the memory we're referencing.
                        _state = null;
                        _job = null;
                    }

                    return true;
                }

                return false;
            }

            private void OnExecuting()
            {
                if (ThreadingEventSource.Instance.IsEnabled())
                {
                    ThreadingEventSource.Instance.PostExecutionStop(GetHashCode());
                }

                // While raising the event, automatically remove the handlers since we'll only
                // raise them once, and we'd like to avoid holding references that may extend
                // the lifetime of our recipients.
                using (var enumerator = _executingCallbacks.EnumerateAndClear())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.OnExecuting(this, EventArgs.Empty);
                    }
                }

                if (_raiseTransitionComplete)
                {
                    _job.Factory.OnTransitionedToMainThread(_job, !_job.Factory.Context.IsOnMainThread);
                }
            }
        }
    }
}
