using System;

namespace ModernApplicationFramework.Interfaces.Threading
{
    public interface IMafExecutionContextTracker
    {
        void SetContextElement(Guid contextTypeGuid, Guid contextElementGuid);

        Guid SetAndGetContextElement(Guid contextTypeGuid, Guid contextElementGuid);

        Guid GetContextElement(Guid contextTypeGuid);

        void PushContext(uint contextCookie);

        void PopContext( uint contextCookie);

        uint GetCurrentContext();

        void ReleaseContext( uint contextCookie);

        void PushContextEx(uint contextCookie, bool fDontTrackAsyncWork);
    }
}