﻿using System;

namespace ModernApplicationFramework.Editor.Interop
{
    [Flags]
    public enum Olecmdf
    {
        Supported = 1,
        Enabled = 2,
        Latched = 4,
        Ninched = 8,
        Invisible = 16,
        HideNnCtxtMenu = 32,
    }
}