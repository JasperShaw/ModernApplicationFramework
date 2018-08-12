using System;

namespace ModernApplicationFramework.Editor.Interop
{
    [Flags]
    public enum FcItemFlags
    {
        IsMarker = 1,
        AllowFgChange = 2,
        AllowBgChange = 4,
        AllowBoldChange = 8,
        AllowCustomColors = 16,
        Plaintext = 32,
    }
}