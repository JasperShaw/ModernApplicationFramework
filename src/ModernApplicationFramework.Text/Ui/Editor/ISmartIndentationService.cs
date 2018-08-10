using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ISmartIndentationService
    {
        int? GetDesiredIndentation(ITextView textView, ITextSnapshotLine line);
    }
}