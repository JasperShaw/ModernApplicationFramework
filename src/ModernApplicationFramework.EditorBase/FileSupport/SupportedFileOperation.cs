using System;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    [Flags]
    public enum SupportedFileOperation
    {
        Open = 0,
        Create = 1,
        OpenCreate = Open | Create
    }
}