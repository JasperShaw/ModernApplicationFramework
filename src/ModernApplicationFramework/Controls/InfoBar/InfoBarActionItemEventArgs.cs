using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar
{
    public class InfoBarActionItemEventArgs : InfoBarEventArgs
    {
        public InfoBarActionItemEventArgs(IInfoBarUiElement uiElement, InfoBarModel infoBar, IInfoBarActionItem actionItem)
            : base(uiElement, infoBar)
        {
            Validate.IsNotNull(actionItem, nameof(actionItem));
            ActionItem = actionItem;
        }

        public IInfoBarActionItem ActionItem { get; }
    }
}
