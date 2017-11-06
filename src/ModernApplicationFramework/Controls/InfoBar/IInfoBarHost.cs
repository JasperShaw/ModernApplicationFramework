namespace ModernApplicationFramework.Controls.InfoBar
{
    public interface IInfoBarHost
    {
        void AddInfoBar(IInfoBarUiElement uiElement);

        void RemoveInfoBar( IInfoBarUiElement uiElement);
    }
}