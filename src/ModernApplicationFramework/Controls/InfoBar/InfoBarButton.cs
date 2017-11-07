using ModernApplicationFramework.Interfaces.Controls.InfoBar;

namespace ModernApplicationFramework.Controls.InfoBar
{
    public class InfoBarButton : InfoBarActionItem
    {
        public InfoBarButton(string text, object actionContext = null)
            : base(text, actionContext)
        {
        }

        public override bool IsButton => true;
    }
}
