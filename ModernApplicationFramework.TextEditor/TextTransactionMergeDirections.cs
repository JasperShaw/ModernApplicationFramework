using System;

namespace ModernApplicationFramework.TextEditor
{
    [Flags]
    internal enum TextTransactionMergeDirections
    {
        Forward = 1,
        Backward = 2,
    }
}