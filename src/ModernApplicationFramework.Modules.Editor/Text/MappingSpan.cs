using System;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class MappingSpan : IMappingSpan
    {
        private SnapshotSpan _anchorSpan;
        private readonly SpanTrackingMode _trackingMode;

        public MappingSpan(SnapshotSpan anchorSpan, SpanTrackingMode trackingMode, IBufferGraph bufferGraph)
        {
            if (anchorSpan.Snapshot == null)
                throw new ArgumentNullException(nameof(anchorSpan));
            switch (trackingMode)
            {
                case SpanTrackingMode.EdgeExclusive:
                case SpanTrackingMode.EdgeInclusive:
                case SpanTrackingMode.EdgePositive:
                case SpanTrackingMode.EdgeNegative:
                    _anchorSpan = anchorSpan;
                    _trackingMode = trackingMode;
                    BufferGraph = bufferGraph ?? throw new ArgumentNullException(nameof(bufferGraph));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public IMappingPoint Start => new MappingPoint(new SnapshotPoint(_anchorSpan.Snapshot, _anchorSpan.Start), _trackingMode == SpanTrackingMode.EdgeInclusive || _trackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive, BufferGraph);

        public IMappingPoint End => new MappingPoint(new SnapshotPoint(_anchorSpan.Snapshot, _anchorSpan.End), _trackingMode == SpanTrackingMode.EdgeExclusive || _trackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive, BufferGraph);

        public ITextBuffer AnchorBuffer => _anchorSpan.Snapshot.TextBuffer;

        public IBufferGraph BufferGraph { get; }

        public NormalizedSnapshotSpanCollection GetSpans(ITextBuffer targetBuffer)
        {
            ITextBuffer anchorBuffer = AnchorBuffer;
            SnapshotSpan span = _anchorSpan.TranslateTo(anchorBuffer.CurrentSnapshot, _trackingMode);
            if (anchorBuffer == targetBuffer)
                return new NormalizedSnapshotSpanCollection(span);
            ITextBuffer topBuffer = BufferGraph.TopBuffer;
            if (targetBuffer == topBuffer)
                return BufferGraph.MapUpToBuffer(span, _trackingMode, topBuffer);
            if (anchorBuffer == topBuffer)
                return BufferGraph.MapDownToBuffer(span, _trackingMode, targetBuffer);
            if (anchorBuffer is IProjectionBufferBase)
            {
                NormalizedSnapshotSpanCollection buffer = BufferGraph.MapDownToBuffer(span, _trackingMode, targetBuffer);
                if (buffer.Count > 0)
                    return buffer;
            }
            return BufferGraph.MapUpToBuffer(span, _trackingMode, targetBuffer);
        }

        public NormalizedSnapshotSpanCollection GetSpans(ITextSnapshot targetSnapshot)
        {
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            NormalizedSnapshotSpanCollection snapshotSpanCollection = GetSpans(targetSnapshot.TextBuffer);
            if (snapshotSpanCollection.Count > 0 && snapshotSpanCollection[0].Snapshot != targetSnapshot)
            {
                FrugalList<SnapshotSpan> frugalList = new FrugalList<SnapshotSpan>();
                foreach (SnapshotSpan snapshotSpan in snapshotSpanCollection)
                    frugalList.Add(snapshotSpan.TranslateTo(targetSnapshot, _trackingMode));
                snapshotSpanCollection = new NormalizedSnapshotSpanCollection(frugalList);
            }
            return snapshotSpanCollection;
        }

        public NormalizedSnapshotSpanCollection GetSpans(Predicate<ITextBuffer> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            ITextBuffer anchorBuffer = AnchorBuffer;
            SnapshotSpan span = _anchorSpan.TranslateTo(anchorBuffer.CurrentSnapshot, _trackingMode);
            if (match(anchorBuffer))
                return new NormalizedSnapshotSpanCollection(span);
            if (anchorBuffer == BufferGraph.TopBuffer)
                return BufferGraph.MapDownToFirstMatch(span, _trackingMode, snapshot => match(snapshot.TextBuffer));
            if (anchorBuffer is IProjectionBufferBase)
            {
                NormalizedSnapshotSpanCollection firstMatch = BufferGraph.MapDownToFirstMatch(span, _trackingMode, snapshot => match(snapshot.TextBuffer));
                if (firstMatch.Count > 0)
                    return firstMatch;
            }
            return BufferGraph.MapUpToFirstMatch(span, _trackingMode, snapshot => match(snapshot.TextBuffer));
        }

        public override string ToString()
        {
            return $"MappingSpan anchored at {_anchorSpan as object}";
        }
    }
}