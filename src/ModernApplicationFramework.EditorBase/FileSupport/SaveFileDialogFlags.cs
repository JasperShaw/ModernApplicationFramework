using System;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    [Flags]
    public enum SaveFileDialogFlags
    {
        OverwritePrompt = 0,
        CreatePrompt = 1
    }
}