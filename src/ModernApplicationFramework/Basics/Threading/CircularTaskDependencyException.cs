using System;
using System.Runtime.Serialization;

namespace ModernApplicationFramework.Basics.Threading
{
    [Serializable]
    public class CircularTaskDependencyException : Exception
    {
        public CircularTaskDependencyException()
            : this("Requested operation would result in circular task dependency causing a deadlock.")
        {
        }

        public CircularTaskDependencyException(string message)
            : base(message)
        {
            HResult = -2147213305;
        }

        protected CircularTaskDependencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            HResult = -2147213305;
        }
    }
}