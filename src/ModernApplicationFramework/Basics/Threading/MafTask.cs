using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Services.TaskSchedulerService;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Threading;
using ModernApplicationFramework.Threading;
using ModernApplicationFramework.Utilities;
using Action = System.Action;

namespace ModernApplicationFramework.Basics.Threading
{
    internal class MafTask : IMafTaskEvents, INotifyPropertyChanged, IMafTask
    {
        private static readonly PropertyChangedEventArgs IsCanceledPropertyChangedEventArgs =
            new PropertyChangedEventArgs(nameof(IsCanceled));

        private static readonly PropertyChangedEventArgs IsCompletedPropertyChangedEventArgs =
            new PropertyChangedEventArgs(nameof(IsCompleted));

        private static readonly PropertyChangedEventArgs IsFaultedPropertyChangedEventArgs =
            new PropertyChangedEventArgs(nameof(IsFaulted));

        private static readonly PropertyChangedEventArgs TaskStatePropertyChangedEventArgs =
            new PropertyChangedEventArgs(nameof(TaskState));

        private List<MafTask> _attachedTasks;
        private JoinableTask _dependencyJoinableTask;
        private string _description;
        private Task<object> _internalTask;
        private JoinableTask _joinableTask;
        private AsyncAutoResetEvent _newDependencyWaiter;
        private MafTaskState _taskState;
        private CancellationTokenSource _uiThreadWaitAbortToken;
        private string _waitMessage;

        public event EventHandler OnBlockingWaitBegin;
        public event EventHandler OnBlockingWaitEnd;
        public event EventHandler<BlockingTaskEventArgs> OnMarkedAsBlocking;
        public event PropertyChangedEventHandler PropertyChanged;

        public object AsyncState { get; }

        public DateTime CreationTime { get; }

        public IMafTask[] DependentTasks { get; private set; }

        public int DependentTasksCount { get; }

        public string Description
        {
            get => _description;
            set
            {
                if (_description == value)
                    return;
                _description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        public bool IsCanceled
        {
            get
            {
                if (InternalTask != null && InternalTask.Status == TaskStatus.Canceled)
                    return true;
                if (IsCancelable)
                    return TaskCancellationTokenSource.IsCancellationRequested;
                return false;
            }
        }

        public bool IsCompleted
        {
            get
            {
                if (InternalTask == null)
                    return false;
                if (!InternalTask.IsCompleted)
                    return InternalTask.IsFaulted;
                return true;
            }
        }

        public bool IsFaulted
        {
            get
            {
                if (InternalTask != null)
                    return InternalTask.IsFaulted;
                return false;
            }
        }

        public MafTaskRunContext TaskContext { get; }

        public MafTaskState TaskState
        {
            get => _taskState;
            set
            {
                if (_taskState == value)
                    return;
                _taskState = value;
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, TaskStatePropertyChangedEventArgs);
                    propertyChanged(this, IsCanceledPropertyChangedEventArgs);
                    propertyChanged(this, IsFaultedPropertyChangedEventArgs);
                    propertyChanged(this, IsCompletedPropertyChangedEventArgs);
                }
            }
        }

        public string WaitMessage
        {
            get => _waitMessage;
            set
            {
                if (_waitMessage == value)
                    return;
                _waitMessage = value;
                RaisePropertyChanged(nameof(WaitMessage));
            }
        }

        CancellationToken IMafTask.CancellationToken => TaskCancellationToken;

        internal Task<object> InternalTask
        {
            get
            {
                if (_internalTask == null)
                    SpinWait.SpinUntil(() => _internalTask != null, TimeSpan.FromSeconds(10.0));
                return _internalTask;
            }
            private set
            {
                Validate.IsNotNull(value, nameof(value));
                if (_internalTask != null)
                    throw new InvalidOperationException("InternalTask is already set.");
                _internalTask = value;
            }
        }

        protected bool IsCancelable { get; }

        protected CancellationToken TaskCancellationToken
        {
            get
            {
                if (!IsCancelable)
                    return CancellationToken.None;
                return TaskCancellationTokenSource.Token;
            }
        }

        protected CancellationTokenSource TaskCancellationTokenSource { get; }

        private static bool IsCallerInJoinableTask =>
            MafTaskSchedulerService.Instance.JoinableTaskContext.IsWithinJoinableTask;

        private TaskScheduler AssignedScheduler => GetSchedulerFromContext(TaskContext);


        internal MafTask(IMafTaskBody action, MafTaskRunContext context, MafTaskCreationOptions options,
            object asyncState)
            : this(null, context, asyncState, !OptionsHasFlag(options, MafTaskCreationOptions.NotCancelable),
                OptionsHasFlag(options, MafTaskCreationOptions.CancelWithParent), false)
        {
            InternalTask = new Task<object>(GetCallback(action, this, context), TaskCancellationToken,
                GetTplOptions(options));
        }

        internal MafTask(TaskCompletionSource<object> completionSource)
            : this(null, MafTaskRunContext.BackgroundThread, completionSource.Task.AsyncState, true, false, false)
        {
            InternalTask = completionSource.Task;
        }

        protected MafTask(IMafTask[] dependentTasks, MafTaskRunContext context, object asyncState, bool isCancelable,
            bool isCanceledWithParent, bool isIndependentlyCanceled)
        {
            DependentTasks = dependentTasks;
            DependentTasksCount = dependentTasks?.Length ?? 0;
            TaskContext = CalculateTaskRunContext(context);
            TaskState = MafTaskState.Created;
            AsyncState = asyncState;
            CreationTime = DateTime.UtcNow;
            IsCancelable = isCancelable;
            if (!IsCancelable)
                return;
            TaskCancellationTokenSource =
                isIndependentlyCanceled || DependentTasks == null || DependentTasks.Length != 1 ||
                !(DependentTasks[0] is MafTask) || !((MafTask) DependentTasks[0]).IsCancelable
                    ? new CancellationTokenSource()
                    : ((MafTask) DependentTasks[0]).TaskCancellationTokenSource;
            if (!isCanceledWithParent)
                return;
            MafRunningTasksManager.GetCurrentTask()?.TaskCancellationToken.Register(Cancel);
        }

        public void AbortIfCanceled()
        {
            if (!IsCanceled)
                return;
            TaskCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        public void Cancel()
        {
            if (!IsCancelable)
                throw new InvalidOperationException();
            TaskCancellationTokenSource.Cancel();
            TaskState = MafTaskState.Canceled;
        }

        public IMafTask ContinueWith(uint context, IMafTaskBody pTaskBody)
        {
            return ContinueWithEx(context, 262144U, pTaskBody, null);
        }

        public IMafTask ContinueWithEx(uint context, uint options, IMafTaskBody pTaskBody, object pAsyncState)
        {
            var options1 = (MafTaskContinuationOptions) options;
            var isCancelable = !OptionsHasFlag(options1, MafTaskContinuationOptions.NotCancelable);
            var isCanceledWithParent = OptionsHasFlag(options1, MafTaskContinuationOptions.CancelWithParent);
            var isIndependentlyCanceled = OptionsHasFlag(options1, MafTaskContinuationOptions.IndependentlyCanceled);
            var mafTask = new MafTask(new[]
            {
                (IMafTask) this
            }, (MafTaskRunContext) context, pAsyncState, isCancelable, isCanceledWithParent, isIndependentlyCanceled);
            mafTask.InternalTask = InternalTask.ContinueWith(
                GetCallbackForSingleParent(pTaskBody, mafTask, mafTask.TaskContext), mafTask.TaskCancellationToken,
                GetTplOptions(options1), mafTask.AssignedScheduler);
            mafTask.TaskState = MafTaskState.Scheduled;
            MafTaskSchedulerService.Instance.RaiseOnTaskCreatedEvent(mafTask);
            if (OptionsHasFlag(options1, MafTaskContinuationOptions.AttachedToParent))
                EnsureTaskIsNotBlocking(mafTask);
            JoinAntecedentJoinableTasks(mafTask);
            return mafTask;
        }

        public object GetResult()
        {
            return InternalGetResult(false);
        }

        public void Start()
        {
            TaskState = MafTaskState.Scheduled;
            InternalTask.Start(AssignedScheduler);
            if ((InternalTask.CreationOptions & TaskCreationOptions.AttachedToParent) == TaskCreationOptions.None)
                return;
            EnsureTaskIsNotBlocking(this);
        }


        public void Wait()
        {
            InternalWait(false, -1, false);
        }

        public bool WaitEx(int millisecondsTimeout, uint options)
        {
            var abortOnCancel = (options & 1U) > 0U;
            return InternalWait(false, millisecondsTimeout, abortOnCancel);
        }

        void IMafTask.AssociateJoinableTask(object joinableTask)
        {
            Validate.IsNotNull(joinableTask, nameof(joinableTask));
            if (_joinableTask != null)
                throw new InvalidOperationException();
            _joinableTask = (JoinableTask) joinableTask;
        }

        internal static MafTaskRunContext CalculateTaskRunContext(MafTaskRunContext inputContext)
        {
            var mafTaskRunContext = inputContext;
            if (inputContext == MafTaskRunContext.CurrentContext)
            {
                var flag = ThreadHelper.CheckAccess();
                mafTaskRunContext =
                    !(TaskScheduler.Current is IMafTaskScheduler current) || flag != current.IsUiThreadScheduler
                        ? (flag ? MafTaskRunContext.UiThreadNormalPriority : MafTaskRunContext.BackgroundThread)
                        : current.SchedulerContext;
            }

            return mafTaskRunContext;
        }

        internal static MafTask ContinueWhenAllCompleted(IMafTask[] tasks, IMafTaskBody taskBody,
            MafTaskRunContext context, MafTaskContinuationOptions options, object asyncState)
        {
            var tasks1 = new Task<object>[tasks.Length];
            for (var index = 0; index < tasks.Length; ++index)
                tasks1[index] = ((MafTask) tasks[index]).InternalTask;
            var isCancelable = !OptionsHasFlag(options, MafTaskContinuationOptions.NotCancelable);
            var isCanceledWithParent = OptionsHasFlag(options, MafTaskContinuationOptions.CancelWithParent);
            var isIndependentlyCanceled = OptionsHasFlag(options, MafTaskContinuationOptions.IndependentlyCanceled);
            var mafTask = new MafTask(tasks, context, asyncState, isCancelable, isCanceledWithParent,
                isIndependentlyCanceled);
            var taskFactory = new TaskFactory<object>(mafTask.AssignedScheduler);
            mafTask.InternalTask = taskFactory.ContinueWhenAll(tasks1,
                GetCallbackForMultipleParent(taskBody, mafTask, context), mafTask.TaskCancellationToken,
                GetTplOptions(options), mafTask.AssignedScheduler);
            MafTaskSchedulerService.Instance.RaiseOnTaskCreatedEvent(mafTask);
            if (OptionsHasFlag(options, MafTaskContinuationOptions.AttachedToParent))
                EnsureTaskIsNotBlocking(mafTask);
            mafTask.TaskState = MafTaskState.Scheduled;
            return mafTask;
        }

        internal static Func<object> GetCallback(IMafTaskBody taskBody, MafTask task, MafTaskRunContext context)
        {
            if (task.DependentTasks != null && task.DependentTasks.Length != 0)
                throw new ArgumentException("Invalid number of parent tasks", nameof(task));
            return () => GetCallbackForMultipleParent(taskBody, task, context)(null);
        }

        internal static Func<Task<object>[], object> GetCallbackForMultipleParent(IMafTaskBody taskBody, MafTask task,
            MafTaskRunContext context)
        {
            return _ =>
            {
                MafRunningTasksManager.PushCurrentTask(task);
                task.TaskState = MafTaskState.Running;
                object pResult;
                try
                {
                    if (MafTaskSchedulerService.Instance.TryAdaptTaskBody(task, taskBody, out var adaptedBody))
                        taskBody = adaptedBody;
                    taskBody.DoWork(task, task.DependentTasks != null ? (uint) task.DependentTasks.Length : 0U,
                        task.DependentTasks, out pResult);
                    task.TaskState = MafTaskState.Completed;
                }
                catch (Exception)
                {
                    task.TaskState = MafTaskState.Faulted;
                    throw;
                }
                finally
                {
                    if (Marshal.IsComObject(taskBody))
                        Marshal.ReleaseComObject(taskBody);
                    task.DependentTasks = null;
                    MafRunningTasksManager.PopCurrentTask();
                }

                return pResult;
            };
        }

        internal static Func<Task<object>, object> GetCallbackForSingleParent(IMafTaskBody taskBody, MafTask task,
            MafTaskRunContext context)
        {
            if (task.DependentTasks == null || task.DependentTasks.Length != 1)
                throw new ArgumentException("Invalid number of parent tasks", nameof(task));
            return internalTask => GetCallbackForMultipleParent(taskBody, task, context)(new Task<object>[1]
            {
                internalTask
            });
        }

        internal static TaskScheduler GetSchedulerFromContext(MafTaskRunContext context)
        {
            switch (context)
            {
                case MafTaskRunContext.BackgroundThread:
                    return TaskScheduler.Default;
                case MafTaskRunContext.UiThreadSend:
                    return MafTaskSchedulerService.Instance.UiContextScheduler;
                case MafTaskRunContext.UiThreadBackgroundPriority:
                    return MafTaskSchedulerService.Instance.UiContextBackgroundPriorityScheduler;
                case MafTaskRunContext.UiThreadIdlePriority:
                    return MafTaskSchedulerService.Instance.UiContextIdleTimeScheduler;
                case MafTaskRunContext.CurrentContext:
                    if (ThreadHelper.CheckAccess())
                    {
                        var taskRunContext = CalculateTaskRunContext(context);
                        if (taskRunContext != MafTaskRunContext.CurrentContext)
                            return GetSchedulerFromContext(taskRunContext);
                        return MafTaskSchedulerService.Instance.UiContextNormalPriorityScheduler;
                    }

                    if (SynchronizationContext.Current != null)
                        return TaskScheduler.FromCurrentSynchronizationContext();
                    return TaskScheduler.Default;
                case MafTaskRunContext.BackgroundThreadLowIoPriority:
                    return MafTaskSchedulerService.Instance.BackgroundThreadLowIoPriorityScheduler;
                case MafTaskRunContext.UiThreadNormalPriority:
                    return MafTaskSchedulerService.Instance.UiContextNormalPriorityScheduler;
                default:
                    throw new ArgumentException("Unknown task run context", nameof(context));
            }
        }

        internal static TaskCreationOptions GetTplOptions(MafTaskCreationOptions options)
        {
            return (TaskCreationOptions) (options & ~(MafTaskCreationOptions.CancelWithParent |
                                                      MafTaskCreationOptions.NotCancelable));
        }

        internal static TaskContinuationOptions GetTplOptions(MafTaskContinuationOptions options)
        {
            return (TaskContinuationOptions) (options & ~(MafTaskContinuationOptions.CancelWithParent |
                                                          MafTaskContinuationOptions.IndependentlyCanceled |
                                                          MafTaskContinuationOptions.NotCancelable) &
                                              ~MafTaskContinuationOptions.ExecuteSynchronously);
        }

        internal static void JoinAntecedentJoinableTasks(IMafTask task)
        {
            if (task is MafTask mafTask && IsCallerInJoinableTask)
                foreach (var allDependentVsTask in mafTask.GetAllDependentMafTasks(false))
                    if (allDependentVsTask._joinableTask != null)
                        allDependentVsTask._joinableTask.JoinAsync().Forget();
                    else if (allDependentVsTask._dependencyJoinableTask != null)
                        allDependentVsTask._dependencyJoinableTask.JoinAsync().Forget();
                    else
                        allDependentVsTask._dependencyJoinableTask =
                            MafTaskSchedulerService.Instance.JoinableTaskContext.Factory.RunAsync(async delegate
                            {
                                if (allDependentVsTask.AssignedScheduler is MafUiThreadBlockableTaskScheduler
                                    vsUiThreadBlockableTaskScheduler)
                                    vsUiThreadBlockableTaskScheduler
                                        .TryExecuteTaskAsync(allDependentVsTask.InternalTask).Forget();
                                var lastNotifiedAttachedTaskIndex = 0;
                                IEnumerable<MafTask> enumerable;
                                while ((enumerable = await allDependentVsTask
                                           .WaitForNewDependenciesAsync(lastNotifiedAttachedTaskIndex)
                                           .ConfigureAwait(false)) != null)
                                    foreach (var item in enumerable)
                                    {
                                        lastNotifiedAttachedTaskIndex++;
                                        JoinAntecedentJoinableTasks(item);
                                    }
                            });
        }

        internal void AbortWait()
        {
            var threadWaitAbortToken = _uiThreadWaitAbortToken;
            if (threadWaitAbortToken == null)
                return;
            IgnoreObjectDisposedException(threadWaitAbortToken.Cancel);
        }

        internal IEnumerable<Task> GetAllDependentInternalTasks()
        {
            foreach (var allDependentMafTask in GetAllDependentMafTasks())
                yield return allDependentMafTask.InternalTask;
        }

        internal IEnumerable<MafTask> GetAllDependentMafTasks(bool ignoreCanceledTasks = true)
        {
            var isActive = ignoreCanceledTasks
                ? (Func<IMafTask, bool>) (t =>
                {
                    if (!t.IsCompleted)
                        return !t.IsCanceled;
                    return false;
                })
                : t => !t.IsCompleted;
            var seenSet = new HashSet<MafTask>();
            var remaining = new Stack<MafTask>();
            remaining.Push(this);
            seenSet.Add(this);
            while (remaining.Count > 0)
            {
                var current = remaining.Pop();
                yield return current;
                var dependentTasks = current.DependentTasks;
                lock (current._attachedTasks)
                {
                    var attachedTasks = current._attachedTasks;
                    var taskArray = dependentTasks;
                    var second = taskArray ?? Enumerable.Empty<IMafTask>();
                    foreach (var task1 in attachedTasks.Union(second).Where(isActive))
                        if (task1 is MafTask task2 && seenSet.Add(task2))
                            remaining.Push(task2);
                }
            }
        }


        internal object InternalGetResult(bool ignoreUiThreadCheck)
        {
            if (InternalTask.IsFaulted)
                RethrowException(InternalTask.Exception);
            if (InternalTask.IsCanceled)
                throw new OperationCanceledException();
            if (!EnsureTaskIsNotBlocking(this, MafTaskDependency.WaitForExecution))
                throw new CircularTaskDependencyException();
            var taskResult = (object) null;
            try
            {
                var flag = ThreadHelper.CheckAccess() && !IsCompleted;
                if (!ignoreUiThreadCheck & flag)
                {
                    InvokeWithWaitDialog(() =>
                    {
                        try
                        {
                            var abortTokenSource = InitializeUiThreadWaitAbortToken();
                            try
                            {
                                IgnoreObjectDisposedException(() =>
                                    UiThreadReentrancyScope.WaitOnTaskComplete(InternalTask, abortTokenSource.Token,
                                        -1));
                            }
                            catch (OperationCanceledException)
                            {
                                var waitAbortRequested = false;
                                IgnoreObjectDisposedException(() =>
                                    waitAbortRequested = abortTokenSource.IsCancellationRequested);
                                if (waitAbortRequested)
                                    throw new TaskSchedulingException();
                            }

                            taskResult = InternalTask.Result;
                        }
                        finally
                        {
                            ClearUiThreadWaitAbortToken();
                        }
                    });
                }
                else
                {
                    if (flag)
                        UiThreadReentrancyScope.WaitOnTaskComplete(InternalTask, CancellationToken.None, -1);
                    taskResult = InternalTask.Result;
                }
            }
            catch (AggregateException ex)
            {
                RethrowException(ex);
            }

            return taskResult;
        }

        internal void InternalWait(bool ignoreUiThreadCheck)
        {
            InternalWait(ignoreUiThreadCheck, -1, false);
        }

        internal bool InternalWait(bool ignoreUiThreadCheck, int millisecondTimeout, bool abortOnCancel)
        {
            if (!EnsureTaskIsNotBlocking(this, MafTaskDependency.WaitForExecution))
                throw new CircularTaskDependencyException();
            try
            {
                if (!ignoreUiThreadCheck && ThreadHelper.CheckAccess() && !IsCompleted)
                    return InvokeWithWaitDialog(() =>
                    {
                        try
                        {
                            var abortTokenSource = InitializeUiThreadWaitAbortToken();
                            if (abortOnCancel)
                                TaskCancellationToken.Register(() =>
                                    IgnoreObjectDisposedException(abortTokenSource.Cancel));
                            var result = false;
                            try
                            {
                                IgnoreObjectDisposedException(() =>
                                    result = UiThreadReentrancyScope.WaitOnTaskComplete(InternalTask,
                                        abortTokenSource.Token, millisecondTimeout));
                            }
                            catch (OperationCanceledException)
                            {
                                var waitAbortRequested = false;
                                IgnoreObjectDisposedException(() =>
                                    waitAbortRequested = abortTokenSource.IsCancellationRequested);
                                if (waitAbortRequested &&
                                    (!abortOnCancel || !TaskCancellationToken.IsCancellationRequested))
                                    throw new TaskSchedulingException();
                                throw;
                            }

                            return result;
                        }
                        finally
                        {
                            ClearUiThreadWaitAbortToken();
                        }
                    });
                return UiThreadReentrancyScope.WaitOnTaskComplete(InternalTask,
                    abortOnCancel ? TaskCancellationToken : CancellationToken.None, millisecondTimeout);
            }
            catch (AggregateException ex)
            {
                throw RethrowException(ex);
            }
        }

        internal bool TryAddDependentTask(IMafTask task, bool checkForCycle)
        {
            Validate.IsNotNull(task, nameof(task));
            if (task is MafTask mafTask)
            {
                if (checkForCycle && mafTask.GetAllDependentMafTasks().Contains(this))
                    return false;
                lock (LazyInitializer.EnsureInitialized(ref _attachedTasks))
                {
                    _attachedTasks.Add((MafTask) task);
                }

                _newDependencyWaiter?.Set();
                if (MafRunningTasksManager.IsBlockingTask(InternalTask))
                {
                    foreach (var mafTask2 in mafTask.GetAllDependentMafTasks().Distinct())
                        mafTask2.RaiseOnMarkedAsBlockingEvent(
                            MafRunningTasksManager.GetCurrentTaskWaitedOnUiThread() ?? this);
                    MafRunningTasksManager.PromoteTaskIfBlocking((MafTask) task, this);
                }
            }

            return true;
        }


        private static bool EnsureTaskIsNotBlocking(MafTask taskToUnblock,
            MafTaskDependency dependencyType = MafTaskDependency.AttachedTask)
        {
            var currentTask = MafRunningTasksManager.GetCurrentTask();
            if (currentTask != null)
            {
                var checkForCycle = dependencyType == MafTaskDependency.WaitForExecution;
                if (!currentTask.TryAddDependentTask(taskToUnblock, checkForCycle))
                    return false;
            }

            return true;
        }

        private static void IgnoreObjectDisposedException(Action action)
        {
            try
            {
                action();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private static bool OptionsHasFlag(MafTaskCreationOptions options, MafTaskCreationOptions flag)
        {
            return (uint) (options & flag) > 0U;
        }

        private static bool OptionsHasFlag(MafTaskContinuationOptions options, MafTaskContinuationOptions flag)
        {
            return (uint) (options & flag) > 0U;
        }

        private static Exception RethrowException(AggregateException e)
        {
            var aggregateException = e.Flatten();
            ExceptionDispatchInfo
                .Capture(aggregateException.InnerExceptions.Count == 1 ? aggregateException.InnerExceptions[0] : e)
                .Throw();
            throw new InvalidOperationException("Unreachable");
        }

        private void ClearUiThreadWaitAbortToken()
        {
            _uiThreadWaitAbortToken = null;
        }

        private List<MafTask> GetNewDependenciesSinceLastCheck(int lastIndexChecked)
        {
            var mafTaskList = (List<MafTask>) null;
            if (_attachedTasks != null)
                lock (_attachedTasks)
                {
                    for (; lastIndexChecked < _attachedTasks.Count; ++lastIndexChecked)
                    {
                        if (mafTaskList == null)
                            mafTaskList = new List<MafTask>();
                        mafTaskList.Add(_attachedTasks[lastIndexChecked]);
                    }
                }

            return mafTaskList;
        }

        private CancellationTokenSource InitializeUiThreadWaitAbortToken()
        {
            return _uiThreadWaitAbortToken = new CancellationTokenSource();
        }

        private T InvokeWithWaitDialog<T>(Func<T> function)
        {
            MafRunningTasksManager.BeginTaskWaitOnUiThread(this);
            try
            {
                var service = IoC.Get<IWaitDialogFactory>();
                IWaitDialog dialog;
                if (service != null)
                    service.CreateInstance(out dialog);
                else
                    dialog = null;

                var flag = false;
                if (dialog != null)
                {
                    var waitMessage = _waitMessage;
                    if (!string.IsNullOrWhiteSpace(waitMessage))
                        flag = dialog.StartWaitDialog(null, waitMessage, null, "Wait", 2, false, true);
                }

                try
                {
                    RaiseOnBlockingWaitEndEvent();
                    foreach (var vsTask in GetAllDependentMafTasks().Distinct())
                        vsTask.RaiseOnMarkedAsBlockingEvent(this);
                    return function();
                }
                finally
                {
                    RaiseOnBlockingWaitEndEvent();
                    if (flag)
                        dialog.EndWaitDialog(out _);
                }
            }
            finally
            {
                MafRunningTasksManager.EndTaskWaitOnUiThread();
            }
        }

        private void InvokeWithWaitDialog(Action action)
        {
            InvokeWithWaitDialog(() =>
            {
                action();
                return (object) null;
            });
        }

        private void RaiseOnBlockingWaitBeginEvent()
        {
            var blockingWaitBegin = OnBlockingWaitBegin;
            if (blockingWaitBegin == null)
                return;
            try
            {
                blockingWaitBegin(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
            }
        }

        private void RaiseOnBlockingWaitEndEvent()
        {
            var onBlockingWaitEnd = OnBlockingWaitEnd;
            if (onBlockingWaitEnd == null)
                return;
            try
            {
                onBlockingWaitEnd(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
            }
        }

        private void RaiseOnMarkedAsBlockingEvent(IMafTask blockedTask)
        {
            var markedAsBlocking = OnMarkedAsBlocking;
            if (markedAsBlocking == null)
                return;
            try
            {
                markedAsBlocking(this, new BlockingTaskEventArgs(this, blockedTask));
            }
            catch (Exception)
            {
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged.RaiseEvent(this, propertyName);
        }

        private async Task<IEnumerable<MafTask>> WaitForNewDependenciesAsync(int lastNotifiedAttachedTaskIndex)
        {
            if (!IsCompleted && !IsCanceled)
            {
                var val = LazyInitializer.EnsureInitialized(ref _newDependencyWaiter,
                    () => new AsyncAutoResetEvent(true));
                if (lastNotifiedAttachedTaskIndex == 0)
                {
                    var newDependenciesSinceLastCheck = GetNewDependenciesSinceLastCheck(lastNotifiedAttachedTaskIndex);
                    if (newDependenciesSinceLastCheck != null) return newDependenciesSinceLastCheck;
                }

                var resetEventTask = val.WaitAsync();
                if (await Task.WhenAny(_internalTask, resetEventTask).ConfigureAwait(false) == resetEventTask)
                    return GetNewDependenciesSinceLastCheck(lastNotifiedAttachedTaskIndex) ??
                           Enumerable.Empty<MafTask>();
            }

            return null;
        }
    }
}