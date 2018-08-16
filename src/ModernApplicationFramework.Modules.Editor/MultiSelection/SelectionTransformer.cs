using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;
using Selection = ModernApplicationFramework.Text.Ui.Text.Selection;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class SelectionTransformer : ISelectionTransformer, IDisposable
    {
        private readonly HashSet<Selection> _historicalSelections = new HashSet<Selection>();
        private Selection _selection;
        private SelectionUiProperties _uiProperties;
        private VirtualSnapshotPoint _preferredXReference;
        private readonly MultiSelectionBroker _broker;
        private ITextSnapshot _currentSnapshot;
        private bool _highFidelityMode;
        private bool _isDisposed;

        public SelectionTransformer(MultiSelectionBroker multiSelectionBroker, Selection selection)
        {
            _broker = multiSelectionBroker;
            _selection = selection;
            _currentSnapshot = _selection.InsertionPoint.Position.Snapshot;
            if (_currentSnapshot != _broker.CurrentSnapshot)
                throw new ArgumentException("The provided selection is on a different snapshot than the broker.", nameof(selection));
            CapturePreferredReferencePoint();
        }

        public Selection Selection
        {
            get
            {
                CheckIsValid();
                return _selection;
            }
            internal set
            {
                CheckIsValid();
                if (!(_selection != value))
                    return;
                _selection = value;
            }
        }

        public SelectionUiProperties UiProperties
        {
            get
            {
                CheckIsValid();
                return _uiProperties ?? (_uiProperties = new SelectionUiProperties(_broker.Factory, _broker, this));
            }
        }

        public void CapturePreferredXReferencePoint()
        {
            CheckIsValid();
            if (_broker.TextView.TextViewLines == null)
            {
                _preferredXReference = new VirtualSnapshotPoint(_currentSnapshot, 0);
                UiProperties.SetPreferredXCoordinate(0.0);
            }
            else
            {
                _preferredXReference = _selection.InsertionPoint;
                UiProperties.SetPreferredXCoordinate(GetXCoordinateFromVirtualBufferPosition(_broker.TextView.GetTextViewLineContainingBufferPosition(_preferredXReference.Position), _preferredXReference));
            }
        }

        public void CapturePreferredYReferencePoint()
        {
            CheckIsValid();
            if (_broker.TextView.TextViewLines == null)
            {
                UiProperties.SetPreferredYCoordinate(0.0);
            }
            else
            {
                var textViewLine = _broker.TextView.GetTextViewLineContainingBufferPosition(_selection.InsertionPoint.Position);
                if (textViewLine.VisibilityState == VisibilityState.Unattached || textViewLine.VisibilityState == VisibilityState.Hidden)
                {
                    textViewLine = _broker.TextView.TextViewLines.LastVisibleLine;
                    var snapshotPoint = _selection.InsertionPoint.Position;
                    var position1 = snapshotPoint.Position;
                    snapshotPoint = textViewLine.Start;
                    var position2 = snapshotPoint.Position;
                    if (position1 < position2)
                        textViewLine = _broker.TextView.TextViewLines.FirstVisibleLine;
                }
                UiProperties.SetPreferredYCoordinate(textViewLine.TextTop + textViewLine.TextHeight * 0.5 - _broker.TextView.ViewportTop);
            }
        }

        public void CapturePreferredReferencePoint()
        {
            CheckIsValid();
            CapturePreferredXReferencePoint();
            CapturePreferredYReferencePoint();
        }

        internal IDisposable HighFidelityOperation()
        {
            _highFidelityMode = true;
            return new DelegateDisposable(() =>
            {
                _highFidelityMode = false;
                _historicalSelections.Clear();
            });
        }

        internal IReadOnlyCollection<Selection> HistoricalRegions => _historicalSelections;

        internal ITextSnapshot CurrentSnapshot
        {
            get => _currentSnapshot;
            set
            {
                if (_currentSnapshot == value)
                    return;
                if (_highFidelityMode)
                    _historicalSelections.Add(_selection);
                _currentSnapshot = value;
                var num1 = _selection.IsEmpty ? 1 : 0;
                _selection = _selection.MapToSnapshot(value, _broker.TextView);
                var num2 = _selection.IsEmpty ? 1 : 0;
                if (num1 != num2)
                    _broker.QueueCaretUpdatedEvent(this);
                CapturePreferredReferencePoint();
            }
        }

        private ITextViewLine GetPreferredLine()
        {
            return _broker.TextView.TextViewLines.GetTextViewLineContainingYCoordinate(PreferredYCoordinate) ?? _broker.TextView.TextViewLines.LastVisibleLine;
        }

        private VirtualSnapshotPoint GetPreferredXLocationOnLine(ITextViewLine line)
        {
            var virtualBufferPosition = GetXCoordinateFromVirtualBufferPosition(_broker.TextView.GetTextViewLineContainingBufferPosition(_preferredXReference.Position), _preferredXReference);
            var xCoordinate = line.MapXCoordinate(_broker.TextView, virtualBufferPosition, _broker.Factory.SmartIndentationService, false);
            return line.GetInsertionBufferPositionFromXCoordinate(xCoordinate);
        }

        internal static double GetXCoordinateFromVirtualBufferPosition(ITextViewLine textLine, VirtualSnapshotPoint bufferPosition)
        {
            if (!bufferPosition.IsInVirtualSpace && !(bufferPosition.Position == textLine.Start))
                return textLine.GetExtendedCharacterBounds(bufferPosition.Position - 1).Trailing;
            return textLine.GetExtendedCharacterBounds(bufferPosition).Leading;
        }

        private double PreferredYCoordinate => Math.Max(_broker.TextView.ViewportTop, Math.Min(_broker.TextView.ViewportBottom, UiProperties.PreferredYCoordinate + _broker.TextView.ViewportTop));

        private static SnapshotPoint GetFirstNonWhiteSpaceCharacterInSpan(SnapshotSpan span)
        {
            var start = span.Start;
            while (start != span.End && char.IsWhiteSpace(span.Snapshot[start]))
                start += 1;
            return start;
        }

        private static bool ShouldStopAtEndOfLine(int endOfWord, int endOfLine, int currentPosition)
        {
            if (endOfWord == endOfLine)
                return endOfLine > currentPosition;
            return false;
        }

        private static bool IsSpanABlankLine(SnapshotSpan currentWord, ITextViewLine currentLine)
        {
            if (currentWord.IsEmpty)
                return currentWord == currentLine.Extent;
            return false;
        }

        private static bool ShouldContinuePastPreviousWord(SnapshotSpan previousWord, ITextViewLine line)
        {
            if (char.IsWhiteSpace(previousWord.Snapshot[previousWord.Start]))
                return !IsSpanABlankLine(previousWord, line);
            return false;
        }

        private SnapshotSpan GetNextWord()
        {
            var snapshotPoint = _selection.InsertionPoint.Position;
            var textExtent = _broker.TextStructureNavigator.GetExtentOfWord(snapshotPoint);
            var currentLine = _broker.TextView.GetTextViewLineContainingBufferPosition(snapshotPoint);
            if (textExtent.Span.End >= _currentSnapshot.Length)
                return new SnapshotSpan(_currentSnapshot, _currentSnapshot.Length, 0);
            SnapshotSpan span;
            if (_selection.InsertionPoint.Position == currentLine.End)
            {
                var includingLineBreak = currentLine.EndIncludingLineBreak;
                var containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(includingLineBreak);
                var extentOfWord = _broker.TextStructureNavigator.GetExtentOfWord(includingLineBreak);
                if (extentOfWord.IsSignificant)
                    return extentOfWord.Span;
                span = extentOfWord.Span;
                if (span.End >= containingBufferPosition.End)
                    return new SnapshotSpan(containingBufferPosition.End, containingBufferPosition.End);
                snapshotPoint = includingLineBreak;
                textExtent = extentOfWord;
                currentLine = containingBufferPosition;
            }
            span = textExtent.Span;
            if (ShouldStopAtEndOfLine(span.End, currentLine.End, snapshotPoint) || IsSpanABlankLine(textExtent.Span, currentLine))
            {
                span = textExtent.Span;
                var end1 = span.End;
                span = textExtent.Span;
                var end2 = span.End;
                return new SnapshotSpan(end1, end2);
            }
            var structureNavigator1 = _broker.TextStructureNavigator;
            span = textExtent.Span;
            var end3 = span.End;
            var extentOfWord1 = structureNavigator1.GetExtentOfWord(end3);
            if (!extentOfWord1.IsSignificant)
            {
                var structureNavigator2 = _broker.TextStructureNavigator;
                span = extentOfWord1.Span;
                var end1 = span.End;
                extentOfWord1 = structureNavigator2.GetExtentOfWord(end1);
            }
            span = extentOfWord1.Span;
            int start1 = span.Start;
            span = extentOfWord1.Span;
            int end4 = span.End;
            var val1 = start1;
            span = textExtent.Span;
            int end5 = span.End;
            var start2 = Math.Max(val1, end5);
            var num = start2;
            var length = end4 - num;
            return new SnapshotSpan(_currentSnapshot, start2, length);
        }

        private SnapshotSpan GetPreviousWord()
        {
            var position = _selection.InsertionPoint.Position;
            var extentOfWord1 = _broker.TextStructureNavigator.GetExtentOfWord(position);
            var containingBufferPosition1 = _broker.TextView.GetTextViewLineContainingBufferPosition(position);
            if (extentOfWord1.Span.Start <= 0)
                return new SnapshotSpan(_currentSnapshot, 0, 0);
            var span = extentOfWord1.Span;
            if (span.Start == containingBufferPosition1.Start && (position != containingBufferPosition1.Start || _selection.InsertionPoint.IsInVirtualSpace))
                return new SnapshotSpan(containingBufferPosition1.Start, containingBufferPosition1.Start);
            span = extentOfWord1.Span;
            if (span.Start != position)
            {
                span = extentOfWord1.Span;
                if (!span.IsEmpty)
                    return extentOfWord1.Span;
            }
            span = extentOfWord1.Span;
            var snapshotPoint = span.Start - 1;
            var extentOfWord2 = _broker.TextStructureNavigator.GetExtentOfWord(snapshotPoint);
            var containingBufferPosition2 = _broker.TextView.GetTextViewLineContainingBufferPosition(snapshotPoint);
            span = extentOfWord2.Span;
            if (span.Start > 0 && ShouldContinuePastPreviousWord(extentOfWord2.Span, containingBufferPosition2))
            {
                span = extentOfWord2.Span;
                extentOfWord2 = _broker.TextStructureNavigator.GetExtentOfWord(span.Start - 1);
            }
            return extentOfWord2.Span;
        }

        public void MoveTo(VirtualSnapshotPoint point, bool select, PositionAffinity insertionPointAffinity)
        {
            CheckIsValid();
            if (!(_selection.ActivePoint != point) && !(_selection.InsertionPoint != point) && (select || !(_selection.AnchorPoint != point)) && _selection.InsertionPointAffinity == insertionPointAffinity)
                return;
            _selection = new Selection(point, select ? _selection.AnchorPoint : point, point, insertionPointAffinity);
            _broker.QueueCaretUpdatedEvent(this);
        }

        public void MoveTo(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint, VirtualSnapshotPoint insertionPoint, PositionAffinity insertionPointAffinity)
        {
            CheckIsValid();
            if (!(_selection.AnchorPoint != anchorPoint) && !(_selection.ActivePoint != activePoint) && (!(_selection.InsertionPoint != insertionPoint) && _selection.InsertionPointAffinity == insertionPointAffinity))
                return;
            _selection = new Selection(insertionPoint, anchorPoint, activePoint, insertionPointAffinity);
            _broker.QueueCaretUpdatedEvent(this);
        }

        public void PerformAction(PredefinedSelectionTransformations action)
        {
            CheckIsValid();
            switch (action)
            {
                case PredefinedSelectionTransformations.ClearSelection:
                    ClearSelection();
                    break;
                case PredefinedSelectionTransformations.MoveToNextCaretPosition:
                    MoveToNextCaretPosition(false);
                    break;
                case PredefinedSelectionTransformations.SelectToNextCaretPosition:
                    MoveToNextCaretPosition(true);
                    break;
                case PredefinedSelectionTransformations.MoveToPreviousCaretPosition:
                    MoveToPreviousCaretPosition(false);
                    break;
                case PredefinedSelectionTransformations.SelectToPreviousCaretPosition:
                    MoveToPreviousCaretPosition(true);
                    break;
                case PredefinedSelectionTransformations.MoveToNextWord:
                    MoveToNextWord(false);
                    break;
                case PredefinedSelectionTransformations.SelectToNextWord:
                    MoveToNextWord(true);
                    break;
                case PredefinedSelectionTransformations.MoveToPreviousWord:
                    MoveToPreviousWord(false);
                    break;
                case PredefinedSelectionTransformations.SelectToPreviousWord:
                    MoveToPreviousWord(true);
                    break;
                case PredefinedSelectionTransformations.MoveToBeginningOfLine:
                    MoveToBeginningOfLine(false);
                    break;
                case PredefinedSelectionTransformations.SelectToBeginningOfLine:
                    MoveToBeginningOfLine(true);
                    break;
                case PredefinedSelectionTransformations.MoveToHome:
                    MoveToHome(false);
                    break;
                case PredefinedSelectionTransformations.SelectToHome:
                    MoveToHome(true);
                    break;
                case PredefinedSelectionTransformations.MoveToEndOfLine:
                    MoveToEndOfLine(false);
                    break;
                case PredefinedSelectionTransformations.SelectToEndOfLine:
                    MoveToEndOfLine(true);
                    break;
                case PredefinedSelectionTransformations.MoveToNextLine:
                    MoveToNextLine(false);
                    break;
                case PredefinedSelectionTransformations.SelectToNextLine:
                    MoveToNextLine(true);
                    break;
                case PredefinedSelectionTransformations.MoveToPreviousLine:
                    MoveToPreviousLine(false);
                    break;
                case PredefinedSelectionTransformations.SelectToPreviousLine:
                    MoveToPreviousLine(true);
                    break;
                case PredefinedSelectionTransformations.MovePageUp:
                    MovePageUp(false);
                    break;
                case PredefinedSelectionTransformations.SelectPageUp:
                    MovePageUp(true);
                    break;
                case PredefinedSelectionTransformations.MovePageDown:
                    MovePageDown(false);
                    break;
                case PredefinedSelectionTransformations.SelectPageDown:
                    MovePageDown(true);
                    break;
                case PredefinedSelectionTransformations.MoveToStartOfDocument:
                    MoveToStartOfDocument(false);
                    break;
                case PredefinedSelectionTransformations.SelectToStartOfDocument:
                    MoveToStartOfDocument(true);
                    break;
                case PredefinedSelectionTransformations.MoveToEndOfDocument:
                    MoveToEndOfDocument(false);
                    break;
                case PredefinedSelectionTransformations.SelectToEndOfDocument:
                    MoveToEndOfDocument(true);
                    break;
                case PredefinedSelectionTransformations.SelectCurrentWord:
                    SelectCurrentWord();
                    break;
            }
        }

        public void ClearSelection()
        {
            if (_selection.IsEmpty)
                return;
            var insertionPoint1 = _selection.InsertionPoint;
            var selection = Selection;
            var insertionPoint2 = selection.InsertionPoint;
            selection = Selection;
            var insertionPoint3 = selection.InsertionPoint;
            var num = 1;
            _selection = new Selection(insertionPoint1, insertionPoint2, insertionPoint3, (PositionAffinity)num);
            _broker.QueueCaretUpdatedEvent(this);
        }

        private void MovePageDown(bool select)
        {
            var lastVisibleLine = _broker.TextView.TextViewLines.LastVisibleLine;
            ITextViewLine line;
            if (lastVisibleLine.VisibilityState == VisibilityState.FullyVisible && lastVisibleLine.End == lastVisibleLine.Snapshot.Length)
            {
                line = lastVisibleLine;
            }
            else
            {
                var bufferPosition = lastVisibleLine.VisibilityState == VisibilityState.FullyVisible || (int)lastVisibleLine.Start == 0 ? lastVisibleLine.Start : lastVisibleLine.Start - 1;
                if (!_broker.TextView.ViewScroller.ScrollViewportVerticallyByPage(ScrollDirection.Down))
                    return;
                line = _broker.TextView.TextViewLines.GetTextViewLineContainingBufferPosition(bufferPosition).Bottom <= _broker.TextView.ViewportTop ? GetPreferredLine() : _broker.TextView.TextViewLines.LastVisibleLine;
            }
            MoveTo(GetPreferredXLocationOnLine(line), select, PositionAffinity.Successor);
        }

        private void MovePageUp(bool select)
        {
            var firstVisibleLine = _broker.TextView.TextViewLines.FirstVisibleLine;
            var bufferPosition = firstVisibleLine.VisibilityState == VisibilityState.FullyVisible ? firstVisibleLine.Start : firstVisibleLine.EndIncludingLineBreak;
            if (!_broker.TextView.ViewScroller.ScrollViewportVerticallyByPage(ScrollDirection.Up))
                return;
            MoveTo(GetPreferredXLocationOnLine(_broker.TextView.ViewportBottom <= _broker.TextView.TextViewLines.GetTextViewLineContainingBufferPosition(bufferPosition).Bottom ? GetPreferredLine() : _broker.TextView.TextViewLines.FirstVisibleLine), select, PositionAffinity.Successor);
        }

        private void MoveToBeginningOfLine(bool select)
        {
            MoveTo(new VirtualSnapshotPoint(_broker.TextView.GetTextViewLineContainingBufferPosition((select ? _selection.InsertionPoint : Selection.Start).Position).Start), select, PositionAffinity.Successor);
            CapturePreferredReferencePoint();
        }

        private void MoveToEndOfDocument(bool select)
        {
            MoveTo(new VirtualSnapshotPoint(_currentSnapshot, _currentSnapshot.Length), select, PositionAffinity.Successor);
            CapturePreferredReferencePoint();
        }

        private void MoveToEndOfLine(bool select)
        {
            var virtualSnapshotPoint = select ? _selection.InsertionPoint : Selection.End;
            var containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(virtualSnapshotPoint.Position);
            if (containingBufferPosition.Extent.IsEmpty)
            {
                if (virtualSnapshotPoint.IsInVirtualSpace)
                {
                    MoveTo(new VirtualSnapshotPoint(virtualSnapshotPoint.Position), select, PositionAffinity.Successor);
                }
                else
                {
                    var desiredIndentation = _broker.Factory.SmartIndentationService.GetDesiredIndentation(_broker.TextView, containingBufferPosition.Start.GetContainingLine());
                    MoveTo(new VirtualSnapshotPoint(virtualSnapshotPoint.Position, desiredIndentation ?? 0), select, PositionAffinity.Successor);
                }
            }
            else
                MoveTo(new VirtualSnapshotPoint(containingBufferPosition.End), select, PositionAffinity.Successor);
            CapturePreferredReferencePoint();
        }

        private void MoveToHome(bool select)
        {
            var containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(_selection.InsertionPoint.Position);
            var position = GetFirstNonWhiteSpaceCharacterInSpan(containingBufferPosition.Extent);
            if (position == _selection.InsertionPoint.Position || position == containingBufferPosition.End)
                position = containingBufferPosition.Start;
            MoveTo(new VirtualSnapshotPoint(position), select, PositionAffinity.Successor);
            CapturePreferredReferencePoint();
        }

        private void MoveToNextCaretPosition(bool select)
        {
            if (!select && !_selection.IsEmpty)
            {
                MoveTo(_selection.End, false, PositionAffinity.Successor);
                CapturePreferredReferencePoint();
            }
            else
            {
                var containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(_selection.InsertionPoint.Position);
                var insertionPoint = _selection.InsertionPoint;
                VirtualSnapshotPoint point = default;
                if (insertionPoint.Position == containingBufferPosition.End && containingBufferPosition.IsLastTextViewLineForSnapshotLine)
                {
                    if (_broker.TextView.IsVirtualSpaceOrBoxSelectionEnabled())
                    {
                        ref var local = ref point;
                        var end = containingBufferPosition.End;
                        insertionPoint = _selection.InsertionPoint;
                        var virtualSpaces = insertionPoint.VirtualSpaces + 1;
                        local = new VirtualSnapshotPoint(end, virtualSpaces);
                    }
                    else
                    {
                        insertionPoint = _selection.InsertionPoint;
                        point = insertionPoint.Position == _broker.TextView.TextSnapshot.Length ? _selection.InsertionPoint : new VirtualSnapshotPoint(containingBufferPosition.EndIncludingLineBreak);
                    }
                }
                else
                {
                    var textView = _broker.TextView;
                    insertionPoint = _selection.InsertionPoint;
                    var position = insertionPoint.Position;
                    point = new VirtualSnapshotPoint(textView.GetTextElementSpan(position).End);
                }
                MoveTo(point, select, PositionAffinity.Successor);
                CapturePreferredReferencePoint();
            }
        }

        private void MoveToNextLine(bool select)
        {
            ITextViewLine containingBufferPosition;
            if (_selection.IsEmpty | select)
            {
                containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(_selection.InsertionPoint.Position);
            }
            else
            {
                var position = _selection.End.Position;
                containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(position);
                if (!containingBufferPosition.IsFirstTextViewLineForSnapshotLine && position.Position == containingBufferPosition.Start.Position)
                    containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(position - 1);
            }
            if (!containingBufferPosition.IsLastTextViewLineForSnapshotLine || containingBufferPosition.LineBreakLength != 0)
                containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(containingBufferPosition.EndIncludingLineBreak);
            MoveTo(GetPreferredXLocationOnLine(containingBufferPosition), select, PositionAffinity.Successor);
            CapturePreferredYReferencePoint();
        }

        private void MoveToNextWord(bool select)
        {
            MoveTo(new VirtualSnapshotPoint(GetNextWord().Start), select, PositionAffinity.Successor);
            CapturePreferredReferencePoint();
        }

        private void SelectCurrentWord()
        {
            var extentOfWord = _broker.TextStructureNavigator.GetExtentOfWord(Selection.InsertionPoint.Position);
            var anchorPoint = new VirtualSnapshotPoint(extentOfWord.Span.Start);
            var virtualSnapshotPoint = new VirtualSnapshotPoint(extentOfWord.Span.End);
            MoveTo(anchorPoint, virtualSnapshotPoint, virtualSnapshotPoint, anchorPoint == virtualSnapshotPoint ? PositionAffinity.Successor : PositionAffinity.Predecessor);
        }

        private void MoveToPreviousCaretPosition(bool select)
        {
            var insertionPoint1 = _selection.InsertionPoint;
            var num1 = insertionPoint1.IsInVirtualSpace ? 1 : 0;
            var textView1 = _broker.TextView;
            insertionPoint1 = _selection.InsertionPoint;
            var position1 = insertionPoint1.Position;
            var containingBufferPosition = textView1.GetTextViewLineContainingBufferPosition(position1);
            VirtualSnapshotPoint insertionPoint2;
            int num2;
            if (num1 == 0)
            {
                insertionPoint2 = _selection.InsertionPoint;
                num2 = insertionPoint2.Position == containingBufferPosition.Start ? 1 : 0;
            }
            else
                num2 = 0;
            if (num2 != 0 && _broker.TextView.IsVirtualSpaceOrBoxSelectionEnabled())
                return;
            if (!select && !_selection.IsEmpty)
            {
                MoveTo(_selection.Start, false, PositionAffinity.Successor);
                CapturePreferredReferencePoint();
            }
            else
            {
                insertionPoint2 = _selection.InsertionPoint;
                VirtualSnapshotPoint point1;
                if (insertionPoint2.IsInVirtualSpace)
                {
                    var textSnapshot = _broker.TextView.TextSnapshot;
                    insertionPoint2 = _selection.InsertionPoint;
                    int position2 = insertionPoint2.Position;
                    var lineFromPosition = textSnapshot.GetLineFromPosition(position2);
                    int num3;
                    if (!_broker.TextView.IsVirtualSpaceOrBoxSelectionEnabled())
                    {
                        num3 = 0;
                    }
                    else
                    {
                        insertionPoint2 = _selection.InsertionPoint;
                        num3 = insertionPoint2.VirtualSpaces - 1;
                    }
                    var virtualSpaces = num3;
                    point1 = new VirtualSnapshotPoint(lineFromPosition.End, virtualSpaces);
                }
                else
                {
                    insertionPoint2 = _selection.InsertionPoint;
                    if (insertionPoint2.Position.Position != 0)
                    {
                        var textView2 = _broker.TextView;
                        insertionPoint2 = _selection.InsertionPoint;
                        var point2 = insertionPoint2.Position - 1;
                        point1 = new VirtualSnapshotPoint(textView2.GetTextElementSpan(point2).Start);
                    }
                    else
                        point1 = _selection.InsertionPoint;
                }
                MoveTo(point1, select, PositionAffinity.Successor);
                CapturePreferredReferencePoint();
            }
        }

        private void MoveToPreviousLine(bool select)
        {
            var line = !(_selection.IsEmpty | select) ? _broker.TextView.GetTextViewLineContainingBufferPosition(_broker.TextView.Selection.Start.Position) : _broker.TextView.GetTextViewLineContainingBufferPosition(_selection.InsertionPoint.Position);
            if (line.Start != 0)
                line = _broker.TextView.GetTextViewLineContainingBufferPosition(line.Start - 1);
            MoveTo(GetPreferredXLocationOnLine(line), select, PositionAffinity.Successor);
            CapturePreferredYReferencePoint();
        }

        private void MoveToPreviousWord(bool select)
        {
            var previousWord = GetPreviousWord();
            var start = previousWord.Start;
            if (_selection == _broker.BoxSelection)
            {
                var containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(_selection.InsertionPoint.Position);
                if (previousWord.End < containingBufferPosition.Start)
                    start = containingBufferPosition.Start;
            }
            MoveTo(new VirtualSnapshotPoint(start), select, PositionAffinity.Successor);
            CapturePreferredReferencePoint();
        }

        private void MoveToStartOfDocument(bool select)
        {
            MoveTo(new VirtualSnapshotPoint(_broker.CurrentSnapshot, 0), select, PositionAffinity.Successor);
            CapturePreferredReferencePoint();
        }

        private void CheckIsValid()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(SelectionTransformer), "Using a selection transformer that is not associated with a selection.");
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
