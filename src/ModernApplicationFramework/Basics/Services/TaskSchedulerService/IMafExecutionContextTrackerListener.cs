using System;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    public interface IMafExecutionContextTrackerListener
    {
        void OnExecutionContextValueChanged(Guid contextValueType, Guid previousValue, Guid newValue);
    }
}