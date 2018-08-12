using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Modules.Editor.Text.Implementation;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Modules.Editor.Text.Projection.Implementation
{
    internal class BufferGraph : IBufferGraph
    {
        internal Dictionary<ITextBuffer, FrugalList<IProjectionBufferBase>> ImportingProjectionBufferMap =
            new Dictionary<ITextBuffer, FrugalList<IProjectionBufferBase>>();

        internal List<WeakEventHookForBufferGraph> EventHooks =
            new List<WeakEventHookForBufferGraph>();

        private readonly GuardedOperations _guardedOperations;

        public BufferGraph(ITextBuffer topBuffer, GuardedOperations guardedOperations)
        {
            TopBuffer = topBuffer ?? throw new ArgumentNullException(nameof(topBuffer));
            _guardedOperations = guardedOperations ?? throw new ArgumentNullException(nameof(guardedOperations));
            ImportingProjectionBufferMap.Add(topBuffer, null);
            EventHooks.Add(new WeakEventHookForBufferGraph(this, topBuffer));
            if (!(topBuffer is IProjectionBufferBase projBuffer))
                return;
            var sourceBuffers = projBuffer.SourceBuffers;
            var addedToGraphBuffers = new FrugalList<ITextBuffer>();
            foreach (var sourceBuffer in sourceBuffers)
                AddSourceBuffer(projBuffer, sourceBuffer, addedToGraphBuffers);
        }

        public ITextBuffer TopBuffer { get; }

        public Collection<ITextBuffer> GetTextBuffers(Predicate<ITextBuffer> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            var frugalList = new FrugalList<ITextBuffer>();
            foreach (var key in ImportingProjectionBufferMap.Keys)
            {
                if (match(key))
                    frugalList.Add(key);
            }

            return new Collection<ITextBuffer>(frugalList);
        }

        public IMappingPoint CreateMappingPoint(SnapshotPoint point, PointTrackingMode trackingMode)
        {
            return new MappingPoint(point, trackingMode, this);
        }

        public IMappingSpan CreateMappingSpan(SnapshotSpan span, SpanTrackingMode trackingMode)
        {
            return new MappingSpan(span, trackingMode, this);
        }

        public SnapshotPoint? MapDownToFirstMatch(SnapshotPoint position, PointTrackingMode trackingMode,
            Predicate<ITextSnapshot> match, PositionAffinity affinity)
        {
            if (position.Snapshot == null)
                throw new ArgumentNullException(nameof(position));
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    if (match == null)
                        throw new ArgumentNullException(nameof(match));
                    switch (affinity)
                    {
                        case PositionAffinity.Predecessor:
                        case PositionAffinity.Successor:
                            if (!ImportingProjectionBufferMap.ContainsKey(position.Snapshot.TextBuffer))
                                return new SnapshotPoint?();
                            var textBuffer = position.Snapshot.TextBuffer;
                            var textSnapshot = textBuffer.CurrentSnapshot;
                            var position1 = position.TranslateTo(textSnapshot, trackingMode).Position;
                            while (!match(textSnapshot))
                            {
                                if (!(textBuffer is IProjectionBufferBase projectionBufferBase))
                                    return new SnapshotPoint?();
                                IProjectionSnapshot currentSnapshot = projectionBufferBase.CurrentSnapshot;
                                if (currentSnapshot.SourceSnapshots.Count == 0)
                                    return new SnapshotPoint?();
                                var sourceSnapshot = currentSnapshot.MapToSourceSnapshot(position1, affinity);
                                position1 = sourceSnapshot.Position;
                                textSnapshot = sourceSnapshot.Snapshot;
                                textBuffer = textSnapshot.TextBuffer;
                            }

                            return new SnapshotPoint(textSnapshot, position1);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(affinity));
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public SnapshotPoint? MapDownToInsertionPoint(SnapshotPoint position, PointTrackingMode trackingMode,
            Predicate<ITextSnapshot> match)
        {
            if (position.Snapshot == null)
                throw new ArgumentNullException(nameof(position));
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    if (match == null)
                        throw new ArgumentNullException(nameof(match));
                    var textBuffer = position.Snapshot.TextBuffer;
                    int position1 = position.TranslateTo(textBuffer.CurrentSnapshot, trackingMode);
                    var snapshot = textBuffer.CurrentSnapshot;
                    while (!match(snapshot))
                    {
                        var projectionBufferBase = textBuffer as IProjectionBufferBase;
                        if (projectionBufferBase == null)
                            return new SnapshotPoint?();
                        IProjectionSnapshot currentSnapshot = projectionBufferBase.CurrentSnapshot;
                        if (currentSnapshot.SourceSnapshots.Count == 0)
                            return new SnapshotPoint?();
                        var sourceSnapshot = currentSnapshot.MapToSourceSnapshot(position1);
                        position1 = sourceSnapshot.Position;
                        snapshot = sourceSnapshot.Snapshot;
                        textBuffer = snapshot.TextBuffer;
                    }

                    return new SnapshotPoint(snapshot, position1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public SnapshotPoint? MapDownToBuffer(SnapshotPoint position, PointTrackingMode trackingMode,
            ITextBuffer targetBuffer, PositionAffinity affinity)
        {
            if (position.Snapshot == null)
                throw new ArgumentNullException(nameof(position));
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    if (targetBuffer == null)
                        throw new ArgumentNullException(nameof(targetBuffer));
                    switch (affinity)
                    {
                        case PositionAffinity.Predecessor:
                        case PositionAffinity.Successor:
                            var textBuffer = position.Snapshot.TextBuffer;
                            var textSnapshot = textBuffer.CurrentSnapshot;
                            var position1 = position.TranslateTo(textSnapshot, trackingMode).Position;
                            for (; textBuffer != targetBuffer; textBuffer = textSnapshot.TextBuffer)
                            {
                                var projectionBufferBase = textBuffer as IProjectionBufferBase;
                                if (projectionBufferBase == null)
                                    return new SnapshotPoint?();
                                IProjectionSnapshot currentSnapshot = projectionBufferBase.CurrentSnapshot;
                                if (currentSnapshot.SourceSnapshots.Count == 0)
                                    return new SnapshotPoint?();
                                var sourceSnapshot = currentSnapshot.MapToSourceSnapshot(position1, affinity);
                                position1 = sourceSnapshot.Position;
                                textSnapshot = sourceSnapshot.Snapshot;
                            }

                            return new SnapshotPoint(textSnapshot, position1);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(affinity));
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public SnapshotPoint? MapDownToSnapshot(SnapshotPoint position, PointTrackingMode trackingMode,
            ITextSnapshot targetSnapshot, PositionAffinity affinity)
        {
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            var nullable = MapDownToBuffer(position, trackingMode, targetSnapshot.TextBuffer, affinity);
            if (nullable.HasValue && nullable.Value.Snapshot != targetSnapshot)
                nullable = nullable.Value.TranslateTo(targetSnapshot, trackingMode);
            return nullable;
        }

        public SnapshotPoint? MapUpToBuffer(SnapshotPoint point, PointTrackingMode trackingMode,
            PositionAffinity affinity, ITextBuffer targetBuffer)
        {
            return CheckedMapUpToBuffer(point, trackingMode,
                snapshot => snapshot.TextBuffer == targetBuffer, affinity);
        }

        public SnapshotPoint? MapUpToSnapshot(SnapshotPoint position, PointTrackingMode trackingMode,
            PositionAffinity affinity, ITextSnapshot targetSnapshot)
        {
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            var nullable = MapUpToBuffer(position, trackingMode, affinity, targetSnapshot.TextBuffer);
            if (nullable.HasValue && nullable.Value.Snapshot != targetSnapshot)
                nullable = nullable.Value.TranslateTo(targetSnapshot, trackingMode);
            return nullable;
        }

        public SnapshotPoint? MapUpToFirstMatch(SnapshotPoint point, PointTrackingMode trackingMode,
            Predicate<ITextSnapshot> match, PositionAffinity affinity)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            return CheckedMapUpToBuffer(point, trackingMode, match, affinity);
        }

        private SnapshotPoint? CheckedMapUpToBuffer(SnapshotPoint point, PointTrackingMode trackingMode,
            Predicate<ITextSnapshot> match, PositionAffinity affinity)
        {
            if (point.Snapshot == null)
                throw new ArgumentNullException(nameof(point));
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    switch (affinity)
                    {
                        case PositionAffinity.Predecessor:
                        case PositionAffinity.Successor:
                            if (!ImportingProjectionBufferMap.ContainsKey(point.Snapshot.TextBuffer))
                                return new SnapshotPoint?();
                            return MapUpToBufferGuts(
                                point.TranslateTo(point.Snapshot.TextBuffer.CurrentSnapshot, trackingMode), affinity,
                                match);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(affinity));
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        private SnapshotPoint? MapUpToBufferGuts(SnapshotPoint currentPoint, PositionAffinity affinity,
            Predicate<ITextSnapshot> match)
        {
            if (match(currentPoint.Snapshot))
                return currentPoint;
            var projectionBuffer =
                ImportingProjectionBufferMap[currentPoint.Snapshot.TextBuffer];
            if (projectionBuffer != null)
            {
                foreach (var projectionBufferBase in projectionBuffer)
                {
                    SnapshotPoint? nullable =
                        projectionBufferBase.CurrentSnapshot.MapFromSourceSnapshot(currentPoint, affinity);
                    if (nullable.HasValue)
                    {
                        nullable = MapUpToBufferGuts(nullable.Value, affinity, match);
                        if (nullable.HasValue)
                            return nullable;
                    }
                }
            }

            return new SnapshotPoint?();
        }

        public NormalizedSnapshotSpanCollection MapDownToFirstMatch(SnapshotSpan span, SpanTrackingMode trackingMode,
            Predicate<ITextSnapshot> match)
        {
            if (span.Snapshot == null)
                throw new ArgumentNullException(nameof(span));
            switch (trackingMode)
            {
                case SpanTrackingMode.EdgeExclusive:
                case SpanTrackingMode.EdgeInclusive:
                case SpanTrackingMode.EdgePositive:
                case SpanTrackingMode.EdgeNegative:
                    if (match == null)
                        throw new ArgumentNullException(nameof(match));
                    if (!ImportingProjectionBufferMap.ContainsKey(span.Snapshot.TextBuffer))
                        return NormalizedSnapshotSpanCollection.Empty;
                    var textBuffer = span.Snapshot.TextBuffer;
                    var span1 = span.TranslateTo(textBuffer.CurrentSnapshot, trackingMode);
                    if (match(textBuffer.CurrentSnapshot))
                        return new NormalizedSnapshotSpanCollection(span1);
                    if (!(textBuffer is IProjectionBufferBase))
                        return NormalizedSnapshotSpanCollection.Empty;
                    var targetSpans = new FrugalList<Span>();
                    var inputSpans = new FrugalList<SnapshotSpan>()
                    {
                        span1
                    };
                    ITextSnapshot chosenSnapshot = null;
                    do
                    {
                        inputSpans =
                            MapDownOneLevel(inputSpans, match, ref chosenSnapshot, ref targetSpans);
                    } while (inputSpans.Count > 0);

                    if (chosenSnapshot != null)
                        return new NormalizedSnapshotSpanCollection(chosenSnapshot, targetSpans);
                    return NormalizedSnapshotSpanCollection.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public NormalizedSnapshotSpanCollection MapDownToBuffer(SnapshotSpan span, SpanTrackingMode trackingMode,
            ITextBuffer targetBuffer)
        {
            if (targetBuffer == null)
                throw new ArgumentNullException(nameof(targetBuffer));
            if (!ImportingProjectionBufferMap.ContainsKey(targetBuffer))
                return NormalizedSnapshotSpanCollection.Empty;
            return MapDownToFirstMatch(span, trackingMode,
                snapshot => snapshot.TextBuffer == targetBuffer);
        }

        public NormalizedSnapshotSpanCollection MapDownToSnapshot(SnapshotSpan span, SpanTrackingMode trackingMode,
            ITextSnapshot targetSnapshot)
        {
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            var snapshotSpanCollection =
                MapDownToBuffer(span, trackingMode, targetSnapshot.TextBuffer);
            if (snapshotSpanCollection.Count > 0 && snapshotSpanCollection[0].Snapshot != targetSnapshot)
            {
                var frugalList = new FrugalList<SnapshotSpan>();
                foreach (var snapshotSpan in snapshotSpanCollection)
                    frugalList.Add(snapshotSpan.TranslateTo(targetSnapshot, trackingMode));
                snapshotSpanCollection = new NormalizedSnapshotSpanCollection(frugalList);
            }

            return snapshotSpanCollection;
        }

        public NormalizedSnapshotSpanCollection MapUpToSnapshot(SnapshotSpan span, SpanTrackingMode trackingMode,
            ITextSnapshot targetSnapshot)
        {
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            var snapshotSpanCollection =
                MapUpToBuffer(span, trackingMode, targetSnapshot.TextBuffer);
            if (snapshotSpanCollection.Count > 0 && snapshotSpanCollection[0].Snapshot != targetSnapshot)
            {
                var frugalList = new FrugalList<SnapshotSpan>();
                foreach (var snapshotSpan in snapshotSpanCollection)
                    frugalList.Add(snapshotSpan.TranslateTo(targetSnapshot, trackingMode));
                snapshotSpanCollection = new NormalizedSnapshotSpanCollection(frugalList);
            }

            return snapshotSpanCollection;
        }

        private static FrugalList<SnapshotSpan> MapDownOneLevel(FrugalList<SnapshotSpan> inputSpans,
            Predicate<ITextSnapshot> match, ref ITextSnapshot chosenSnapshot, ref FrugalList<Span> targetSpans)
        {
            var frugalList = new FrugalList<SnapshotSpan>();
            foreach (var inputSpan in inputSpans)
            {
                IProjectionSnapshot currentSnapshot =
                    ((IProjectionBufferBase) inputSpan.Snapshot.TextBuffer).CurrentSnapshot;
                if (currentSnapshot.SourceSnapshots.Count > 0)
                {
                    IList<SnapshotSpan> sourceSnapshots =
                        currentSnapshot.MapToSourceSnapshots(inputSpan);
                    foreach (var snapshotSpan in sourceSnapshots)
                    {
                        var textBuffer = snapshotSpan.Snapshot.TextBuffer;
                        if (textBuffer.CurrentSnapshot == chosenSnapshot)
                            targetSpans.Add(snapshotSpan.Span);
                        else if (chosenSnapshot == null && match(textBuffer.CurrentSnapshot))
                        {
                            chosenSnapshot = textBuffer.CurrentSnapshot;
                            targetSpans.Add(snapshotSpan.Span);
                        }
                        else if (textBuffer is IProjectionBufferBase)
                            frugalList.Add(snapshotSpan);
                    }
                }
            }

            return frugalList;
        }

        public NormalizedSnapshotSpanCollection MapUpToBuffer(SnapshotSpan span, SpanTrackingMode trackingMode,
            ITextBuffer targetBuffer)
        {
            if (!ImportingProjectionBufferMap.ContainsKey(targetBuffer))
                return NormalizedSnapshotSpanCollection.Empty;
            return CheckedMapUpToBuffer(span, trackingMode,
                snapshot => snapshot.TextBuffer == targetBuffer);
        }

        public NormalizedSnapshotSpanCollection MapUpToFirstMatch(SnapshotSpan span, SpanTrackingMode trackingMode,
            Predicate<ITextSnapshot> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            return CheckedMapUpToBuffer(span, trackingMode, match);
        }

        public NormalizedSnapshotSpanCollection CheckedMapUpToBuffer(SnapshotSpan span, SpanTrackingMode trackingMode,
            Predicate<ITextSnapshot> match)
        {
            if (span.Snapshot == null)
                throw new ArgumentNullException(nameof(span));
            switch (trackingMode)
            {
                case SpanTrackingMode.EdgeExclusive:
                case SpanTrackingMode.EdgeInclusive:
                case SpanTrackingMode.EdgePositive:
                case SpanTrackingMode.EdgeNegative:
                    var textBuffer = span.Snapshot.TextBuffer;
                    if (!ImportingProjectionBufferMap.ContainsKey(textBuffer))
                        return NormalizedSnapshotSpanCollection.Empty;
                    var span1 = span.TranslateTo(textBuffer.CurrentSnapshot, trackingMode);
                    if (match(textBuffer.CurrentSnapshot))
                        return new NormalizedSnapshotSpanCollection(span1);
                    ITextSnapshot chosenSnapshot = null;
                    var topSpans = new FrugalList<Span>();
                    var spans = new FrugalList<SnapshotSpan>()
                    {
                        span1
                    };
                    do
                    {
                        spans = MapUpOneLevel(spans, ref chosenSnapshot, match, topSpans);
                    } while (spans.Count > 0);

                    if (chosenSnapshot == null)
                        return NormalizedSnapshotSpanCollection.Empty;
                    return new NormalizedSnapshotSpanCollection(chosenSnapshot, topSpans);
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        private FrugalList<SnapshotSpan> MapUpOneLevel(FrugalList<SnapshotSpan> spans, ref ITextSnapshot chosenSnapshot,
            Predicate<ITextSnapshot> match, FrugalList<Span> topSpans)
        {
            var frugalList1 = new FrugalList<SnapshotSpan>();
            foreach (var span1 in spans)
            {
                if (ImportingProjectionBufferMap.TryGetValue(span1.Snapshot.TextBuffer, out var frugalList2) &&
                    frugalList2 != null)
                {
                    foreach (var projectionBufferBase in frugalList2)
                    {
                        var list =
                            (IList<Span>) projectionBufferBase.CurrentSnapshot.MapFromSourceSnapshot(span1);
                        if (projectionBufferBase.CurrentSnapshot == chosenSnapshot)
                            topSpans.AddRange(list);
                        else if (chosenSnapshot == null && match(projectionBufferBase.CurrentSnapshot))
                        {
                            chosenSnapshot = projectionBufferBase.CurrentSnapshot;
                            topSpans.AddRange(list);
                        }
                        else
                        {
                            foreach (var span2 in list)
                                frugalList1.Add(new SnapshotSpan(projectionBufferBase.CurrentSnapshot,
                                    span2));
                        }
                    }
                }
            }

            return frugalList1;
        }

        public void RaiseGraphBuffersChanged(GraphBuffersChangedEventArgs args)
        {
            var graphBuffersChanged = GraphBuffersChanged;
            if (graphBuffersChanged == null)
                return;
            _guardedOperations.RaiseEvent(this, graphBuffersChanged, args);
        }

        private void SourceBuffersChanged(object sender, ProjectionSourceBuffersChangedEventArgs e)
        {
            var projBuffer = (IProjectionBufferBase) sender;
            var addedToGraphBuffers = new FrugalList<ITextBuffer>();
            var removedFromGraphBuffers = new FrugalList<ITextBuffer>();
            foreach (var addedBuffer in e.AddedBuffers)
                AddSourceBuffer(projBuffer, addedBuffer, addedToGraphBuffers);
            foreach (var removedBuffer in e.RemovedBuffers)
                RemoveSourceBuffer(projBuffer, removedBuffer, removedFromGraphBuffers);
            if (addedToGraphBuffers.Count <= 0 && removedFromGraphBuffers.Count <= 0 ||
                GraphBuffersChanged == null)
                return;
            ((BaseBuffer) projBuffer).Group.EnqueueEvents(
                new GraphEventRaiser(this,
                    new GraphBuffersChangedEventArgs(addedToGraphBuffers,
                        removedFromGraphBuffers)),  null);
        }

        private void AddSourceBuffer(IProjectionBufferBase projBuffer, ITextBuffer sourceBuffer,
            ICollection<ITextBuffer> addedToGraphBuffers)
        {
            var flag = false;
            if (!ImportingProjectionBufferMap.TryGetValue(sourceBuffer, out var frugalList))
            {
                addedToGraphBuffers.Add(sourceBuffer);
                flag = true;
                frugalList = new FrugalList<IProjectionBufferBase>();
                ImportingProjectionBufferMap.Add(sourceBuffer, frugalList);
                EventHooks.Add(new WeakEventHookForBufferGraph(this, sourceBuffer));
            }

            frugalList.Add(projBuffer);
            if (!flag)
                return;
            if (!(sourceBuffer is IProjectionBufferBase projBuffer1))
                return;
            foreach (var sourceBuffer1 in projBuffer1.SourceBuffers)
                AddSourceBuffer(projBuffer1, sourceBuffer1, addedToGraphBuffers);
        }

        private void RemoveSourceBuffer(IProjectionBufferBase projBuffer, ITextBuffer sourceBuffer,
            FrugalList<ITextBuffer> removedFromGraphBuffers)
        {
            var projectionBuffer = ImportingProjectionBufferMap[sourceBuffer];
            projectionBuffer.Remove(projBuffer);
            if (projectionBuffer.Count != 0)
                return;
            removedFromGraphBuffers.Add(sourceBuffer);
            ImportingProjectionBufferMap.Remove(sourceBuffer);
            for (var index = 0; index < EventHooks.Count; ++index)
            {
                if (EventHooks[index].SourceBuffer == sourceBuffer)
                {
                    EventHooks[index].UnsubscribeFromSourceBuffer();
                    EventHooks.RemoveAt(index);
                    break;
                }
            }

            var projBuffer1 = sourceBuffer as IProjectionBufferBase;
            if (projBuffer1 == null)
                return;
            foreach (var sourceBuffer1 in projBuffer1.SourceBuffers)
                RemoveSourceBuffer(projBuffer1, sourceBuffer1, removedFromGraphBuffers);
        }

        protected void ContentTypeChanged(object sender, ContentTypeChangedEventArgs args)
        {
            // ISSUE: reference to a compiler-generated field
            var
                contentTypeChanged = GraphBufferContentTypeChanged;
            contentTypeChanged?.Invoke(this,
                new GraphBufferContentTypeChangedEventArgs((ITextBuffer) sender, args.BeforeContentType,
                    args.AfterContentType));
        }

        public event EventHandler<GraphBuffersChangedEventArgs> GraphBuffersChanged;

        public event EventHandler<GraphBufferContentTypeChangedEventArgs> GraphBufferContentTypeChanged;

        private class GraphEventRaiser : BaseBuffer.ITextEventRaiser
        {
            private readonly BufferGraph _graph;
            private readonly GraphBuffersChangedEventArgs _args;

            public GraphEventRaiser(BufferGraph graph, GraphBuffersChangedEventArgs args)
            {
                _graph = graph;
                _args = args;
            }

            public void RaiseEvent(BaseBuffer baseBuffer, bool immediate)
            {
                _graph.RaiseGraphBuffersChanged(_args);
            }

            public bool HasPostEvent => false;
        }

        internal class WeakEventHookForBufferGraph
        {
            private readonly WeakReference<BufferGraph> _targetGraph;

            public WeakEventHookForBufferGraph(BufferGraph targetGraph, ITextBuffer sourceBuffer)
            {
                _targetGraph = new WeakReference<BufferGraph>(targetGraph);
                SourceBuffer = sourceBuffer;
                sourceBuffer.ContentTypeChanged +=
                    OnSourceBufferContentTypeChanged;
                var projectionBuffer = sourceBuffer as ProjectionBuffer;
                if (projectionBuffer == null)
                    return;
                projectionBuffer.SourceBuffersChangedImmediate +=
                    OnSourceBuffersChanged;
            }

            public ITextBuffer SourceBuffer { get; private set; }

            public BufferGraph GetTargetGraph()
            {
                if (_targetGraph.TryGetTarget(out var target))
                    return target;
                UnsubscribeFromSourceBuffer();
                return null;
            }

            private void OnSourceBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
            {
                GetTargetGraph()?.ContentTypeChanged(sender, e);
            }

            private void OnSourceBuffersChanged(object sender, ProjectionSourceBuffersChangedEventArgs e)
            {
                GetTargetGraph()?.SourceBuffersChanged(sender, e);
            }

            public void UnsubscribeFromSourceBuffer()
            {
                if (SourceBuffer == null)
                    return;
                SourceBuffer.ContentTypeChanged -=
                    OnSourceBufferContentTypeChanged;
                if (SourceBuffer is ProjectionBuffer sourceBuffer)
                    sourceBuffer.SourceBuffersChangedImmediate -=
                        OnSourceBuffersChanged;
                SourceBuffer = null;
            }
        }
    }
}