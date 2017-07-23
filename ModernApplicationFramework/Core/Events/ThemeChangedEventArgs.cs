using System;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Core.Events
{
    /// <inheritdoc />
    /// <summary>
    /// The event args when <see cref="E:ModernApplicationFramework.Core.Themes.ThemeManager.OnThemeChanged" /> was fired
    /// </summary>
    /// <seealso cref="T:System.EventArgs" />
    public class ThemeChangedEventArgs : EventArgs
    {
        public ThemeChangedEventArgs(Theme newTheme, Theme oldTheme)
        {
            NewTheme = newTheme;
            OldTheme = oldTheme;
        }

        /// <summary>
        /// The new theme.
        /// </summary>
        public Theme NewTheme { get; }

        /// <summary>
        /// The old theme.
        /// </summary>
        public Theme OldTheme { get; }
    }
}