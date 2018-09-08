using System;
using System.Runtime.Serialization;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    [Serializable]
    public class TaskSchedulingException : Exception
    {
        public const int VsETaskschedulerfail = -2147213304;

        public TaskSchedulingException()
            : this("Task scheduling could not be completed in the requested context.")
        {
        }

        public TaskSchedulingException(string message)
            : base(message)
        {
            HResult = -2147213304;
        }

        protected TaskSchedulingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            HResult = -2147213304;
        }
    }
}