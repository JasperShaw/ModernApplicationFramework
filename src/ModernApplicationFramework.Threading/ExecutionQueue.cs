using System;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    internal class ExecutionQueue : AsyncQueue<JoinableTaskFactory.SingleExecuteProtector>
    {
        private readonly JoinableTask _owningJob;

        protected override int InitialCapacity => 1;


        internal ExecutionQueue(JoinableTask owningJob)
        {
            Validate.IsNotNull(owningJob, nameof(owningJob));
            _owningJob = owningJob;
        }

        protected override void OnEnqueued(JoinableTaskFactory.SingleExecuteProtector value, bool alreadyDispatched)
        {
            base.OnEnqueued(value, alreadyDispatched);
            if (!alreadyDispatched)
            {
                Validate.IsNotNull(value, nameof(value));
                value.AddExecutingCallback(this);
                if (value.HasBeenExecuted)
                    Scavenge();
            }
        }

        protected override void OnDequeued(JoinableTaskFactory.SingleExecuteProtector value)
        {
            Validate.IsNotNull(value, nameof(value));

            base.OnDequeued(value);
            value.RemoveExecutingCallback(this);
        }

        protected override void OnCompleted()
        {
            base.OnCompleted();

            _owningJob.OnQueueCompleted();
        }

        internal void OnExecuting(object sender, EventArgs e)
        {
            Scavenge();
        }

        private void Scavenge()
        {
            while (TryDequeue(p => p.HasBeenExecuted, out _))
            {
            }
        }
    }
}
