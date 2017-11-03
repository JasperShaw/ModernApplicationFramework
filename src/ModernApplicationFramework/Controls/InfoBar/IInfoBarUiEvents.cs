namespace ModernApplicationFramework.Controls.InfoBar
{
    public interface IInfoBarUiEvents
    {
        void OnClosed(IInfoBarUiElement infoBarUiElement);

        void OnActionItemClicked(IInfoBarUiElement infoBarUiElement, IInfoBarActionItem actionItem);
    }
}