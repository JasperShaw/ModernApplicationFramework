using System.Threading;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Threading
{
    public class DelegatingJoinableTaskFactory : JoinableTaskFactory
    {
        private readonly JoinableTaskFactory _innerFactory;

        protected DelegatingJoinableTaskFactory(JoinableTaskFactory innerFactory)
            : base(innerFactory.Context, innerFactory.Collection)
        {
            _innerFactory = innerFactory;
        }

        protected internal override void WaitSynchronously(Task task)
        {
            _innerFactory.WaitSynchronously(task);
        }

        protected internal override void PostToUnderlyingSynchronizationContext(SendOrPostCallback callback, object state)
        {
            _innerFactory.PostToUnderlyingSynchronizationContext(callback, state);
        }

        protected internal override void OnTransitioningToMainThread(JoinableTask joinableTask)
        {
            _innerFactory.OnTransitioningToMainThread(joinableTask);
        }

        protected internal override void OnTransitionedToMainThread(JoinableTask joinableTask, bool canceled)
        {
            _innerFactory.OnTransitionedToMainThread(joinableTask, canceled);
        }
    }
}
