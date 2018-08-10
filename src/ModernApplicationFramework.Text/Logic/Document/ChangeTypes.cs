using System;

namespace ModernApplicationFramework.Text.Logic.Document
{
    [Flags]
    public enum ChangeTypes
    {
        None = 0,
        ChangedSinceOpened = 1,
        ChangedSinceSaved = 2
    }
}