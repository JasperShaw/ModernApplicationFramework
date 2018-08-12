using System;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    [Flags]
    internal enum LineBreakBoundaryConditions : byte
    {
        None = 0,
        PrecedingReturn = 1,
        SucceedingNewline = 2,
    }
}