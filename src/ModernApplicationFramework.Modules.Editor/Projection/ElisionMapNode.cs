using System.Collections.Generic;
using System.Globalization;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using TextChange = ModernApplicationFramework.Modules.Editor.Text.TextChange;

namespace ModernApplicationFramework.Modules.Editor.Projection
{
    internal class ElisionMapNode
    {
        private readonly bool _leftmostElision;
        private readonly int _exposedSize;
        private readonly int _sourceSize;
        private readonly int _exposedLineBreakCount;
        private readonly int _sourceLineBreakCount;
        private readonly ElisionMapNode _left;
        private readonly ElisionMapNode _right;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Exp:{0} Src:{1} TExp:{2} Tsrc:{3}", _exposedSize, _sourceSize, TotalExposedSize, TotalSourceSize);
        }

        public ElisionMapNode(int exposedSize, int sourceSize, int exposedLineBreakCount, int sourceLineBreakCount, bool leftmostElision)
        {
            _exposedSize = exposedSize;
            _sourceSize = sourceSize;
            _exposedLineBreakCount = exposedLineBreakCount;
            _sourceLineBreakCount = sourceLineBreakCount;
            TotalExposedSize = exposedSize;
            TotalSourceSize = sourceSize;
            TotalExposedLineBreakCount = exposedLineBreakCount;
            TotalSourceLineBreakCount = sourceLineBreakCount;
            _leftmostElision = leftmostElision;
        }

        public ElisionMapNode(int exposedSize, int sourceSize, int exposedLineBreakCount, int sourceLineBreakCount, ElisionMapNode left, ElisionMapNode right, bool leftmostElision)
        {
            _exposedSize = exposedSize;
            _sourceSize = sourceSize;
            _exposedLineBreakCount = exposedLineBreakCount;
            _sourceLineBreakCount = sourceLineBreakCount;
            _left = left;
            _right = right;
            TotalExposedSize = LeftTotalExposedSize() + exposedSize + RightTotalExposedSize();
            TotalSourceSize = LeftTotalSourceSize() + sourceSize + RightTotalSourceSize();
            TotalExposedLineBreakCount = LeftTotalExposedLineBreakCount() + exposedLineBreakCount + RightTotalExposedLineBreakCount();
            TotalSourceLineBreakCount = LeftTotalSourceLineBreakCount() + sourceLineBreakCount + RightTotalSourceLineBreakCount();
            _leftmostElision = leftmostElision;
        }

        public int TotalExposedSize { get; }

        public int TotalSourceSize { get; }

        public int TotalExposedLineBreakCount { get; }

        public int TotalSourceLineBreakCount { get; }

        private int LeftTotalSourceSize()
        {
            return _left?.TotalSourceSize ?? 0;
        }

        private int LeftTotalExposedSize()
        {
            return _left?.TotalExposedSize ?? 0;
        }

        private int LeftTotalHiddenSize()
        {
            if (_left != null)
                return _left.TotalSourceSize - _left.TotalExposedSize;
            return 0;
        }

        private int LeftTotalExposedLineBreakCount()
        {
            if (_left != null)
                return _left.TotalExposedLineBreakCount;
            return 0;
        }

        private int LeftTotalSourceLineBreakCount()
        {
            if (_left != null)
                return _left.TotalSourceLineBreakCount;
            return 0;
        }

        private int LeftTotalHiddenLineBreakCount()
        {
            if (_left != null)
                return _left.TotalSourceLineBreakCount - _left.TotalExposedLineBreakCount;
            return 0;
        }

        private int RightTotalSourceSize()
        {
            if (_right != null)
                return _right.TotalSourceSize;
            return 0;
        }

        private int RightTotalExposedSize()
        {
            if (_right != null)
                return _right.TotalExposedSize;
            return 0;
        }

        private int RightTotalExposedLineBreakCount()
        {
            if (_right != null)
                return _right.TotalExposedLineBreakCount;
            return 0;
        }

        private int RightTotalSourceLineBreakCount()
        {
            if (_right != null)
                return _right.TotalSourceLineBreakCount;
            return 0;
        }

        public void Dump(int level)
        {
            _left?.Dump(level + 1);
            _right?.Dump(level + 1);
        }

        public void GetSourceSpans(ITextSnapshot sourceSnapshot, ref int rank, ref int sourcePrefixSize, int startSpanIndex, int endSpanIndex, IList<SnapshotSpan> result)
        {
            _left?.GetSourceSpans(sourceSnapshot, ref rank, ref sourcePrefixSize, startSpanIndex, endSpanIndex, result);
            if (!_leftmostElision)
                ++rank;
            if (rank >= startSpanIndex && rank < endSpanIndex)
                result.Add(new SnapshotSpan(sourceSnapshot, sourcePrefixSize, _exposedSize));
            sourcePrefixSize += _sourceSize;
            _right?.GetSourceSpans(sourceSnapshot, ref rank, ref sourcePrefixSize, startSpanIndex, endSpanIndex, result);
        }

        public SnapshotPoint MapToSourceSnapshot(ITextSnapshot sourceSnapshot, int exposedPosition, int sourcePrefixLength, PositionAffinity affinity)
        {
            var num = LeftTotalExposedSize();
            if (affinity == PositionAffinity.Predecessor && exposedPosition <= num && _left != null && !_left._leftmostElision || affinity == PositionAffinity.Successor && exposedPosition < num)
                return _left.MapToSourceSnapshot(sourceSnapshot, exposedPosition, sourcePrefixLength, affinity);
            if (affinity == PositionAffinity.Predecessor && exposedPosition <= num + _exposedSize || affinity == PositionAffinity.Successor && exposedPosition < num + _exposedSize)
                return new SnapshotPoint(sourceSnapshot, sourcePrefixLength + LeftTotalSourceSize() + (exposedPosition - num));
            if (_right == null)
                return new SnapshotPoint(sourceSnapshot, sourcePrefixLength + LeftTotalSourceSize() + _exposedSize);
            return _right.MapToSourceSnapshot(sourceSnapshot, exposedPosition - (num + _exposedSize), sourcePrefixLength + LeftTotalSourceSize() + _sourceSize, affinity);
        }

        public SnapshotPoint? MapFromSourceSnapshot(ITextSnapshot snapshot, int sourcePosition, int exposedPrefixLength)
        {
            var num = LeftTotalSourceSize();
            if (sourcePosition < num)
            {
                if (_left == null)
                    return new SnapshotPoint?();
                return _left.MapFromSourceSnapshot(snapshot, sourcePosition, exposedPrefixLength);
            }
            if (sourcePosition < num + _sourceSize)
            {
                if (sourcePosition <= num + _exposedSize && !_leftmostElision)
                    return new SnapshotPoint(snapshot, exposedPrefixLength + LeftTotalExposedSize() + (sourcePosition - num));
                return new SnapshotPoint?();
            }
            if (_right != null)
                return _right.MapFromSourceSnapshot(snapshot, sourcePosition - (num + _sourceSize), exposedPrefixLength + LeftTotalExposedSize() + _exposedSize);
            if (_exposedSize < _sourceSize)
                return new SnapshotPoint?();
            return new SnapshotPoint(snapshot, exposedPrefixLength + LeftTotalExposedSize() + _exposedSize);
        }

        public int MapFromSourceSnapshotToNearest(int sourcePosition, int exposedPrefixLength)
        {
            if (sourcePosition < LeftTotalSourceSize())
                return _left.MapFromSourceSnapshotToNearest(sourcePosition, exposedPrefixLength);
            exposedPrefixLength += LeftTotalExposedSize();
            sourcePosition -= LeftTotalSourceSize();
            if (sourcePosition < _sourceSize)
            {
                if (sourcePosition < _exposedSize)
                    return exposedPrefixLength + sourcePosition;
                return exposedPrefixLength + _exposedSize;
            }
            if (_right == null)
                return exposedPrefixLength + _exposedSize;
            exposedPrefixLength += _exposedSize;
            sourcePosition -= _sourceSize;
            return _right.MapFromSourceSnapshotToNearest(sourcePosition, exposedPrefixLength);
        }

        public void MapToSourceSnapshots(ITextSnapshot sourceSnapshot, Span mapSpan, int sourcePrefixSize, FrugalList<SnapshotSpan> result)
        {
            var num = LeftTotalExposedSize();
            var span1 = new Span(0, num);
            var span2 = new Span(num, _exposedSize);
            var span3 = new Span(num + _exposedSize, RightTotalExposedSize());
            var nullable1 = mapSpan.Overlap(span1);
            var nullable2 = mapSpan.Overlap(span2);
            var nullable3 = mapSpan.Overlap(span3);
            if (nullable1.HasValue)
                _left.MapToSourceSnapshots(sourceSnapshot, nullable1.Value, sourcePrefixSize, result);
            if (nullable2.HasValue)
            {
                var span4 = new Span(nullable2.Value.Start + sourcePrefixSize + LeftTotalHiddenSize(), nullable2.Value.Length);
                result.Add(new SnapshotSpan(sourceSnapshot, span4));
            }
            if (!nullable3.HasValue)
                return;
            var right = _right;
            var sourceSnapshot1 = sourceSnapshot;
            var span5 = nullable3.Value;
            var start = span5.Start - num - _exposedSize;
            span5 = nullable3.Value;
            var length = span5.Length;
            var mapSpan1 = new Span(start, length);
            var sourcePrefixSize1 = sourcePrefixSize + LeftTotalSourceSize() + _sourceSize;
            var result1 = result;
            right.MapToSourceSnapshots(sourceSnapshot1, mapSpan1, sourcePrefixSize1, result1);
        }

        public void MapFromSourceSnapshot(Span mapSpan, int exposedPrefixSize, FrugalList<Span> result)
        {
            var num = LeftTotalSourceSize();
            var span1 = new Span(0, num);
            var span2 = new Span(num, _exposedSize);
            var span3 = new Span(num + _sourceSize, RightTotalSourceSize());
            var nullable1 = mapSpan.Overlap(span1);
            var nullable2 = mapSpan.Overlap(span2);
            var nullable3 = mapSpan.Overlap(span3);
            if (nullable1.HasValue)
                _left.MapFromSourceSnapshot(nullable1.Value, exposedPrefixSize, result);
            if (nullable2.HasValue)
                result.Add(new Span(nullable2.Value.Start + exposedPrefixSize - LeftTotalHiddenSize(), nullable2.Value.Length));
            if (!nullable3.HasValue)
                return;
            _right.MapFromSourceSnapshot(new Span(nullable3.Value.Start - num - _sourceSize, nullable3.Value.Length), exposedPrefixSize + LeftTotalExposedSize() + _exposedSize, result);
        }

        public void MapNullSpanFromSourceSnapshot(Span nullSourceSpan, int exposedPrefixSize, FrugalList<Span> result)
        {
            var num = LeftTotalSourceSize();
            var span1 = new Span(num, _exposedSize);
            if (_left != null && new Span(0, num).IntersectsWith(nullSourceSpan))
                _left.MapNullSpanFromSourceSnapshot(nullSourceSpan, exposedPrefixSize, result);
            if (!_leftmostElision && span1.IntersectsWith(nullSourceSpan))
            {
                var span2 = span1.Intersection(nullSourceSpan).Value;
                result.Add(new Span(span2.Start + exposedPrefixSize - LeftTotalHiddenSize(), 0));
            }
            if (_right == null)
                return;
            var span3 = new Span(num + _sourceSize, RightTotalSourceSize());
            if (!span3.IntersectsWith(nullSourceSpan))
                return;
            _right.MapNullSpanFromSourceSnapshot(new Span(span3.Intersection(nullSourceSpan).Value.Start - num - _sourceSize, 0), exposedPrefixSize + LeftTotalExposedSize() + _exposedSize, result);
        }

        public void MapInsertionPointToSourceSnapshots(IElisionSnapshot elisionSnapshot, int exposedPosition, int sourcePrefixLength, FrugalList<SnapshotPoint> points)
        {
            var num = LeftTotalExposedSize();
            if (_left != null && exposedPosition <= num)
                _left.MapInsertionPointToSourceSnapshots(elisionSnapshot, exposedPosition, sourcePrefixLength, points);
            if ((!_leftmostElision ? 0 : (elisionSnapshot.Length > 0 ? 1 : 0)) == 0 && exposedPosition >= num && exposedPosition <= num + _exposedSize)
                points.Add(new SnapshotPoint(elisionSnapshot.SourceSnapshot, sourcePrefixLength + LeftTotalSourceSize() + (exposedPosition - num)));
            if (_right == null || exposedPosition < num + _exposedSize)
                return;
            _right.MapInsertionPointToSourceSnapshots(elisionSnapshot, exposedPosition - (LeftTotalExposedSize() + _exposedSize), sourcePrefixLength + LeftTotalSourceSize() + _sourceSize, points);
        }

        public ProjectionLineInfo GetLineFromPosition(ITextSnapshot sourceSnapshot, int exposedPosition, int sourcePrefixLineBreakCount, int hiddenPrefixLineBreakCount, int sourcePrefixSize, int exposedPrefixSize, int level)
        {
            var span = new Span(LeftTotalExposedSize(), _exposedSize);
            var calculationState1 = ProjectionLineCalculationState.Primary;
            var exposedPosition1 = exposedPosition;
            var projectionLineInfo = new ProjectionLineInfo();
            do
            {
                if (exposedPosition1 < span.Start)
                {
                    var lineFromPosition = _left.GetLineFromPosition(sourceSnapshot, exposedPosition1, sourcePrefixLineBreakCount, hiddenPrefixLineBreakCount, sourcePrefixSize, exposedPrefixSize, level + 1);
                    if (calculationState1 == ProjectionLineCalculationState.Primary)
                    {
                        if (lineFromPosition.EndComplete)
                            return lineFromPosition;
                        calculationState1 = ProjectionLineCalculationState.Append;
                        projectionLineInfo = lineFromPosition;
                        exposedPosition1 = span.Start;
                    }
                    else
                    {
                        if (projectionLineInfo.LineNumber == lineFromPosition.LineNumber)
                        {
                            projectionLineInfo.Start = lineFromPosition.Start;
                            projectionLineInfo.StartComplete = lineFromPosition.StartComplete;
                        }
                        else
                            projectionLineInfo.StartComplete = true;
                        if (calculationState1 != ProjectionLineCalculationState.Bipend)
                            return projectionLineInfo;
                        calculationState1 = ProjectionLineCalculationState.Append;
                        exposedPosition1 = span.End;
                    }
                }
                else if (exposedPosition1 < span.End || _right == null)
                {
                    var position = sourcePrefixSize + LeftTotalHiddenSize() + exposedPosition1;
                    var lineFromPosition = sourceSnapshot.GetLineFromPosition(position);
                    var calculationState2 = ProjectionLineCalculationState.Primary;
                    var num1 = lineFromPosition.LineNumber - (LeftTotalHiddenLineBreakCount() + hiddenPrefixLineBreakCount);
                    if (calculationState1 == ProjectionLineCalculationState.Primary)
                    {
                        projectionLineInfo = new ProjectionLineInfo {LineNumber = num1};
                    }
                    var num2 = position - lineFromPosition.Start;
                    var num3 = exposedPosition1 - num2;
                    if (calculationState1 == ProjectionLineCalculationState.Prepend && num1 < projectionLineInfo.LineNumber)
                        projectionLineInfo.StartComplete = true;
                    else if (calculationState1 == ProjectionLineCalculationState.Primary || calculationState1 == ProjectionLineCalculationState.Prepend)
                    {
                        if (num3 > span.Start)
                        {
                            projectionLineInfo.Start = exposedPrefixSize + num3;
                            projectionLineInfo.StartComplete = true;
                        }
                        else
                        {
                            projectionLineInfo.Start = exposedPrefixSize + span.Start;
                            projectionLineInfo.StartComplete = false;
                            if (LeftTotalExposedSize() > 0)
                            {
                                calculationState2 = ProjectionLineCalculationState.Prepend;
                                exposedPosition1 = span.Start - 1;
                            }
                        }
                    }
                    if (calculationState1 == ProjectionLineCalculationState.Primary || calculationState1 == ProjectionLineCalculationState.Append)
                    {
                        var num4 = num3 + lineFromPosition.LengthIncludingLineBreak;
                        if (num4 <= span.End)
                        {
                            projectionLineInfo.End = num4 + exposedPrefixSize - lineFromPosition.LineBreakLength;
                            projectionLineInfo.EndComplete = true;
                            projectionLineInfo.LineBreakLength = lineFromPosition.LineBreakLength;
                        }
                        else
                        {
                            projectionLineInfo.End = exposedPrefixSize + span.End;
                            projectionLineInfo.EndComplete = false;
                            if (_right == null)
                            {
                                if (calculationState2 != ProjectionLineCalculationState.Prepend)
                                    return projectionLineInfo;
                            }
                            else if (calculationState2 == ProjectionLineCalculationState.Prepend)
                            {
                                calculationState2 = ProjectionLineCalculationState.Bipend;
                            }
                            else
                            {
                                calculationState2 = ProjectionLineCalculationState.Append;
                                exposedPosition1 = span.End;
                            }
                        }
                    }
                    if (calculationState2 == ProjectionLineCalculationState.Primary)
                        return projectionLineInfo;
                    calculationState1 = calculationState2;
                }
                else
                {
                    var lineFromPosition = _right.GetLineFromPosition(sourceSnapshot, exposedPosition1 - (LeftTotalExposedSize() + _exposedSize), sourcePrefixLineBreakCount + LeftTotalSourceLineBreakCount() + _sourceLineBreakCount, hiddenPrefixLineBreakCount + LeftTotalHiddenLineBreakCount() + (_sourceLineBreakCount - _exposedLineBreakCount), sourcePrefixSize + LeftTotalSourceSize() + _sourceSize, exposedPrefixSize + LeftTotalExposedSize() + _exposedSize, level + 1);
                    if (calculationState1 == ProjectionLineCalculationState.Primary)
                    {
                        if (lineFromPosition.StartComplete)
                            return lineFromPosition;
                        calculationState1 = ProjectionLineCalculationState.Prepend;
                        projectionLineInfo = lineFromPosition;
                        exposedPosition1 = span.End - 1;
                    }
                    else
                    {
                        projectionLineInfo.End = lineFromPosition.End;
                        projectionLineInfo.EndComplete = lineFromPosition.EndComplete;
                        projectionLineInfo.LineBreakLength = lineFromPosition.LineBreakLength;
                        return projectionLineInfo;
                    }
                }
            }
            while (exposedPosition1 >= 0 && exposedPosition1 <= TotalExposedSize);
            return projectionLineInfo;
        }

        public ProjectionLineInfo GetLineFromLineNumber(ITextSnapshot sourceSnapshot, int exposedLineNumber)
        {
            var positionFromLineNumber = GetPositionFromLineNumber(sourceSnapshot, exposedLineNumber, 0, 0);
            return GetLineFromPosition(sourceSnapshot, positionFromLineNumber, 0, 0, 0, 0, 0);
        }

        private int GetPositionFromLineNumber(ITextSnapshot sourceSnapshot, int relativeExposedLineNumber, int sourcePrefixLineBreakCount, int sourcePrefixHiddenSize)
        {
            if (relativeExposedLineNumber < LeftTotalExposedLineBreakCount())
                return _left.GetPositionFromLineNumber(sourceSnapshot, relativeExposedLineNumber, sourcePrefixLineBreakCount, sourcePrefixHiddenSize);
            if (relativeExposedLineNumber >= LeftTotalExposedLineBreakCount() + _exposedLineBreakCount && _right != null)
                return _right.GetPositionFromLineNumber(sourceSnapshot, relativeExposedLineNumber - (LeftTotalExposedLineBreakCount() + _exposedLineBreakCount), sourcePrefixLineBreakCount + LeftTotalSourceLineBreakCount() + _sourceLineBreakCount, sourcePrefixHiddenSize + LeftTotalHiddenSize() + (_sourceSize - _exposedSize));
            var lineNumber = sourcePrefixLineBreakCount + LeftTotalHiddenLineBreakCount() + relativeExposedLineNumber;
            return sourceSnapshot.GetLineFromLineNumber(lineNumber).End - sourcePrefixHiddenSize - LeftTotalHiddenSize();
        }

        public int GetLineNumberFromPosition(ITextSnapshot sourceSnapshot, int exposedPosition, int hiddenPrefixLineBreakCount, int sourcePrefixSize)
        {
            var span = new Span(LeftTotalExposedSize(), _exposedSize);
            if (exposedPosition < span.Start)
                return _left.GetLineNumberFromPosition(sourceSnapshot, exposedPosition, hiddenPrefixLineBreakCount, sourcePrefixSize);
            if (exposedPosition < span.End || _right == null)
                return sourceSnapshot.GetLineNumberFromPosition(exposedPosition + sourcePrefixSize + LeftTotalSourceSize() - LeftTotalExposedSize()) - (LeftTotalHiddenLineBreakCount() + hiddenPrefixLineBreakCount);
            return _right.GetLineNumberFromPosition(sourceSnapshot, exposedPosition - span.End, hiddenPrefixLineBreakCount + LeftTotalHiddenLineBreakCount() + (_sourceLineBreakCount - _exposedLineBreakCount), sourcePrefixSize + LeftTotalSourceSize() + _sourceSize);
        }

        public ElisionMapNode IncorporateChange(ITextSnapshot beforeSourceSnapshot, ITextSnapshot afterSourceSnapshot, ITextSnapshot beforeElisionSnapshot, int? sourceInsertionPosition, StringRebuilder newText, Span? sourceDeletionSpan, int absoluteSourceOldPosition, int absoluteSourceNewPosition, int projectedPrefixSize, FrugalList<TextChange> projectedChanges, int incomingAccumulatedDelta, ref int outgoingAccumulatedDelta, ref int accumulatedDelete)
        {
            var left = _left;
            var right1 = _right;
            var exposedLineBreakCount = _exposedLineBreakCount;
            var sourceLineBreakCount = _sourceLineBreakCount;
            var exposedSize = _exposedSize;
            var sourceSize = _sourceSize;
            var leftmostElision = _leftmostElision;
            var num1 = LeftTotalSourceSize();
            var span1 = new Span(0, num1);
            var span2 = new Span(num1, _exposedSize);
            var span3 = new Span(span2.End, _sourceSize - _exposedSize);
            var span4 = new Span(span3.End, TotalSourceSize - num1 - _sourceSize);
            var nullable1 = span1.Overlap(sourceDeletionSpan);
            var flag1 = sourceInsertionPosition.HasValue && sourceInsertionPosition.Value < num1;
            var sourceInsertionPosition1 = new int?();
            if (flag1)
            {
                if (nullable1.HasValue && nullable1.Value.End == span2.Start)
                {
                    flag1 = false;
                    sourceInsertionPosition = span2.Start;
                }
                else
                    sourceInsertionPosition1 = sourceInsertionPosition;
            }
            if (flag1 || nullable1.HasValue)
                left = _left.IncorporateChange(beforeSourceSnapshot, afterSourceSnapshot, beforeElisionSnapshot, sourceInsertionPosition1, newText, span1.Overlap(sourceDeletionSpan), absoluteSourceOldPosition, absoluteSourceNewPosition, projectedPrefixSize, projectedChanges, incomingAccumulatedDelta, ref outgoingAccumulatedDelta, ref accumulatedDelete);
            var nullable2 = span2.Overlap(sourceDeletionSpan);
            var nullable3 = span3.Overlap(sourceDeletionSpan);
            if (sourceInsertionPosition.HasValue)
            {
                if (_leftmostElision)
                {
                    if (sourceInsertionPosition.Value <= _sourceSize)
                    {
                        sourceSize += newText.Length;
                        ComputeIncrementalLineCountForHiddenInsertion(afterSourceSnapshot, absoluteSourceNewPosition, newText, out var incrementalLineCount);
                        sourceLineBreakCount += incrementalLineCount;
                        sourceInsertionPosition = new int?();
                    }
                }
                else if (num1 <= sourceInsertionPosition.Value && sourceInsertionPosition.Value <= num1 + _exposedSize)
                {
                    exposedSize += newText.Length;
                    sourceSize += newText.Length;
                    var oldPosition = projectedPrefixSize + sourceInsertionPosition.Value - LeftTotalHiddenSize() - incomingAccumulatedDelta;
                    var num2 = 0;
                    if (nullable2.HasValue)
                        num2 = nullable2.Value.Length;
                    ComputeIncrementalLineCountForExposedInsertion(sourceInsertionPosition.Value > span2.Start ? afterSourceSnapshot[absoluteSourceNewPosition - 1] : new char?(), sourceInsertionPosition.Value + num2 < span2.End ? afterSourceSnapshot[absoluteSourceNewPosition + newText.Length] : new char?(), newText, out var incrementalLineCount, out var boundaryConditions);
                    exposedLineBreakCount += incrementalLineCount;
                    sourceLineBreakCount += incrementalLineCount;
                    var empty = StringRebuilder.Empty;
                    var newText1 = newText;
                    var num3 = (int)boundaryConditions;
                    var textChange = new TextChange(oldPosition, empty, newText1, (LineBreakBoundaryConditions)num3);
                    projectedChanges.Add(textChange);
                    outgoingAccumulatedDelta += textChange.Delta;
                    sourceInsertionPosition = new int?();
                }
                else if (num1 + _exposedSize < sourceInsertionPosition.Value && (sourceInsertionPosition.Value < num1 + _sourceSize || _right == null))
                {
                    if (_right != null && nullable3.HasValue && nullable3.Value.End == span3.End)
                    {
                        sourceInsertionPosition = span3.End;
                    }
                    else
                    {
                        sourceSize += newText.Length;
                        ComputeIncrementalLineCountForHiddenInsertion(afterSourceSnapshot, absoluteSourceNewPosition, newText, out var incrementalLineCount);
                        sourceLineBreakCount += incrementalLineCount;
                        sourceInsertionPosition = new int?();
                    }
                }
            }
            Span span5;
            if (nullable2.HasValue)
            {
                exposedSize -= nullable2.Value.Length;
                sourceSize -= nullable2.Value.Length;
                var num2 = projectedPrefixSize;
                span5 = nullable2.Value;
                var start1 = span5.Start;
                var num3 = num2 + start1 - LeftTotalHiddenSize();
                var num4 = absoluteSourceOldPosition - accumulatedDelete;
                var snapshot = beforeSourceSnapshot;
                var start2 = num4;
                span5 = nullable2.Value;
                var length1 = span5.Length;
                var span6 = new Span(start2, length1);
                var oldText = BufferFactoryService.StringRebuilderFromSnapshotAndSpan(snapshot, span6);
                var beforeSnapshot = beforeElisionSnapshot;
                var start3 = num3 - incomingAccumulatedDelta;
                span5 = nullable2.Value;
                var length2 = span5.Length;
                var deletionSpan = new Span(start3, length2);
                var deletedText = oldText;
                ComputeIncrementalLineCountForDeletion(beforeSnapshot, deletionSpan, deletedText, out var num5, out var boundaryConditions);
                exposedLineBreakCount += num5;
                sourceLineBreakCount += num5;
                var textChange = new TextChange(num3 - incomingAccumulatedDelta, oldText, StringRebuilder.Empty, boundaryConditions);
                projectedChanges.Add(textChange);
                outgoingAccumulatedDelta += textChange.Delta;
                accumulatedDelete += textChange.Delta;
            }
            if (nullable3.HasValue)
            {
                var num2 = absoluteSourceOldPosition - accumulatedDelete;
                var snapshot = beforeSourceSnapshot;
                var start1 = num2;
                span5 = nullable3.Value;
                var length1 = span5.Length;
                var span6 = new Span(start1, length1);
                var stringRebuilder = BufferFactoryService.StringRebuilderFromSnapshotAndSpan(snapshot, span6);
                var beforeSnapshot = beforeSourceSnapshot;
                var start2 = num2;
                span5 = nullable3.Value;
                var length2 = span5.Length;
                var deletionSpan = new Span(start2, length2);
                var deletedText = stringRebuilder;
                ComputeIncrementalLineCountForDeletion(beforeSnapshot, deletionSpan, deletedText, out var num3, out var boundaryConditions);
                sourceLineBreakCount += num3;
                var num4 = sourceSize;
                span5 = nullable3.Value;
                var length3 = span5.Length;
                sourceSize = num4 - length3;
                ref var local3 = ref accumulatedDelete;
                var num5 = accumulatedDelete;
                span5 = nullable3.Value;
                var length4 = span5.Length;
                var num6 = num5 - length4;
                local3 = num6;
            }
            var otherSpan = sourceDeletionSpan;
            var nullable4 = span4.Overlap(otherSpan);
            var flag2 = sourceInsertionPosition.HasValue && _right != null && num1 + _sourceSize <= sourceInsertionPosition.Value;
            if (nullable4.HasValue | flag2)
            {
                var right2 = _right;
                var beforeSourceSnapshot1 = beforeSourceSnapshot;
                var afterSourceSnapshot1 = afterSourceSnapshot;
                var beforeElisionSnapshot1 = beforeElisionSnapshot;
                var sourceInsertionPosition2 = flag2 ? sourceInsertionPosition.Value - num1 - _sourceSize : new int?();
                var newText1 = flag2 ? newText : StringRebuilder.Empty;
                Span? sourceDeletionSpan1;
                if (!nullable4.HasValue)
                {
                    sourceDeletionSpan1 = new Span?();
                }
                else
                {
                    span5 = nullable4.Value;
                    var start = span5.Start - (num1 + _sourceSize);
                    span5 = nullable4.Value;
                    var length = span5.Length;
                    sourceDeletionSpan1 = new Span(start, length);
                }
                var absoluteSourceOldPosition1 = absoluteSourceOldPosition;
                var absoluteSourceNewPosition1 = absoluteSourceNewPosition;
                var projectedPrefixSize1 = projectedPrefixSize + LeftTotalExposedSize() + _exposedSize;
                var projectedChanges1 = projectedChanges;
                var incomingAccumulatedDelta1 = incomingAccumulatedDelta;
                ref var local1 = ref outgoingAccumulatedDelta;
                ref var local2 = ref accumulatedDelete;
                right1 = right2.IncorporateChange(beforeSourceSnapshot1, afterSourceSnapshot1, beforeElisionSnapshot1, sourceInsertionPosition2, newText1, sourceDeletionSpan1, absoluteSourceOldPosition1, absoluteSourceNewPosition1, projectedPrefixSize1, projectedChanges1, incomingAccumulatedDelta1, ref local1, ref local2);
            }
            return new ElisionMapNode(exposedSize, sourceSize, exposedLineBreakCount, sourceLineBreakCount, left, right1, leftmostElision);
        }

        private static void ComputeIncrementalLineCountForHiddenInsertion(ITextSnapshot afterSnapshot, int start, StringRebuilder insertedText, out int incrementalLineCount)
        {
            var lineBreakCount = insertedText.LineBreakCount;
            var boundaryConditions = LineBreakBoundaryConditions.None;
            if (start > 0 && afterSnapshot[start - 1] == '\r')
            {
                boundaryConditions |= LineBreakBoundaryConditions.PrecedingReturn;
                if (insertedText.FirstCharacter == '\n')
                    --lineBreakCount;
            }
            var index = start + insertedText.Length;
            if (index < afterSnapshot.Length && afterSnapshot[index] == '\n')
            {
                boundaryConditions |= LineBreakBoundaryConditions.SucceedingNewline;
                if (insertedText.LastCharacter == '\r')
                    --lineBreakCount;
            }
            if (boundaryConditions == (LineBreakBoundaryConditions.PrecedingReturn | LineBreakBoundaryConditions.SucceedingNewline))
                ++lineBreakCount;
            incrementalLineCount = lineBreakCount;
        }

        private static void ComputeIncrementalLineCountForExposedInsertion(char? predecessor, char? successor, StringRebuilder insertedText, out int incrementalLineCount, out LineBreakBoundaryConditions boundaryConditions)
        {
            var lineBreakCount = insertedText.LineBreakCount;
            var boundaryConditions1 = LineBreakBoundaryConditions.None;
            var nullable1 = predecessor;
            var nullable2 = nullable1;
            var num1 = 13;
            if ((nullable2.GetValueOrDefault() == num1 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
            {
                boundaryConditions1 |= LineBreakBoundaryConditions.PrecedingReturn;
                if (insertedText.FirstCharacter == '\n')
                    --lineBreakCount;
            }
            var nullable3 = successor;
            nullable2 = nullable3;
            var num2 = 10;
            if ((nullable2.GetValueOrDefault() == num2 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
            {
                boundaryConditions1 |= LineBreakBoundaryConditions.SucceedingNewline;
                if (insertedText.LastCharacter == '\r')
                    --lineBreakCount;
            }
            if (boundaryConditions1 == (LineBreakBoundaryConditions.PrecedingReturn | LineBreakBoundaryConditions.SucceedingNewline))
                ++lineBreakCount;
            incrementalLineCount = lineBreakCount;
            boundaryConditions = boundaryConditions1;
        }

        private static void ComputeIncrementalLineCountForDeletion(ITextSnapshot beforeSnapshot, Span deletionSpan, StringRebuilder deletedText, out int incrementalLineCount, out LineBreakBoundaryConditions boundaryConditions)
        {
            var num = -deletedText.LineBreakCount;
            var boundaryConditions1 = LineBreakBoundaryConditions.None;
            if (deletionSpan.Start > 0 && beforeSnapshot[deletionSpan.Start - 1] == '\r')
            {
                boundaryConditions1 |= LineBreakBoundaryConditions.PrecedingReturn;
                if (deletedText[0] == '\n')
                    ++num;
            }
            if (deletionSpan.End < beforeSnapshot.Length && beforeSnapshot[deletionSpan.End] == '\n')
            {
                boundaryConditions1 |= LineBreakBoundaryConditions.SucceedingNewline;
                if (deletedText[deletedText.Length - 1] == '\r')
                    ++num;
            }
            if (boundaryConditions1 == (LineBreakBoundaryConditions.PrecedingReturn | LineBreakBoundaryConditions.SucceedingNewline))
                --num;
            incrementalLineCount = num;
            boundaryConditions = boundaryConditions1;
        }
    }
}