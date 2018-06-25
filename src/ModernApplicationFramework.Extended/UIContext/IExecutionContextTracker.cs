using System;

namespace ModernApplicationFramework.Extended.UIContext
{
    public interface IExecutionContextTracker
    {
        void SetContextElement(Guid contextTypeGuid, Guid contextElement);

        Guid SetAndGetContextElement(Guid contextTypeGuid, Guid contextElement);

        void PushContext(uint cookie);

        void PopContext(uint cookie);

        uint GetCurrentContext();

        void ReleaseContext(uint cookie);
    }
}
