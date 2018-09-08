using System;

namespace ModernApplicationFramework.Interfaces
{
    public interface IMafExecutionContextTrackerListener
    {
        void OnExecutionContextValueChanged(Guid contextValueType, Guid previousValue, Guid newValue);
    }
}