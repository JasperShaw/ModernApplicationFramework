using System;
using System.Threading;

namespace ModernApplicationFramework.Threading
{
    public struct SpecializedSyncContext : IDisposable
    {
        private readonly bool _initialized;
        private readonly SynchronizationContext _prior;

        public SpecializedSyncContext(SynchronizationContext syncContext, bool checkForChangesOnRevert)
        {
            _initialized = true;
            _prior = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(syncContext);
        }

        public static SpecializedSyncContext Apply(SynchronizationContext syncContext,
            bool checkForChangesOnRevert = true)
        {
            return new SpecializedSyncContext(syncContext, checkForChangesOnRevert);
        }

        public void Dispose()
        {
            if (_initialized)
                SynchronizationContext.SetSynchronizationContext(_prior);
        }
    }
}
