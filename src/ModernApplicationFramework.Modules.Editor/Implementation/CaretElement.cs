using System;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Modules.Editor.MultiSelection;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;
using Selection = ModernApplicationFramework.Text.Ui.Text.Selection;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class CaretElement : UIElement, ITextCaret
    {
        private readonly TextView _textView;
        private readonly ISmartIndentationService _smartIndentationService;
        private readonly GuardedOperations _guardedOperations;
        private readonly IMultiSelectionBroker _multiSelectionBroker;
        private CaretPosition _oldPosition;
        private bool _caretGeometryNeedsToBeUpdated = true;
        private bool _forceVirtualSpace;
        private bool _emptySelection;
        private bool _isClosed;

        public event EventHandler<CaretPositionChangedEventArgs> PositionChanged;
        public double Bottom {
            get
            {
                var uiProperties = UiProperties;
                if (uiProperties == null)
                    return 0.0;
                return uiProperties.CaretBounds.TextBottom;
            }
        }
        public ITextViewLine ContainingTextViewLine => UiProperties.ContainingTextViewLine;

        public double Height
        {
            get
            {
                var uiProperties = UiProperties;
                if (uiProperties == null)
                    return 0.0;
                return uiProperties.CaretBounds.TextHeight;
            }
        }
        public bool InVirtualSpace => _multiSelectionBroker.PrimarySelection.InsertionPoint.IsInVirtualSpace;
        public bool IsHidden
        {
            get => !_textView.Options.GetOptionValue(DefaultTextViewOptions.ShouldCaretsBeRenderedId);
            set => _textView.Options.SetOptionValue(DefaultTextViewOptions.ShouldCaretsBeRenderedId, !value);
        }
        public double Left
        {
            get
            {
                var uiProperties = UiProperties;
                if (uiProperties == null)
                    return 0.0;
                return uiProperties.CaretBounds.Left;
            }
        }
        public bool OverwriteMode => UiProperties.IsOverwriteMode;

        public CaretPosition Position
        {
            get
            {
                var primarySelection = _multiSelectionBroker.PrimarySelection;
                var insertionPoint = primarySelection.InsertionPoint;
                var bufferGraph = _textView.BufferGraph;
                primarySelection = _multiSelectionBroker.PrimarySelection;
                var position = primarySelection.InsertionPoint.Position;
                var num = 0;
                var mappingPoint = bufferGraph.CreateMappingPoint(position, (PointTrackingMode)num);
                primarySelection = _multiSelectionBroker.PrimarySelection;
                var insertionPointAffinity = (int)primarySelection.InsertionPointAffinity;
                return new CaretPosition(insertionPoint, mappingPoint, (PositionAffinity)insertionPointAffinity);
            }
        }
        public double Right
        {
            get
            {
                var uiProperties = UiProperties;
                if (uiProperties == null)
                    return 0.0;
                return uiProperties.CaretBounds.Right;
            }
        }
        public double Top
        {
            get
            {
                var uiProperties = UiProperties;
                if (uiProperties == null)
                    return 0.0;
                return uiProperties.CaretBounds.TextTop;
            }
        }
        public double Width
        {
            get
            {
                var uiProperties = UiProperties;
                if (uiProperties == null)
                    return 0.0;
                return uiProperties.CaretBounds.Width;
            }
        }

        public double PreferredYCoordinate => UiProperties.PreferredYCoordinate;

        public bool IsShownOnScreen
        {
            get
            {
                var uiProperties = UiProperties;
                return uiProperties != null && uiProperties.IsWithinViewport;
            }
        }

        internal double PreferredXCoordinate
        {
            get
            {
                var uiProperties = UiProperties;
                if (uiProperties == null)
                    return 0.0;
                return uiProperties.PreferredXCoordinate;
            }
        }

        private AbstractSelectionPresentationProperties UiProperties
        {
            get
            {
                if (_multiSelectionBroker.TryGetSelectionPresentationProperties(_multiSelectionBroker.PrimarySelection, out var properties))
                    return properties;
                return null;
            }
        }

        private bool IsVirtualSpaceOrBoxSelectionEnabled
        {
            get
            {
                if (!_textView.Options.IsVirtualSpaceEnabled())
                    return _textView.Selection.Mode == TextSelectionMode.Box;
                return true;
            }
        }

        public CaretElement(TextView textView, ISmartIndentationService smartIndentationService, GuardedOperations guardedOperations, IMultiSelectionBroker multiSelectionBroker)
        {
            _textView = textView;
            _smartIndentationService = smartIndentationService;
            _guardedOperations = guardedOperations;
            _multiSelectionBroker = multiSelectionBroker;
            SubscribeEvents();
            _oldPosition = Position;
            IsHitTestVisible = false;
            _multiSelectionBroker.MultiSelectionSessionChanged += OnMultiSelectionSessionChanged;
        }

        public void EnsureVisible()
        {
            var primarySelection = _multiSelectionBroker.PrimarySelection;
            _textView.ViewScroller.EnsureSpanVisible(new VirtualSnapshotSpan(primarySelection.InsertionPoint, primarySelection.InsertionPoint), EnsureSpanVisibleOptions.MinimumScroll);
        }

        public CaretPosition MoveTo(ITextViewLine textLine, double xCoordinate)
        {
            return MoveTo(textLine, xCoordinate, true);
        }

        public CaretPosition MoveTo(ITextViewLine textLine, double xCoordinate, bool captureHorizontalPosition)
        {
            if (textLine == null)
                throw new ArgumentNullException(nameof(textLine));
            if (double.IsNaN(xCoordinate))
                throw new ArgumentOutOfRangeException(nameof(xCoordinate));
            xCoordinate = textLine.MapXCoordinate(_textView, xCoordinate, _smartIndentationService, true);
            InternalMoveCaretToTextViewLine(textLine, xCoordinate, true, captureHorizontalPosition, true);
            return Position;
        }

        public CaretPosition MoveTo(ITextViewLine textLine)
        {
            if (textLine == null)
                throw new ArgumentNullException(nameof(textLine));
            var xCoordinate = textLine.MapXCoordinate(_textView, PreferredXCoordinate, _smartIndentationService, false);
            InternalMoveCaretToTextViewLine(textLine, xCoordinate, true, false, true);
            return Position;
        }

        public CaretPosition MoveTo(SnapshotPoint bufferPosition)
        {
            InternalMoveTo(new VirtualSnapshotPoint(bufferPosition), PositionAffinity.Successor, true, true);
            return Position;
        }

        public CaretPosition MoveTo(SnapshotPoint bufferPosition, PositionAffinity caretAffinity)
        {
            InternalMoveTo(new VirtualSnapshotPoint(bufferPosition), caretAffinity, true, true);
            return Position;
        }

        public CaretPosition MoveTo(SnapshotPoint bufferPosition, PositionAffinity caretAffinity,
            bool captureHorizontalPosition)
        {
            InternalMoveTo(new VirtualSnapshotPoint(bufferPosition), caretAffinity, captureHorizontalPosition, true);
            return Position;
        }

        public CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition)
        {
            InternalMoveTo(bufferPosition, PositionAffinity.Successor, true, true);
            return Position;
        }

        public CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity)
        {
            InternalMoveTo(bufferPosition, caretAffinity, true, true);
            return Position;
        }

        public CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity,
            bool captureHorizontalPosition)
        {
            InternalMoveTo(bufferPosition, caretAffinity, captureHorizontalPosition, true);
            return Position;
        }

        public CaretPosition MoveToNextCaretPosition()
        {
            var position = Position;
            var containingBufferPosition = _textView.GetTextViewLineContainingBufferPosition(position.BufferPosition);
            if (!(position.BufferPosition == containingBufferPosition.End) || !containingBufferPosition.IsLastTextViewLineForSnapshotLine)
                return MoveTo(_textView.GetTextElementSpan(position.BufferPosition).End, PositionAffinity.Successor, true);
            if (IsVirtualSpaceOrBoxSelectionEnabled)
                return MoveTo(new VirtualSnapshotPoint(containingBufferPosition.End, position.VirtualSpaces + 1));
            if (position.BufferPosition == _textView.TextSnapshot.Length)
                return position;
            return MoveTo(containingBufferPosition.EndIncludingLineBreak, PositionAffinity.Successor, true);
        }

        public CaretPosition MoveToPreferredCoordinates()
        {
            var textLine = _textView.TextViewLines.GetTextViewLineContainingYCoordinate(PreferredYCoordinate) ?? _textView.TextViewLines.LastVisibleLine;
            var xCoordinate = textLine.MapXCoordinate(_textView, PreferredXCoordinate, _smartIndentationService, false);
            InternalMoveCaretToTextViewLine(textLine, xCoordinate, IsVirtualSpaceOrBoxSelectionEnabled, false, false);
            return Position;
        }

        public void Close()
        {
            if (_isClosed)
                return;
            _isClosed = true;
            UnsubscribeEvents();
        }

        public CaretPosition MoveToPreviousCaretPosition()
        {
            var position = Position;
            if (position.VirtualSpaces > 0)
                return MoveTo(new VirtualSnapshotPoint(_textView.TextSnapshot.GetLineFromPosition(position.BufferPosition).End, IsVirtualSpaceOrBoxSelectionEnabled ? position.VirtualSpaces - 1 : 0));
            if (position.BufferPosition == 0)
                return position;
            return MoveTo(_textView.GetTextElementSpan(position.BufferPosition - 1).Start, PositionAffinity.Successor, true);
        }

        internal static VirtualSnapshotPoint NormalizePosition(VirtualSnapshotPoint bufferPosition, ITextViewLine textLine)
        {
            if ((bufferPosition.IsInVirtualSpace ? (!textLine.IsLastTextViewLineForSnapshotLine ? 1 : (bufferPosition.Position != textLine.End ? 1 : 0)) : (bufferPosition.Position != textLine.Start ? 1 : 0)) != 0)
                bufferPosition = new VirtualSnapshotPoint(bufferPosition.Position < textLine.End ? textLine.GetTextElementSpan(bufferPosition.Position).Start : textLine.End);
            return bufferPosition;
        }

        private void InternalMoveCaretToTextViewLine(ITextViewLine textLine, double xCoordinate, bool allowPlacementInVirtualSpace, bool captureHorizontalPosition, bool captureVerticalPosition)
        {
            var bufferPosition = textLine.GetInsertionBufferPositionFromXCoordinate(xCoordinate);
            if (!allowPlacementInVirtualSpace)
                bufferPosition = new VirtualSnapshotPoint(bufferPosition.Position);
            var caretAffinity = textLine.IsLastTextViewLineForSnapshotLine || !(bufferPosition.Position == textLine.End) ? PositionAffinity.Successor : PositionAffinity.Predecessor;
            InternalMoveCaret(bufferPosition, caretAffinity, captureHorizontalPosition, captureVerticalPosition);
        }

        private void RefreshCaret(bool preserveCoordinates, bool clearVirtualSpace = false)
        {
            var bufferPosition = Position.VirtualBufferPosition;
            if (clearVirtualSpace && !_forceVirtualSpace)
                bufferPosition = new VirtualSnapshotPoint(bufferPosition.Position);
            InternalMoveTo(bufferPosition, Position.Affinity, preserveCoordinates, preserveCoordinates, false);
        }

        private void InternalMoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity, bool captureHorizontalPosition, bool captureVerticalPosition, bool clearSecondaryCarets = true)
        {
            if (bufferPosition.Position.Snapshot != _textView.TextSnapshot)
                throw new ArgumentException();
            var containingTextViewLine = GetContainingTextViewLine(bufferPosition.Position, caretAffinity);
            InternalMoveCaret(NormalizePosition(bufferPosition, containingTextViewLine), caretAffinity, captureHorizontalPosition, captureVerticalPosition, clearSecondaryCarets);
        }

        private ITextViewLine GetContainingTextViewLine(SnapshotPoint bufferPosition, PositionAffinity caretAffinity)
        {
            var containingBufferPosition = _textView.GetTextViewLineContainingBufferPosition(bufferPosition);
            if (caretAffinity == PositionAffinity.Predecessor && containingBufferPosition.Start == bufferPosition && _textView.TextSnapshot.GetLineFromPosition(bufferPosition).Start != bufferPosition)
                containingBufferPosition = _textView.GetTextViewLineContainingBufferPosition(bufferPosition - 1);
            return containingBufferPosition;
        }

        private void InternalMoveCaret(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity, bool captureHorizontalPosition, bool captureVerticalPosition, bool clearSecondaryCarets = true)
        {
            using (_multiSelectionBroker.BeginBatchOperation())
            {
                Selection before;
                if (_multiSelectionBroker.IsBoxSelection)
                {
                    before = _multiSelectionBroker.BoxSelection;
                }
                else
                {
                    if (clearSecondaryCarets)
                        _multiSelectionBroker.ClearSecondarySelections();
                    before = _multiSelectionBroker.PrimarySelection;
                }
                _multiSelectionBroker.TryPerformActionOnSelection(before, transformer =>
                {
                    var activePoint = bufferPosition;
                    var anchorPoint = bufferPosition;
                    if (!transformer.Selection.IsEmpty)
                    {
                        activePoint = transformer.Selection.ActivePoint;
                        anchorPoint = transformer.Selection.AnchorPoint;
                    }
                    transformer.MoveTo(anchorPoint, activePoint, bufferPosition, caretAffinity);
                    if (captureHorizontalPosition)
                        transformer.CapturePreferredXReferencePoint();
                    if (!captureVerticalPosition)
                        return;
                    transformer.CapturePreferredYReferencePoint();
                }, out _);
                _forceVirtualSpace = _multiSelectionBroker.PrimarySelection.InsertionPoint.IsInVirtualSpace && !IsVirtualSpaceOrBoxSelectionEnabled;
                _emptySelection = _textView.Selection.IsEmpty;
            }
        }

        private void UnsubscribeEvents()
        {
            IsVisibleChanged -= OnVisibleChanged;
            _textView.Options.OptionChanged -= OnOptionsChanged;
            _textView.Selection.SelectionChanged -= OnSelectionChanged;
            InputLanguageManager.Current.InputLanguageChanged -= OnInputLanguageChanged;
        }

        private void SubscribeEvents()
        {
            IsVisibleChanged += OnVisibleChanged;
            _textView.Options.OptionChanged += OnOptionsChanged;
            _textView.Selection.SelectionChanged += OnSelectionChanged;
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                InputLanguageManager.Current.InputLanguageChanged += OnInputLanguageChanged;
                InvalidateVisual();
                _caretGeometryNeedsToBeUpdated = true;
            }
            else
                InputLanguageManager.Current.InputLanguageChanged -= OnInputLanguageChanged;
        }

        private void OnOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (string.Equals(e.OptionId, DefaultTextViewOptions.OverwriteModeId.Name, StringComparison.Ordinal))
            {
                RefreshCaret(false);
            }
            else
            {
                if (!string.Equals(e.OptionId, DefaultTextViewOptions.UseVirtualSpaceId.Name, StringComparison.Ordinal) || IsVirtualSpaceOrBoxSelectionEnabled)
                    return;
                RefreshCaret(false, true);
            }
        }

        private void OnInputLanguageChanged(object sender, InputLanguageEventArgs e)
        {
            InvalidateVisual();
            _caretGeometryNeedsToBeUpdated = true;
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!_textView.Options.IsOverwriteModeEnabled() || _textView.Selection.IsEmpty == _emptySelection)
                return;
            RefreshCaret(false);
        }

        private void OnMultiSelectionSessionChanged(object sender, EventArgs e)
        {
            var position = Position;
            var oldPosition = _oldPosition;
            _oldPosition = position;
            if (!(oldPosition.VirtualBufferPosition.TranslateTo(position.VirtualBufferPosition.Position.Snapshot) != position.VirtualBufferPosition))
                return;
            var positionChanged = PositionChanged;
            if (positionChanged == null)
                return;
            _textView.BufferGraph.CreateMappingPoint(oldPosition.VirtualBufferPosition.Position, PointTrackingMode.Positive);
            _guardedOperations.RaiseEvent(this, positionChanged, new CaretPositionChangedEventArgs(_textView, oldPosition, Position));
        }
    }
}