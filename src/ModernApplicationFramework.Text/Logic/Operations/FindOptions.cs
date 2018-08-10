using System;

namespace ModernApplicationFramework.Text.Logic.Operations
{
    [Flags]
    public enum FindOptions
    {
        None = 0,
        MatchCase = 1,
        UseRegularExpressions = 2,
        WholeWord = 4,
        SearchReverse = 8,
        Wrap = 16, // 0x00000010
        Multiline = 32, // 0x00000020
        OrdinalComparison = 64, // 0x00000040
        SingleLine = 128 // 0x00000080
    }
}