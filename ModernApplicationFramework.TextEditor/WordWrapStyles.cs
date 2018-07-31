using System;

namespace ModernApplicationFramework.TextEditor
{
    [Flags]
    public enum WordWrapStyles
    {
        None = 0,
        WordWrap = 1,
        VisibleGlyphs = 2,
        AutoIndent = 4,
    }
}