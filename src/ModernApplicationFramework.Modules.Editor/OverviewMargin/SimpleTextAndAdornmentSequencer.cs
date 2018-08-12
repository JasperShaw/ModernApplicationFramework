using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal class SimpleTextAndAdornmentSequencer : ITextAndAdornmentSequencer
    {
        public event EventHandler<TextAndAdornmentSequenceChangedEventArgs> SequenceChanged;
        public IBufferGraph BufferGraph { get; }

        public ITextBuffer SourceBuffer => TopBuffer;

        public ITextBuffer TopBuffer { get; }

        public SimpleTextAndAdornmentSequencer(IBufferGraph graph, ITextBuffer topBuffer)
        {
            BufferGraph = graph;
            TopBuffer = topBuffer;
        }

        public ITextAndAdornmentCollection CreateTextAndAdornmentCollection(SnapshotSpan topSpan,
            ITextSnapshot sourceTextSnapshot)
        {
            return new SimpleTextAndAdornmentCollection(this,
                BufferGraph.CreateMappingSpan(topSpan, SpanTrackingMode.EdgeExclusive));
        }

        public ITextAndAdornmentCollection CreateTextAndAdornmentCollection(ITextSnapshotLine topLine,
            ITextSnapshot sourceTextSnapshot)
        {
            return new SimpleTextAndAdornmentCollection(this,
                BufferGraph.CreateMappingSpan(topLine.ExtentIncludingLineBreak, SpanTrackingMode.EdgeExclusive));
        }

        private class SimpleSequenceElement : ISequenceElement
        {
            public bool ShouldRenderText => true;

            public IMappingSpan Span { get; }

            public SimpleSequenceElement(IMappingSpan span)
            {
                Span = span;
            }

            public override string ToString()
            {
                return "SimpleSequenceElement Element span " + Span;
            }
        }

        private class SimpleTextAndAdornmentCollection : ReadOnlyCollection<ISequenceElement>,
            ITextAndAdornmentCollection
        {
            public ITextAndAdornmentSequencer Sequencer { get; }

            public SimpleTextAndAdornmentCollection(ITextAndAdornmentSequencer sequencer, IMappingSpan span)
                : base(new List<ISequenceElement>(1)
                {
                    new SimpleSequenceElement(span)
                })
            {
                Sequencer = sequencer;
            }
        }
    }
}