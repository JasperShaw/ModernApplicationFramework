using System;

namespace ModernApplicationFramework.Modules.Editor.Operations
{
    [Flags]
    internal enum TextTransactionMergeDirections
    {
        Forward = 1,
        Backward = 2
    }
}