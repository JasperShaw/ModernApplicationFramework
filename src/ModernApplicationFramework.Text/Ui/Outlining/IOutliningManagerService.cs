using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Outlining
{
    public interface IOutliningManagerService
    {
        IOutliningManager GetOutliningManager(ITextView textView);
    }
}