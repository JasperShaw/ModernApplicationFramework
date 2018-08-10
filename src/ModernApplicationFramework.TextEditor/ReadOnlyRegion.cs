using System.Globalization;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor
{
    internal class ReadOnlyRegion : IReadOnlyRegion
    {
        internal ReadOnlyRegion(ITextVersion version, Span span, SpanTrackingMode trackingMode, EdgeInsertionMode edgeInsertionMode, DynamicReadOnlyRegionQuery callback)
        {
            EdgeInsertionMode = edgeInsertionMode;
            Span = new ForwardFidelityTrackingSpan(version, span, trackingMode);
            QueryCallback = callback;
        }

        public DynamicReadOnlyRegionQuery QueryCallback { get; }

        public ITrackingSpan Span { get; }

        public EdgeInsertionMode EdgeInsertionMode { get; }

        public override string ToString()
        {
            Span span = Span.GetSpan(Span.TextBuffer.CurrentSnapshot);
            return string.Format(CultureInfo.InvariantCulture, "RO: {2}{0}..{1}{3}", span.Start, span.End,
                EdgeInsertionMode == EdgeInsertionMode.Deny ?  "[" : "(",
                EdgeInsertionMode == EdgeInsertionMode.Deny ?  "]" : ")");
        }
    }
}