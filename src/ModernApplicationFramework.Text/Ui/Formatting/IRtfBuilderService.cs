using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface IRtfBuilderService
    {
        string GenerateRtf(NormalizedSnapshotSpanCollection spans, string delimiter);

        string GenerateRtf(NormalizedSnapshotSpanCollection spans);

        string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView, string delimiter);

        string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView);
    }
}