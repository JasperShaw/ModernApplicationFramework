using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal static class MappingHelper
    {
        internal static ITextSnapshot FindCorrespondingSnapshot(ITextSnapshot sourceSnapshot, ITextBuffer targetBuffer)
        {
            if (sourceSnapshot.TextBuffer == targetBuffer)
                return sourceSnapshot;
            return (sourceSnapshot as IProjectionSnapshot)?.GetMatchingSnapshotInClosure(targetBuffer);
        }

        internal static ITextSnapshot FindCorrespondingSnapshot(ITextSnapshot sourceSnapshot, Predicate<ITextBuffer> match)
        {
            if (match(sourceSnapshot.TextBuffer))
                return sourceSnapshot;
            return (sourceSnapshot as IProjectionSnapshot)?.GetMatchingSnapshotInClosure(match);
        }

        internal static NormalizedSnapshotSpanCollection MapDownToBufferNoTrack(SnapshotSpan sourceSpan, ITextBuffer targetBuffer, bool mapByContentType = false)
        {
            var frugalList = new FrugalList<SnapshotSpan>();
            MapDownToFirstMatchNoTrack(sourceSpan, b => b == targetBuffer, frugalList, mapByContentType);
            return new NormalizedSnapshotSpanCollection(frugalList);
        }

        internal static void MapDownToBufferNoTrack(SnapshotSpan sourceSpan, ITextBuffer targetBuffer, IList<SnapshotSpan> mappedSpans, bool mapByContentType = false)
        {
            SnapshotSpan snapshotSpan;
            ReadOnlyCollection<SnapshotSpan> sourceSnapshots;
            for (snapshotSpan = sourceSpan; snapshotSpan.Snapshot.TextBuffer != targetBuffer; snapshotSpan = sourceSnapshots[0])
            {
                if (!(snapshotSpan.Snapshot is IProjectionSnapshot snapshot) || mapByContentType && !snapshot.ContentType.IsOfType("projection"))
                    return;
                sourceSnapshots = snapshot.MapToSourceSnapshots(snapshotSpan);
                if (sourceSnapshots.Count != 1)
                {
                    if (sourceSnapshots.Count == 0)
                        return;
                    SplitMapDownToBufferNoTrack(new FrugalList<SnapshotSpan>(sourceSnapshots), targetBuffer, mappedSpans, mapByContentType);
                    return;
                }
            }
            mappedSpans.Add(snapshotSpan);
        }

        private static void SplitMapDownToBufferNoTrack(FrugalList<SnapshotSpan> unmappedSpans, ITextBuffer targetBuffer, IList<SnapshotSpan> mappedSpans, bool mapByContentType)
        {
            while (unmappedSpans.Count > 0)
            {
                var unmappedSpan = unmappedSpans[unmappedSpans.Count - 1];
                unmappedSpans.RemoveAt(unmappedSpans.Count - 1);
                if (unmappedSpan.Snapshot.TextBuffer == targetBuffer)
                {
                    mappedSpans.Add(unmappedSpan);
                }
                else
                {
                    if (unmappedSpan.Snapshot is IProjectionSnapshot snapshot && (!mapByContentType || unmappedSpan.Snapshot.TextBuffer.ContentType.IsOfType("projection")))
                        unmappedSpans.AddRange(snapshot.MapToSourceSnapshots(unmappedSpan));
                }
            }
        }

        internal static void MapDownToFirstMatchNoTrack(SnapshotSpan sourceSpan, Predicate<ITextBuffer> match, IList<SnapshotSpan> mappedSpans, bool mapByContentType = false)
        {
            SnapshotSpan snapshotSpan;
            ReadOnlyCollection<SnapshotSpan> sourceSnapshots;
            for (snapshotSpan = sourceSpan; !match(snapshotSpan.Snapshot.TextBuffer); snapshotSpan = sourceSnapshots[0])
            {
                var snapshot = snapshotSpan.Snapshot as IProjectionSnapshot;
                if (snapshot == null || mapByContentType && !snapshot.ContentType.IsOfType("projection"))
                    return;
                sourceSnapshots = snapshot.MapToSourceSnapshots(snapshotSpan);
                if (sourceSnapshots.Count != 1)
                {
                    if (sourceSnapshots.Count == 0)
                        return;
                    SplitMapDownToFirstMatchNoTrack(new FrugalList<SnapshotSpan>(sourceSnapshots), match, mappedSpans, mapByContentType);
                    return;
                }
            }
            mappedSpans.Add(snapshotSpan);
        }

        private static void SplitMapDownToFirstMatchNoTrack(FrugalList<SnapshotSpan> unmappedSpans, Predicate<ITextBuffer> match, IList<SnapshotSpan> mappedSpans, bool mapByContentType)
        {
            ITextSnapshot textSnapshot = null;
            while (unmappedSpans.Count > 0)
            {
                var unmappedSpan = unmappedSpans[unmappedSpans.Count - 1];
                unmappedSpans.RemoveAt(unmappedSpans.Count - 1);
                if (unmappedSpan.Snapshot == textSnapshot)
                    mappedSpans.Add(unmappedSpan);
                else if (match(unmappedSpan.Snapshot.TextBuffer))
                {
                    mappedSpans.Add(unmappedSpan);
                    textSnapshot = unmappedSpan.Snapshot;
                }
                else
                {
                    if (unmappedSpan.Snapshot is IProjectionSnapshot snapshot && (!mapByContentType || unmappedSpan.Snapshot.TextBuffer.ContentType.IsOfType("projection")))
                        unmappedSpans.AddRange(snapshot.MapToSourceSnapshots(unmappedSpan));
                }
            }
        }

        internal static SnapshotPoint? MapDownToBufferNoTrack(SnapshotPoint position, ITextBuffer targetBuffer, PositionAffinity affinity)
        {
            IProjectionSnapshot snapshot;
            for (; position.Snapshot.TextBuffer != targetBuffer; position = snapshot.MapToSourceSnapshot(position, affinity))
            {
                snapshot = position.Snapshot as IProjectionSnapshot;
                if (snapshot == null || snapshot.SourceSnapshots.Count == 0)
                    return new SnapshotPoint?();
            }
            return position;
        }

        internal static SnapshotPoint? MapDownToFirstMatchNoTrack(SnapshotPoint position, Predicate<ITextBuffer> match, PositionAffinity affinity)
        {
            IProjectionSnapshot snapshot;
            for (; !match(position.Snapshot.TextBuffer); position = snapshot.MapToSourceSnapshot(position, affinity))
            {
                snapshot = position.Snapshot as IProjectionSnapshot;
                if (snapshot == null || snapshot.SourceSnapshots.Count == 0)
                    return new SnapshotPoint?();
            }
            return position;
        }

        internal static SnapshotPoint? MapDownToBufferNoTrack(SnapshotPoint position, ITextBuffer targetBuffer)
        {
            IProjectionSnapshot snapshot;
            for (; position.Snapshot.TextBuffer != targetBuffer; position = snapshot.MapToSourceSnapshot(position))
            {
                snapshot = position.Snapshot as IProjectionSnapshot;
                if (snapshot == null || snapshot.SourceSnapshots.Count == 0)
                    return new SnapshotPoint?();
            }
            return position;
        }

        internal static SnapshotPoint? MapDownToFirstMatchNoTrack(SnapshotPoint position, Predicate<ITextBuffer> match)
        {
            IProjectionSnapshot snapshot;
            for (; !match(position.Snapshot.TextBuffer); position = snapshot.MapToSourceSnapshot(position))
            {
                snapshot = position.Snapshot as IProjectionSnapshot;
                if (snapshot == null || snapshot.SourceSnapshots.Count == 0)
                    return new SnapshotPoint?();
            }
            return position;
        }
    }
}