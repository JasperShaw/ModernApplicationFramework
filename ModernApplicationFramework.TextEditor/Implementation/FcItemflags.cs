using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Flags]
    public enum FcItemflags
    {
        IsMarker = 1,
        AllowFgChange = 2,
        AllowBgChange = 4,
        AllowBoldChange = 8,
        AllowCustomColors = 16,
        Plaintext = 32,
    }
}