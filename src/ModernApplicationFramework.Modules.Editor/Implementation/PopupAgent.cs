using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Adornments;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Outlining;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class PopupAgent : ISpaceReservationAgent
    {
        internal readonly ISpaceReservationManager Manager;
        internal readonly PopupOrWindowContainer Popup;
        internal readonly TextView TextView;
        internal IInputElement MouseContainer;
        internal PopupStyles Style;
        internal ITrackingSpan VisualSpan;

        public event EventHandler GotFocus;

        public event EventHandler LostFocus;

        public bool HasFocus
        {
            get
            {
                if (Popup.IsVisible)
                    return Popup.IsKeyboardFocusWithin;
                return false;
            }
        }

        public bool IsMouseOver
        {
            get
            {
                if (!Popup.IsVisible)
                    return false;
                return Popup.Content.IsMouseOver;
            }
        }

        private IOutliningManager OutliningManager
        {
            get
            {
                if (TextView.TextViewModel != null && TextView.ComponentContext.OutliningManagerService != null)
                    return TextView.ComponentContext.OutliningManagerService.GetOutliningManager(TextView);
                return null;
            }
        }

        public PopupAgent(TextView textView, ISpaceReservationManager manager, ITrackingSpan visualSpan,
            PopupStyles style, UIElement content)
        {
            if (!style.HasFlag(PopupStyles.None))
                throw new ArgumentOutOfRangeException(nameof(style));
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            if ((style & PopupStyles.DismissOnMouseLeaveText) != PopupStyles.None &&
                (style & PopupStyles.DismissOnMouseLeaveTextOrContent) != PopupStyles.None)
                throw new ArgumentException(
                    "Can't specify both PopupStyles.DismissOnMouseLeaveText and PopupStyles.DismissOnMouseLeaveTextOrContent",
                    nameof(style));
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));
            VisualSpan = visualSpan ?? throw new ArgumentNullException(nameof(visualSpan));
            Style = style;
            var singletonProperty = textView.Properties.GetOrCreateSingletonProperty(() =>
                new Dictionary<WeakReferenceForDictionaryKey, PopupOrWindowContainer>(10));
            if (singletonProperty.TryGetValue(new WeakReferenceForDictionaryKey(content), out Popup))
                return;
            Popup = PopupOrWindowContainer.Create(content, TextView.VisualElement);
            if (singletonProperty.Count == 10)
                singletonProperty.Clear();
            singletonProperty.Add(new WeakReferenceForDictionaryKey(content), Popup);
        }

        public void Hide()
        {
            if (!Popup.IsVisible)
                return;
            Popup.Hide();
            UnregisterForEvents();
        }

        public Geometry PositionAndDisplay(Geometry reservedSpace)
        {
            var span = VisualSpan.GetSpan(TextView.TextSnapshot);
            if ((Style & (PopupStyles.DismissOnMouseLeaveText | PopupStyles.DismissOnMouseLeaveTextOrContent)) !=
                PopupStyles.None && ShouldClearToolTipOnMouseMove(Mouse.GetPosition(TextView.VisualElement)))
                return null;
            var nullable = new Rect?();
            if (span.Length > 0)
            {
                var num1 = double.MaxValue;
                var num2 = double.MaxValue;
                var val1_1 = double.MinValue;
                var val1_2 = double.MinValue;
                foreach (var normalizedTextBound in TextView.TextViewLines.GetNormalizedTextBounds(span))
                {
                    num1 = Math.Min(num1, normalizedTextBound.Left);
                    num2 = Math.Min(num2, normalizedTextBound.TextTop);
                    val1_1 = Math.Max(val1_1, normalizedTextBound.Right);
                    val1_2 = Math.Max(val1_2, normalizedTextBound.TextBottom);
                }

                var containingBufferPosition1 =
                    TextView.TextViewLines.GetTextViewLineContainingBufferPosition(span.Start);
                if (containingBufferPosition1 != null)
                {
                    var extendedCharacterBounds = containingBufferPosition1.GetExtendedCharacterBounds(span.Start);
                    if (extendedCharacterBounds.Left < val1_1 &&
                        extendedCharacterBounds.Left >= TextView.ViewportLeft &&
                        extendedCharacterBounds.Left < TextView.ViewportRight)
                        num1 = extendedCharacterBounds.Left;
                }

                var containingBufferPosition2 =
                    TextView.TextViewLines.GetTextViewLineContainingBufferPosition(span.End);
                if (containingBufferPosition2 != null && containingBufferPosition2.Start == span.End)
                    val1_2 = Math.Max(val1_2, containingBufferPosition2.TextBottom);
                if (num1 < val1_1)
                    nullable = new Rect(num1, num2, val1_1 - num1, val1_2 - num2);
            }
            else
            {
                var containingBufferPosition =
                    TextView.TextViewLines.GetTextViewLineContainingBufferPosition(span.Start);
                if (containingBufferPosition != null)
                {
                    var characterBounds = containingBufferPosition.GetCharacterBounds(span.Start);
                    nullable = new Rect(characterBounds.Left, characterBounds.TextTop, 0.0, characterBounds.TextHeight);
                }
            }

            if (nullable.HasValue && !nullable.Value.IsEmpty)
            {
                var rect1 = new Rect(TextView.ViewportLeft, TextView.ViewportTop, TextView.ViewportWidth,
                    TextView.ViewportHeight);
                var rect2 = nullable.Value;
                rect2.Intersect(rect1);
                if (!rect2.IsEmpty)
                {
                    var rect3 = new Rect(rect2.Left, rect2.Top, rect2.Right - rect2.Left,
                        rect2.Bottom - rect2.Top + 3.0);
                    var rect4 = new Rect(GetScreenPointFromTextXY(rect2.Left, rect2.Top),
                        GetScreenPointFromTextXY(rect2.Right, rect2.Bottom));
                    var rect5 = new Rect(GetScreenPointFromTextXY(rect3.Left, rect3.Top),
                        GetScreenPointFromTextXY(rect3.Right, rect3.Bottom));
                    var screenRect = WpfHelper.GetScreenRect(rect4.TopLeft);
                    var desiredSize = Popup.Size;
                    desiredSize = new Size(desiredSize.Width / WpfHelper.DeviceScaleX,
                        desiredSize.Height / WpfHelper.DeviceScaleY);
                    var popupStyles = Style ^ PopupStyles.PreferLeftOrTopPosition;
                    var rect6 = reservedSpace.Bounds;
                    rect6 = new Rect(new Point(Math.Min(rect4.Left, rect6.Left), Math.Min(rect4.Top, rect6.Top)),
                        new Point(Math.Max(rect4.Right, rect6.Right), Math.Max(rect4.Bottom, rect6.Bottom)));
                    Tuple<PopupStyles, Rect>[] tupleArray;
                    if (!rect6.IsEmpty)
                    {
                        if ((Style & PopupStyles.PositionClosest) == PopupStyles.None)
                            tupleArray = new[]
                            {
                                new Tuple<PopupStyles, Rect>(Style, rect4),
                                new Tuple<PopupStyles, Rect>(Style, rect5),
                                new Tuple<PopupStyles, Rect>(Style, rect6),
                                new Tuple<PopupStyles, Rect>(popupStyles, rect4),
                                new Tuple<PopupStyles, Rect>(popupStyles, rect5),
                                new Tuple<PopupStyles, Rect>(popupStyles, rect6)
                            };
                        else
                            tupleArray = new[]
                            {
                                new Tuple<PopupStyles, Rect>(Style, rect4),
                                new Tuple<PopupStyles, Rect>(Style, rect5),
                                new Tuple<PopupStyles, Rect>(popupStyles, rect4),
                                new Tuple<PopupStyles, Rect>(popupStyles, rect5),
                                new Tuple<PopupStyles, Rect>(Style, rect6),
                                new Tuple<PopupStyles, Rect>(popupStyles, rect6)
                            };
                    }
                    else
                    {
                        tupleArray = new[]
                        {
                            new Tuple<PopupStyles, Rect>(Style, rect4),
                            new Tuple<PopupStyles, Rect>(Style, rect5),
                            new Tuple<PopupStyles, Rect>(popupStyles, rect4),
                            new Tuple<PopupStyles, Rect>(popupStyles, rect5)
                        };
                    }

                    var rect7 = Rect.Empty;
                    foreach (var tuple in tupleArray)
                    {
                        var location = GetLocation(tuple.Item1, desiredSize, rect4, tuple.Item2, screenRect);
                        if (DisjointWithPadding(reservedSpace, location) && ContainsWithPadding(screenRect, location))
                        {
                            rect7 = location;
                            Style = tuple.Item1;
                            break;
                        }
                    }

                    if (rect7 == Rect.Empty)
                        return null;
                    if (!Popup.IsVisible)
                        RegisterForEvents();
                    Popup.DisplayAt(rect7.TopLeft);
                    return new GeometryGroup
                    {
                        Children =
                        {
                            new RectangleGeometry(rect4),
                            new RectangleGeometry(rect7)
                        }
                    };
                }
            }

            return null;
        }

        public void SetVisualSpan(ITrackingSpan visualSpan)
        {
            VisualSpan = visualSpan;
        }

        internal static bool ContainsWithPadding(Rect outer, Rect inner)
        {
            if (outer.Left - 0.01 <= inner.Left && inner.Right <= outer.Right + 0.01 && outer.Top - 0.01 <= inner.Top)
                return inner.Bottom <= outer.Bottom + 0.01;
            return false;
        }

        internal static bool DisjointWithPadding(Geometry reserved, Rect location)
        {
            var x = location.Left + 0.1;
            var y = location.Top + 0.1;
            var width = location.Right - 0.1 - x;
            var height = location.Bottom - 0.1 - y;
            if (width <= 0.0 || height <= 0.0)
                return true;
            Geometry geometry = new RectangleGeometry(new Rect(x, y, width, height));
            return reserved.FillContainsWithDetail(geometry) == IntersectionDetail.Empty;
        }

        internal static Rect GetLocation(PopupStyles style, Size desiredSize, Rect spanRectInScreenCoordinates,
            Rect reservedRect, Rect screenRect)
        {
            if ((style & PopupStyles.PositionLeftOrRight) != PopupStyles.None)
                return ShiftVerticallyToFitScreen(
                    (style & PopupStyles.PreferLeftOrTopPosition) != PopupStyles.None
                        ? reservedRect.Left - desiredSize.Width
                        : reservedRect.Right,
                    (style & PopupStyles.RightOrBottomJustify) != PopupStyles.None
                        ? spanRectInScreenCoordinates.Bottom - desiredSize.Height
                        : spanRectInScreenCoordinates.Top, desiredSize, screenRect);
            return ShiftHorizontallyToFitScreen(
                (style & PopupStyles.RightOrBottomJustify) != PopupStyles.None
                    ? spanRectInScreenCoordinates.Right - desiredSize.Width
                    : spanRectInScreenCoordinates.Left,
                (style & PopupStyles.PreferLeftOrTopPosition) != PopupStyles.None
                    ? reservedRect.Top - desiredSize.Height
                    : reservedRect.Bottom, desiredSize, screenRect);
        }

        internal static Rect ShiftHorizontallyToFitScreen(double x, double y, Size desiredSize, Rect screenRect)
        {
            if (x + desiredSize.Width > screenRect.Right)
                x = screenRect.Right - desiredSize.Width;
            if (x < screenRect.Left)
                x = screenRect.Left;
            return new Rect(x, y, desiredSize.Width, desiredSize.Height);
        }

        internal static Rect ShiftVerticallyToFitScreen(double x, double y, Size desiredSize, Rect screenRect)
        {
            if (y + desiredSize.Height > screenRect.Bottom)
                y = screenRect.Bottom - desiredSize.Height;
            if (y < screenRect.Top)
                y = screenRect.Top;
            return new Rect(x, y, desiredSize.Width, desiredSize.Height);
        }

        internal Point GetScreenPointFromTextXY(double x, double y)
        {
            return TextView.VisualElement.PointToScreen(new Point(x - TextView.ViewportLeft, y - TextView.ViewportTop));
        }

        internal bool InnerShouldClearToolTipOnMouseMove(Point mousePt)
        {
            if (mousePt.X >= 0.0 && mousePt.X < TextView.ViewportWidth && mousePt.Y >= 0.0 &&
                mousePt.Y < TextView.ViewportHeight)
            {
                var containingYcoordinate =
                    TextView.TextViewLines.GetTextViewLineContainingYCoordinate(mousePt.Y + TextView.ViewportTop);
                if (containingYcoordinate != null)
                {
                    var span = VisualSpan.GetSpan(TextView.TextSnapshot);
                    if (span.IntersectsWith(containingYcoordinate.ExtentIncludingLineBreak))
                    {
                        var xCoordinate = mousePt.X + TextView.ViewportLeft;
                        var positionFromXcoordinate =
                            containingYcoordinate.GetBufferPositionFromXCoordinate(xCoordinate, true);
                        var nullable = positionFromXcoordinate;
                        if (!nullable.HasValue && containingYcoordinate.LineBreakLength == 0 &&
                            containingYcoordinate.IsLastTextViewLineForSnapshotLine &&
                            containingYcoordinate.TextRight <= xCoordinate && xCoordinate <
                            containingYcoordinate.TextRight + containingYcoordinate.EndOfLineWidth)
                            return span.End < TextView.TextSnapshot.Length;
                        if (nullable.HasValue)
                            return !span.Contains(nullable.Value);
                    }
                }
            }

            return true;
        }

        internal bool ShouldClearToolTipOnMouseMove(Point mousePt)
        {
            if (!TextView.VisualElement.IsMouseOver)
                return true;
            return InnerShouldClearToolTipOnMouseMove(mousePt);
        }

        private void OnContentGotFocus(object sender, RoutedEventArgs e)
        {
            var gotFocus = GotFocus;
            gotFocus?.Invoke(sender, e);
        }

        private void OnContentLostFocus(object sender, RoutedEventArgs e)
        {
            var lostFocus = LostFocus;
            lostFocus?.Invoke(sender, e);
        }

        private void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextView.QueueSpaceReservationStackRefresh();
        }

        private void OnLocationChanged(object sender, EventArgs e)
        {
            Manager.RemoveAgent(this);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (MouseContainer != null)
            {
                MouseContainer.MouseLeave -= OnMouseLeave;
                MouseContainer = null;
            }

            var target = e.MouseDevice.Target;
            var flag = false;
            if (target == null || !TextView.IsMouseOverViewOrAdornments)
                flag = true;
            else if ((Style & PopupStyles.DismissOnMouseLeaveText) != PopupStyles.None &&
                     ShouldClearToolTipOnMouseMove(e.GetPosition(TextView.VisualElement)))
                flag = true;
            if (flag)
            {
                Manager.RemoveAgent(this);
            }
            else
            {
                MouseContainer = target;
                MouseContainer.MouseLeave += OnMouseLeave;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!Popup.IsVisible || !ShouldClearToolTipOnMouseMove(e.GetPosition(TextView.VisualElement)))
                return;
            Manager.RemoveAgent(this);
        }

        private void OnOutliningManager_RegionsCollapsed(object sender, RegionsCollapsedEventArgs e)
        {
            if (!Popup.IsVisible)
                return;
            foreach (var collapsedRegion in e.CollapsedRegions)
                if (VisualSpan.TextBuffer == collapsedRegion.Extent.TextBuffer)
                {
                    var currentSnapshot = VisualSpan.TextBuffer.CurrentSnapshot;
                    if (VisualSpan.GetSpan(currentSnapshot)
                        .IntersectsWith(collapsedRegion.Extent.GetSpan(currentSnapshot)))
                        Manager.RemoveAgent(this);
                }
        }

        private void OnViewFocusLost(object sender, EventArgs e)
        {
            if (!Popup.IsVisible)
                return;
            Manager.RemoveAgent(this);
        }

        private void RegisterForEvents()
        {
            if ((Style & (PopupStyles.DismissOnMouseLeaveText | PopupStyles.DismissOnMouseLeaveTextOrContent)) !=
                PopupStyles.None)
            {
                TextView.VisualElement.PreviewMouseMove += OnMouseMove;
                MouseContainer = Mouse.DirectlyOver;
                if (MouseContainer != null)
                    MouseContainer.MouseLeave += OnMouseLeave;
            }

            TextView.LostAggregateFocus += OnViewFocusLost;
            Popup.Content.LostFocus += OnContentLostFocus;
            Popup.Content.GotFocus += OnContentGotFocus;
            if (Popup.Content is FrameworkElement content)
                content.SizeChanged += OnContentSizeChanged;
            var window = Window.GetWindow(TextView.VisualElement);
            if (window != null)
                window.LocationChanged += OnLocationChanged;
            if (OutliningManager == null)
                return;
            OutliningManager.RegionsCollapsed += OnOutliningManager_RegionsCollapsed;
        }

        private void UnregisterForEvents()
        {
            if ((Style & (PopupStyles.DismissOnMouseLeaveText | PopupStyles.DismissOnMouseLeaveTextOrContent)) !=
                PopupStyles.None)
            {
                TextView.VisualElement.PreviewMouseMove -= OnMouseMove;
                if (MouseContainer != null)
                    MouseContainer.MouseLeave -= OnMouseLeave;
            }

            TextView.LostAggregateFocus -= OnViewFocusLost;
            Popup.Content.LostFocus -= OnContentLostFocus;
            Popup.Content.GotFocus -= OnContentGotFocus;
            if (Popup.Content is FrameworkElement content)
                content.SizeChanged -= OnContentSizeChanged;
            var window = Window.GetWindow(TextView.VisualElement);
            if (window != null)
                window.LocationChanged -= OnLocationChanged;
            if (OutliningManager == null)
                return;
            OutliningManager.RegionsCollapsed -= OnOutliningManager_RegionsCollapsed;
        }

        internal abstract class PopupOrWindowContainer
        {
            protected UIElement PlacementTarget;

            public UIElement Content { get; }

            public abstract bool IsKeyboardFocusWithin { get; }

            public abstract bool IsVisible { get; }

            public abstract Size Size { get; }

            protected PopupOrWindowContainer(UIElement content, UIElement placementTarget)
            {
                Content = content;
                PlacementTarget = placementTarget;
            }

            public static PopupOrWindowContainer Create(UIElement content, UIElement placementTarget)
            {
                return new PopUpContainer(content, placementTarget);
            }

            public abstract void DisplayAt(Point point);

            public abstract void Hide();
        }

        private class PopUpContainer : PopupOrWindowContainer
        {
            private readonly Popup _popup = new NoTopmostPopup();
            private readonly ContentControl _popupContentContainer = new ContentControl();

            public override bool IsKeyboardFocusWithin => _popup.IsKeyboardFocusWithin;

            public override bool IsVisible => _popupContentContainer.Content != null;

            public override Size Size
            {
                get
                {
                    Content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    return Content.DesiredSize;
                }
            }

            public PopUpContainer(UIElement content, UIElement placementTarget)
                : base(content, placementTarget)
            {
                _popup.AllowsTransparency = true;
                _popup.PlacementTarget = PlacementTarget;
                _popup.Placement = PlacementMode.Absolute;
                _popup.UseLayoutRounding = true;
                _popup.SnapsToDevicePixels = true;
                _popup.Child = _popupContentContainer;
                _popup.Closed += OnPopupClosed;
                TextOptions.SetTextFormattingMode(_popup, TextFormattingMode.Display);
            }

            public override void DisplayAt(Point point)
            {
                _popup.HorizontalOffset = point.X * WpfHelper.DeviceScaleX;
                _popup.VerticalOffset = point.Y * WpfHelper.DeviceScaleY;
                if (Content == _popupContentContainer.Content)
                    return;
                if (VisualTreeHelper.GetParent(Content) == null)
                {
                    _popupContentContainer.Content = Content;
                    _popup.IsOpen = true;
                }
                else
                {
                    _popupContentContainer.Content = null;
                }
            }

            public override void Hide()
            {
                _popup.IsOpen = false;
            }

            private void OnPopupClosed(object sender, EventArgs e)
            {
                _popupContentContainer.Content = null;
            }

            private class NoTopmostPopup : Popup
            {
                protected override void OnOpened(EventArgs e)
                {
                    base.OnOpened(e);
                    WpfHelper.SetNoTopmost(Child);
                }
            }
        }
    }
}