using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    public interface ISmartIndentationService
    {
        int? GetDesiredIndentation(ITextView textView, ITextSnapshotLine line);
    }
}