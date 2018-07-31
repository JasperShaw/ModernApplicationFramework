using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    internal class CaretElement : UIElement, ITextCaret
    {
        private double _newOpacity = 1.0;
        private bool _updateNeeded = true;
        internal bool CaretGeometryNeedsToBeUpdated = true;
        internal Brush CaretBrush;
        internal Brush RegularBrush;
        internal Brush OverwriteBrush;
        internal Brush DefaultOverwriteBrush;
        internal Brush DefaultRegularBrush;
        private readonly DispatcherTimer _blinkTimer;
        private int _blinkInterval;
        private AccessibleCaret _accessibleCaret;
        private VirtualSnapshotPoint _insertionPoint;
        private PositionAffinity _caretAffinity;
        private readonly TextView _textView;
        private readonly TextSelection _selection;
        private readonly GuardedOperations _guardedOperations;
        internal double PreferredXCoordinate;
        private double _preferredYOffset;
        private double _displayedHeight;
        private double _displayedWidth;
        private bool _isClosed;
        private bool _emptySelection;
        private bool _isHidden;
        private bool _forceVirtualSpace;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly IClassificationFormatMap _classificationFormatMap;
        internal Win32Caret _win32Caret;
        internal Rect Bounds;
        internal Geometry CaretGeometry;
        internal bool IsContainedByView;
        internal ISmartIndentationService SmartIndentationService;
        private bool _overwriteMode;
        public const double CaretHorizontalPadding = 2.0;
        public const double HorizontalScrollbarPadding = 200.0;

        public CaretElement(TextView textView, TextSelection selection, ISmartIndentationService smartIndentationService, 
            IEditorFormatMap editorFormatMap, IClassificationFormatMap classificationFormatMap, GuardedOperations guardedOperations)
        {
            _textView = textView;
            _selection = selection;
            _guardedOperations = guardedOperations;
            SmartIndentationService = smartIndentationService;
            _caretAffinity = PositionAffinity.Successor;
            _insertionPoint = new VirtualSnapshotPoint(new SnapshotPoint(_textView.TextSnapshot, 0));
            _editorFormatMap = editorFormatMap;
            _classificationFormatMap = classificationFormatMap;
            SubscribeEvents();
            UpdateDefaultBrushes();
            UpdateRegularCaretBrush();
            UpdateOverwriteCaretBrush();
            CaretBrush = RegularBrush;
            IsHitTestVisible = false;
            _blinkInterval = CaretBlinkTimeManager.GetCaretBlinkTime();
            if (_blinkInterval > 0)
                _blinkTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, _blinkInterval), DispatcherPriority.Normal, OnTimerElapsed, Dispatcher);
            UpdateBlinkTimer();
            _win32Caret = new Win32Caret(this, _textView);
        }

        public void EnsureVisible()
        {
            _textView.DoActionThatShouldOnlyBeDoneAfterViewIsLoaded(InnerEnsureVisible);
        }

        private void InnerEnsureVisible()
        {
            if (_textView.IsClosed)
                return;
            var containingTextViewLine1 = GetContainingTextViewLine(_insertionPoint.Position, _caretAffinity);
            if (containingTextViewLine1.VisibilityState != VisibilityState.FullyVisible)
            {
                var start = containingTextViewLine1.Start;
                var nullable = new ViewRelativePosition?();
                if (containingTextViewLine1.VisibilityState != VisibilityState.Unattached)
                {
                    if (containingTextViewLine1.Height <= _textView.ViewportHeight + 0.01)
                        nullable = containingTextViewLine1.Top >= _textView.ViewportTop ? ViewRelativePosition.Bottom : ViewRelativePosition.Top;
                    else if (containingTextViewLine1.Bottom < _textView.ViewportBottom)
                        nullable = ViewRelativePosition.Bottom;
                    else if (containingTextViewLine1.Top > _textView.ViewportTop)
                        nullable = ViewRelativePosition.Top;
                }
                else
                    nullable = start < _textView.TextViewLines.FormattedSpan.Start == containingTextViewLine1.Height <= _textView.ViewportHeight + 0.01 ? ViewRelativePosition.Top : ViewRelativePosition.Bottom;
                if (nullable.HasValue)
                {
                    _textView.DisplayTextLineContainingBufferPosition(start, 0.0, nullable.Value);
                }
            }
            var num = Math.Max(2.0, Math.Min(200.0, _textView.ViewportWidth / 4.0));
            if (_textView.ViewportWidth == 0.0)
                _textView.ViewportLeft = 0.0;
            else if (Bounds.Left - 2.0 < _textView.ViewportLeft)
            {
                _textView.ViewportLeft = Bounds.Left - num;
            }
            else
            {
                if (Bounds.Right + 2.0 <= _textView.ViewportRight)
                    return;
                _textView.ViewportLeft = Bounds.Right + num - _textView.ViewportWidth;
            }
        }

        public CaretPosition MoveToPreferredCoordinates()
        {
            var textLine = _textView.TextViewLines.GetTextViewLineContainingYCoordinate(PreferredYCoordinate) ?? _textView.TextViewLines.LastVisibleLine;
            var xCoordinate = MapXCoordinate(textLine, PreferredXCoordinate, false);
            InternalMoveCaretToTextViewLine(textLine, xCoordinate, IsVirtualSpaceOrBoxSelectionEnabled, false, false, true);
            return Position;
        }

        public CaretPosition MoveTo(ITextViewLine textLine)
        {
            if (textLine == null)
                throw new ArgumentNullException(nameof(textLine));
            var xCoordinate = MapXCoordinate(textLine, PreferredXCoordinate, false);
            InternalMoveCaretToTextViewLine(textLine, xCoordinate, true, false, true, true);
            return Position;
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
            xCoordinate = MapXCoordinate(textLine, xCoordinate, true);
            InternalMoveCaretToTextViewLine(textLine, xCoordinate, true, captureHorizontalPosition, true, true);
            return Position;
        }

        public CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition)
        {
            InternalMoveTo(bufferPosition, PositionAffinity.Successor, true, true, true);
            return Position;
        }

        public CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity)
        {
            InternalMoveTo(bufferPosition, caretAffinity, true, true, true);
            return Position;
        }

        public CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity, bool captureHorizontalPosition)
        {
            InternalMoveTo(bufferPosition, caretAffinity, captureHorizontalPosition, true, true);
            return Position;
        }

        public CaretPosition MoveTo(SnapshotPoint bufferPosition)
        {
            InternalMoveTo(new VirtualSnapshotPoint(bufferPosition), PositionAffinity.Successor, true, true, true);
            return Position;
        }

        public CaretPosition MoveTo(SnapshotPoint bufferPosition, PositionAffinity caretAffinity)
        {
            InternalMoveTo(new VirtualSnapshotPoint(bufferPosition), caretAffinity, true, true, true);
            return Position;
        }

        public CaretPosition MoveTo(SnapshotPoint bufferPosition, PositionAffinity caretAffinity, bool captureHorizontalPosition)
        {
            InternalMoveTo(new VirtualSnapshotPoint(bufferPosition), caretAffinity, captureHorizontalPosition, true, true);
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

        public CaretPosition MoveToPreviousCaretPosition()
        {
            var position = Position;
            if (position.VirtualSpaces > 0)
                return MoveTo(new VirtualSnapshotPoint(_textView.TextSnapshot.GetLineFromPosition(position.BufferPosition).End, IsVirtualSpaceOrBoxSelectionEnabled ? position.VirtualSpaces - 1 : 0));
            if (position.BufferPosition == 0)
                return position;
            return MoveTo(_textView.GetTextElementSpan(position.BufferPosition - 1).Start, PositionAffinity.Successor, true);
        }

        public bool InVirtualSpace => _insertionPoint.IsInVirtualSpace;

        public bool OverwriteMode
        {
            get => _overwriteMode;
            private set
            {
                if (_overwriteMode == value)
                    return;
                _overwriteMode = value;
                UpdateCaretBrush();
            }
        }

        public double Left => Bounds.Left;

        public double Width => Bounds.Width;

        public double Right => Bounds.Right;

        public double Top
        {
            get
            {
                if (!IsContainedByView)
                    throw new InvalidOperationException();
                return Bounds.Top;
            }
        }

        public double Height
        {
            get
            {
                if (!IsContainedByView)
                    throw new InvalidOperationException();
                return Bounds.Height;
            }
        }

        public double Bottom
        {
            get
            {
                if (!IsContainedByView)
                    throw new InvalidOperationException();
                return Bounds.Bottom;
            }
        }

        public CaretPosition Position => new CaretPosition(_insertionPoint, _textView.BufferGraph.CreateMappingPoint(_insertionPoint.Position, PointTrackingMode.Positive), _caretAffinity);

        public ITextViewLine ContainingTextViewLine
        {
            get
            {
                var position = Position;
                return GetContainingTextViewLine(position.BufferPosition, position.Affinity);
            }
        }

        public double PreferredYCoordinate => Math.Max(_textView.ViewportTop, Math.Min(_textView.ViewportBottom, _preferredYOffset + _textView.ViewportTop));

        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                _isHidden = value;
                if (_isHidden)
                    return;
                InvalidateVisual();
            }
        }

        public event EventHandler<CaretPositionChangedEventArgs> PositionChanged;

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_updateNeeded)
                UpdateCaret();
            Opacity = _newOpacity;
            base.OnRender(drawingContext);
            if (IsShownOnScreen)
            {
                if (_blinkTimer != null && !_blinkTimer.IsEnabled)
                    UpdateBlinkTimer();
                drawingContext.DrawGeometry(CaretBrush, null, CaretGeometry);
                if (_win32Caret.IsVisible)
                    _win32Caret.Update();
                else
                    _win32Caret.Show();
            }
            else
                DisableBlinkTimer();
        }

        private bool UpdateDefaultBrushes()
        {
            var defaultTextProperties = _classificationFormatMap.DefaultTextProperties;
            if (defaultTextProperties.ForegroundBrushEmpty)
            {
                DefaultRegularBrush = SystemColors.WindowTextBrush;
                DefaultOverwriteBrush = SystemColors.WindowTextBrush.Clone();
                DefaultOverwriteBrush.Opacity = 0.5;
                return true;
            }
            if (defaultTextProperties.ForegroundBrushSame(DefaultRegularBrush))
                return false;
            DefaultRegularBrush = defaultTextProperties.ForegroundBrush;
            DefaultOverwriteBrush = DefaultRegularBrush.Clone();
            DefaultOverwriteBrush.Opacity = 0.5;
            return true;
        }

        private void UpdateRegularCaretBrush()
        {
            var properties = _editorFormatMap.GetProperties("Caret");
            RegularBrush = !properties.Contains("ForegroundColor") ? (!properties.Contains("Foreground") ? DefaultRegularBrush : (Brush)properties["Foreground"]) : new SolidColorBrush((Color)properties["ForegroundColor"]);
            if (!RegularBrush.CanFreeze)
                return;
            RegularBrush.Freeze();
        }

        private void UpdateOverwriteCaretBrush()
        {
            var properties = _editorFormatMap.GetProperties("Overwrite Caret");
            if (properties.Contains("ForegroundColor"))
            {
                OverwriteBrush = new SolidColorBrush((Color) properties["ForegroundColor"]) {Opacity = 0.5};
            }
            else
                OverwriteBrush = !properties.Contains("Foreground") ? DefaultOverwriteBrush : (Brush)properties["Foreground"];
            if (!OverwriteBrush.CanFreeze)
                return;
            OverwriteBrush.Freeze();
        }

        private ITextViewLine GetContainingTextViewLine(SnapshotPoint bufferPosition, PositionAffinity caretAffinity)
        {
            var containingBufferPosition = _textView.GetTextViewLineContainingBufferPosition(bufferPosition);
            if (caretAffinity == PositionAffinity.Predecessor && containingBufferPosition.Start == bufferPosition && _textView.TextSnapshot.GetLineFromPosition(bufferPosition).Start != bufferPosition)
                containingBufferPosition = _textView.GetTextViewLineContainingBufferPosition(bufferPosition - 1);
            return containingBufferPosition;
        }

        private void UpdateBlinkTimer()
        {
            Opacity = 1.0;
            if (_blinkTimer == null)
                return;
            var caretBlinkTime = CaretBlinkTimeManager.GetCaretBlinkTime();
            if (_textView.VisualElement.IsVisible && caretBlinkTime > 0)
            {
                if (_blinkInterval != caretBlinkTime)
                {
                    _blinkTimer.Interval = new TimeSpan(0, 0, 0, 0, caretBlinkTime);
                    _blinkInterval = caretBlinkTime;
                }
                _blinkTimer.Start();
            }
            else
                DisableBlinkTimer();
            _newOpacity = 1.0;
        }

        private void DisableBlinkTimer()
        {
            _blinkTimer?.Stop();
        }

        private void UpdateCaretBrush()
        {
            CaretBrush = !_overwriteMode ? RegularBrush : OverwriteBrush;
            if (!IsShownOnScreen)
                return;
            InvalidateVisual();
        }

        private void InternalMoveCaretToTextViewLine(ITextViewLine textLine, double xCoordinate, bool allowPlacementInVirtualSpace, bool captureHorizontalPosition, bool captureVerticalPosition, bool raiseEvent)
        {
            var bufferPosition = textLine.GetInsertionBufferPositionFromXCoordinate(xCoordinate);
            if (!allowPlacementInVirtualSpace)
                bufferPosition = new VirtualSnapshotPoint(bufferPosition.Position);
            var caretAffinity = textLine.IsLastTextViewLineForSnapshotLine || !(bufferPosition.Position == textLine.End) ? PositionAffinity.Successor : PositionAffinity.Predecessor;
            InternalMoveCaret(bufferPosition, caretAffinity, textLine, captureHorizontalPosition, captureVerticalPosition, raiseEvent);
        }

        private void InternalMoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity, bool captureHorizontalPosition, bool captureVerticalPosition, bool raiseEvent)
        {
            if (bufferPosition.Position.Snapshot != _textView.TextSnapshot)
                throw new ArgumentException();
            var containingTextViewLine = GetContainingTextViewLine(bufferPosition.Position, caretAffinity);
            var bufferPosition1 = NormalizePosition(bufferPosition, containingTextViewLine);
            if (bufferPosition1 != bufferPosition)
                raiseEvent = true;
            InternalMoveCaret(bufferPosition1, caretAffinity, containingTextViewLine, captureHorizontalPosition, captureVerticalPosition, raiseEvent);
        }

        internal static VirtualSnapshotPoint NormalizePosition(VirtualSnapshotPoint bufferPosition, ITextViewLine textLine)
        {
            if ((bufferPosition.IsInVirtualSpace ? (!textLine.IsLastTextViewLineForSnapshotLine ? 1 : (bufferPosition.Position != textLine.End ? 1 : 0)) : (bufferPosition.Position != textLine.Start ? 1 : 0)) != 0)
                bufferPosition = new VirtualSnapshotPoint(bufferPosition.Position < textLine.End ? textLine.GetTextElementSpan(bufferPosition.Position).Start : textLine.End);
            return bufferPosition;
        }

        private void InternalMoveCaret(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity, ITextViewLine textLine, bool captureHorizontalPosition, bool captureVerticalPosition, bool raiseEvent)
        {
            var position1 = Position;
            _caretAffinity = caretAffinity;
            _insertionPoint = bufferPosition;
            _forceVirtualSpace = _insertionPoint.IsInVirtualSpace && !IsVirtualSpaceOrBoxSelectionEnabled;
            _emptySelection = _textView.Selection.IsEmpty;
            IsContainedByView = (uint)textLine.VisibilityState > 0U;
            OverwriteMode = !bufferPosition.IsInVirtualSpace && !(textLine.End == bufferPosition.Position) && (_textView.Options.IsOverwriteModeEnabled() && _emptySelection);
            double x;
            double width;
            if (_overwriteMode)
            {
                var extendedCharacterBounds = textLine.GetExtendedCharacterBounds(bufferPosition);
                x = extendedCharacterBounds.Left;
                width = extendedCharacterBounds.Width;
            }
            else
            {
                x = GetXCoordinateFromVirtualBufferPosition(textLine, bufferPosition);
                width = SystemParameters.CaretWidth;
            }
            Bounds = new Rect(x, textLine.TextTop, width, textLine.TextHeight);
            CapturePreferredPositions(captureHorizontalPosition, captureVerticalPosition);
            var position2 = Position;
            if (position2 != position1)
            {
                UpdateBlinkTimer();
                if (raiseEvent)
                {
                    if (_selection.IsEmpty)
                        _selection.RaiseChangedEvent(true, true, true);
                    // ISSUE: reference to a compiler-generated field
                    var positionChanged = PositionChanged;
                    if (positionChanged != null)
                        _guardedOperations.RaiseEvent(this, positionChanged, new CaretPositionChangedEventArgs(_textView, position1, position2));
                }
            }
            InvalidateVisual();
            _updateNeeded = true;
        }

        internal static double GetXCoordinateFromVirtualBufferPosition(ITextViewLine textLine, VirtualSnapshotPoint bufferPosition)
        {
            if (!bufferPosition.IsInVirtualSpace && !(bufferPosition.Position == textLine.Start))
                return textLine.GetExtendedCharacterBounds(bufferPosition.Position - 1).Trailing;
            return textLine.GetExtendedCharacterBounds(bufferPosition).Leading;
        }

        private void ConstructCaretGeometry()
        {
            _displayedWidth = Bounds.Width;
            _displayedHeight = Bounds.Height;
            CaretGeometryNeedsToBeUpdated = false;
            var pathGeometry = new PathGeometry();
            pathGeometry.AddGeometry(new RectangleGeometry(new Rect(0.0, 0.0, _displayedWidth, _displayedHeight)));
            if (InputLanguageManager.Current.CurrentInputLanguage.TextInfo.IsRightToLeft)
                pathGeometry.Figures.Add(new PathFigure()
                {
                    StartPoint = new Point(0.0, 0.0),
                    Segments = {
            new LineSegment(new Point(-2.0, 0.0), true),
            new LineSegment(new Point(0.0, _displayedHeight / 10.0), true)
          },
                    IsClosed = true
                });
            CaretGeometry = pathGeometry;
        }

        internal void UpdateCaret()
        {
            _updateNeeded = false;
            if (!IsContainedByView)
                return;
            if (CaretGeometryNeedsToBeUpdated || _displayedWidth != Bounds.Width || _displayedHeight != Bounds.Height)
                ConstructCaretGeometry();
            CaretGeometry.Transform = new TranslateTransform(Bounds.Left, Bounds.Top);
            CaretGeometry.Transform.Freeze();
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            var flag = false;
            if (e.ChangedItems.Contains("Caret"))
            {
                UpdateRegularCaretBrush();
                flag = true;
            }
            if (e.ChangedItems.Contains("Overwrite Caret"))
            {
                UpdateOverwriteCaretBrush();
                flag = true;
            }
            if (!flag)
                return;
            UpdateCaretBrush();
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            if (!UpdateDefaultBrushes())
                return;
            UpdateRegularCaretBrush();
            UpdateOverwriteCaretBrush();
            UpdateCaretBrush();
        }

        internal void LayoutChanged(ITextSnapshot oldSnapshot, ITextSnapshot newSnapshot)
        {
            if (oldSnapshot != newSnapshot)
                _insertionPoint = _insertionPoint.TranslateTo(newSnapshot);
            UpdateBlinkTimer();
            var preserveCoordinates = AnyTextChanges(oldSnapshot.Version, newSnapshot.Version);
            RefreshCaret(preserveCoordinates);
            if (!preserveCoordinates || !_textView.Options.IsAutoScrollEnabled() || _insertionPoint.Position.GetContainingLine().LineNumber != newSnapshot.LineCount - 1)
                return;
            _textView.VisualElement.Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action) EnsureVisible);
        }

        private static bool AnyTextChanges(ITextVersion oldVersion, ITextVersion currentVersion)
        {
            for (; oldVersion != currentVersion; oldVersion = oldVersion.Next)
            {
                if (oldVersion.Changes.Count > 0)
                    return true;
            }
            return false;
        }

        private void RefreshCaret(bool preserveCoordinates, bool clearVirtualSpace = false)
        {
            var bufferPosition = Position.VirtualBufferPosition;
            if (clearVirtualSpace && !_forceVirtualSpace)
                bufferPosition = new VirtualSnapshotPoint(bufferPosition.Position);
            InternalMoveTo(bufferPosition, Position.Affinity, preserveCoordinates, preserveCoordinates, false);
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                InputLanguageManager.Current.InputLanguageChanged += OnInputLanguageChanged;
                InvalidateVisual();
                CaretGeometryNeedsToBeUpdated = true;
                UpdateBlinkTimer();
            }
            else
            {
                InputLanguageManager.Current.InputLanguageChanged -= OnInputLanguageChanged;
                if (_win32Caret.IsVisible)
                    _win32Caret.Hide();
                DisableBlinkTimer();
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!_textView.Options.IsOverwriteModeEnabled() || _textView.Selection.IsEmpty == _emptySelection)
                return;
            RefreshCaret(false);
        }

        private void OnOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId == DefaultTextViewOptions.OverwriteModeId.Name)
            {
                RefreshCaret(false);
            }
            else
            {
                if (e.OptionId != DefaultTextViewOptions.UseVirtualSpaceId.Name || IsVirtualSpaceOrBoxSelectionEnabled)
                    return;
                RefreshCaret(false, true);
            }
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            InvalidateVisual();
            _newOpacity = _newOpacity == 0.0 ? 1.0 : 0.0;
        }

        private void OnInputLanguageChanged(object sender, InputLanguageEventArgs e)
        {
            InvalidateVisual();
            _updateNeeded = true;
            CaretGeometryNeedsToBeUpdated = true;
        }

        private void CapturePreferredYCoordinate()
        {
            var textViewLine = ContainingTextViewLine;
            if (textViewLine.VisibilityState == VisibilityState.Unattached || textViewLine.VisibilityState == VisibilityState.Hidden)
            {
                textViewLine = _textView.TextViewLines.LastVisibleLine;
                var snapshotPoint = _insertionPoint.Position;
                var position1 = snapshotPoint.Position;
                snapshotPoint = textViewLine.Start;
                var position2 = snapshotPoint.Position;
                if (position1 < position2)
                    textViewLine = _textView.TextViewLines.FirstVisibleLine;
            }
            _preferredYOffset = textViewLine.Top + textViewLine.Height * 0.5 - _textView.ViewportTop;
        }

        private void CapturePreferredPositions(bool captureHorizontalPosition, bool captureVerticalPosition)
        {
            if (captureHorizontalPosition)
                PreferredXCoordinate = Bounds.Left;
            if (!captureVerticalPosition)
                return;
            CapturePreferredYCoordinate();
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

        private double MapXCoordinate(ITextViewLine textLine, double xCoordinate, bool userSpecifiedXCoordinate)
        {
            if (xCoordinate > textLine.TextRight && !IsVirtualSpaceOrBoxSelectionEnabled)
            {
                var num = 0.0;
                if (textLine.End == textLine.Start)
                {
                    var desiredIndentation = SmartIndentationService.GetDesiredIndentation(_textView, textLine.Start.GetContainingLine());
                    if (desiredIndentation.HasValue)
                    {
                        num = Math.Max(0.0, desiredIndentation.Value * _textView.FormattedLineSource.ColumnWidth - textLine.TextWidth);
                        if (userSpecifiedXCoordinate && xCoordinate < textLine.TextRight + num)
                            num = 0.0;
                    }
                }
                xCoordinate = textLine.TextRight + num;
            }
            return xCoordinate;
        }

        private void UnsubscribeEvents()
        {
            IsVisibleChanged -= OnVisibleChanged;
            _textView.Options.OptionChanged -= OnOptionsChanged;
            _textView.Selection.SelectionChanged -= OnSelectionChanged;
            _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
            _classificationFormatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMappingChanged;
            if (InputLanguageManager.Current == null)
                return;
            InputLanguageManager.Current.InputLanguageChanged -= OnInputLanguageChanged;
        }

        private void SubscribeEvents()
        {
            IsVisibleChanged += OnVisibleChanged;
            _textView.Options.OptionChanged += OnOptionsChanged;
            _textView.Selection.SelectionChanged += OnSelectionChanged;
            _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
            _classificationFormatMap.ClassificationFormatMappingChanged += OnClassificationFormatMappingChanged;
        }

        public IAccessible AccessibleCaret => _accessibleCaret ?? (_accessibleCaret = new AccessibleCaret(this, _win32Caret));

        public void Close()
        {
            if (_isClosed)
                return;
            _isClosed = true;
            UnsubscribeEvents();
            DisableBlinkTimer();
            _win32Caret.Destroy();
            _win32Caret.Dispose();
        }

        public bool IsShownOnScreen
        {
            get
            {
                if (IsContainedByView && Visibility == Visibility.Visible)
                    return !_isHidden;
                return false;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Caret")]
        [UserVisible(false)]
        internal sealed class CaretProperties : EditorFormatDefinition
        {
            public CaretProperties()
            {
                BackgroundCustomizable = false;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Overwrite Caret")]
        [UserVisible(false)]
        internal sealed class OverwriteCaretProperties : EditorFormatDefinition
        {
            public OverwriteCaretProperties()
            {
                BackgroundCustomizable = false;
            }
        }

        [SuppressUnmanagedCodeSecurity]
        internal static class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern void NotifyWinEvent(int winEvent, IntPtr hwnd, int objType, int objID);

            [DllImport("oleacc.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, IntPtr pAcc);
        }

        internal class Win32Caret
        {
            private readonly CaretElement _wpfCaret;
            private readonly ITextView _wpfTextView;
            private IntPtr _windowHandle;
            private Win32CaretLocation _lastPosition;
            private bool _isCreated;

            public Win32Caret(CaretElement wpfCaret, ITextView wpfTextView)
            {
                _wpfCaret = wpfCaret;
                _wpfTextView = wpfTextView;
                _windowHandle = IntPtr.Zero;
                _isCreated = false;
                PresentationSource.AddSourceChangedHandler(_wpfCaret, OnPresentationSourceChanged);
                var presentationSource = GetPresentationSource();
                if (presentationSource != null)
                    GetWindowHandle(presentationSource);
                Create();
            }

            private void OnPresentationSourceChanged(object sender, SourceChangedEventArgs e)
            {
                Destroy();
                GetWindowHandle(e.NewSource);
                Create();
            }

            public void Update()
            {
                if (!(_windowHandle != IntPtr.Zero))
                    return;
                SetPosition(_windowHandle);
            }

            public void Destroy()
            {
                if (!_isCreated || !(_windowHandle != IntPtr.Zero))
                    return;
                NativeMethods.NotifyWinEvent(32769, _windowHandle, -8, 0);
                _isCreated = false;
            }

            public void Dispose()
            {
                PresentationSource.RemoveSourceChangedHandler(_wpfCaret, OnPresentationSourceChanged);
            }

            public void Show()
            {
                if (IsVisible)
                    return;
                if (_windowHandle != IntPtr.Zero)
                {
                    NativeMethods.NotifyWinEvent(32770, _windowHandle, -8, 0);
                    IsVisible = true;
                }
                if (!IsVisible)
                    return;
                SetPosition(_windowHandle);
            }

            public void Create()
            {
                if (!(_windowHandle != IntPtr.Zero))
                    return;
                NativeMethods.NotifyWinEvent(32768, _windowHandle, -8, 0);
                _isCreated = true;
            }

            public void Hide()
            {
                if (!IsVisible)
                    return;
                if (_windowHandle != IntPtr.Zero)
                    NativeMethods.NotifyWinEvent(32771, _windowHandle, -8, 0);
                IsVisible = false;
            }

            private void SetPosition(IntPtr hWnd)
            {
                var win32CaretLocation = new Win32CaretLocation(_wpfCaret);
                if (!(_lastPosition != win32CaretLocation))
                    return;
                _lastPosition = win32CaretLocation;
                var rect = _wpfCaret.RenderTransform.TransformBounds(_wpfCaret.Bounds);
                TopLeft = _wpfCaret.PointToScreen(rect.TopLeft);
                BottomRight = _wpfCaret.PointToScreen(rect.BottomRight);
                NativeMethods.NotifyWinEvent(32779, hWnd, -8, 0);
            }

            private void GetWindowHandle(PresentationSource pSource)
            {
                _windowHandle = pSource is IWin32Window win32Window ? win32Window.Handle : IntPtr.Zero;
            }

            private PresentationSource GetPresentationSource()
            {
                new UIPermission(UIPermissionWindow.AllWindows).Assert();
                try
                {
                    return PresentationSource.FromVisual(_wpfTextView.VisualElement);
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
            }

            public bool IsVisible { get; private set; }

            public Point TopLeft { get; private set; }

            public Point BottomRight { get; private set; }

            private struct Win32CaretLocation
            {
                private readonly int _bufferPosition;
                private readonly int _snapshotVersion;
                private readonly int _virtualSpaces;

                public Win32CaretLocation(CaretElement wpfCaret)
                {
                    _bufferPosition = wpfCaret._insertionPoint.Position.Position;
                    _snapshotVersion = wpfCaret._insertionPoint.Position.Snapshot.Version.VersionNumber;
                    _virtualSpaces = wpfCaret._insertionPoint.VirtualSpaces;
                }

                public static bool operator ==(Win32CaretLocation a, Win32CaretLocation b)
                {
                    if (a._bufferPosition == b._bufferPosition && a._snapshotVersion == b._snapshotVersion)
                        return a._virtualSpaces == b._virtualSpaces;
                    return false;
                }

                public static bool operator !=(Win32CaretLocation a, Win32CaretLocation b)
                {
                    return !(a == b);
                }

                public override bool Equals(object obj)
                {
                    if (obj is Win32CaretLocation nullable)
                        return nullable == this;
                    return false;
                }

                public override int GetHashCode()
                {
                    throw new InvalidOperationException("W32CaretLocation is never meant to be hashed");
                }
            }
        }
    }
}