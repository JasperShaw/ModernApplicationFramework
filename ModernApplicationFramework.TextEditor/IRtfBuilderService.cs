namespace ModernApplicationFramework.TextEditor
{
    public interface IRtfBuilderService
    {
        string GenerateRtf(NormalizedSnapshotSpanCollection spans, string delimiter);

        string GenerateRtf(NormalizedSnapshotSpanCollection spans);

        string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView, string delimiter);

        string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView);
    }
}