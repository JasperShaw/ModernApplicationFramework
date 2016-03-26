using System;
using System.ComponentModel;
using ModernApplicationFramework.Core.Events;

namespace ModernApplicationFramework.Core.Themes
{
    public interface IHasTheme
    {

        event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        Theme Theme { get; set; }

        //void ChangeTheme(Theme oldValue, Theme newValue);
    }
}
