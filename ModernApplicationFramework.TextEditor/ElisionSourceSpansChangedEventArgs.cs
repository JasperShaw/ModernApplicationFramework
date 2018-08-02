using System;

namespace ModernApplicationFramework.TextEditor
{
    public class ElisionSourceSpansChangedEventArgs : TextContentChangedEventArgs
    {
        public ElisionSourceSpansChangedEventArgs(ITextSnapshot beforeSnapshot, IProjectionSnapshot afterSnapshot, NormalizedSpanCollection elidedSpans, NormalizedSpanCollection expandedSpans, object sourceToken)
            : base(beforeSnapshot, afterSnapshot, EditOptions.None, sourceToken)
        {
            ElidedSpans = elidedSpans ?? throw new ArgumentNullException(nameof(elidedSpans));
            ExpandedSpans = expandedSpans ?? throw new ArgumentNullException(nameof(expandedSpans));
        }

        public NormalizedSpanCollection ElidedSpans { get; }

        public NormalizedSpanCollection ExpandedSpans { get; }

        public new IProjectionSnapshot Before => (IProjectionSnapshot) base.Before;

        public new IProjectionSnapshot After => (IProjectionSnapshot) base.After;
    }
}