using System;

namespace ModernApplicationFramework.Extended.Core.LayoutManagement
{
    [Flags]
    public enum ProcessStateOption
    {
        Complete = 1,
        ToolsOnly = 2,
        DocumentsOnly = 4,
        UseShouldReopenOnStart = 8
    }
}