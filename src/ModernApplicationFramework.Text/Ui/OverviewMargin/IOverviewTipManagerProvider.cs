using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.OverviewMargin
{
    public interface IOverviewTipManagerProvider
    {
        IOverviewTipManager GetOverviewTipManager(ITextViewHost host);
    }
}