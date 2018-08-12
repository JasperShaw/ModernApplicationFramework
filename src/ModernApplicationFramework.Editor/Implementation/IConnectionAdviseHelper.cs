using System;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal interface IConnectionAdviseHelper
    {
        bool Advise(Type eventType, object eventSink);

        bool Unadvise(Type eventType, object eventSink);
    }
}