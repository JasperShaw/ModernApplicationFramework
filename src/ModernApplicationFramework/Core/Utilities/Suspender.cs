using System;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.Utilities
{
    public sealed class Suspender : ISuspendable
    {
        private int _suspendCount;
        private readonly Action _resumeAction;

        public Suspender(Action resumeAction = null)
        {
            _resumeAction = resumeAction;
        }

        public IDisposable Suspend()
        {
            return new SuspendScope(this);
        }

        public bool IsSuspended => _suspendCount > 0;

        public int SuspendCount => _suspendCount;

        void ISuspendable.Suspend()
        {
            _suspendCount = _suspendCount + 1;
        }

        void ISuspendable.Resume()
        {
            int num = _suspendCount - 1;
            _suspendCount = num;
            if (num != 0 || _resumeAction == null)
                return;
            _resumeAction();
        }

        private class SuspendScope : DisposableObject
        {
            private readonly ISuspendable _suspendable;

            public SuspendScope(ISuspendable suspender)
            {
                _suspendable = suspender;
                _suspendable.Suspend();
            }

            protected override void DisposeManagedResources()
            {
                _suspendable.Resume();
            }
        }
    }
}