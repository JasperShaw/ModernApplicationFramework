using System;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public class ElisionSourceSpansChangedEventArgs : TextContentChangedEventArgs
    {
        public new IProjectionSnapshot After => (IProjectionSnapshot) base.After;

        public new IProjectionSnapshot Before => (IProjectionSnapshot) base.Before;

        public NormalizedSpanCollection ElidedSpans { get; }

        public NormalizedSpanCollection ExpandedSpans { get; }

        public ElisionSourceSpansChangedEventArgs(ITextSnapshot beforeSnapshot, IProjectionSnapshot afterSnapshot,
            NormalizedSpanCollection elidedSpans, NormalizedSpanCollection expandedSpans, object sourceToken)
            : base(beforeSnapshot, afterSnapshot, EditOptions.None, sourceToken)
        {
            ElidedSpans = elidedSpans ?? throw new ArgumentNullException(nameof(elidedSpans));
            ExpandedSpans = expandedSpans ?? throw new ArgumentNullException(nameof(expandedSpans));
        }
    }
}