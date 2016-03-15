using System;

namespace ModernApplicationFramework.Core.Themes
{
    public interface IChangeTheme
    {

        event EventHandler OnThemeChanged;

        void ChangeTheme(Theme oldValue, Theme newValue);
    }
}
