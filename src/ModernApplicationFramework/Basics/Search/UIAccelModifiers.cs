using System;

namespace ModernApplicationFramework.Basics.Search
{
    [Flags]
    public enum UiAccelModifiers : uint
    {
        None = 0,
        Shift = 1,
        Control = 2,
        Alt = 4,
        Windows = 8,
    }
}