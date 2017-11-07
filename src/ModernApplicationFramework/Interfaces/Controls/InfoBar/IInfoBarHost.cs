using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Interfaces.Controls.InfoBar
{
    public interface IInfoBarHost
    {
        void AddInfoBar(IInfoBarUiElement uiElement);

        void RemoveInfoBar( IInfoBarUiElement uiElement);
    }
}