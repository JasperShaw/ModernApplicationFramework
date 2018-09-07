using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal sealed class MafTaskSchedulerService : IMafTaskSchedulerService
    {
        internal event EventHandler<TaskCreatedEventArgs> OnTaskCreated;

        internal readonly JoinableTaskContext JoinableTaskContext;
        private static MafTaskSchedulerService _instance;
        private Func<IMafTask, IMafTaskBody, IMafTaskBody> _adapterFunctionDelegate;

        public MafUiBackgroundPriorityScheduler UiContextBackgroundPriorityScheduler { get; }

        public TaskScheduler UiContextScheduler { get; }

        public MafIdleTimeScheduler UiContextIdleTimeScheduler { get; set; }

        public MafUiNormalPriorityScheduler UiContextNormalPriorityScheduler { get; }

        public MafThreadpoolLowIoScheduler BackgroundThreadLowIoPriorityScheduler { get; }

        internal static MafTaskSchedulerService Instance
        {
            get
            {
                if (_instance == null)
                    new MafTaskSchedulerService();
                return _instance;
            }
        }


        public MafTaskSchedulerService()
        {
            _instance = this;
            JoinableTaskContext = new JoinableTaskContext(Thread.CurrentThread,
                new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher, DispatcherPriority.Background));
            UiContextBackgroundPriorityScheduler = new MafUiBackgroundPriorityScheduler();
            UiContextScheduler = new MafUiThreadScheduler();
            UiContextIdleTimeScheduler = new MafIdleTimeScheduler();
            UiContextNormalPriorityScheduler = new MafUiNormalPriorityScheduler();
            BackgroundThreadLowIoPriorityScheduler = new MafThreadpoolLowIoScheduler();
        }

        public JoinableTaskContext GetAsyncTaskContext()
        {
            return JoinableTaskContext;
        }

        public object GetTaskScheduler(MafTaskRunContext context)
        {
            return MafTask.GetSchedulerFromContext(context);
        }

        public IMafTask ContinueWhenAllCompleted(MafTaskRunContext context, uint dwTasks, IMafTask[] pTasks, IMafTaskBody pTaskBody)
        {
            return ContinueWhenAllCompletedEx(context, dwTasks, pTasks, 0U, pTaskBody, null);
        }

        public IMafTask ContinueWhenAllCompletedEx(MafTaskRunContext context, uint dwTasks, IMafTask[] pDependentTasks, uint options, IMafTaskBody pTaskBody, object pAsyncState)
        {
            return MafTask.ContinueWhenAllCompleted(pDependentTasks, pTaskBody, context, (MafTaskContinuationOptions)options, pAsyncState);
        }

        public IMafTask CreateTask(uint context, IMafTaskBody pTaskBody)
        {
            return CreateTaskEx(context, 0U, pTaskBody, null);
        }

        public IMafTask CreateTaskEx(uint context, uint options, IMafTaskBody pTaskBody, object pAsyncState)
        {
            var newTask = new MafTask(pTaskBody, (MafTaskRunContext)context, (MafTaskCreationOptions)options, pAsyncState);
            RaiseOnTaskCreatedEvent(newTask);
            return newTask;
        }

        public IMafTaskCompletionSource CreateTaskCompletionSource()
        {
            return CreateTaskCompletionSourceEx(0U, null);
        }

        public IMafTaskCompletionSource CreateTaskCompletionSourceEx(uint options, object asyncState)
        {
            return new MafTaskCompletionSource(asyncState, (MafTaskCreationOptions)options);
        }

        internal IEnumerable<IMafUiThreadBlockableTaskScheduler> GetAllUiThreadSchedulers()
        {
            yield return UiContextBackgroundPriorityScheduler;
            yield return UiContextIdleTimeScheduler;
            yield return UiContextNormalPriorityScheduler;
        }

        internal bool HandleMessages(int msg)
        {
            if (msg != NativeMethods.ProcessUiBackgroundTask)
                return false;
            UiContextBackgroundPriorityScheduler?.ProcessQueue("CustomMessage");
            return true;
        }

        internal Func<IMafTask, IMafTaskBody, IMafTaskBody> SetTaskBodyAdapter(Func<IMafTask, IMafTaskBody, IMafTaskBody> adapterFunc)
        {
            var functionDelegate = _adapterFunctionDelegate;
            _adapterFunctionDelegate = adapterFunc;
            return functionDelegate;
        }

        internal bool TryAdaptTaskBody(IMafTask task, IMafTaskBody existingBody, out IMafTaskBody adaptedBody)
        {
            adaptedBody = null;
            if (_adapterFunctionDelegate == null)
                return false;
            adaptedBody = _adapterFunctionDelegate(task, existingBody);
            return true;
        }

        internal void RaiseOnTaskCreatedEvent(MafTask newTask)
        {
            OnTaskCreated?.Invoke(this, new TaskCreatedEventArgs(newTask));
        }
    }

    public interface IMafTaskCompletionSource
    {
        IMafTask Task { get; }

        void SetResult(object result);

        void SetCanceled();

        void SetFaulted(int hr);

        void AddDependentTask(IMafTask pTask);
    }

    internal sealed class MafManagedTaskBody : IMafTaskBody
    {
        private readonly MafTaskBodyCallback _taskBody;

        public MafManagedTaskBody(MafTaskBodyCallback action)
        {
            _taskBody = action;
        }

        public void DoWork(IMafTask pTask, uint dwCount, IMafTask[] pParentTasks, out object pResult)
        {
            pResult = _taskBody(pTask, pParentTasks);
        }
    }

    public delegate object MafTaskBodyCallback(IMafTask task, IMafTask[] parentTasks);

    public interface IMafTaskBody
    {
        void DoWork(IMafTask pTask, uint dwCount, IMafTask[] pParentTasks, out object pResult);
    }

    internal class TaskCreatedEventArgs : EventArgs
    {
        public MafTask NewTask { get; }

        public TaskCreatedEventArgs(MafTask newTask)
        {
            NewTask = newTask;
        }
    }

    [Flags]
    public enum MafTaskCreationOptions
    {
        None = 0,
        PreferFairness = 1,
        LongRunning = 2,
        AttachedToParent = 4,
        DenyChildAttach = 8,
        CancelWithParent = 536870912,
        NotCancelable = -2147483648 
    }

    internal class MafTaskCompletionSource : IMafTaskCompletionSource
    {
        internal MafTask InternalTask { get; }

        internal TaskCompletionSource<object> InternalCompletionSource { get; }

        public MafTaskCompletionSource(object asyncState, MafTaskCreationOptions options)
        {
            InternalCompletionSource = new TaskCompletionSource<object>(asyncState, MafTask.GetTplOptions(options));
            InternalTask = new MafTask(InternalCompletionSource);
            AsyncState = asyncState;
        }

        public void SetCanceled()
        {
            InternalCompletionSource.SetCanceled();
            InternalTask.Cancel();
        }

        public void SetFaulted(int hr)
        {
            InternalCompletionSource.SetException(Marshal.GetExceptionForHR(hr));
        }

        public void SetResult(object result)
        {
            InternalCompletionSource.SetResult(result);
        }

        public void AddDependentTask(IMafTask task)
        {
            InternalTask.TryAddDependentTask(task, false);
            MafTask.JoinAntecedentJoinableTasks(task);
        }

        public IMafTask Task => InternalTask;

        public object AsyncState { get; }
    }

    public delegate Task<T> MafInvokableAsyncFunction<T>(IMafTaskCompletionSource tcs);

    [Flags]
    public enum MafTaskContinuationOptions
    {
        None = 0,
        PreferFairness = 1,
        LongRunning = 2,
        AttachedToParent = 4,
        DenyChildAttach = 8,
        LazyCancelation = 32, // 0x00000020
        NotOnRanToCompletion = 65536, // 0x00010000
        NotOnFaulted = 131072, // 0x00020000
        NotOnCanceled = 262144, // 0x00040000
        OnlyOnFaulted = NotOnCanceled | NotOnRanToCompletion, // 0x00050000
        OnlyOnRanToCompletion = NotOnCanceled | NotOnFaulted, // 0x00060000
        ExecuteSynchronously = 524288, // 0x00080000
        CancelWithParent = 536870912, // 0x20000000
        IndependentlyCanceled = 1073741824, // 0x40000000
        NotCancelable = -2147483648, // -0x80000000
        Default = NotOnFaulted, // 0x00020000
    }
}
