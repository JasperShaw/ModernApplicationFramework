using System;
using System.Runtime.CompilerServices;
using System.Threading;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    internal class InlineResumable : ICriticalNotifyCompletion
    {
        private Action _continuation;
        private SynchronizationContext _capturedSynchronizationContext;

        public bool IsCompleted { get; private set; }

        public void GetResult()
        {

        }

        public void OnCompleted(Action continuation)
        {
            Validate.IsNotNull(continuation, nameof(continuation));
            if (_continuation != null)
                throw new Exception();
            _capturedSynchronizationContext = SynchronizationContext.Current;
            _continuation = continuation;

        }

        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompleted(continuation);
        }

        public InlineResumable GetAwaiter() => this;

        public void Resume()
        {
            IsCompleted = true;
            var continuation = _continuation;
            _continuation = null;
            using (_capturedSynchronizationContext.Apply())
                continuation?.Invoke();
        }
    }
}
