using System;

namespace ModernApplicationFramework.TextEditor
{
    [Flags]
    internal enum LineBreakBoundaryConditions : byte
    {
        None = 0,
        PrecedingReturn = 1,
        SucceedingNewline = 2,
    }
}