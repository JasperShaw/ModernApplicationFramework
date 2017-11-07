using ModernApplicationFramework.Controls.InfoBar;

namespace ModernApplicationFramework.Interfaces.Controls.InfoBar
{
    public abstract class InfoBarActionItem : InfoBarTextSpan, IInfoBarActionItem
    {
        public abstract bool IsButton { get; }

        public object ActionContext { get; }

        protected InfoBarActionItem(string text, object actionContext = null) : base(text)
        {
            ActionContext = actionContext;
        }
    }
}
