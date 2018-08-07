﻿using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Flags]
    public enum FontColorFlags
    {
        MustRestart = 1,
        OnlyTtFonts = 2,
        SaveAll = 4,
        OnlyNewInstances = 8,
        AutoFont = 16,
    }
}