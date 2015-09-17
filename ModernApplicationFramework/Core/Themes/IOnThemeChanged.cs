namespace ModernApplicationFramework.Core.Themes
{
    public interface IOnThemeChanged
    {
        void OnThemeChanged(Theme oldValue, Theme newValue);
    }
}
