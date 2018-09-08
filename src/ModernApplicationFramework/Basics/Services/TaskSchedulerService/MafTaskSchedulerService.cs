using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Threading;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal sealed class MafTaskSchedulerService : IMafTaskSchedulerService
    {
        private static MafTaskSchedulerService _instance;

        internal readonly JoinableTaskContext JoinableTaskContext;
        private Func<IMafTask, IMafTaskBody, IMafTaskBody> _adapterFunctionDelegate;
        internal event EventHandler<TaskCreatedEventArgs> OnTaskCreated;

        public MafThreadpoolLowIoScheduler BackgroundThreadLowIoPriorityScheduler { get; }

        public MafUiBackgroundPriorityScheduler UiContextBackgroundPriorityScheduler { get; }

        public MafIdleTimeScheduler UiContextIdleTimeScheduler { get; set; }

        public MafUiNormalPriorityScheduler UiContextNormalPriorityScheduler { get; }

        public TaskScheduler UiContextScheduler { get; }

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

        public IMafTask ContinueWhenAllCompleted(MafTaskRunContext context, uint dwTasks, IMafTask[] pTasks,
            IMafTaskBody pTaskBody)
        {
            return ContinueWhenAllCompletedEx(context, dwTasks, pTasks, 0U, pTaskBody, null);
        }

        public IMafTask ContinueWhenAllCompletedEx(MafTaskRunContext context, uint dwTasks, IMafTask[] pDependentTasks,
            MafTaskContinuationOptions options, IMafTaskBody pTaskBody, object pAsyncState)
        {
            return MafTask.ContinueWhenAllCompleted(pDependentTasks, pTaskBody, context, options, pAsyncState);
        }

        public IMafTask CreateTask(MafTaskRunContext context, IMafTaskBody pTaskBody)
        {
            return CreateTaskEx(context, 0U, pTaskBody, null);
        }

        public IMafTaskCompletionSource CreateTaskCompletionSource()
        {
            return CreateTaskCompletionSourceEx(0U, null);
        }

        public IMafTaskCompletionSource CreateTaskCompletionSourceEx(MafTaskCreationOptions options, object asyncState)
        {
            return new MafTaskCompletionSource(asyncState, options);
        }

        public IMafTask CreateTaskEx(MafTaskRunContext context, MafTaskCreationOptions options, IMafTaskBody pTaskBody,
            object pAsyncState)
        {
            var newTask = new MafTask(pTaskBody, context, options, pAsyncState);
            RaiseOnTaskCreatedEvent(newTask);
            return newTask;
        }

        public JoinableTaskContext GetAsyncTaskContext()
        {
            return JoinableTaskContext;
        }

        public object GetTaskScheduler(MafTaskRunContext context)
        {
            return MafTask.GetSchedulerFromContext(context);
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

        internal void RaiseOnTaskCreatedEvent(MafTask newTask)
        {
            OnTaskCreated?.Invoke(this, new TaskCreatedEventArgs(newTask));
        }

        internal Func<IMafTask, IMafTaskBody, IMafTaskBody> SetTaskBodyAdapter(
            Func<IMafTask, IMafTaskBody, IMafTaskBody> adapterFunc)
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
    }
}