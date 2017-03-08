using System;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Core.Events
{
    public class ThemeChangedEventArgs : EventArgs
    {
        public ThemeChangedEventArgs(Theme newTheme, Theme oldTheme)
        {
            NewTheme = newTheme;
            OldTheme = oldTheme;
        }

        public Theme NewTheme { get; }

        public Theme OldTheme { get; }
    }
}