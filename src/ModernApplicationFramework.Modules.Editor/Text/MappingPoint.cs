﻿using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class MappingPoint : IMappingPoint
    {
        private readonly PointTrackingMode _trackingMode;
        private SnapshotPoint _anchorPoint;

        public ITextBuffer AnchorBuffer => _anchorPoint.Snapshot.TextBuffer;

        public IBufferGraph BufferGraph { get; }

        public MappingPoint(SnapshotPoint anchorPoint, PointTrackingMode trackingMode, IBufferGraph bufferGraph)
        {
            if (anchorPoint.Snapshot == null)
                throw new ArgumentNullException(nameof(anchorPoint));
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    _anchorPoint = anchorPoint;
                    _trackingMode = trackingMode;
                    BufferGraph = bufferGraph ?? throw new ArgumentNullException(nameof(bufferGraph));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public SnapshotPoint? GetInsertionPoint(Predicate<ITextBuffer> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            return BufferGraph.MapDownToInsertionPoint(
                _anchorPoint.TranslateTo(AnchorBuffer.CurrentSnapshot, _trackingMode), _trackingMode,
                snapshot => match(snapshot.TextBuffer));
        }

        public SnapshotPoint? GetPoint(ITextBuffer targetBuffer, PositionAffinity affinity)
        {
            if (targetBuffer == null)
                throw new ArgumentNullException(nameof(targetBuffer));
            var anchorBuffer = AnchorBuffer;
            var snapshotPoint = _anchorPoint.TranslateTo(anchorBuffer.CurrentSnapshot, _trackingMode);
            if (anchorBuffer == targetBuffer)
                return snapshotPoint;
            var topBuffer = BufferGraph.TopBuffer;
            if (targetBuffer == topBuffer)
                return BufferGraph.MapUpToBuffer(snapshotPoint, _trackingMode, affinity, topBuffer);
            if (anchorBuffer == topBuffer)
                return BufferGraph.MapDownToBuffer(snapshotPoint, _trackingMode, targetBuffer, affinity);
            if (anchorBuffer is IProjectionBufferBase)
            {
                var buffer = BufferGraph.MapDownToBuffer(snapshotPoint, _trackingMode, targetBuffer, affinity);
                if (buffer.HasValue)
                    return buffer;
            }

            return BufferGraph.MapUpToBuffer(snapshotPoint, _trackingMode, affinity, targetBuffer);
        }

        public SnapshotPoint? GetPoint(ITextSnapshot targetSnapshot, PositionAffinity affinity)
        {
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            var nullable = GetPoint(targetSnapshot.TextBuffer, affinity);
            if (nullable.HasValue && nullable.Value.Snapshot != targetSnapshot)
                nullable = nullable.Value.TranslateTo(targetSnapshot, _trackingMode);
            return nullable;
        }

        public SnapshotPoint? GetPoint(Predicate<ITextBuffer> match, PositionAffinity affinity)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            var anchorBuffer = AnchorBuffer;
            var snapshotPoint = _anchorPoint.TranslateTo(anchorBuffer.CurrentSnapshot, _trackingMode);
            if (match(anchorBuffer))
                return snapshotPoint;
            if (anchorBuffer == BufferGraph.TopBuffer)
                return BufferGraph.MapDownToFirstMatch(snapshotPoint, _trackingMode,
                    snapshot => match(snapshot.TextBuffer), affinity);
            if (anchorBuffer is IProjectionBufferBase)
            {
                var firstMatch = BufferGraph.MapDownToFirstMatch(snapshotPoint, _trackingMode,
                    snapshot => match(snapshot.TextBuffer), affinity);
                if (firstMatch.HasValue)
                    return firstMatch;
            }

            if (match(BufferGraph.TopBuffer))
                return BufferGraph.MapUpToBuffer(snapshotPoint, _trackingMode, affinity, BufferGraph.TopBuffer);
            return BufferGraph.MapUpToFirstMatch(snapshotPoint, _trackingMode, snapshot => match(snapshot.TextBuffer),
                affinity);
        }
    }
}