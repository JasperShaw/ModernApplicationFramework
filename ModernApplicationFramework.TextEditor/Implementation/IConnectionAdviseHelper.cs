using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal interface IConnectionAdviseHelper
    {
        bool Advise(Type eventType, object eventSink);

        bool Unadvise(Type eventType, object eventSink);
    }
}