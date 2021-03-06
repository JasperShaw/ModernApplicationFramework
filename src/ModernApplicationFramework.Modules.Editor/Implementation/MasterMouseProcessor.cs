﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Modules.Editor.MultiSelection;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal sealed class MasterMouseProcessor : WpfMouseProcessor
    {
        private double _deltaThreshold = 0.5;
        private readonly ITextView _wpfTextView;
        private readonly IEditorOperations _editorOperations;
        private readonly InputControllerViewCreationListener _factory;
        private bool _touchStartedZoom;
        private bool _touchStartedScroll;
        internal Point? MouseDownPoint;
        private bool _handlingMove;
        internal DispatcherTimer ScrollTimer;
        private double _touchZoomLevel;
        private IMultiSelectionBroker _multiSelectionBroker;
        private TouchScrollMode _currentTouchScrollMode;
        private const double MaxScrollingDelay = 250.0;
        internal double CurrentScrollDelay;
        private MultiSelectionMouseState _mouseState;

        internal IList<IMouseProcessor> MouseProcessors
        {
            get => _mouseProcessors;
            set => _mouseProcessors = value;
        }

        public MasterMouseProcessor(ITextView wpfTextView, IEditorOperations editorOperations, IList<IMouseProcessor> mouseProcessors, InputControllerViewCreationListener factory)
          : base(wpfTextView.VisualElement, mouseProcessors, factory.GuardedOperations, wpfTextView.ManipulationLayer)
        {
            _factory = factory;
            _wpfTextView = wpfTextView;
            _editorOperations = editorOperations;
            ScrollTimer = null;
        }

        private void StartOrContinueScrolling(ScrollDirection direction, double deltaY, double deltaX)
        {
            var num = ScrollTimer == null ? 0 : (ScrollTimer.IsEnabled ? 1 : 0);
            CurrentScrollDelay = Math.Max(1.0, Math.Ceiling(250.0 - Math.Pow(deltaY, 1.0 + deltaY / 250.0)));
            if (num != 0)
            {
                if (deltaY >= _wpfTextView.LineHeight)
                    return;
                StopScrolling();
            }
            else
            {
                if (deltaY < _wpfTextView.LineHeight)
                    return;
                ScrollTimer =
                    new DispatcherTimer(DispatcherPriority.Input)
                    {
                        Interval = TimeSpan.FromMilliseconds(CurrentScrollDelay)
                    };
                ScrollAndExtendSelection(direction, deltaX);
                ScrollTimer.Tick += (sender, args) => HandleScrollTimerTick(sender, direction, deltaX);
                ScrollTimer.Start();
            }
        }

        private void StopScrolling()
        {
            if (ScrollTimer == null)
                return;
            ScrollTimer.Stop();
            ScrollTimer = null;
        }

        private IMultiSelectionBroker MultiSelectionBroker => _multiSelectionBroker ?? (_multiSelectionBroker = _wpfTextView.GetMultiSelectionBroker());

        private MultiSelectionMouseState MouseState => _mouseState ?? (_mouseState = MultiSelectionMouseState.GetStateForView(_wpfTextView));

        private void ScrollAndExtendSelection(ScrollDirection direction, double deltaX)
        {
            _wpfTextView.ViewScroller.ScrollViewportVerticallyByLine(direction);
            var textViewLines = _wpfTextView.TextViewLines;
            if (textViewLines == null)
                return;
            var virtualSnapshotPoint = GetVirtualSnapshotPoint(direction == ScrollDirection.Up ? textViewLines.FirstVisibleLine : textViewLines.LastVisibleLine, deltaX);
            var newActive = virtualSnapshotPoint.Item1;
            var affinity = virtualSnapshotPoint.Item2;
            if (MouseState.ProvisionalSelection != Selection.Invalid)
            {
                var provisionalSelection = MouseState.ProvisionalSelection;
                MouseState.ProvisionalSelection = new Selection(newActive, provisionalSelection.AnchorPoint, newActive, affinity);
            }
            else
            {
                _multiSelectionBroker.TryPerformActionOnSelection(MultiSelectionBroker.IsBoxSelection ? MultiSelectionBroker.BoxSelection : MultiSelectionBroker.PrimarySelection, transformer => transformer.MoveTo(newActive, true, affinity), out var after);
            }
        }

        private ValueTuple<VirtualSnapshotPoint, PositionAffinity> GetVirtualSnapshotPoint(ITextViewLine textLine, double xCoordinate)
        {
            var virtualSnapshotPoint = textLine.GetInsertionBufferPositionFromXCoordinate(xCoordinate);
            if (virtualSnapshotPoint.IsInVirtualSpace && !_wpfTextView.IsVirtualSpaceOrBoxSelectionEnabled())
                virtualSnapshotPoint = new VirtualSnapshotPoint(virtualSnapshotPoint.Position);
            var positionAffinity = textLine.IsLastTextViewLineForSnapshotLine || !(virtualSnapshotPoint.Position == textLine.End) ? PositionAffinity.Successor : PositionAffinity.Predecessor;
            return new ValueTuple<VirtualSnapshotPoint, PositionAffinity>(virtualSnapshotPoint, positionAffinity);
        }

        private void HandleScrollTimerTick(object timer, ScrollDirection direction, double deltaX)
        {
            if (timer != ScrollTimer || !ScrollTimer.IsEnabled)
                return;
            if (ScrollTimer.Interval.Milliseconds != CurrentScrollDelay)
                ScrollTimer.Interval = TimeSpan.FromMilliseconds(CurrentScrollDelay);
            ScrollAndExtendSelection(direction, deltaX);
            if (direction == ScrollDirection.Up)
            {
                if (_wpfTextView.TextViewLines.FirstVisibleLine.Start != 0)
                    return;
                StopScrolling();
            }
            else
            {
                if (_wpfTextView.TextViewLines.LastVisibleLine.EndIncludingLineBreak != _wpfTextView.TextSnapshot.Length)
                    return;
                StopScrolling();
            }
        }

        private ITextViewLine GetClosestTextViewLine(double yCoordinate)
        {
            ITextViewLine textViewLine = null;
            var textViewLines = _wpfTextView.TextViewLines;
            if (textViewLines != null && textViewLines.Count > 0)
            {
                textViewLine = textViewLines.GetTextViewLineContainingYCoordinate(yCoordinate);
                if (textViewLine == null)
                {
                    if (yCoordinate <= textViewLines[0].Bottom)
                        textViewLine = textViewLines[0];
                    else if (yCoordinate >= textViewLines[textViewLines.Count - 1].Top)
                        textViewLine = textViewLines[textViewLines.Count - 1];
                }
            }
            return textViewLine;
        }

        protected override void DefaultMouseLeftButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            e.Handled = HandleMouseLeftButtonDown(e.GetPosition(_wpfTextView.VisualElement));
        }

        protected override void DefaultStylusSystemGestureHandler(object sender, StylusSystemGestureEventArgs e)
        {
            if (e.SystemGesture != SystemGesture.Tap)
                return;
            var position = e.GetPosition(_wpfTextView.VisualElement);
            position.X += _wpfTextView.ViewportLeft;
            position.Y += _wpfTextView.ViewportTop;
            var closestTextViewLine = GetClosestTextViewLine(position.Y);
            if (closestTextViewLine == null)
                return;
            _editorOperations.MoveCaret(closestTextViewLine, position.X, false);
            _wpfTextView.Caret.EnsureVisible();
        }

        protected override void DefaultManipulationStartingHandler(object sender, ManipulationStartingEventArgs e)
        {
            if (VisualTreeHelper.GetParent(_wpfTextView.VisualElement) is UIElement parent)
                e.ManipulationContainer = parent;
            _touchZoomLevel = _wpfTextView.ZoomLevel;
            _currentTouchScrollMode = TouchScrollMode.Unknown;
            e.Handled = true;
        }

        protected override void DefaultManipulationInertiaStartingHandler(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.00096;
            e.Handled = true;
        }

        protected override void DefaultManipulationCompletedHandler(object sender, ManipulationCompletedEventArgs e)
        {
            if (_touchStartedZoom)
            {
                _touchStartedZoom = false;
            }
            if (!_touchStartedScroll)
                return;
            _touchStartedScroll = false;
        }

        protected override void DefaultManipulationDeltaHandler(object sender, ManipulationDeltaEventArgs e)
        {
            var x = e.DeltaManipulation.Scale.X;
            if (x != 1.0 && _wpfTextView.Roles.Contains("ZOOMABLE"))
            {
                _touchZoomLevel *= x;
                if (Math.Abs(_wpfTextView.ZoomLevel - _touchZoomLevel) > _wpfTextView.ZoomLevel * 0.1)
                {
                    if (_touchZoomLevel > 95.0 && _touchZoomLevel < 105.0)
                        _touchZoomLevel = 100.0;
                    _wpfTextView.Options.GlobalOptions.SetOptionValue<double>(DefaultViewOptions.ZoomLevelId, _touchZoomLevel);
                    _touchStartedZoom = true;
                }
            }
            else if (e.DeltaManipulation.Translation.X != 0.0 || e.DeltaManipulation.Translation.Y != 0.0)
            {
                var num1 = Math.Abs(e.DeltaManipulation.Translation.X);
                var num2 = Math.Abs(e.DeltaManipulation.Translation.Y);
                if (num1 - num2 > _deltaThreshold)
                {
                    if (_currentTouchScrollMode == TouchScrollMode.Unknown)
                        _currentTouchScrollMode = TouchScrollMode.Horizontal;
                    else if (_currentTouchScrollMode == TouchScrollMode.Vertical && num2 != 0.0)
                        _currentTouchScrollMode = TouchScrollMode.Diagonal;
                }
                else if (_currentTouchScrollMode == TouchScrollMode.Unknown)
                    _currentTouchScrollMode = TouchScrollMode.Vertical;
                else if (_currentTouchScrollMode == TouchScrollMode.Horizontal && num1 != 0.0 && num2 - num1 > _deltaThreshold)
                    _currentTouchScrollMode = TouchScrollMode.Diagonal;
                var num3 = 1.0;
                if (_wpfTextView.ZoomLevel != 0.0)
                    num3 = 100.0 / _wpfTextView.ZoomLevel;
                if (_currentTouchScrollMode == TouchScrollMode.Vertical || _currentTouchScrollMode == TouchScrollMode.Diagonal)
                {
                    _wpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(e.DeltaManipulation.Translation.Y * num3);
                    _touchStartedScroll = true;
                }
                if (_currentTouchScrollMode == TouchScrollMode.Horizontal || _currentTouchScrollMode == TouchScrollMode.Diagonal)
                {
                    _wpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(e.DeltaManipulation.Translation.X * -1.0 * num3);
                    _touchStartedScroll = true;
                }
            }
            e.Handled = true;
        }

        internal bool HandleMouseLeftButtonDown(Point pt)
        {
            pt.X += _wpfTextView.ViewportLeft;
            pt.Y += _wpfTextView.ViewportTop;
            if (GetClosestTextViewLine(pt.Y) != null)
                MouseDownPoint = pt;
            return true;
        }

        protected override void DefaultMouseMoveHandler(object sender, MouseEventArgs e)
        {
            e.Handled = HandleMouseMove(e.LeftButton, e.GetPosition(_wpfTextView.VisualElement));
        }

        internal bool HandleMouseMove(MouseButtonState leftButton, Point pt)
        {
            if (!MouseDownPoint.HasValue || leftButton != MouseButtonState.Pressed)
                return false;
            pt.X += _wpfTextView.ViewportLeft;
            pt.Y += _wpfTextView.ViewportTop;
            if (!_handlingMove)
            {
                var vector = pt - MouseDownPoint.Value;
                if (Math.Abs(vector.X) >= SystemParameters.MinimumHorizontalDragDistance || Math.Abs(vector.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    _handlingMove = true;
                    _wpfTextView.VisualElement.CaptureMouse();
                }
            }
            if (_handlingMove)
            {
                using (_factory.PerformanceBlockMarker.CreateBlock("VsTextEditor.Scroll.MouseDrag"))
                {
                    if (pt.Y < _wpfTextView.ViewportTop)
                        StartOrContinueScrolling(ScrollDirection.Up, _wpfTextView.ViewportTop - pt.Y, pt.X);
                    else if (pt.Y > _wpfTextView.ViewportBottom)
                    {
                        StartOrContinueScrolling(ScrollDirection.Down, pt.Y - _wpfTextView.ViewportBottom, pt.X);
                    }
                    else
                    {
                        StopScrolling();
                        if (GetClosestTextViewLine(pt.Y) != null)
                        {
                            var selection = MouseState.ProvisionalSelection != Selection.Invalid ? MouseState.ProvisionalSelection : MultiSelectionBroker.PrimarySelection;
                            _wpfTextView.ViewScroller.EnsureSpanVisible(new VirtualSnapshotSpan(selection.InsertionPoint, selection.InsertionPoint), EnsureSpanVisibleOptions.MinimumScroll);
                        }
                    }
                }
            }
            return true;
        }

        internal void HandleMousePostLeftButtonUp()
        {
            if (_handlingMove)
            {
                _wpfTextView.VisualElement.ReleaseMouseCapture();
                StopScrolling();
            }
            _handlingMove = false;
            MouseDownPoint = new Point?();
        }

        protected override void DefaultMouseLeftButtonUpPostprocessor(MouseButtonEventArgs e)
        {
            HandleMousePostLeftButtonUp();
        }

        private enum TouchScrollMode
        {
            Unknown,
            Horizontal,
            Vertical,
            Diagonal,
        }
    }
}