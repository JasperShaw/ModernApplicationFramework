using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Docking.NativeMethods;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking.Controls
{
    public class DragUndockHeader : ContentControl, INonClientArea
    {

        public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent("DragStarted",
            RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteEventArgs>), typeof(DragUndockHeader));

        public static readonly RoutedEvent DragAbsoluteEvent = EventManager.RegisterRoutedEvent("DragAbsolute",
            RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteEventArgs>), typeof(DragUndockHeader));

        public static readonly RoutedEvent DragCompletedAbsoluteEvent =
            EventManager.RegisterRoutedEvent("DragCompletedAbsolute", RoutingStrategy.Bubble,
                typeof(EventHandler<DragAbsoluteCompletedEventArgs>), typeof(DragUndockHeader));
        public static readonly RoutedEvent DragDeltaEvent = Thumb.DragDeltaEvent.AddOwner(typeof(DragUndockHeader));

        public static readonly RoutedEvent DragHeaderClickedEvent =
            EventManager.RegisterRoutedEvent("DragHeaderClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(DragUndockHeader));

        public static readonly RoutedEvent DragHeaderContextMenuEvent =
            EventManager.RegisterRoutedEvent("DragHeaderContextMenu", RoutingStrategy.Bubble,
                typeof(EventHandler<DragUndockHeaderContextMenuEventArgs>), typeof(DragUndockHeader));



        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(LayoutContent), typeof(DragUndockHeader),
                new FrameworkPropertyMetadata(null, OnModelChanged));

        private static readonly DependencyPropertyKey LayoutItemPropertyKey
            = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(DragUndockHeader),
                new FrameworkPropertyMetadata((LayoutItem) null));

        public static readonly DependencyProperty IsWindowTitleBarProperty =
            DependencyProperty.Register(nameof(IsWindowTitleBar), typeof(bool), typeof(DragUndockHeader),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        public static readonly DependencyProperty IsDragEnabledProperty =
            DependencyProperty.Register(nameof(IsDragEnabled), typeof(bool), typeof(DragUndockHeader),
                new FrameworkPropertyMetadata(Boxes.BooleanTrue));

        private static readonly DependencyPropertyKey IsDraggingPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsDragging), typeof(bool), typeof(DragUndockHeader),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;


        public static readonly DependencyProperty LayoutItemProperty
            = LayoutItemPropertyKey.DependencyProperty;

        private HwndSource _currentSource;
        private bool _movedDuringDrag;
        private TabItem _tabItem;
        private Point _originalScreenPoint;
        private Point _lastScreenPoint;


        public LayoutItem LayoutItem => (LayoutItem) GetValue(LayoutItemProperty);

        public LayoutContent Model
        {
            get => (LayoutContent) GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public bool IsWindowTitleBar
        {
            get => (bool)GetValue(IsWindowTitleBarProperty);
            set => SetValue(IsWindowTitleBarProperty, Boxes.Box(value));
        }
        public bool IsDragEnabled
        {
            get => (bool)GetValue(IsDragEnabledProperty);
            set => SetValue(IsDragEnabledProperty, Boxes.Box(value));
        }

        public bool IsDragging
        {
            get => (bool)GetValue(IsDraggingProperty);
            protected set => SetValue(IsDraggingPropertyKey, Boxes.Box(value));
        }


        private HwndSource CurrentSource
        {
            get => _currentSource;
            set
            {
                if (_currentSource == value)
                    return;
                _currentSource?.RemoveHook(WndProc);
                _currentSource = value;
                _currentSource?.AddHook(WndProc);
            }
        }

        private bool IsInReorderableTabItem => _tabItem?.FindAncestor<ReorderTabPanel>() != null;

        static DragUndockHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragUndockHeader),
                new FrameworkPropertyMetadata(typeof(DragUndockHeader)));
        }

        public DragUndockHeader()
        {
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);
        }

        public int HitTest(Point point)
        {
            return IsWindowTitleBar ? 2 : 0;
        }

        public void CancelDrag()
        {
            if (!IsDragging)
                return;
            ReleaseCapture();
            RaiseDragCompletedAbsolute(_lastScreenPoint, false);
        }

        internal static Point GetMessagePoint()
        {
            var messagePos = User32.GetMessagePos();
            return new Point(NativeMethods.NativeMethods.LoWord(messagePos), NativeMethods.NativeMethods.HiWord(messagePos));
        }

        internal bool IsInTabItem => _tabItem != null;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var flag = false;
            _tabItem = this.FindAncestor<TabItem>();
            if (_tabItem != null)
            {
                var draggedTabInfo = DockingManager.Instance.DraggedTabInfo;
                if (draggedTabInfo != null && draggedTabInfo.DraggedViewElement == Model)
                {
                    flag = true;
                    var reorderTabPanel = _tabItem.Parent as ReorderTabPanel ?? VisualTreeHelper.GetParent(_tabItem) as ReorderTabPanel;
                    if (reorderTabPanel != null && draggedTabInfo != null && (draggedTabInfo.TabStrip != reorderTabPanel || reorderTabPanel.Children.Count != draggedTabInfo.TabRects.Count))
                    {
                        draggedTabInfo.TabStrip = reorderTabPanel;
                        draggedTabInfo.TabStrip.IsNotificationNeeded = true;
                    }
                }
            }
            if (!flag || !NativeMethods.NativeMethods.IsLeftButtonPressed())
                return;
            BeginDragging(NativeMethods.NativeMethods.GetCursorPos());
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!IsMouseCaptured || !IsDragging || !this.IsConnectedToPresentationSource())
                return;
            _movedDuringDrag = true;
            var screen = PointToScreen(e.GetPosition(this));
            RaiseEvent(new DragDeltaEventArgs(screen.X - _lastScreenPoint.X, screen.Y - _lastScreenPoint.Y));
            RaiseDragAbsolute(screen);
            if (IsOutsideSensitivity(screen))
            {
                if (_tabItem != null)
                    DockingManager.Instance.ComputeTabItemLengths(_tabItem);
                RaiseDragStarted(_originalScreenPoint);
            }

            _lastScreenPoint = screen;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.IsConnectedToPresentationSource() && IsDragEnabled && !IsWindowTitleBar)
            {
                BeginDragging(PointToScreen(e.GetPosition(this)));
                RaiseHeaderClicked();
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            CaptureMouse();
            base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured && IsDragging && this.IsConnectedToPresentationSource())
            {
                _lastScreenPoint = PointToScreen(e.GetPosition(this));
                CompleteDrag();
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (!IsDragging)
            {
                if (IsMouseCaptured)
                    ReleaseMouseCapture();
                if (PresentationSource.FromVisual(this) is HwndSource source)
                {
                    if (ShouldShowWindowMenu())
                        ModernChromeWindow.ShowWindowMenu(source, this, e.GetPosition(this), RenderSize);
                    else
                        RaiseEvent(new DragUndockHeaderContextMenuEventArgs(DragHeaderContextMenuEvent, e.GetPosition(this)));
                }
                e.Handled = true;
            }
            else
                base.OnMouseRightButtonUp(e);
        }

        protected virtual bool ShouldShowWindowMenu()
        {
            return IsWindowTitleBar && !IsInTabItem;
        }

        private bool IsOutsideSensitivity(Point point)
        {
            var flag = IsInReorderableTabItem;
            var draggedTabInfo = DockingManager.Instance.DraggedTabInfo;
            if (draggedTabInfo != null)
                flag = draggedTabInfo.TabStripRect.Contains(point);
            point.Offset(-_originalScreenPoint.X, -_originalScreenPoint.Y);
            if (flag)
                return false;
            if (Math.Abs(point.X) <= SystemParameters.MinimumHorizontalDragDistance)
                return Math.Abs(point.Y) > SystemParameters.MinimumVerticalDragDistance;
            return true;
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            SetLayoutItem(Model?.Root.Manager.GetLayoutItemFromModel(Model));
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(LayoutItemPropertyKey, value);
        }

        protected void RaiseDragStarted(Point point)
        {
            RaiseEvent(new DragAbsoluteEventArgs(DragStartedEvent, point));
        }

        internal void RaiseDragAbsolute(Point point)
        {
            RaiseEvent(new DragAbsoluteEventArgs(DragAbsoluteEvent, point));
        }

        protected void RaiseDragCompletedAbsolute(Point point, bool isCompleted)
        {
            RaiseEvent(new DragAbsoluteCompletedEventArgs(DragCompletedAbsoluteEvent, point, isCompleted));
        }

        protected void RaiseHeaderClicked()
        {
            RaiseEvent(new RoutedEventArgs(DragHeaderClickedEvent));
        }

        private void BeginDragging(Point screenPoint)
        {
            if (!CaptureMouse())
                return;
            IsDragging = true;
            _originalScreenPoint = screenPoint;
            _lastScreenPoint = screenPoint;
            _movedDuringDrag = false;
        }

        private void CompleteDrag()
        {
            if (!IsDragging)
                return;
            ReleaseCapture();
            RaiseDragCompletedAbsolute(_lastScreenPoint, _movedDuringDrag);
        }

        private void ReleaseCapture()
        {
            if (!IsDragging)
                return;
            ClearValue(IsDraggingPropertyKey);
            if (!IsMouseCaptured)
                return;
            ReleaseMouseCapture();
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DragUndockHeader) d).OnModelChanged(e);
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs args)
        {
            CurrentSource = args.NewSource as HwndSource;
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 534:
                    WmMoving(ref handled);
                    break;
                case 561:
                    WmEnterSizeMove();
                    break;
                case 562:
                    WmExitSizeMove(ref handled);
                    break;
            }
            return IntPtr.Zero;
        }

        private void WmMoving(ref bool handled)
        {
            if (!IsWindowTitleBar)
                return;
            _movedDuringDrag = true;
            RaiseDragAbsolute(GetMessagePoint());
            handled = CurrentSource == null || CurrentSource.IsDisposed;
        }

        private void WmEnterSizeMove()
        {
            if (!IsWindowTitleBar)
                return;
            _movedDuringDrag = false;
            RaiseDragStarted(GetMessagePoint());
        }

        private void WmExitSizeMove(ref bool handled)
        {
            if (!IsWindowTitleBar)
                return;
            RaiseDragCompletedAbsolute(GetMessagePoint(), _movedDuringDrag);
            handled = CurrentSource == null || CurrentSource.IsDisposed;
        }
    }

    public class DragAbsoluteEventArgs : RoutedEventArgs
    {
        public DragAbsoluteEventArgs(RoutedEvent evt, Point point)
            : base(evt)
        {
            ScreenPoint = point;
        }

        public Point ScreenPoint { get; }
    }

    public class DragAbsoluteCompletedEventArgs : DragAbsoluteEventArgs
    {
        public DragAbsoluteCompletedEventArgs(RoutedEvent evt, Point point, bool isCompleted)
            : base(evt, point)
        {
            IsCompleted = isCompleted;
        }

        public bool IsCompleted { get; set; }
    }

    public class DragUndockHeaderContextMenuEventArgs : RoutedEventArgs
    {
        public DragUndockHeaderContextMenuEventArgs(RoutedEvent evt, Point headerPoint)
            : base(evt)
        {
            HeaderPoint = headerPoint;
        }

        public Point HeaderPoint { get; }
    }
}