using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Modules.Editor.Projection
{
    internal class ProjectionSnapshot : BaseProjectionSnapshot
    {
        private readonly ProjectionBuffer _projectionBuffer;
        private readonly ReadOnlyCollection<SnapshotSpan> _sourceSpans;
        private readonly Dictionary<ITextSnapshot, List<InvertedSource>> _sourceSnapshotMap;
        private readonly int[] _cumulativeLineBreakCounts;
        private readonly int[] _cumulativeLengths;

        public ProjectionSnapshot(ProjectionBuffer projectionBuffer, ITextVersion version, StringRebuilder content, IList<SnapshotSpan> sourceSpans)
            : base(version, content)
        {
            _projectionBuffer = projectionBuffer;
            _sourceSpans = new ReadOnlyCollection<SnapshotSpan>(sourceSpans);
            _cumulativeLengths = new int[sourceSpans.Count + 1];
            _cumulativeLineBreakCounts = new int[sourceSpans.Count + 1];
            _sourceSnapshotMap = new Dictionary<ITextSnapshot, List<InvertedSource>>();
            for (var index = 0; index < sourceSpans.Count; ++index)
            {
                var sourceSpan = sourceSpans[index];
                TotalLength += sourceSpan.Length;
                _cumulativeLengths[index + 1] = _cumulativeLengths[index] + sourceSpan.Length;
                var num = sourceSpan.Snapshot.GetLineNumberFromPosition(sourceSpan.End) - sourceSpan.Snapshot.GetLineNumberFromPosition(sourceSpan.Start);
                TotalLineCount += num;
                _cumulativeLineBreakCounts[index + 1] = _cumulativeLineBreakCounts[index] + num;
                var snapshot = sourceSpan.Snapshot;
                if (!_sourceSnapshotMap.TryGetValue(snapshot, out var invertedSourceList))
                {
                    invertedSourceList = new List<InvertedSource>();
                    _sourceSnapshotMap.Add(snapshot, invertedSourceList);
                }
                invertedSourceList.Add(new InvertedSource(sourceSpan.Span, _cumulativeLengths[index]));
            }
            SourceSnapshots = new ReadOnlyCollection<ITextSnapshot>(new List<ITextSnapshot>(_sourceSnapshotMap.Keys));
            foreach (var invertedSourceList in _sourceSnapshotMap.Values)
                invertedSourceList.Sort((left, right) =>
                {
                    var sourceSpan1 = left.SourceSpan;
                    var start1 = sourceSpan1.Start;
                    sourceSpan1 = right.SourceSpan;
                    var start2 = sourceSpan1.Start;
                    if (start1 != start2)
                    {
                        var sourceSpan2 = left.SourceSpan;
                        var start3 = sourceSpan2.Start;
                        sourceSpan2 = right.SourceSpan;
                        var start4 = sourceSpan2.Start;
                        return start3 - start4;
                    }
                    var sourceSpan3 = left.SourceSpan;
                    var end1 = sourceSpan3.End;
                    sourceSpan3 = right.SourceSpan;
                    var end2 = sourceSpan3.End;
                    return end1 - end2;
                });
            if (TotalLength != version.Length)
                throw new InvalidOperationException();
            OverlapCheck();
        }

        private void OverlapCheck()
        {
            var groundSourceSpansMap = new Dictionary<ITextSnapshot, List<Span>>();
            MapDownToGround(_sourceSpans, groundSourceSpansMap);
            foreach (var keyValuePair in groundSourceSpansMap)
            {
                var num1 = 0;
                foreach (var span in keyValuePair.Value)
                    num1 += span.Length;
                var normalizedSpanCollection = new NormalizedSpanCollection(keyValuePair.Value);
                var num2 = 0;
                foreach (var span in normalizedSpanCollection)
                    num2 += span.Length;
                if (num1 != num2)
                    throw new InvalidOperationException();
            }
        }

        private static void MapDownToGround(IList<SnapshotSpan> spans, Dictionary<ITextSnapshot, List<Span>> groundSourceSpansMap)
        {
            foreach (var span in spans)
            {
                if (!(span.Snapshot is IProjectionSnapshot snapshot))
                {
                    if (!groundSourceSpansMap.TryGetValue(span.Snapshot, out var spanList))
                    {
                        spanList = new List<Span>();
                        groundSourceSpansMap.Add(span.Snapshot, spanList);
                    }
                    spanList.Add(span);
                }
                else
                    MapDownToGround(snapshot.MapToSourceSnapshots(span), groundSourceSpansMap);
            }
        }

        public override IProjectionBufferBase TextBuffer => _projectionBuffer;

        protected override ITextBuffer TextBufferHelper => _projectionBuffer;

        public override int SpanCount => _sourceSpans.Count;

        public override ReadOnlyCollection<ITextSnapshot> SourceSnapshots { get; }

        public override ITextSnapshot GetMatchingSnapshot(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            foreach (var key in _sourceSnapshotMap.Keys)
            {
                if (key.TextBuffer == textBuffer)
                    return key;
            }
            return null;
        }

        public override ITextSnapshot GetMatchingSnapshotInClosure(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            foreach (var key in _sourceSnapshotMap.Keys)
            {
                if (key.TextBuffer == textBuffer)
                    return key;
                if (key is IProjectionSnapshot projectionSnapshot2)
                {
                    var snapshotInClosure = projectionSnapshot2.GetMatchingSnapshotInClosure(textBuffer);
                    if (snapshotInClosure != null)
                        return snapshotInClosure;
                }
            }
            return null;
        }

        public override ITextSnapshot GetMatchingSnapshotInClosure(Predicate<ITextBuffer> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            foreach (var key in _sourceSnapshotMap.Keys)
            {
                if (match(key.TextBuffer))
                    return key;
                if (key is IProjectionSnapshot projectionSnapshot2)
                {
                    var snapshotInClosure = projectionSnapshot2.GetMatchingSnapshotInClosure(match);
                    if (snapshotInClosure != null)
                        return snapshotInClosure;
                }
            }
            return null;
        }

        public override ReadOnlyCollection<SnapshotSpan> GetSourceSpans()
        {
            return _sourceSpans;
        }

        public override ReadOnlyCollection<SnapshotSpan> GetSourceSpans(int startSpanIndex, int count)
        {
            if (startSpanIndex < 0 || startSpanIndex > SpanCount)
                throw new ArgumentOutOfRangeException(nameof(startSpanIndex));
            if (count < 0 || startSpanIndex + count > SpanCount)
                throw new ArgumentOutOfRangeException(nameof(count));
            var snapshotSpanList = new List<SnapshotSpan>(count);
            for (var index = 0; index < count; ++index)
                snapshotSpanList.Add(_sourceSpans[startSpanIndex + index]);
            return new ReadOnlyCollection<SnapshotSpan>(snapshotSpanList);
        }

        internal SnapshotSpan GetSourceSpan(int position)
        {
            return _sourceSpans[position];
        }

        public override ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshotsForRead(Span span)
        {
            if (span.End > Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            var frugalList = new FrugalList<SnapshotSpan>();
            if (span.Length == 0)
            {
                if (span.Start == 0 && _sourceSpans.Count == 0)
                    return new ReadOnlyCollection<SnapshotSpan>(frugalList);
                var sourceSnapshots = MapInsertionPointToSourceSnapshots(span.Start, null);
                foreach (var snapshotPoint in sourceSnapshots)
                {
                    var snapshotSpan = new SnapshotSpan(snapshotPoint.Snapshot, snapshotPoint.Position, 0);
                    if (frugalList.Count == 0 || snapshotSpan != frugalList[frugalList.Count - 1])
                        frugalList.Add(snapshotSpan);
                }
            }
            else
            {
                var spanIndexOfPosition = FindHighestSpanIndexOfPosition(span.Start);
                var sourceSpan1 = _sourceSpans[spanIndexOfPosition];
                var start = sourceSpan1.Start + (span.Start - _cumulativeLengths[spanIndexOfPosition]);
                var length = start.Position + span.Length < (int)sourceSpan1.End ? span.Length : sourceSpan1.End.Position - start;
                frugalList.Add(new SnapshotSpan(start, length));
                while (length < span.Length)
                {
                    var sourceSpan2 = _sourceSpans[++spanIndexOfPosition];
                    if (span.End >= _cumulativeLengths[spanIndexOfPosition + 1])
                    {
                        length += sourceSpan2.Length;
                        frugalList.Add(sourceSpan2);
                    }
                    else
                    {
                        length += span.End - _cumulativeLengths[spanIndexOfPosition];
                        frugalList.Add(new SnapshotSpan(sourceSpan2.Snapshot, new Span(sourceSpan2.Start, span.End - _cumulativeLengths[spanIndexOfPosition])));
                    }
                }
            }
            return new ReadOnlyCollection<SnapshotSpan>(frugalList);
        }

        public override ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshots(Span span)
        {
            return MapToSourceSnapshotsForRead(span);
        }

        internal override ReadOnlyCollection<SnapshotSpan> MapReplacementSpanToSourceSnapshots(Span replacementSpan, ITextBuffer excludedBuffer)
        {
            var frugalList = new FrugalList<SnapshotSpan>();
            var spanIndexOfPosition1 = FindLowestSpanIndexOfPosition(replacementSpan.Start);
            var spanIndexOfPosition2 = FindHighestSpanIndexOfPosition(replacementSpan.End);
            var sourceSpan1 = _sourceSpans[spanIndexOfPosition1];
            var start = sourceSpan1.Start + (replacementSpan.Start - _cumulativeLengths[spanIndexOfPosition1]);
            var length = start.Position + replacementSpan.Length < (int)sourceSpan1.End ? replacementSpan.Length : sourceSpan1.End.Position - start;
            var snapshotSpan1 = new SnapshotSpan(start, length);
            if (snapshotSpan1.Length > 0 || snapshotSpan1.Snapshot.TextBuffer != excludedBuffer)
                frugalList.Add(new SnapshotSpan(start, length));
            while (spanIndexOfPosition1 < spanIndexOfPosition2)
            {
                ++spanIndexOfPosition1;
                var sourceSpan2 = _sourceSpans[spanIndexOfPosition1];
                var snapshotSpan2 = replacementSpan.End >= _cumulativeLengths[spanIndexOfPosition1 + 1] ? sourceSpan2 : new SnapshotSpan(sourceSpan2.Snapshot, new Span(sourceSpan2.Start, replacementSpan.End - _cumulativeLengths[spanIndexOfPosition1]));
                if (snapshotSpan2.Length > 0 || snapshotSpan2.Snapshot.TextBuffer != excludedBuffer)
                    frugalList.Add(snapshotSpan2);
            }
            return new ReadOnlyCollection<SnapshotSpan>(frugalList);
        }

        public override ReadOnlyCollection<Span> MapFromSourceSnapshot(SnapshotSpan sourceSpan)
        {
            if (!_sourceSnapshotMap.TryGetValue(sourceSpan.Snapshot, out var invertedSourceList))
                throw new ArgumentException("The span does not belong to a source snapshot of the projection snapshot");
            var span1 = sourceSpan.Span;
            var num1 = 0;
            var num2 = invertedSourceList.Count - 1;
            var index1 = 0;
            while (num1 <= num2)
            {
                index1 = (num1 + num2) / 2;
                if (span1.Start < invertedSourceList[index1].SourceSpan.Start)
                    num2 = index1 - 1;
                else if (span1.Start > invertedSourceList[index1].SourceSpan.End)
                    num1 = index1 + 1;
                else
                    break;
            }
            var frugalList1 = new FrugalList<Span>();
            var start1 = span1.Start;
            var sourceSpan1 = invertedSourceList[index1].SourceSpan;
            var end = sourceSpan1.End;
            if (start1 > end)
                ++index1;
            for (var index2 = index1; index2 < invertedSourceList.Count; ++index2)
            {
                var nullable = span1.Intersection(invertedSourceList[index2].SourceSpan);
                if (nullable.HasValue)
                {
                    sourceSpan1 = nullable.Value;
                    if (sourceSpan1.Length > 0 || span1.Length == 0)
                    {
                        var frugalList2 = frugalList1;
                        var projectedPosition = invertedSourceList[index2].ProjectedPosition;
                        sourceSpan1 = nullable.Value;
                        var start2 = sourceSpan1.Start;
                        sourceSpan1 = invertedSourceList[index2].SourceSpan;
                        var start3 = sourceSpan1.Start;
                        var num3 = start2 - start3;
                        var start4 = projectedPosition + num3;
                        sourceSpan1 = nullable.Value;
                        var length = sourceSpan1.Length;
                        var span2 = new Span(start4, length);
                        frugalList2.Add(span2);
                    }
                }
                else
                    break;
            }
            return new ReadOnlyCollection<Span>(frugalList1);
        }

        public override SnapshotPoint MapToSourceSnapshot(int position)
        {
            if (position < 0 || position > TotalLength)
                throw new ArgumentOutOfRangeException(nameof(position));
            var sourceSnapshots = MapInsertionPointToSourceSnapshots(position, _projectionBuffer.LiteralBuffer);
            if (sourceSnapshots.Count == 1)
                return sourceSnapshots[0];
            if (_projectionBuffer.Resolver == null)
                return sourceSnapshots[sourceSnapshots.Count - 1];
            return sourceSnapshots[_projectionBuffer.Resolver.GetTypicalInsertionPosition(new SnapshotPoint(this, position), sourceSnapshots)];
        }

        public override SnapshotPoint MapToSourceSnapshot(int position, PositionAffinity affinity)
        {
            if (position < 0 || position > Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            if (affinity < PositionAffinity.Predecessor || affinity > PositionAffinity.Successor)
                throw new ArgumentOutOfRangeException(nameof(affinity));
            var index = affinity == PositionAffinity.Predecessor ? FindLowestSpanIndexOfPosition(position) : FindHighestSpanIndexOfPosition(position);
            if (index < 0)
                throw new InvalidOperationException();
            return _sourceSpans[index].Start + (position - _cumulativeLengths[index]);
        }

        public override SnapshotPoint? MapFromSourceSnapshot(SnapshotPoint sourcePoint, PositionAffinity affinity)
        {
            switch (affinity)
            {
                case PositionAffinity.Predecessor:
                case PositionAffinity.Successor:
                    if (!_sourceSnapshotMap.TryGetValue(sourcePoint.Snapshot, out var invertedSourceList))
                        throw new ArgumentException("The point does not belong to a source snapshot of the projection snapshot");
                    var position = sourcePoint.Position;
                    var nullable = new SnapshotPoint?();
                    var num1 = 0;
                    var num2 = invertedSourceList.Count - 1;
                    while (num1 <= num2)
                    {
                        var index = (num1 + num2) / 2;
                        var sourceSpan = invertedSourceList[index].SourceSpan;
                        if (position < sourceSpan.Start)
                            num2 = index - 1;
                        else if (position > sourceSpan.End)
                        {
                            num1 = index + 1;
                        }
                        else
                        {
                            nullable = new SnapshotPoint(this, invertedSourceList[index].ProjectedPosition + sourcePoint.Position - sourceSpan.Start);
                            if (position > sourceSpan.Start && position < sourceSpan.End || position == sourceSpan.Start && affinity == PositionAffinity.Successor || position == sourceSpan.End && affinity == PositionAffinity.Predecessor)
                                return nullable;
                            if (position == sourceSpan.Start)
                                num2 = index - 1;
                            else if (position == sourceSpan.End)
                                num1 = index + 1;
                            else
                                break;
                        }
                    }
                    return nullable;
                default:
                    throw new ArgumentOutOfRangeException(nameof(affinity));
            }
        }

        internal override ReadOnlyCollection<SnapshotPoint> MapInsertionPointToSourceSnapshots(int position, ITextBuffer excludedBuffer)
        {
            if (position < 0 || position > Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            var spanIndexOfPosition = FindLowestSpanIndexOfPosition(position);
            var sourceSpan = _sourceSpans[spanIndexOfPosition];
            if (position < _cumulativeLengths[spanIndexOfPosition + 1])
                return new ReadOnlyCollection<SnapshotPoint>(new FrugalList<SnapshotPoint>()
                {
                    sourceSpan.Start + (position - _cumulativeLengths[spanIndexOfPosition])
                });
            var frugalList = new FrugalList<SnapshotPoint>();
            var snapshotPoint = new SnapshotPoint(sourceSpan.Snapshot, sourceSpan.End);
            if (sourceSpan.Snapshot.TextBuffer != excludedBuffer)
                frugalList.Add(snapshotPoint);
            while (++spanIndexOfPosition < _sourceSpans.Count && _cumulativeLengths[spanIndexOfPosition] == _cumulativeLengths[spanIndexOfPosition + 1])
            {
                sourceSpan = _sourceSpans[spanIndexOfPosition];
                if (sourceSpan.Snapshot.TextBuffer != excludedBuffer)
                    frugalList.Add(new SnapshotPoint(sourceSpan.Snapshot, sourceSpan.Start));
            }
            if (spanIndexOfPosition < _sourceSpans.Count)
            {
                sourceSpan = _sourceSpans[spanIndexOfPosition];
                if (sourceSpan.Snapshot.TextBuffer != excludedBuffer)
                    frugalList.Add(new SnapshotPoint(sourceSpan.Snapshot, sourceSpan.Start));
            }
            if (frugalList.Count == 0)
                frugalList.Add(snapshotPoint);
            return new ReadOnlyCollection<SnapshotPoint>(frugalList);
        }

        internal int FindHighestSpanIndexOfPosition(int position)
        {
            var num1 = 0;
            var num2 = _sourceSpans.Count - 1;
            while (num1 <= num2)
            {
                var index = (num1 + num2) / 2;
                if (position < _cumulativeLengths[index])
                {
                    num2 = index - 1;
                }
                else
                {
                    if (position < _cumulativeLengths[index + 1])
                        return index;
                    num1 = index + 1;
                }
            }
            return _sourceSpans.Count - 1;
        }

        internal int FindLowestSpanIndexOfPosition(int position)
        {
            var num1 = 0;
            var num2 = _sourceSpans.Count - 1;
            while (num1 <= num2)
            {
                var index = (num1 + num2) / 2;
                if (position < _cumulativeLengths[index] || index > 0 && position == _cumulativeLengths[index])
                {
                    num2 = index - 1;
                }
                else
                {
                    if (position <= _cumulativeLengths[index + 1])
                        return index;
                    num1 = index + 1;
                }
            }
            return _sourceSpans.Count - 1;
        }

        internal int FindLowestSpanIndexOfLineNumber(int lineNumber)
        {
            var num1 = 0;
            var num2 = _sourceSpans.Count - 1;
            while (num1 <= num2)
            {
                var index = (num1 + num2) / 2;
                if (lineNumber <= _cumulativeLineBreakCounts[index] && index > 0)
                {
                    num2 = index - 1;
                }
                else
                {
                    if (lineNumber <= _cumulativeLineBreakCounts[index + 1])
                        return index;
                    num1 = index + 1;
                }
            }
            return _sourceSpans.Count - 1;
        }

        private string LocalToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Snapshot {0,10} V{1}\r\n", TextUtilities.GetTagOrContentType((ITextBuffer)_projectionBuffer), Version.VersionNumber);
            var start = 0;
            foreach (var sourceSpan in _sourceSpans)
            {
                stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0,12} {1,10} {2,4} {3,12} {4}\r\n",
                    new Span(start, sourceSpan.Length),
                     TextUtilities.GetTagOrContentType(sourceSpan.Snapshot.TextBuffer),
                     ("V" + sourceSpan.Snapshot.Version.VersionNumber), sourceSpan.Span,
                     TextUtilities.Escape(sourceSpan.GetText()));
                start += sourceSpan.Length;
            }
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return LocalToString();
        }

        private struct InvertedSource
        {
            public readonly Span SourceSpan;
            public readonly int ProjectedPosition;

            public InvertedSource(Span sourceSpan, int projectedPosition)
            {
                SourceSpan = sourceSpan;
                ProjectedPosition = projectedPosition;
            }
        }
    }
}