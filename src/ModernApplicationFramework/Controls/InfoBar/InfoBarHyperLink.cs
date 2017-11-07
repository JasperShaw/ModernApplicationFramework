using ModernApplicationFramework.Interfaces.Controls.InfoBar;

namespace ModernApplicationFramework.Controls.InfoBar
{
    public class InfoBarHyperlink : InfoBarActionItem
    {
        public InfoBarHyperlink(string text, object actionContext = null) : base(text, actionContext)
        {
        }

        public override bool IsButton => false;
    }
}
