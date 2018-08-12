using System;
using System.Collections.Generic;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Modules.Editor.Projection
{
    internal class ElisionMap
    {
        private readonly ElisionMapNode _root;

        public int Length => _root.TotalExposedSize;

        public int LineCount => _root.TotalExposedLineBreakCount + 1;

        public int SpanCount { get; }

        public ElisionMap(ITextSnapshot sourceSnapshot, NormalizedSpanCollection exposedSpans)
        {
            SpanCount = exposedSpans.Count;
            if (exposedSpans.Count == 0)
            {
                _root = new ElisionMapNode(0, sourceSnapshot.Length, 0, sourceSnapshot.LineCount - 1, true);
            }
            else
            {
                var lineNumbers = new int[exposedSpans.Count * 2 + 1];
                for (var index = 0; index < exposedSpans.Count; ++index)
                {
                    lineNumbers[index * 2] = sourceSnapshot.GetLineNumberFromPosition(exposedSpans[index].Start);
                    lineNumbers[index * 2 + 1] = sourceSnapshot.GetLineNumberFromPosition(exposedSpans[index].End);
                }

                lineNumbers[exposedSpans.Count * 2] = sourceSnapshot.LineCount - 1;
                _root = Build(new SnapshotSpan(sourceSnapshot, 0, sourceSnapshot.Length), exposedSpans, lineNumbers,
                    new Span(0, exposedSpans.Count));
            }

            if (!BufferGroup.Tracing)
                return;
            _root.Dump(0);
        }

        private ElisionMap(ElisionMapNode root, int spanCount)
        {
            _root = root;
            SpanCount = spanCount;
        }

        public ElisionMap EditSpans(ITextSnapshot sourceSnapshot, NormalizedSpanCollection spansToElide,
            NormalizedSpanCollection spansToExpand, out FrugalList<TextChange> textChanges)
        {
            textChanges = new FrugalList<TextChange>();
            NormalizedSpanCollection left =
                new NormalizedSnapshotSpanCollection(GetSourceSpans(sourceSnapshot, 0, SpanCount));
            var normalizedSpanCollection1 = NormalizedSpanCollection.Difference(left, spansToElide);
            foreach (var span in NormalizedSpanCollection.Difference(left, normalizedSpanCollection1))
                textChanges.Add(TextChange.Create(_root.MapFromSourceSnapshotToNearest(span.Start, 0),
                    BufferFactoryService.StringRebuilderFromSnapshotAndSpan(sourceSnapshot, span),
                    StringRebuilder.Empty, sourceSnapshot));
            var normalizedSpanCollection2 = NormalizedSpanCollection.Union(normalizedSpanCollection1, spansToExpand);
            foreach (var span in NormalizedSpanCollection.Difference(normalizedSpanCollection2,
                normalizedSpanCollection1))
                textChanges.Add(TextChange.Create(_root.MapFromSourceSnapshotToNearest(span.Start, 0),
                    StringRebuilder.Empty,
                    BufferFactoryService.StringRebuilderFromSnapshotAndSpan(sourceSnapshot, span), sourceSnapshot));
            if (textChanges.Count <= 0)
                return this;
            return new ElisionMap(sourceSnapshot, normalizedSpanCollection2);
        }

        public int GetLineNumberFromPosition(int position, ITextSnapshot sourceSnapshot)
        {
            return _root.GetLineNumberFromPosition(sourceSnapshot, position, 0, 0);
        }

        public IList<SnapshotSpan> GetSourceSpans(ITextSnapshot sourceSnapshot, int startSpanIndex, int count)
        {
            var frugalList = new FrugalList<SnapshotSpan>();
            var rank = -1;
            var sourcePrefixSize = 0;
            _root.GetSourceSpans(sourceSnapshot, ref rank, ref sourcePrefixSize, startSpanIndex, startSpanIndex + count,
                frugalList);
            return frugalList;
        }

        public ElisionMap IncorporateChanges(INormalizedTextChangeCollection sourceChanges,
            FrugalList<TextChange> projectedChanges, ITextSnapshot beforeSourceSnapshot, ITextSnapshot sourceSnapshot,
            ITextSnapshot beforeElisionSnapshot)
        {
            var root = _root;
            var incomingAccumulatedDelta = 0;
            foreach (var sourceChange in sourceChanges)
            {
                var accumulatedDelete = 0;
                var outgoingAccumulatedDelta = 0;
                var newText = TextChange.NewStringRebuilder(sourceChange);
                root = root.IncorporateChange(beforeSourceSnapshot, sourceSnapshot, beforeElisionSnapshot,
                    sourceChange.NewLength > 0 ? sourceChange.NewPosition : new int?(), newText,
                    new Span(sourceChange.NewPosition, sourceChange.OldLength), sourceChange.OldPosition,
                    sourceChange.NewPosition, 0, projectedChanges, incomingAccumulatedDelta,
                    ref outgoingAccumulatedDelta, ref accumulatedDelete);
                incomingAccumulatedDelta += outgoingAccumulatedDelta;
            }

            if (root.TotalSourceSize != sourceSnapshot.Length)
                throw new InvalidOperationException();
            if (root.TotalSourceLineBreakCount + 1 != sourceSnapshot.LineCount)
                throw new InvalidOperationException();
            return new ElisionMap(root, SpanCount);
        }

        public SnapshotPoint? MapFromSourceSnapshot(ITextSnapshot snapshot, int position)
        {
            return _root.MapFromSourceSnapshot(snapshot, position, 0);
        }

        public void MapFromSourceSnapshot(Span span, FrugalList<Span> result)
        {
            if (span.Length == 0)
                _root.MapNullSpanFromSourceSnapshot(span, 0, result);
            else
                _root.MapFromSourceSnapshot(span, 0, result);
        }

        public SnapshotPoint MapFromSourceSnapshotToNearest(ITextSnapshot snapshot, int position)
        {
            return new SnapshotPoint(snapshot, _root.MapFromSourceSnapshotToNearest(position, 0));
        }

        public FrugalList<SnapshotPoint> MapInsertionPointToSourceSnapshots(IElisionSnapshot elisionSnapshot,
            int exposedPosition)
        {
            var points = new FrugalList<SnapshotPoint>();
            _root.MapInsertionPointToSourceSnapshots(elisionSnapshot, exposedPosition, 0, points);
            return points;
        }

        public SnapshotPoint MapToSourceSnapshot(ITextSnapshot sourceSnapshot, int position, PositionAffinity affinity)
        {
            return _root.MapToSourceSnapshot(sourceSnapshot, position, 0, affinity);
        }

        public void MapToSourceSnapshots(IElisionSnapshot elisionSnapshot, Span span, FrugalList<SnapshotSpan> result)
        {
            if (span.Length == 0)
                MapNullSpansToSourceSnapshots(elisionSnapshot, span, result);
            else
                _root.MapToSourceSnapshots(elisionSnapshot.SourceSnapshot, span, 0, result);
        }

        public void MapToSourceSnapshotsInFillInMode(ITextSnapshot sourceSnapshot, Span span,
            FrugalList<SnapshotSpan> result)
        {
            SnapshotPoint? nullable1;
            SnapshotPoint? nullable2;
            if (span.Length == 0)
            {
                nullable1 = span.Start == 0
                    ? new SnapshotPoint(sourceSnapshot, 0)
                    : _root.MapToSourceSnapshot(sourceSnapshot, span.Start, 0, PositionAffinity.Predecessor);
                nullable2 = span.End == Length
                    ? new SnapshotPoint(sourceSnapshot, sourceSnapshot.Length)
                    : _root.MapToSourceSnapshot(sourceSnapshot, span.End, 0, PositionAffinity.Successor);
            }
            else
            {
                nullable1 = _root.MapToSourceSnapshot(sourceSnapshot, span.Start, 0, PositionAffinity.Successor);
                nullable2 = _root.MapToSourceSnapshot(sourceSnapshot, span.End, 0, PositionAffinity.Predecessor);
            }

            result.Add(new SnapshotSpan(sourceSnapshot, Span.FromBounds(nullable1.Value, nullable2.Value)));
        }

        private ElisionMapNode Build(SnapshotSpan sourceSpan, NormalizedSpanCollection exposedSpans, int[] lineNumbers,
            Span slice)
        {
            var end = slice.Start + slice.Length / 2;
            var exposedSpan = exposedSpans[end];
            var slice1 = Span.FromBounds(slice.Start, end);
            Span span1;
            ElisionMapNode left;
            if (slice1.Length > 0)
            {
                span1 = Span.FromBounds(sourceSpan.Start, exposedSpan.Start);
                left = Build(new SnapshotSpan(sourceSpan.Snapshot, span1), exposedSpans, lineNumbers, slice1);
            }
            else if (slice.Start == 0 && exposedSpan.Start != 0)
            {
                span1 = Span.FromBounds(0, exposedSpan.Start);
                left = new ElisionMapNode(0, span1.Length, 0,
                    TextUtilities.ScanForLineCount(sourceSpan.Snapshot.GetText(span1)), true);
            }
            else
            {
                span1 = new Span(exposedSpan.Start, 0);
                left = null;
            }

            var slice2 = Span.FromBounds(end + 1, slice.End);
            Span span2;
            ElisionMapNode right;
            if (slice2.Length > 0)
            {
                span2 = Span.FromBounds(exposedSpans[end + 1].Start, sourceSpan.End);
                right = Build(new SnapshotSpan(sourceSpan.Snapshot, span2), exposedSpans, lineNumbers, slice2);
            }
            else
            {
                span2 = new Span(sourceSpan.End, 0);
                right = null;
            }

            Span.FromBounds(exposedSpan.End, span2.Start);
            var lineNumber1 = lineNumbers[2 * end];
            var lineNumber2 = lineNumbers[2 * end + 1];
            var lineNumber3 = lineNumbers[2 * end + 2];
            var exposedLineBreakCount = lineNumber2 - lineNumber1;
            var num1 = lineNumber2;
            var num2 = lineNumber3 - num1;
            return new ElisionMapNode(exposedSpan.Length, sourceSpan.Length - (span1.Length + span2.Length),
                exposedLineBreakCount, exposedLineBreakCount + num2, left, right, false);
        }

        private void MapNullSpansToSourceSnapshots(IElisionSnapshot elisionSnapshot, Span span,
            FrugalList<SnapshotSpan> result)
        {
            var sourceSnapshots = MapInsertionPointToSourceSnapshots(elisionSnapshot, span.Start);
            foreach (var snapshotPoint in sourceSnapshots)
            {
                var snapshotSpan = new SnapshotSpan(snapshotPoint.Snapshot, snapshotPoint.Position, 0);
                if (result.Count == 0 || snapshotSpan != result[result.Count - 1])
                    result.Add(snapshotSpan);
            }
        }
    }
}