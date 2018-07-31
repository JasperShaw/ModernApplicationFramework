using System.Threading;

namespace ModernApplicationFramework.TextEditor
{
    public interface IRtfBuilderService2 : IRtfBuilderService
    {
        string GenerateRtf(NormalizedSnapshotSpanCollection spans, string delimiter, CancellationToken cancellationToken);

        string GenerateRtf(NormalizedSnapshotSpanCollection spans, CancellationToken cancellationToken);

        string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView, string delimiter, CancellationToken cancellationToken);

        string GenerateRtf(NormalizedSnapshotSpanCollection spans, ITextView textView, CancellationToken cancellationToken);
    }
}