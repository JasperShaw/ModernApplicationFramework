using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Operations;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class SelectionMouseProcessor : MouseProcessorBase, IMouseProcessor2
    {
        private readonly Stopwatch _doubleTapStopWatch = new Stopwatch();
        private readonly IEditorOperations _editorOperations;
        private readonly IViewPrimitives _editorPrimitives;
        private readonly TimeSpan _maximumElapsedDoubleTap = new TimeSpan(0, 0, 0, 0, 600);
        private readonly ITextView _textView;
        private Point _currentTapPosition;
        private bool _doingLineSelection;
        private bool _doingWordSelection;

        private TimeSpan _elapsedSinceLastTap;
        private int _ignoreSelectionChangedEvents;
        private Point _lastTapPosition;
        private ITrackingSpan _originalSelectedLine;
        private ITrackingSpan _originalSelectedWord;

        public SelectionMouseProcessor(ITextView textView, IEditorOperationsFactoryService editorOperationsProvider,
            IEditorPrimitivesFactoryService editorPrimitivesFactory)
        {
            if (editorOperationsProvider == null)
                throw new ArgumentNullException(nameof(editorOperationsProvider));
            if (editorPrimitivesFactory == null)
                throw new ArgumentNullException(nameof(editorPrimitivesFactory));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _textView.Selection.SelectionChanged += SelectionChanged;
            _originalSelectedWord = null;
            _originalSelectedLine = null;
            _editorOperations = editorOperationsProvider.GetEditorOperations(textView);
            _editorPrimitives = editorPrimitivesFactory.GetViewPrimitives(_textView);
            _ignoreSelectionChangedEvents = 0;
        }

        public void HandleLeftButtonDown(InputEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            var shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            var control = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            switch (GetClickCount(e))
            {
                case 1:
                    e.Handled = HandleSingleClick(e, shift, control);
                    break;
                case 2:
                    e.Handled = HandleDoubleClick(e, shift, control);
                    break;
                case 3:
                    e.Handled = HandleTripleClick(e, shift, control);
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        public void PostprocessManipulationCompleted(ManipulationCompletedEventArgs e)
        {
        }

        public void PostprocessManipulationDelta(ManipulationDeltaEventArgs e)
        {
        }

        public void PostprocessManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
        }

        public void PostprocessManipulationStarting(ManipulationStartingEventArgs e)
        {
        }

        public override void PostprocessMouseUp(MouseButtonEventArgs e)
        {
            _doingLineSelection = false;
            _doingWordSelection = false;
            _originalSelectedLine = null;
            _originalSelectedWord = null;
            _lastTapPosition = GetAdjustedPosition(e, _textView);
            _doubleTapStopWatch.Restart();
        }

        public void PostprocessStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
        }

        public void PostprocessTouchDown(TouchEventArgs e)
        {
        }

        public void PostprocessTouchUp(TouchEventArgs e)
        {
            _doingLineSelection = false;
            _doingWordSelection = false;
            _originalSelectedLine = null;
            _originalSelectedWord = null;
        }

        public void PreprocessManipulationCompleted(ManipulationCompletedEventArgs e)
        {
        }

        public void PreprocessManipulationDelta(ManipulationDeltaEventArgs e)
        {
        }

        public void PreprocessManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
        }

        public void PreprocessManipulationStarting(ManipulationStartingEventArgs e)
        {
        }

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            HandleLeftButtonDown(e);
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            e.Handled = PreprocessMouseMoveByPosition(GetAdjustedPosition(e, _textView), e.LeftButton);
        }

        public void PreprocessStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
        }

        public void PreprocessTouchDown(TouchEventArgs e)
        {
            _currentTapPosition = GetAdjustedPosition(e, _textView);
            _elapsedSinceLastTap = _doubleTapStopWatch.Elapsed;
            _doubleTapStopWatch.Restart();
            HandleLeftButtonDown(e);
            _lastTapPosition = _currentTapPosition;
        }

        public void PreprocessTouchUp(TouchEventArgs e)
        {
        }


        internal int GetClickCount(InputEventArgs e)
        {
            var num = 1;
            if (e is MouseButtonEventArgs args)
            {
                num = args.ClickCount;
            }
            else if (e is TouchEventArgs)
            {
                num = 1;
                var flag = Math.Abs(_currentTapPosition.X - _lastTapPosition.X) < 30.0 &&
                           Math.Abs(_currentTapPosition.Y - _lastTapPosition.Y) < 30.0;
                if (((!(_elapsedSinceLastTap != TimeSpan.Zero)
                         ? 0
                         : (_elapsedSinceLastTap < _maximumElapsedDoubleTap ? 1 : 0)) & (flag ? 1 : 0)) != 0)
                    num = 2;
            }

            return num;
        }

        internal bool HandleDoubleClick(InputEventArgs e, bool shift, bool control)
        {
            if (shift | control)
                return HandleSingleClick(e, shift, control);
            SelectWordUnderCaret();
            _doingWordSelection = true;
            return true;
        }

        internal bool HandleSingleClick(InputEventArgs e, bool shift, bool control)
        {
            if (control)
                return HandleSingleControlClick(GetAdjustedPosition(e, _textView), shift);
            return false;
        }

        internal bool HandleSingleControlClick(Point pt, bool shift)
        {
            var positionFromPoint = GetBufferPositionFromPoint(pt);
            if (!positionFromPoint.HasValue)
                return false;
            if (shift)
            {
                ExtendWordSelection(positionFromPoint.Value);
                _doingWordSelection = true;
                return true;
            }

            SelectWordAtBufferPosition(positionFromPoint.Value);
            _doingWordSelection = true;
            return true;
        }

        internal bool HandleTripleClick(InputEventArgs e, bool shift, bool control)
        {
            if (shift | control)
                return HandleSingleClick(e, shift, control);
            var viewLineUnderPoint = GetTextViewLineUnderPoint(GetAdjustedPosition(e, _textView));
            if (viewLineUnderPoint == null)
                return false;
            SelectLine(viewLineUnderPoint);
            _doingLineSelection = true;
            return true;
        }

        internal bool PreprocessMouseMoveByPosition(Point pt, MouseButtonState leftButtonState)
        {
            if (leftButtonState == MouseButtonState.Released)
            {
                _doingWordSelection = false;
                _doingLineSelection = false;
            }

            if (!_doingWordSelection && !_doingLineSelection)
                return false;
            var positionFromPoint = GetBufferPositionFromPoint(pt);
            if (!positionFromPoint.HasValue)
                return false;
            if (_doingLineSelection)
                return ExtendLineSelection(pt);
            ExtendWordSelection(positionFromPoint.Value);
            return true;
        }

        private static Point GetAdjustedPosition(InputEventArgs e, ITextView view)
        {
            var point = new Point(0.0, 0.0);
            if (e is MouseEventArgs args)
                point = args.GetPosition(view.VisualElement);
            else if (e is TouchEventArgs eventArgs)
                point = eventArgs.GetTouchPoint(view.VisualElement).Position;
            point.X += view.ViewportLeft;
            point.Y += view.ViewportTop;
            return point;
        }

        private bool ExtendLineSelection(Point ptMousePosition)
        {
            try
            {
                ++_ignoreSelectionChangedEvents;
                if (_originalSelectedLine == null)
                {
                    _doingLineSelection = false;
                    return false;
                }

                var viewLineUnderPoint = GetTextViewLineUnderPoint(ptMousePosition);
                if (viewLineUnderPoint == null)
                    return false;
                var includingLineBreak = viewLineUnderPoint.ExtentIncludingLineBreak;
                var position1 = Math.Min(includingLineBreak.Start,
                    _originalSelectedLine.GetStartPoint(_textView.TextSnapshot));
                var position2 = Math.Max(includingLineBreak.End,
                    _originalSelectedLine.GetEndPoint(_textView.TextSnapshot));
                TextPoint textPoint1 = _editorPrimitives.View.GetTextPoint(position1);
                TextPoint textPoint2 = _editorPrimitives.View.GetTextPoint(position2);
                _textView.Selection.Mode = TextSelectionMode.Stream;
                if (!viewLineUnderPoint.Start.Position.Equals(position1))
                    _editorPrimitives.Selection.SelectRange(textPoint1, textPoint2);
                else
                    _editorPrimitives.Selection.SelectRange(textPoint2, textPoint1);
                return true;
            }
            finally
            {
                --_ignoreSelectionChangedEvents;
            }
        }

        private void ExtendWordSelection(SnapshotPoint clickPosition)
        {
            try
            {
                ++_ignoreSelectionChangedEvents;
                if (_originalSelectedWord == null)
                    SelectWordAtBufferPosition(_textView.Selection.AnchorPoint.Position);
                var span = _originalSelectedWord.GetSpan(_textView.TextSnapshot);
                var position1 = Math.Min(span.Start.Position, clickPosition.Position);
                var position2 = Math.Max(span.End.Position, clickPosition.Position);
                var num1 = clickPosition.Position == position1 ? 1 : 0;
                var flag1 = clickPosition.Position < span.Start.Position;
                var flag2 = clickPosition.Position > span.End.Position;
                TextPoint textPoint1 = _editorPrimitives.View.GetTextPoint(position1);
                TextPoint textPoint2 = _editorPrimitives.View.GetTextPoint(position2);
                var currentWord1 = textPoint1.GetCurrentWord();
                var currentWord2 = textPoint2.GetCurrentWord();
                var flag3 = currentWord1.GetEndPoint().CurrentPosition != textPoint1.EndOfLine &&
                            (textPoint1.CurrentPosition == currentWord1.GetStartPoint().CurrentPosition ||
                             textPoint1.CurrentPosition == textPoint1.GetNextWord().GetStartPoint().CurrentPosition);
                var num2 = currentWord2.GetStartPoint().CurrentPosition == textPoint2.StartOfLine
                    ? 0
                    : (textPoint2.CurrentPosition == currentWord2.GetEndPoint().CurrentPosition
                        ? 1
                        : (textPoint2.CurrentPosition == textPoint2.GetPreviousWord().GetEndPoint().CurrentPosition
                            ? 1
                            : 0));
                if (((textPoint1.CurrentPosition == textPoint1.EndOfLine ? 0 : (!flag3 ? 1 : 0)) & (flag1 ? 1 : 0)) !=
                    0)
                    textPoint1 = currentWord1.GetStartPoint();
                var num3 = 0;
                if ((num2 == num3) & flag2)
                    textPoint2 = currentWord2.GetEndPoint();
                _textView.Selection.Mode = TextSelectionMode.Stream;
                if (num1 == 0)
                    _editorPrimitives.Selection.SelectRange(textPoint1, textPoint2);
                else
                    _editorPrimitives.Selection.SelectRange(textPoint2, textPoint1);
            }
            finally
            {
                --_ignoreSelectionChangedEvents;
            }
        }

        private SnapshotPoint? GetBufferPositionFromPoint(Point pt)
        {
            return GetTextViewLineUnderPoint(pt)?.GetInsertionBufferPositionFromXCoordinate(pt.X).Position;
        }

        private ITextViewLine GetTextViewLineUnderPoint(Point pt)
        {
            return _textView.TextViewLines.GetTextViewLineContainingYCoordinate(pt.Y);
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            if (_ignoreSelectionChangedEvents != 0)
                return;
            _doingWordSelection = false;
            _doingLineSelection = false;
            _originalSelectedWord = null;
            _originalSelectedLine = null;
        }

        private void SelectLine(ITextViewLine line)
        {
            try
            {
                ++_ignoreSelectionChangedEvents;
                _textView.Selection.Mode = TextSelectionMode.Stream;
                _editorOperations.SelectLine(line, false);
                _originalSelectedLine = _textView.TextSnapshot.CreateTrackingSpan(
                    _textView.Selection.StreamSelectionSpan.SnapshotSpan, SpanTrackingMode.EdgeExclusive);
            }
            finally
            {
                --_ignoreSelectionChangedEvents;
            }
        }

        private void SelectWordAtBufferPosition(SnapshotPoint clickPosition)
        {
            try
            {
                ++_ignoreSelectionChangedEvents;
                var virtualSnapshotPoint = new VirtualSnapshotPoint(clickPosition);
                _editorOperations.SelectAndMoveCaret(virtualSnapshotPoint, virtualSnapshotPoint);
                SelectWordUnderCaret();
            }
            finally
            {
                --_ignoreSelectionChangedEvents;
            }
        }

        private void SelectWordUnderCaret()
        {
            try
            {
                ++_ignoreSelectionChangedEvents;
                _textView.Selection.Mode = TextSelectionMode.Stream;
                _editorOperations.SelectCurrentWord();
                _originalSelectedWord = _textView.TextSnapshot.CreateTrackingSpan(
                    _textView.Selection.StreamSelectionSpan.SnapshotSpan, SpanTrackingMode.EdgeExclusive);
            }
            finally
            {
                --_ignoreSelectionChangedEvents;
            }
        }
    }
}