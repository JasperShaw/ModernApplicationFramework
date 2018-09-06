using System.Threading;
using System.Windows.Threading;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Basics.Services
{
    internal sealed class MafTaskSchedulerService : IMafTaskSchedulerService
    {
        internal readonly JoinableTaskContext JoinableTaskContext;


        public MafTaskSchedulerService()
        {
            JoinableTaskContext = new JoinableTaskContext(Thread.CurrentThread,
                new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher, DispatcherPriority.Background));
        }

        public JoinableTaskContext GetAsyncTaskContext()
        {
            return JoinableTaskContext;
        }
    }
}
