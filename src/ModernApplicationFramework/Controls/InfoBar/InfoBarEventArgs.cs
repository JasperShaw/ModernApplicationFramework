using System;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar
{
    public class InfoBarEventArgs : EventArgs
    {
        public InfoBarEventArgs(IInfoBarUiElement uiElement, InfoBarModel infoBar)
        {
            Validate.IsNotNull(uiElement, nameof(uiElement));
            InfoBarUiElement = uiElement;
            InfoBarModel = infoBar;
        }

        public IInfoBarUiElement InfoBarUiElement { get; }

        public InfoBarModel InfoBarModel { get; }
    }
}
