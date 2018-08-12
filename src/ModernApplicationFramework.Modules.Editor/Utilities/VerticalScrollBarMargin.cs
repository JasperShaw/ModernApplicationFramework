using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal class VerticalScrollBarMargin : ShiftClickScrollBarMargin, IVerticalScrollBar
    {
        private readonly PerformanceBlockMarker _performanceBlockMarker;
        private int _initialScrollBarDispatchDelay;
        private DateTime _lastUpdateUtc;
        private int _scrollBarDispatchDelay;
        private bool _scrollShortDistanceSynchronously;
        private double _trackBottom = double.NaN;
        private double _trackTop = double.NaN;
        private DispatcherTimer _updateTimer;

        public event EventHandler TrackSpanChanged;

        public override bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return TextView.Options.IsVerticalScrollBarEnabled();
            }
        }

        public IScrollMap Map { get; }

        public ITextView TextView { get; }

        public double ThumbHeight
        {
            get
            {
                EnsureTrackTopAndBottom();
                return Map.ThumbSize / (Map.End - Map.Start + Map.ThumbSize) * (_trackBottom - _trackTop);
            }
        }

        public double TrackSpanBottom
        {
            get
            {
                EnsureTrackTopAndBottom();
                return _trackBottom;
            }
        }

        public double TrackSpanHeight
        {
            get
            {
                EnsureTrackTopAndBottom();
                return _trackBottom - _trackTop;
            }
        }

        public double TrackSpanTop
        {
            get
            {
                EnsureTrackTopAndBottom();
                return _trackTop;
            }
        }

        internal VerticalScrollBarMargin(ITextView textView, IScrollMap scrollMap, string name,
            PerformanceBlockMarker performanceBlockMarker)
            : base(Orientation.Vertical, name)
        {
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
            Map = scrollMap;
            _performanceBlockMarker = performanceBlockMarker;
            Name = "EditorUIVerticalScrollbar";
            Orientation = Orientation.Vertical;
            SmallChange = 1.0;
            MinWidth = 15.0;
            HorizontalAlignment = HorizontalAlignment.Center;
            OnOptionsChanged(null, null);
            textView.Options.OptionChanged += OnOptionsChanged;
            IsVisibleChanged += (sender, e) =>
            {
                if ((bool) e.NewValue)
                {
                    if (TextView.IsClosed)
                        return;
                    TextView.LayoutChanged += OnEditorLayoutChanged;
                    SizeChanged += OnSizeChanged;
                    LeftShiftClick += OnLeftShiftClick;
                    Scroll += OnVerticalScrollBarScrolled;
                    if (TextView.InLayout)
                        return;
                    OnEditorLayoutChanged(null, null);
                }
                else
                {
                    TextView.LayoutChanged -= OnEditorLayoutChanged;
                    SizeChanged -= OnSizeChanged;
                    LeftShiftClick -= OnLeftShiftClick;
                    Scroll -= OnVerticalScrollBarScrolled;
                }
            };
            SetResourceReference(StyleProperty, typeof(ScrollBar));
        }

        public SnapshotPoint GetBufferPositionOfYCoordinate(double y)
        {
            EnsureTrackTopAndBottom();
            var start = Map.Start;
            var num = Map.End - start;
            return Map.GetBufferPositionAtCoordinate(start + (y - _trackTop) * (num + Map.ThumbSize) /
                                                     (_trackBottom - _trackTop));
        }

        public double GetYCoordinateOfBufferPosition(SnapshotPoint bufferPosition)
        {
            return GetYCoordinateOfScrollMapPosition(Map.GetCoordinateAtBufferPosition(bufferPosition));
        }

        public double GetYCoordinateOfScrollMapPosition(double scrollMapPosition)
        {
            EnsureTrackTopAndBottom();
            var start = Map.Start;
            var num = Map.End - start;
            return _trackTop + (scrollMapPosition - start) * (_trackBottom - _trackTop) / (num + Map.ThumbSize);
        }

        public override void OnDispose()
        {
            TextView.Options.OptionChanged -= OnOptionsChanged;
        }

        public virtual void OnEditorLayoutChanged(object sender, EventArgs e)
        {
            if (TextView.IsClosed)
                return;
            var num1 = 0.0;
            var firstVisibleLine = TextView.TextViewLines.FirstVisibleLine;
            if (firstVisibleLine.Top < TextView.ViewportTop)
                num1 = 0.25;
            var num2 = Map.GetCoordinateAtBufferPosition(firstVisibleLine.Start) + num1;
            var num3 = Math.Max(1.0,
                GetCoordinateOfLineBottom(firstVisibleLine, TextView.TextViewLines.LastVisibleLine) - num2);
            var thumbSize = Map.ThumbSize;
            Maximum = Math.Max((Minimum = Map.Start) + 1.0, Map.End - num3 + thumbSize);
            ViewportSize = num3;
            LargeChange = num3;
            Value = num2;
        }

        public virtual void ScrollToCoordinate(double coordinate)
        {
            TextView.DisplayTextLineContainingBufferPosition(Map.GetBufferPositionAtCoordinate(coordinate), 0.0,
                ViewRelativePosition.Top);
        }

        internal void OnVerticalScrollBarScrolled(object sender, ScrollEventArgs e)
        {
            if (TextView.IsClosed)
                return;
            var scrollEventType = e.ScrollEventType;
            using (CreateScrollMarker(scrollEventType.ToString()))
            {
                scrollEventType = e.ScrollEventType;
                switch (scrollEventType)
                {
                    case ScrollEventType.LargeDecrement:
                        TextView.ViewScroller.ScrollViewportVerticallyByPixels(TextView.ViewportHeight);
                        break;
                    case ScrollEventType.LargeIncrement:
                        TextView.ViewScroller.ScrollViewportVerticallyByPixels(-TextView.ViewportHeight);
                        break;
                    case ScrollEventType.SmallDecrement:
                        TextView.ViewScroller.ScrollViewportVerticallyByPixels(TextView.LineHeight);
                        break;
                    case ScrollEventType.SmallIncrement:
                        TextView.ViewScroller.ScrollViewportVerticallyByPixels(-TextView.LineHeight);
                        break;
                    case ScrollEventType.ThumbTrack:
                        OnThumbScroll();
                        break;
                    default:
                        ScrollToCoordinate(e.NewValue);
                        break;
                }
            }
        }

        private IDisposable CreateScrollMarker(string scrollKind)
        {
            return _performanceBlockMarker.CreateBlock("TextEditor.Scroll." + scrollKind);
        }

        private void EnsureTrackTopAndBottom()
        {
            if (!double.IsNaN(_trackTop))
                return;
            _trackTop = 0.0;
            _trackBottom = ActualHeight;
            if (VisualTreeHelper.GetChildrenCount(this) != 1)
                return;
            var child = VisualTreeHelper.GetChild(this, 0);
            var grid = child as Grid;
            if (grid == null)
                if (child is Border border)
                    grid = border.Child as Grid;
            if (grid == null || grid.RowDefinitions.Count != 3)
                return;
            var rowDefinition = grid.RowDefinitions[1];
            _trackTop = rowDefinition.Offset;
            _trackBottom = _trackTop + rowDefinition.ActualHeight;
        }

        private double GetCoordinateOfLineBottom(ITextViewLine firstLine, ITextViewLine lastLine)
        {
            if (lastLine.EndIncludingLineBreak.Position < TextView.TextSnapshot.Length ||
                TextView.ViewportHeight == 0.0)
                return Map.GetCoordinateAtBufferPosition(lastLine.End);
            return Map.End + Math.Floor(Map.ThumbSize *
                                        Math.Max(0.0,
                                            1.0 - (lastLine.Bottom - firstLine.Bottom) / TextView.ViewportHeight));
        }

        private void OnLeftShiftClick(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TextView.IsClosed)
                return;
            using (CreateScrollMarker("VerticalScrollShiftClick"))
            {
                ScrollToCoordinate(e.NewValue);
            }
        }

        private void OnOptionsChanged(object sender, EventArgs e)
        {
            Visibility = Enabled ? Visibility.Visible : Visibility.Hidden;
            if (!TextView.Options.IsSimpleGraphicsEnabled())
            {
                _scrollBarDispatchDelay = 15;
                _initialScrollBarDispatchDelay = 30;
                _scrollShortDistanceSynchronously = true;
            }
            else
            {
                _scrollBarDispatchDelay = 150;
                _initialScrollBarDispatchDelay = 150;
                _scrollShortDistanceSynchronously = true;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (TextView.IsClosed)
                return;
            _trackTop = _trackBottom = double.NaN;
            var trackSpanChanged = TrackSpanChanged;
            trackSpanChanged?.Invoke(this, new EventArgs());
        }

        private void OnThumbScroll()
        {
            if (_scrollShortDistanceSynchronously &&
                PositionIsShortDistanceFromVisibleRegion(Map.GetBufferPositionAtCoordinate(Value)))
            {
                ScrollToCurrentThumbPosition();
            }
            else
            {
                if (_updateTimer != null)
                    return;
                var barDispatchDelay = _scrollBarDispatchDelay;
                if (_lastUpdateUtc == new DateTime() ||
                    DateTime.UtcNow - _lastUpdateUtc > TimeSpan.FromMilliseconds(500.0))
                    barDispatchDelay = _initialScrollBarDispatchDelay;
                _updateTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
                {
                    Interval = TimeSpan.FromMilliseconds(barDispatchDelay)
                };
                _updateTimer.Tick += (sender, args) => ScrollToCurrentThumbPosition();
                _updateTimer.Start();
            }
        }

        private bool PositionIsShortDistanceFromVisibleRegion(SnapshotPoint point)
        {
            var thumbSize = Map.ThumbSize;
            return Math.Abs(Map.GetCoordinateAtBufferPosition(TextView.TextViewLines.FirstVisibleLine.Start) -
                            Map.GetCoordinateAtBufferPosition(point)) <= thumbSize;
        }

        private void ScrollToCurrentThumbPosition()
        {
            if (TextView == null || TextView.IsClosed)
                return;
            ScrollToCoordinate(Value);
            if (_updateTimer != null)
            {
                _updateTimer.Stop();
                _updateTimer = null;
            }

            _lastUpdateUtc = DateTime.UtcNow;
        }
    }
}