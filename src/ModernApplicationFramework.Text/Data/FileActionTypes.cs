using System;

namespace ModernApplicationFramework.Text.Data
{
    [Flags]
    public enum FileActionTypes
    {
        ContentSavedToDisk = 1,
        ContentLoadedFromDisk = 2,
        DocumentRenamed = 4,
    }
}