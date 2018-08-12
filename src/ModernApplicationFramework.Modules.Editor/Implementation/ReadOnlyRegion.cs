using System.Globalization;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class ReadOnlyRegion : IReadOnlyRegion
    {
        public EdgeInsertionMode EdgeInsertionMode { get; }

        public DynamicReadOnlyRegionQuery QueryCallback { get; }

        public ITrackingSpan Span { get; }

        internal ReadOnlyRegion(ITextVersion version, Span span, SpanTrackingMode trackingMode,
            EdgeInsertionMode edgeInsertionMode, DynamicReadOnlyRegionQuery callback)
        {
            EdgeInsertionMode = edgeInsertionMode;
            Span = new ForwardFidelityTrackingSpan(version, span, trackingMode);
            QueryCallback = callback;
        }

        public override string ToString()
        {
            Span span = Span.GetSpan(Span.TextBuffer.CurrentSnapshot);
            return string.Format(CultureInfo.InvariantCulture, "RO: {2}{0}..{1}{3}", span.Start, span.End,
                EdgeInsertionMode == EdgeInsertionMode.Deny ? "[" : "(",
                EdgeInsertionMode == EdgeInsertionMode.Deny ? "]" : ")");
        }
    }
}