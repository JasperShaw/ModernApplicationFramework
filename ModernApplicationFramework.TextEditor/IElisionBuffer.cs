using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface IElisionBuffer : IProjectionBufferBase
    {
        ITextBuffer SourceBuffer { get; }

        new IElisionSnapshot CurrentSnapshot { get; }

        IProjectionSnapshot ElideSpans(NormalizedSpanCollection spansToElide);

        IProjectionSnapshot ExpandSpans(NormalizedSpanCollection spansToExpand);

        IProjectionSnapshot ModifySpans(NormalizedSpanCollection spansToElide, NormalizedSpanCollection spansToExpand);

        ElisionBufferOptions Options { get; }

        event EventHandler<ElisionSourceSpansChangedEventArgs> SourceSpansChanged;
    }
}