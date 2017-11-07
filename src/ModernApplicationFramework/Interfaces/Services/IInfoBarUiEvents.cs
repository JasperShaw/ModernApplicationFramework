using ModernApplicationFramework.Interfaces.Controls.InfoBar;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IInfoBarUiEvents
    {
        void OnClosed(IInfoBarUiElement infoBarUiElement);

        void OnActionItemClicked(IInfoBarUiElement infoBarUiElement, IInfoBarActionItem actionItem);
    }
}