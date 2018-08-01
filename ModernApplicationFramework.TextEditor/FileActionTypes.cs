using System;

namespace ModernApplicationFramework.TextEditor
{
    [Flags]
    public enum FileActionTypes
    {
        ContentSavedToDisk = 1,
        ContentLoadedFromDisk = 2,
        DocumentRenamed = 4,
    }
}