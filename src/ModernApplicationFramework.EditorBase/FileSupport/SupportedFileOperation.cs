using System;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    [Flags]
    public enum SupportedFileOperation
    {
        None = 0,
        Open = 1,
        Create = 2,
        OpenCreate = Open | Create
    }
}