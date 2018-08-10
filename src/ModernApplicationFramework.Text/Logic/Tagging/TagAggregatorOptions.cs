using System;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    [Flags]
    public enum TagAggregatorOptions
    {
        None = 0,
        MapByContentType = 1,
        NoProjection = 4
    }
}