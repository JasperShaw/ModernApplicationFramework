using System;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public interface IBufferGraph
    {
        event EventHandler<GraphBufferContentTypeChangedEventArgs> GraphBufferContentTypeChanged;

        event EventHandler<GraphBuffersChangedEventArgs> GraphBuffersChanged;
        ITextBuffer TopBuffer { get; }

        IMappingPoint CreateMappingPoint(SnapshotPoint point, PointTrackingMode trackingMode);

        IMappingSpan CreateMappingSpan(SnapshotSpan span, SpanTrackingMode trackingMode);

        Collection<ITextBuffer> GetTextBuffers(Predicate<ITextBuffer> match);

        SnapshotPoint? MapDownToBuffer(SnapshotPoint position, PointTrackingMode trackingMode, ITextBuffer targetBuffer,
            PositionAffinity affinity);

        NormalizedSnapshotSpanCollection MapDownToBuffer(SnapshotSpan span, SpanTrackingMode trackingMode,
            ITextBuffer targetBuffer);

        SnapshotPoint? MapDownToFirstMatch(SnapshotPoint position, PointTrackingMode trackingMode,
            Predicate<ITextSnapshot> match, PositionAffinity affinity);

        NormalizedSnapshotSpanCollection MapDownToFirstMatch(SnapshotSpan span, SpanTrackingMode trackingMode,
            Predicate<ITextSnapshot> match);

        SnapshotPoint? MapDownToInsertionPoint(SnapshotPoint position, PointTrackingMode trackingMode,
            Predicate<ITextSnapshot> match);

        SnapshotPoint? MapDownToSnapshot(SnapshotPoint position, PointTrackingMode trackingMode,
            ITextSnapshot targetSnapshot, PositionAffinity affinity);

        NormalizedSnapshotSpanCollection MapDownToSnapshot(SnapshotSpan span, SpanTrackingMode trackingMode,
            ITextSnapshot targetSnapshot);

        SnapshotPoint? MapUpToBuffer(SnapshotPoint point, PointTrackingMode trackingMode, PositionAffinity affinity,
            ITextBuffer targetBuffer);

        NormalizedSnapshotSpanCollection MapUpToBuffer(SnapshotSpan span, SpanTrackingMode trackingMode,
            ITextBuffer targetBuffer);

        SnapshotPoint? MapUpToFirstMatch(SnapshotPoint point, PointTrackingMode trackingMode,
            Predicate<ITextSnapshot> match, PositionAffinity affinity);

        NormalizedSnapshotSpanCollection MapUpToFirstMatch(SnapshotSpan span, SpanTrackingMode trackingMode,
            Predicate<ITextSnapshot> match);

        SnapshotPoint? MapUpToSnapshot(SnapshotPoint point, PointTrackingMode trackingMode, PositionAffinity affinity,
            ITextSnapshot targetSnapshot);

        NormalizedSnapshotSpanCollection MapUpToSnapshot(SnapshotSpan span, SpanTrackingMode trackingMode,
            ITextSnapshot targetSnapshot);
    }
}