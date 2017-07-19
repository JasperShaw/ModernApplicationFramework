using System;
using System.Collections.Generic;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IThemeManager
    {
        event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        Theme Theme { get; set; }

        Theme StartUpTheme { get; }

        IEnumerable<Theme> Themes { get; }

        void SaveTheme(Theme theme);
    }
}