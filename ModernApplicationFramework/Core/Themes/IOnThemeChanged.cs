namespace ModernApplicationFramework.Core.Themes
{
    internal interface IOnThemeChanged
    {
        void OnThemeChanged(Theme oldValue, Theme newValue);
    }
}
