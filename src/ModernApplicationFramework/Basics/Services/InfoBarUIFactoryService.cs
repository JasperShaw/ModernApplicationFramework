using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.InfoBar;
using ModernApplicationFramework.Basics.InfoBar.Internal;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(IInfoBarUiFactory))]
    internal class InfoBarUiIFactoryService : IInfoBarUiFactory
    {
        public IInfoBarUiElement CreateInfoBar(InfoBarModel infoBar)
        {
            return new InfoBarUiElement(infoBar);
        }
    }
}
