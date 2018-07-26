using System;

namespace ModernApplicationFramework.TextEditor
{
    [Flags]
    public enum TagAggregatorOptions
    {
        None = 0,
        MapByContentType = 1,
        NoProjection = 4,
    }
}