using System;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Flags]
    internal enum CaretMoveType
    {
        NonDestructiveCaretMove = 1,
        DestructiveCaretMove = 2,
        FirstVisit = 4,
        ArbitraryLocation = 8,
        NonMergeable = 16
    }
}