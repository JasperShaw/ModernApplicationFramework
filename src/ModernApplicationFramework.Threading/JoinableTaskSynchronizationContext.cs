using System;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    internal class JoinableTaskSynchronizationContext : SynchronizationContext
    {
        private readonly JoinableTaskFactory _jobFactory;
        private JoinableTask _job;

        internal bool MainThreadAffinitized { get; }

        internal JoinableTaskSynchronizationContext(JoinableTaskFactory owner)
        {
            Validate.IsNotNull(owner, nameof(owner));

            _jobFactory = owner;
            MainThreadAffinitized = true;
        }

        internal JoinableTaskSynchronizationContext(JoinableTask joinableTask, bool mainThreadAffinitized)
            : this(joinableTask?.Factory)
        {
            _job = joinableTask;
            MainThreadAffinitized = mainThreadAffinitized;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            var job = _job;
            if (job != null)
            {
                job.Post(d, state, MainThreadAffinitized);
            }
            else
            {
                _jobFactory.Post(d, state, MainThreadAffinitized);
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            Validate.IsNotNull(d, nameof(d));

            if (MainThreadAffinitized)
            {
                if (_jobFactory.Context.IsOnMainThread)
                {
                    d(state);
                }
                else
                {
                    _jobFactory.Context.UnderlyingSynchronizationContext.Send(d, state);
                }
            }
            else
            {
                var isThreadPoolThread = Thread.CurrentThread.IsThreadPoolThread;
                if (isThreadPoolThread)
                {
                    d(state);
                }
                else
                {
                    Task.Factory.StartNew(
                        s =>
                        {
                            var tuple = (Tuple<SendOrPostCallback, object>)s;
                            tuple.Item1(tuple.Item2);
                        },
                        Tuple.Create(d, state),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskScheduler.Default).Wait();
                }
            }
        }

        internal void OnCompleted()
        {
            _job = null;
        }
    }
}
