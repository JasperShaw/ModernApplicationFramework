using System;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    [Flags]
    public enum StringDifferenceTypes
    {
        Line = 1,
        Word = 2,
        Character = 4
    }
}