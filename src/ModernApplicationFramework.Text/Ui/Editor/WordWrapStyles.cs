using System;

namespace ModernApplicationFramework.Text.Ui.Editor
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