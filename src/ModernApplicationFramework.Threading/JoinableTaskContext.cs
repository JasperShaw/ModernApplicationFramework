using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    /// <summary>
    /// A common context within which joinable tasks may be created and interact to avoid deadlocks.
    /// </summary>
    /// <devremarks>
    /// Lots of documentation and FAQ on Joinable Tasks is available on OneNote: <![CDATA[
    /// http://devdiv/sites/vspe/prjbld/_layouts/OneNote.aspx?id=%2fsites%2fvspe%2fprjbld%2fOneNote%2fTeamInfo&wd=target%28VS%20Threading.one%7c46FEAAD0-0131-45EE-8C52-C9893F1FD331%2fThreading%20Rules%7cD0EEFAB9-99C0-4B8F-AA5F-4287DD69A38F%2f%29
    /// ]]>
    /// </devremarks>
    /// <remarks>
    /// There are three rules that should be strictly followed when using or interacting
    /// with JoinableTasks:
    ///  1. If a method has certain thread apartment requirements (STA or MTA) it must either:
    ///       a) Have an asynchronous signature, and asynchronously marshal to the appropriate
    ///          thread if it isn't originally invoked on a compatible thread.
    ///          The recommended way to switch to the main thread is:
    ///          <code>
    ///          await JoinableTaskFactory.SwitchToMainThreadAsync();
    ///          </code>
    ///       b) Have a synchronous signature, and throw an exception when called on the wrong thread.
    ///     In particular, no method is allowed to synchronously marshal work to another thread
    ///     (blocking while that work is done). Synchronous blocks in general are to be avoided
    ///     whenever possible.
    ///  2. When an implementation of an already-shipped public API must call asynchronous code
    ///     and block for its completion, it must do so by following this simple pattern:
    ///     <code>
    ///     JoinableTaskFactory.Run(async delegate {
    ///         await SomeOperationAsync(...);
    ///     });
    ///     </code>
    ///  3. If ever awaiting work that was started earlier, that work must be Joined.
    ///     For example, one service kicks off some asynchronous work that may later become
    ///     synchronously blocking:
    ///     <code>
    ///     JoinableTask longRunningAsyncWork = JoinableTaskFactory.RunAsync(async delegate {
    ///         await SomeOperationAsync(...);
    ///     });
    ///     </code>
    ///     Then later that async work becomes blocking:
    ///     <code>
    ///     longRunningAsyncWork.Join();
    ///     </code>
    ///     or perhaps:
    ///     <code>
    ///     await longRunningAsyncWork;
    ///     </code>
    ///     Note however that this extra step is not necessary when awaiting is done
    ///     immediately after kicking off an asynchronous operation.
    /// </remarks>
    public class JoinableTaskContext : IDisposable, IHangReportContributor
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _syncContextLock = new object();
        private readonly AsyncLocal<WeakReference<JoinableTask>> _joinableOperation = new AsyncLocal<WeakReference<JoinableTask>>();
        private readonly HashSet<JoinableTask> _pendingTasks = new HashSet<JoinableTask>();
        private readonly Stack<JoinableTask> _initializingSynchronouslyMainThreadTasks = new Stack<JoinableTask>(2);
        private readonly HashSet<JoinableTaskContextNode> _hangNotifications = new HashSet<JoinableTaskContextNode>();
        private readonly int _mainThreadManagedThreadId;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JoinableTaskFactory _nonJoinableFactory;

        /// <summary>
        /// Gets the factory which creates joinable tasks
        /// that do not belong to a joinable task collection.
        /// </summary>
        public JoinableTaskFactory Factory
        {
            get
            {
                using (NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (SyncContextLock)
                    {
                        return _nonJoinableFactory ?? (_nonJoinableFactory = CreateDefaultFactory());
                    }
                }
            }
        }

        /// <summary>
        /// Gets the main thread that can be shared by tasks created by this context.
        /// </summary>
        public Thread MainThread { get; }

        /// <summary>
        /// Gets a value indicating whether the caller is executing on the main thread.
        /// </summary>
        public bool IsOnMainThread => Environment.CurrentManagedThreadId == _mainThreadManagedThreadId;

        /// <summary>
        /// Gets a value indicating whether the caller is currently running within the context of a joinable task.
        /// </summary>
        /// <remarks>
        /// Use of this property is generally discouraged, as any operation that becomes a no-op when no
        /// ambient JoinableTask is present is very cheap. For clients that have complex algorithms that are
        /// only relevant if an ambient joinable task is present, this property may serve to skip that for
        /// performance reasons.
        /// </remarks>
        public bool IsWithinJoinableTask => AmbientTask != null;


        internal SynchronizationContext UnderlyingSynchronizationContext { get; }

        internal object SyncContextLock => _syncContextLock;

        internal JoinableTask AmbientTask
        {
            get
            {
                JoinableTask result = null;
                _joinableOperation.Value?.TryGetTarget(out result);
                return result;
            }

            set => _joinableOperation.Value = value?.WeakSelf;
        }

        protected internal virtual SynchronizationContext NoMessagePumpSynchronizationContext
        {
            get
            {
                Debugger.NotifyOfCrossThreadDependency();
                return NoMessagePumpSyncContext.Default;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinableTaskContext"/> class
        /// assuming the current thread is the main thread and
        /// <see cref="SynchronizationContext.Current"/> will provide the means to switch
        /// to the main thread from another thread.
        /// </summary>
        public JoinableTaskContext() : this(Thread.CurrentThread, SynchronizationContext.Current)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinableTaskContext"/> class.
        /// </summary>
        /// <param name="mainThread">
        /// The thread to switch to in <see cref="JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken)"/>.
        /// If omitted, the current thread will be assumed to be the main thread.
        /// </param>
        /// <param name="synchronizationContext">
        /// The synchronization context to use to switch to the main thread.
        /// </param>
        public JoinableTaskContext(Thread mainThread = null, SynchronizationContext synchronizationContext = null)
        {
            MainThread = mainThread ?? Thread.CurrentThread;
            _mainThreadManagedThreadId = MainThread.ManagedThreadId;
            UnderlyingSynchronizationContext = synchronizationContext ?? SynchronizationContext.Current; // may still be null after this.
        }

        /// <summary>
        /// Conceals any JoinableTask the caller is associated with until the returned value is disposed.
        /// </summary>
        /// <returns>A value to dispose of to restore visibility into the caller's associated JoinableTask, if any.</returns>
        /// <remarks>
        /// <para>In some cases asynchronous work may be spun off inside a delegate supplied to Run,
        /// so that the work does not have privileges to re-enter the Main thread until the
        /// <see cref="JoinableTaskFactory.Run(Func{Task})"/> call has returned and the UI thread is idle.
        /// To prevent the asynchronous work from automatically being allowed to re-enter the Main thread,
        /// wrap the code that calls the asynchronous task in a <c>using</c> block with a call to this method
        /// as the expression.</para>
        /// <example>
        /// <code>
        /// this.JoinableTaskContext.RunSynchronously(async delegate {
        ///     using(this.JoinableTaskContext.SuppressRelevance()) {
        ///         var asyncOperation = Task.Run(async delegate {
        ///             // Some background work.
        ///             await this.JoinableTaskContext.SwitchToMainThreadAsync();
        ///             // Some Main thread work, that cannot begin until the outer RunSynchronously call has returned.
        ///         });
        ///     }
        ///
        ///     // Because the asyncOperation is not related to this Main thread work (it was suppressed),
        ///     // the following await *would* deadlock if it were uncommented.
        ///     ////await asyncOperation;
        /// });
        /// </code>
        /// </example>
        /// </remarks>
        public RevertRelevance SuppressRelevance()
        {
            return new RevertRelevance(this);
        }

        /// <summary>
        /// Gets a value indicating whether the main thread is blocked for the caller's completion.
        /// </summary>
        public bool IsMainThreadBlocked()
        {
            var ambientTask = AmbientTask;
            if (ambientTask != null)
            {
                if (ambientTask.HasMainThreadSynchronousTaskWaiting)
                {
                    return true;
                }

                // The JoinableTask dependent chain gives a fast way to check IsMainThreadBlocked.
                // However, it only works when the main thread tasks is in the CompleteOnCurrentThread loop.
                // The dependent chain won't be added when a synchronous task is in the initialization phase.
                // In that case, we still need to follow the descendent of the task in the initialization stage.
                // We hope the dependency tree is relatively small in that stage.
                using (NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (SyncContextLock)
                    {
                        var allJoinedJobs = new HashSet<JoinableTask>();
                        lock (_initializingSynchronouslyMainThreadTasks)
                        {
                            // our read lock doesn't cover this collection
                            foreach (var initializingTask in _initializingSynchronouslyMainThreadTasks)
                            {
                                if (!initializingTask.HasMainThreadSynchronousTaskWaiting)
                                {
                                    // This task blocks the main thread. If it has joined the ambient task
                                    // directly or indirectly, then our ambient task is considered blocking
                                    // the main thread.
                                    initializingTask.AddSelfAndDescendentOrJoinedJobs(allJoinedJobs);
                                    if (allJoinedJobs.Contains(ambientTask))
                                    {
                                        return true;
                                    }

                                    allJoinedJobs.Clear();
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a joinable task factory that automatically adds all created tasks
        /// to a collection that can be jointly joined.
        /// </summary>
        /// <param name="collection">The collection that all tasks should be added to.</param>
        public virtual JoinableTaskFactory CreateFactory(JoinableTaskCollection collection)
        {
            Validate.IsNotNull(collection, nameof(collection));
            return new JoinableTaskFactory(collection);
        }

        /// <summary>
        /// Creates a collection for in-flight joinable tasks.
        /// </summary>
        /// <returns>A new joinable task collection.</returns>
        public JoinableTaskCollection CreateCollection()
        {
            return new JoinableTaskCollection(this);
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
        /// Invoked when a hang is suspected to have occurred involving the main thread.
        /// </summary>
        /// <param name="hangDuration">The duration of the current hang.</param>
        /// <param name="notificationCount">The number of times this hang has been reported, including this one.</param>
        /// <param name="hangId">A random GUID that uniquely identifies this particular hang.</param>
        /// <remarks>
        /// A single hang occurrence may invoke this method multiple times, with increasing
        /// values in the <paramref name="hangDuration"/> parameter.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected internal virtual void OnHangDetected(TimeSpan hangDuration, int notificationCount, Guid hangId)
        {
            List<JoinableTaskContextNode> listeners;
            using (NoMessagePumpSynchronizationContext.Apply())
            {
                lock (_hangNotifications)
                {
                    listeners = _hangNotifications.ToList();
                }
            }

            var blockingTask = JoinableTask.TaskCompletingOnThisThread;
            var hangDetails = new HangDetails(
                hangDuration,
                notificationCount,
                hangId,
                blockingTask?.EntryMethodInfo);
            foreach (var listener in listeners)
            {
                try
                {
                    listener.OnHangDetected(hangDetails);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Invoked when an earlier hang report is false alarm.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected internal virtual void OnFalseHangDetected(TimeSpan hangDuration, Guid hangId)
        {
            List<JoinableTaskContextNode> listeners;
            using (NoMessagePumpSynchronizationContext.Apply())
            {
                lock (_hangNotifications)
                {
                    listeners = _hangNotifications.ToList();
                }
            }

            foreach (var listener in listeners)
            {
                try
                {
                    listener.OnFalseHangDetected(hangDuration, hangId);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Creates a factory without a <see cref="JoinableTaskCollection"/>.
        /// </summary>
        /// <remarks>
        /// Used for initializing the <see cref="Factory"/> property.
        /// </remarks>
        protected internal virtual JoinableTaskFactory CreateDefaultFactory()
        {
            return new JoinableTaskFactory(this);
        }

        internal void OnJoinableTaskStarted(JoinableTask task)
        {
            Validate.IsNotNull(task, nameof(task));

            using (NoMessagePumpSynchronizationContext.Apply())
            {
                lock (_pendingTasks)
                {
                    if (!(_pendingTasks.Add(task)))
                        throw new Exception();
                }

                if ((task.State & JoinableTask.JoinableTaskFlags.SynchronouslyBlockingMainThread) == JoinableTask.JoinableTaskFlags.SynchronouslyBlockingMainThread)
                {
                    lock (_initializingSynchronouslyMainThreadTasks)
                    {
                        _initializingSynchronouslyMainThreadTasks.Push(task);
                    }
                }
            }
        }

        internal void OnJoinableTaskCompleted(JoinableTask task)
        {
            Validate.IsNotNull(task, nameof(task));

            using (NoMessagePumpSynchronizationContext.Apply())
            {
                lock (_pendingTasks)
                {
                    _pendingTasks.Remove(task);
                }
            }
        }

        internal void OnSynchronousJoinableTaskToCompleteOnMainThread(JoinableTask task)
        {
            Validate.IsNotNull(task, nameof(task));

            using (NoMessagePumpSynchronizationContext.Apply())
            {
                lock (_initializingSynchronouslyMainThreadTasks)
                {
                    if (!(_initializingSynchronouslyMainThreadTasks.Count > 0))
                        throw new Exception();
                    if (_initializingSynchronouslyMainThreadTasks.Peek() != task)
                        throw new Exception();

                    _initializingSynchronouslyMainThreadTasks.Pop();
                }
            }
        }

        internal IDisposable RegisterHangNotifications(JoinableTaskContextNode node)
        {
            Validate.IsNotNull(node, nameof(node));

            using (NoMessagePumpSynchronizationContext.Apply())
            {
                lock (_hangNotifications)
                {
                    if (!_hangNotifications.Add(node))
                    {
                        throw new InvalidOperationException("Context Node already registered");
                    }
                }
            }

            return new HangNotificationRegistration(node);
        }

        /// <inheritdoc />
        /// <summary>
        /// Contributes data for a hang report.
        /// </summary>
        /// <returns>The hang report contribution.</returns>
        HangReportContribution IHangReportContributor.GetHangReport()
        {
            return GetHangReport();
        }

        /// <summary>
        /// Contributes data for a hang report.
        /// </summary>
        /// <returns>The hang report contribution. Null values should be ignored.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual HangReportContribution GetHangReport()
        {
            using (NoMessagePumpSynchronizationContext.Apply())
            {
                lock (SyncContextLock)
                {
                    var dgml = CreateTemplateDgml(out var nodes, out var links);

                    var pendingTasksElements = CreateNodesForPendingTasks();
                    var taskLabels = CreateNodeLabels(pendingTasksElements);
                    var pendingTaskCollections = CreateNodesForJoinableTaskCollections(pendingTasksElements.Keys);
                    nodes.Add(pendingTasksElements.Values);
                    nodes.Add(pendingTaskCollections.Values);
                    nodes.Add(taskLabels.Select(t => t.Item1));
                    links.Add(CreatesLinksBetweenNodes(pendingTasksElements));
                    links.Add(CreateCollectionContainingTaskLinks(pendingTasksElements, pendingTaskCollections));
                    links.Add(taskLabels.Select(t => t.Item2));

                    return new HangReportContribution(
                        dgml.ToString(),
                        "application/xml",
                        "JoinableTaskContext.dgml");
                }
            }
        }

        private static XDocument CreateTemplateDgml(out XElement nodes, out XElement links)
        {
            return Dgml.Create(out nodes, out links)
                .WithCategories(
                    Dgml.Category("MainThreadBlocking", "Blocking main thread", "#FFF9FF7F", isTag: true),
                    Dgml.Category("NonEmptyQueue", "Non-empty queue", "#FFFF0000", isTag: true));
        }

        private static ICollection<XElement> CreatesLinksBetweenNodes(Dictionary<JoinableTask, XElement> pendingTasksElements)
        {
            Validate.IsNotNull(pendingTasksElements, nameof(pendingTasksElements));

            var links = new List<XElement>();
            foreach (var joinableTaskAndElement in pendingTasksElements)
            {
                foreach (var joinedTask in joinableTaskAndElement.Key.ChildOrJoinedJobs)
                {
                    if (pendingTasksElements.TryGetValue(joinedTask, out XElement joinedTaskElement))
                    {
                        links.Add(Dgml.Link(joinableTaskAndElement.Value, joinedTaskElement));
                    }
                }
            }

            return links;
        }

        private static ICollection<XElement> CreateCollectionContainingTaskLinks(Dictionary<JoinableTask, XElement> tasks, Dictionary<JoinableTaskCollection, XElement> collections)
        {
            Validate.IsNotNull(tasks, nameof(tasks));
            Validate.IsNotNull(collections, nameof(collections));

            var result = new List<XElement>();
            foreach (var task in tasks)
            {
                foreach (var collection in task.Key.ContainingCollections)
                {
                    var collectionElement = collections[collection];
                    result.Add(Dgml.Link(collectionElement, task.Value).WithCategories("Contains"));
                }
            }

            return result;
        }

        private static Dictionary<JoinableTaskCollection, XElement> CreateNodesForJoinableTaskCollections(IEnumerable<JoinableTask> tasks)
        {
            Validate.IsNotNull(tasks, nameof(tasks));

            var collectionsSet = new HashSet<JoinableTaskCollection>(tasks.SelectMany(t => t.ContainingCollections));
            var result = new Dictionary<JoinableTaskCollection, XElement>(collectionsSet.Count);
            int collectionId = 0;
            foreach (var collection in collectionsSet)
            {
                collectionId++;
                var label = string.IsNullOrEmpty(collection.DisplayName) ? "Collection #" + collectionId : collection.DisplayName;
                var element = Dgml.Node("Collection#" + collectionId, label, "Expanded")
                    .WithCategories("Collection");
                result.Add(collection, element);
            }

            return result;
        }

        private static List<Tuple<XElement, XElement>> CreateNodeLabels(Dictionary<JoinableTask, XElement> tasksAndElements)
        {
            Validate.IsNotNull(tasksAndElements, nameof(tasksAndElements));

            var result = new List<Tuple<XElement, XElement>>();
            foreach (var tasksAndElement in tasksAndElements)
            {
                var pendingTask = tasksAndElement.Key;
                var node = tasksAndElement.Value;
                int queueIndex = 0;
                foreach (var pendingTasksElement in pendingTask.MainThreadQueueContents)
                {
                    queueIndex++;
                    var callstackNode = Dgml.Node(node.Attribute("Id").Value + "MTQueue#" + queueIndex, GetAsyncReturnStack(pendingTasksElement));
                    var callstackLink = Dgml.Link(callstackNode, node);
                    result.Add(Tuple.Create(callstackNode, callstackLink));
                }

                foreach (var pendingTasksElement in pendingTask.ThreadPoolQueueContents)
                {
                    queueIndex++;
                    var callstackNode = Dgml.Node(node.Attribute("Id").Value + "TPQueue#" + queueIndex, GetAsyncReturnStack(pendingTasksElement));
                    var callstackLink = Dgml.Link(callstackNode, node);
                    result.Add(Tuple.Create(callstackNode, callstackLink));
                }
            }

            return result;
        }

        private Dictionary<JoinableTask, XElement> CreateNodesForPendingTasks()
        {
            var pendingTasksElements = new Dictionary<JoinableTask, XElement>();
            lock (_pendingTasks)
            {
                int taskId = 0;
                foreach (var pendingTask in _pendingTasks)
                {
                    taskId++;

                    string methodName = string.Empty;
                    var entryMethodInfo = pendingTask.EntryMethodInfo;
                    if (entryMethodInfo != null)
                    {
                        methodName = string.Format(
                            CultureInfo.InvariantCulture,
                            " ({0}.{1})",
                            entryMethodInfo.DeclaringType.FullName,
                            entryMethodInfo.Name);
                    }

                    var node = Dgml.Node("Task#" + taskId, "Task #" + taskId + methodName)
                        .WithCategories("Task");
                    if (pendingTask.HasNonEmptyQueue)
                    {
                        node.WithCategories("NonEmptyQueue");
                    }

                    if (pendingTask.State.HasFlag(JoinableTask.JoinableTaskFlags.SynchronouslyBlockingMainThread))
                    {
                        node.WithCategories("MainThreadBlocking");
                    }

                    pendingTasksElements.Add(pendingTask, node);
                }
            }

            return pendingTasksElements;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static string GetAsyncReturnStack(JoinableTaskFactory.SingleExecuteProtector singleExecuteProtector)
        {
            Validate.IsNotNull(singleExecuteProtector, nameof(singleExecuteProtector));

            var stringBuilder = new StringBuilder();
            try
            {
                foreach (var frame in singleExecuteProtector.WalkAsyncReturnStackFrames())
                {
                    stringBuilder.AppendLine(frame);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return stringBuilder.ToString().TrimEnd();
        }


        /// <summary>
        /// A structure that clears CallContext and SynchronizationContext async/thread statics and
        /// restores those values when this structure is disposed.
        /// </summary>
        public struct RevertRelevance : IDisposable
        {
            private readonly JoinableTaskContext _pump;
            private SpecializedSyncContext _temporarySyncContext;
            private readonly JoinableTask _oldJoinable;

            /// <summary>
            /// Initializes a new instance of the <see cref="RevertRelevance"/> struct.
            /// </summary>
            /// <param name="pump">The instance that created this value.</param>
            internal RevertRelevance(JoinableTaskContext pump)
            {
                Validate.IsNotNull(pump, nameof(pump));
                _pump = pump;

                _oldJoinable = pump.AmbientTask;
                pump.AmbientTask = null;

                if (SynchronizationContext.Current is JoinableTaskSynchronizationContext jobSyncContext)
                {
                    SynchronizationContext appliedSyncContext = null;
                    if (jobSyncContext.MainThreadAffinitized)
                    {
                        appliedSyncContext = pump.UnderlyingSynchronizationContext;
                    }

                    _temporarySyncContext = appliedSyncContext.Apply(); // Apply() extension method allows null receiver
                }
                else
                {
                    _temporarySyncContext = default;
                }
            }

            /// <summary>
            /// Reverts the async local and thread static values to their original values.
            /// </summary>
            public void Dispose()
            {
                if (_pump != null)
                {
                    _pump.AmbientTask = _oldJoinable;
                }

                _temporarySyncContext.Dispose();
            }
        }

        private class HangNotificationRegistration : IDisposable
        {
            private JoinableTaskContextNode _node;

            internal HangNotificationRegistration(JoinableTaskContextNode node)
            {
                Validate.IsNotNull(node, nameof(node));
                _node = node;
            }

            public void Dispose()
            {
                var node = _node;
                if (node != null)
                {
                    using (node.Context.NoMessagePumpSynchronizationContext.Apply())
                    {
                        lock (node.Context._hangNotifications)
                        {
                            if (!(node.Context._hangNotifications.Remove(node)))
                                throw new Exception();
                        }
                    }

                    _node = null;
                }
            }
        }

        /// <summary>
        /// A class to encapsulate the details of a possible hang.
        /// An instance of this <see cref="HangDetails"/> class will be passed to the
        /// <see cref="JoinableTaskContextNode"/> instances who registered the hang notifications.
        /// </summary>
        public class HangDetails
        {
            /// <summary>Initializes a new instance of the <see cref="HangDetails"/> class.</summary>
            /// <param name="hangDuration">The duration of the current hang.</param>
            /// <param name="notificationCount">The number of times this hang has been reported, including this one.</param>
            /// <param name="hangId">A random GUID that uniquely identifies this particular hang.</param>
            /// <param name="entryMethod">The method that served as the entrypoint for the JoinableTask.</param>
            public HangDetails(TimeSpan hangDuration, int notificationCount, Guid hangId, MethodInfo entryMethod)
            {
                HangDuration = hangDuration;
                NotificationCount = notificationCount;
                HangId = hangId;
                EntryMethod = entryMethod;
            }

            /// <summary>
            /// Gets the length of time this hang has lasted so far.
            /// </summary>
            public TimeSpan HangDuration { get; }

            /// <summary>
            /// Gets the number of times this particular hang has been reported, including this one.
            /// </summary>
            public int NotificationCount { get; }

            /// <summary>
            /// Gets a unique GUID identifying this particular hang.
            /// If the same hang is reported multiple times (with increasing duration values)
            /// the value of this property will remain constant.
            /// </summary>
            public Guid HangId { get; }

            /// <summary>
            /// Gets the method that served as the entrypoint for the JoinableTask that now blocks a thread.
            /// </summary>
            /// <remarks>
            /// The method indicated here may not be the one that is actually blocking a thread,
            /// but typically a deadlock is caused by a violation of a threading rule which is under
            /// the entrypoint's control. So usually regardless of where someone chooses the block
            /// a thread for the completion of a <see cref="JoinableTask"/>, a hang usually indicates
            /// a bug in the code that created it.
            /// This value may be used to assign the hangs to different buckets based on this method info.
            /// </remarks>
            public MethodInfo EntryMethod { get; }
        }
    }
}
