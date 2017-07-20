using System;
using System.Collections.Generic;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// An <see cref="IThemeManager"/> is responsible for the Theme of the Framework
    /// </summary>
    public interface IThemeManager
    {
        /// <summary>
        /// Fired when the current theme is changed
        /// </summary>
        event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        /// <summary>
        /// The current theme of the environment
        /// </summary>
        Theme Theme { get; set; }

        /// <summary>
        /// Gets the theme saved when re-launching the environment
        /// </summary>
        Theme StartUpTheme { get; }

        /// <summary>
        /// A list of all installed themes
        /// </summary>
        IEnumerable<Theme> Themes { get; }

        /// <summary>
        /// Saves a theme for re-launch
        /// </summary>
        /// <param name="theme">The theme that should be saved</param>
        void SaveTheme(Theme theme);
    }
}