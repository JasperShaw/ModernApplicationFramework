using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor
{
    internal class ReadOnlySpan : ForwardFidelityTrackingSpan
    {
        internal ReadOnlySpan(ITextVersion version, Span span, SpanTrackingMode trackingMode, EdgeInsertionMode startEdgeInsertionMode, EdgeInsertionMode endEdgeInsertionMode)
            : base(version, span, trackingMode)
        {
            StartEdgeInsertionMode = startEdgeInsertionMode;
            EndEdgeInsertionMode = endEdgeInsertionMode;
        }

        internal ReadOnlySpan(ITextVersion version, IReadOnlyRegion readOnlyRegion)
            : base(version, readOnlyRegion.Span.GetSpan(version), readOnlyRegion.Span.TrackingMode)
        {
            StartEdgeInsertionMode = readOnlyRegion.EdgeInsertionMode;
            EndEdgeInsertionMode = readOnlyRegion.EdgeInsertionMode;
        }

        public EdgeInsertionMode StartEdgeInsertionMode { get; }

        public EdgeInsertionMode EndEdgeInsertionMode { get; }

        public bool IsReplaceAllowed(Span span, ITextSnapshot textSnapshot)
        {
            if (span.Length == 0)
                return IsInsertAllowed(span.Start, textSnapshot);
            Span span1 = GetSpan(textSnapshot);
            return span1.Length == 0 || !(span1 == span) && !span1.OverlapsWith(span);
        }

        public bool IsInsertAllowed(int position, ITextSnapshot textSnapshot)
        {
            Span span = GetSpan(textSnapshot);
            return (span.Start >= position || span.End <= position) && (StartEdgeInsertionMode != EdgeInsertionMode.Deny || position != span.Start) && (EndEdgeInsertionMode != EdgeInsertionMode.Deny || position != span.End);
        }
    }
}