using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal class MappingPointSnapshot : IMappingPoint
    {
        internal ITextSnapshot Root;
        internal SnapshotPoint Anchor;
        internal PointTrackingMode TrackingMode;
        internal bool Unmappable;

        public static IMappingPoint Create(ITextSnapshot root, SnapshotPoint anchor, PointTrackingMode trackingMode, IBufferGraph graph)
        {
            return new MappingPointSnapshot(root, anchor, trackingMode, graph);
        }

        private MappingPointSnapshot(ITextSnapshot root, SnapshotPoint anchor, PointTrackingMode trackingMode, IBufferGraph graph)
        {
            var correspondingSnapshot = MappingHelper.FindCorrespondingSnapshot(root, anchor.Snapshot.TextBuffer);
            Root = root;
            if (correspondingSnapshot != null)
            {
                Anchor = anchor.TranslateTo(correspondingSnapshot, trackingMode);
            }
            else
            {
                Anchor = anchor;
                Unmappable = true;
            }
            TrackingMode = trackingMode;
            BufferGraph = graph;
        }

        public SnapshotPoint? GetPoint(ITextBuffer targetBuffer, PositionAffinity affinity)
        {
            if (targetBuffer == null)
                throw new ArgumentNullException(nameof(targetBuffer));
            if (Unmappable)
                return new SnapshotPoint?();
            var bufferNoTrack = MappingHelper.MapDownToBufferNoTrack(Anchor, targetBuffer, affinity);
            if (!bufferNoTrack.HasValue)
                return MapUpToBufferNoTrack(targetBuffer, affinity);
            return bufferNoTrack;
        }

        public SnapshotPoint? GetPoint(ITextSnapshot targetSnapshot, PositionAffinity affinity)
        {
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            if (Unmappable)
                return new SnapshotPoint?();
            var nullable = GetPoint(targetSnapshot.TextBuffer, affinity);
            if (nullable.HasValue && nullable.Value.Snapshot != targetSnapshot)
                nullable = nullable.Value.TranslateTo(targetSnapshot, TrackingMode);
            return nullable;
        }

        public SnapshotPoint? GetPoint(Predicate<ITextBuffer> match, PositionAffinity affinity)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            if (Unmappable)
                return new SnapshotPoint?();
            var firstMatchNoTrack = MappingHelper.MapDownToFirstMatchNoTrack(Anchor, match, affinity);
            if (!firstMatchNoTrack.HasValue)
                return MapUpToFirstMatchNoTrack(match, affinity);
            return firstMatchNoTrack;
        }

        public SnapshotPoint? GetInsertionPoint(Predicate<ITextBuffer> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            if (Unmappable)
                return new SnapshotPoint?();
            return MappingHelper.MapDownToFirstMatchNoTrack(Anchor, match);
        }

        public ITextBuffer AnchorBuffer => Anchor.Snapshot.TextBuffer;

        public IBufferGraph BufferGraph { get; }

        private SnapshotPoint? MapUpToBufferNoTrack(ITextBuffer targetBuffer, PositionAffinity affinity)
        {
            var correspondingSnapshot = MappingHelper.FindCorrespondingSnapshot(Root, targetBuffer);
            if (correspondingSnapshot != null)
                return MapUpToSnapshotNoTrack(correspondingSnapshot, affinity);
            return new SnapshotPoint?();
        }

        private SnapshotPoint? MapUpToFirstMatchNoTrack(Predicate<ITextBuffer> match, PositionAffinity affinity)
        {
            var correspondingSnapshot = MappingHelper.FindCorrespondingSnapshot(Root, match);
            if (correspondingSnapshot != null)
                return MapUpToSnapshotNoTrack(correspondingSnapshot, affinity);
            return new SnapshotPoint?();
        }

        private SnapshotPoint? MapUpToSnapshotNoTrack(ITextSnapshot targetSnapshot, PositionAffinity affinity)
        {
            return MapUpToSnapshotNoTrack(targetSnapshot, Anchor, affinity);
        }

        public static SnapshotPoint? MapUpToSnapshotNoTrack(ITextSnapshot targetSnapshot, SnapshotPoint anchor, PositionAffinity affinity)
        {
            if (anchor.Snapshot == targetSnapshot)
                return anchor;
            if (targetSnapshot is IProjectionSnapshot projectionSnapshot)
            {
                var sourceSnapshots = projectionSnapshot.SourceSnapshots;
                foreach (var t in sourceSnapshots)
                {
                    var snapshotNoTrack = MapUpToSnapshotNoTrack(t, anchor, affinity);
                    if (snapshotNoTrack.HasValue)
                    {
                        var nullable = projectionSnapshot.MapFromSourceSnapshot(snapshotNoTrack.Value, affinity);
                        if (nullable.HasValue)
                            return nullable;
                    }
                }
            }
            return new SnapshotPoint?();
        }
    }
}