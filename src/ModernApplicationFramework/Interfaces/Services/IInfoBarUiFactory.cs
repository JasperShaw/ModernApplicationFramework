using ModernApplicationFramework.Controls.InfoBar;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IInfoBarUiFactory
    {
        IInfoBarUiElement CreateInfoBar(InfoBarModel infoBar);
    }
}