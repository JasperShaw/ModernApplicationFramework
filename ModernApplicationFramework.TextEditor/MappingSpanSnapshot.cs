using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal class MappingSpanSnapshot : IMappingSpan
    {
        private readonly ITextSnapshot _root;
        private readonly SnapshotSpan _anchor;
        private readonly SpanTrackingMode _trackingMode;
        private readonly bool _unmappable;

        public static IMappingSpan Create(ITextSnapshot root, SnapshotSpan anchor, SpanTrackingMode trackingMode, IBufferGraph graph)
        {
            return new MappingSpanSnapshot(root, anchor, trackingMode, graph);
        }

        private MappingSpanSnapshot(ITextSnapshot root, SnapshotSpan anchor, SpanTrackingMode trackingMode, IBufferGraph graph)
        {
            var correspondingSnapshot = MappingHelper.FindCorrespondingSnapshot(root, anchor.Snapshot.TextBuffer);
            _root = root;
            if (correspondingSnapshot != null)
            {
                _anchor = anchor.TranslateTo(correspondingSnapshot, trackingMode);
            }
            else
            {
                _anchor = anchor;
                _unmappable = true;
            }
            _trackingMode = trackingMode;
            BufferGraph = graph;
        }

        public NormalizedSnapshotSpanCollection GetSpans(ITextBuffer targetBuffer)
        {
            if (targetBuffer == null)
                throw new ArgumentNullException(nameof(targetBuffer));
            if (_unmappable)
                return NormalizedSnapshotSpanCollection.Empty;
            if (targetBuffer.Properties.ContainsProperty("IdentityMapping") && (ITextBuffer)targetBuffer.Properties["IdentityMapping"] == _anchor.Snapshot.TextBuffer)
                return new NormalizedSnapshotSpanCollection(new SnapshotSpan(MappingHelper.FindCorrespondingSnapshot(_root, targetBuffer), _anchor.Span));
            var mappedSpans = new FrugalList<SnapshotSpan>();
            MappingHelper.MapDownToBufferNoTrack(_anchor, targetBuffer, mappedSpans);
            if (mappedSpans.Count == 0)
                MapUpToBufferNoTrack(targetBuffer, mappedSpans);
            return new NormalizedSnapshotSpanCollection(mappedSpans);
        }

        public NormalizedSnapshotSpanCollection GetSpans(ITextSnapshot targetSnapshot)
        {
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            if (_unmappable)
                return NormalizedSnapshotSpanCollection.Empty;
            var snapshotSpanCollection = GetSpans(targetSnapshot.TextBuffer);
            if (snapshotSpanCollection.Count > 0 && snapshotSpanCollection[0].Snapshot != targetSnapshot)
            {
                var frugalList = new FrugalList<SnapshotSpan>();
                foreach (var snapshotSpan in snapshotSpanCollection)
                    frugalList.Add(snapshotSpan.TranslateTo(targetSnapshot, _trackingMode));
                snapshotSpanCollection = new NormalizedSnapshotSpanCollection(frugalList);
            }
            return snapshotSpanCollection;
        }

        public NormalizedSnapshotSpanCollection GetSpans(Predicate<ITextBuffer> match)
        {
            if (_unmappable)
                return NormalizedSnapshotSpanCollection.Empty;
            var mappedSpans = new FrugalList<SnapshotSpan>();
            MappingHelper.MapDownToFirstMatchNoTrack(_anchor, match, mappedSpans);
            if (mappedSpans.Count == 0)
                MapUpToBufferNoTrack(match, mappedSpans);
            return new NormalizedSnapshotSpanCollection(mappedSpans);
        }

        public IMappingPoint Start => MappingPointSnapshot.Create(_root, _anchor.Start, _trackingMode == SpanTrackingMode.EdgeInclusive || _trackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive, BufferGraph);

        public IMappingPoint End => MappingPointSnapshot.Create(_root, _anchor.End, _trackingMode == SpanTrackingMode.EdgeExclusive || _trackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive, BufferGraph);

        public ITextBuffer AnchorBuffer => _anchor.Snapshot.TextBuffer;

        public IBufferGraph BufferGraph { get; }

        private void MapUpToBufferNoTrack(ITextBuffer targetBuffer, FrugalList<SnapshotSpan> mappedSpans)
        {
            var correspondingSnapshot = MappingHelper.FindCorrespondingSnapshot(_root, targetBuffer);
            if (correspondingSnapshot == null)
                return;
            MapUpToSnapshotNoTrack(correspondingSnapshot, mappedSpans);
        }

        private void MapUpToBufferNoTrack(Predicate<ITextBuffer> match, FrugalList<SnapshotSpan> mappedSpans)
        {
            var correspondingSnapshot = MappingHelper.FindCorrespondingSnapshot(_root, match);
            if (correspondingSnapshot == null)
                return;
            MapUpToSnapshotNoTrack(correspondingSnapshot, mappedSpans);
        }

        private void MapUpToSnapshotNoTrack(ITextSnapshot targetSnapshot, FrugalList<SnapshotSpan> mappedSpans)
        {
            MapUpToSnapshotNoTrack(targetSnapshot, _anchor, mappedSpans);
        }

        public static void MapUpToSnapshotNoTrack(ITextSnapshot targetSnapshot, SnapshotSpan anchor, IList<SnapshotSpan> mappedSpans)
        {
            if (anchor.Snapshot == targetSnapshot)
            {
                mappedSpans.Add(anchor);
            }
            else
            {
                var projectionSnapshot = targetSnapshot as IProjectionSnapshot;
                if (projectionSnapshot == null)
                    return;
                var sourceSnapshots = projectionSnapshot.SourceSnapshots;
                foreach (var t2 in sourceSnapshots)
                {
                    var frugalList = new FrugalList<SnapshotSpan>();
                    MapUpToSnapshotNoTrack(t2, anchor, frugalList);
                    foreach (var t1 in frugalList)
                    {
                        var readOnlyCollection = projectionSnapshot.MapFromSourceSnapshot(t1);
                        foreach (var t in readOnlyCollection)
                            mappedSpans.Add(new SnapshotSpan(targetSnapshot, t));
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"MappingSpanSnapshot anchored at { _anchor}";
        }
    }
}