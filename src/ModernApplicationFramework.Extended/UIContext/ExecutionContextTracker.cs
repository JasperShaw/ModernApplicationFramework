using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.UIContext
{
    [Export(typeof(IExecutionContextTracker))]
    internal class ExecutionContextTracker : DisposableObject, IExecutionContextTracker
    {
        private static ExecutionContextTracker Instance;

        public ExecutionContextTracker()
        {
            Instance = this;
        }

        public void SetContextElement(Guid contextTypeGuid, Guid contextElement)
        {
            SetAndGetContextElement(contextTypeGuid, contextElement);
        }

        public Guid SetAndGetContextElement(Guid contextTypeGuid, Guid contextElement)
        {
            if (IsDisposed)
                return Guid.Empty;

            return Guid.Empty;
        }

        public void PushContext(uint cookie)
        {
            PushContextEx(cookie, false);
        }

        private void PushContextEx(uint cookie, bool dontTrackAsyncWork)
        {
        }

        public void PopContext(uint cookie)
        {
            if (cookie == 0 || IsDisposed)
                return;
        }

        public uint GetCurrentContext()
        {
            throw new NotImplementedException();
        }

        public void ReleaseContext(uint cookie)
        {
            if (cookie == 0)
                return;

        }
    }
}
