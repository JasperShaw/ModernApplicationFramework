using System;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public interface IElisionBuffer : IProjectionBufferBase
    {
        event EventHandler<ElisionSourceSpansChangedEventArgs> SourceSpansChanged;

        new IElisionSnapshot CurrentSnapshot { get; }

        ElisionBufferOptions Options { get; }
        ITextBuffer SourceBuffer { get; }

        IProjectionSnapshot ElideSpans(NormalizedSpanCollection spansToElide);

        IProjectionSnapshot ExpandSpans(NormalizedSpanCollection spansToExpand);

        IProjectionSnapshot ModifySpans(NormalizedSpanCollection spansToElide, NormalizedSpanCollection spansToExpand);
    }
}