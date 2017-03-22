using System;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Interfaces
{
    public interface IHasTheme
    {
        event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        Theme Theme { get; set; }

        //void ChangeTheme(Theme oldValue, Theme newValue);
    }
}