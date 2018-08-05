using System;

namespace ModernApplicationFramework.TextEditor.Text.Document
{
    [Flags]
    public enum ChangeTypes
    {
        None = 0,
        ChangedSinceOpened = 1,
        ChangedSinceSaved = 2,
    }
}