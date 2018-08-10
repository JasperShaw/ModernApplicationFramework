using System;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    [Flags]
    public enum StringDifferenceTypes
    {
        Line = 1,
        Word = 2,
        Character = 4
    }
}