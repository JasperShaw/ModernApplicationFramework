using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.Text.Ui.OverviewMargin;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal sealed class OverviewElement : Grid, IDisposable, IVerticalScrollBar, IOverviewMarginTest
    {
        private static readonly TimeSpan TipWindowDelay = TimeSpan.FromMilliseconds(400.0);
        private double _viewportTop = double.MinValue;
        private double _viewportBottom = double.MinValue;
        private readonly IList<Lazy<IOverviewTipManagerProvider, ITipMetadata>> _orderedTipFactoryProviders = new List<Lazy<IOverviewTipManagerProvider, ITipMetadata>>();
        private readonly IEditorFormatMap _formatMap;
        private readonly IOutliningManager _outliningManager;
        private readonly OverviewElementFactory _factory;
        private readonly Border _leftEdge;
        private readonly ElisionElement _elisionElement;
        private readonly DispatcherTimer _previewTimer;
        private Brush _backgroundBrush;
        private Brush _offScreenEdgeBrush;
        private Brush _visibleBrush;
        private bool _timerElapsed;
        private MouseEventArgs _lastEventArgs;
        private ITextView _tipView;
        private bool _showTips;
        private int _tipSize;
        private double _scrollBias;
        private IElisionBuffer _visualBuffer;
        private IList<IOverviewTipManager> _tipFactories;

        private OverviewElement(ITextViewHost textViewHost, OverviewElementFactory factory)
        {
            TextViewHost = textViewHost;
            _factory = factory;
            _outliningManager = factory.OutliningManagerService?.GetOutliningManager(textViewHost.TextView);
            _formatMap = factory.EditorFormatMapService.GetEditorFormatMap(textViewHost.TextView);
            Map = factory.ScrollMapFactory.Create(TextViewHost.TextView, true);
            var border = new Border
            {
                BorderThickness = new Thickness(0.0),
                Focusable = false,
                SnapsToDevicePixels = true
            };
            _leftEdge = border;
            _previewTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher) { Interval = TipWindowDelay };
            _previewTimer.Tick += OnTimerElapsed;
            MinWidth = 17.0;
            Children.Add(_leftEdge);
            _elisionElement = new ElisionElement(this);
            Children.Add(_elisionElement);
            ClipToBounds = true;
            var roles = TextViewHost.TextView.Roles;
            foreach (var orderedTipProvider in factory.OrderedTipProviders)
            {
                if (roles.ContainsAny(orderedTipProvider.Metadata.TextViewRoles))
                    _orderedTipFactoryProviders.Add(orderedTipProvider);
            }
        }

        public static OverviewElement Create(ITextViewHost textViewHost, OverviewElementFactory factory)
        {
            var overviewElement = new OverviewElement(textViewHost, factory);
            overviewElement.OnTextViewOptionsChanged(null, null);
            overviewElement.RegisterEvents();
            return overviewElement;
        }

        public ToolTip TipWindow { get; private set; }

        internal Brush ElisionBrush { get; private set; }

        internal Pen ElisionPen { get; private set; }

        internal Pen VisiblePen { get; private set; }

        internal ITextViewHost TextViewHost { get; }

        public void Dispose()
        {
            UnregisterEvents();
            _previewTimer.Tick -= OnTimerElapsed;
            CloseTip();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (ActualWidth <= 0.0)
                return;
            double xLeft;
            double num;
            if (SystemParameters.HighContrast)
            {
                xLeft = 4.0;
                num = 2.0;
            }
            else
                xLeft = num = 0.5;
            DrawRoundedRectangle(drawingContext, _visibleBrush, VisiblePen, 2.0, xLeft, _viewportTop, ActualWidth - num, _viewportBottom);
        }

        private void RegisterEvents()
        {
            SizeChanged += OnContainerMarginSizeChanged;
            _formatMap.FormatMappingChanged += OnFormatMappingChanged;
            OnFormatMappingChanged(null, null);
            TextViewHost.TextView.LayoutChanged += OnLayoutChanged;
            if (TextViewHost.TextView.Roles.Contains("STRUCTURED"))
            {
                _visualBuffer = TextViewHost.TextView.VisualSnapshot.TextBuffer as IElisionBuffer;
                if (_visualBuffer != null)
                    _visualBuffer.SourceSpansChanged += OnSourceSpansChanged;
            }
            Map.MappingChanged += OnMappingChanged;
            PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseMove += OnMouseMove;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            TextViewHost.TextView.TextDataModel.ContentTypeChanged += OnContentTypeChanged;
            TextViewHost.TextView.Options.OptionChanged += OnTextViewOptionsChanged;
        }

        private void UnregisterEvents()
        {
            _timerElapsed = false;
            _previewTimer.Stop();
            CloseTip();
            SizeChanged -= OnContainerMarginSizeChanged;
            _formatMap.FormatMappingChanged -= OnFormatMappingChanged;
            TextViewHost.TextView.LayoutChanged -= OnLayoutChanged;
            if (_visualBuffer != null)
                _visualBuffer.SourceSpansChanged -= OnSourceSpansChanged;
            Map.MappingChanged -= OnMappingChanged;
            PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
            MouseMove -= OnMouseMove;
            MouseEnter -= OnMouseEnter;
            MouseLeave -= OnMouseLeave;
            MouseLeftButtonUp -= OnMouseLeftButtonUp;
            TextViewHost.TextView.TextDataModel.ContentTypeChanged -= OnContentTypeChanged;
        }

        public IScrollMap Map { get; }

        public double GetYCoordinateOfBufferPosition(SnapshotPoint bufferPosition)
        {
            return GetYCoordinateOfScrollMapPosition(Map.GetCoordinateAtBufferPosition(bufferPosition));
        }

        public double GetYCoordinateOfScrollMapPosition(double scrollMapPosition)
        {
            var num1 = Map.Start - 0.5;
            var num2 = Map.End + 0.5 - num1;
            var trackSpanTop = TrackSpanTop;
            var trackSpanBottom = TrackSpanBottom;
            return trackSpanTop + (scrollMapPosition - num1) * (trackSpanBottom - trackSpanTop) / (num2 + Map.ThumbSize);
        }

        private double GetScrollMapPositionOfYCoordinate(double y)
        {
            var num1 = Map.Start - 0.5;
            var num2 = Map.End + 0.5 - num1;
            var trackSpanTop = TrackSpanTop;
            var trackSpanBottom = TrackSpanBottom;
            return num1 + (y - trackSpanTop) * (num2 + Map.ThumbSize) / (trackSpanBottom - trackSpanTop);
        }

        public SnapshotPoint GetBufferPositionOfYCoordinate(double y)
        {
            return Map.GetBufferPositionAtCoordinate(GetScrollMapPositionOfYCoordinate(y));
        }

        public double ThumbHeight => Map.ThumbSize / (Map.End - Map.Start + Map.ThumbSize) * TrackSpanHeight;

        public double TrackSpanTop { get; }

        public double TrackSpanBottom => ActualHeight;

        public double TrackSpanHeight => ActualHeight;

        public event EventHandler TrackSpanChanged;

        private void OnContainerMarginSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (TextViewHost.IsClosed)
                return;
            EnsureViewport();
            _elisionElement.InvalidateVisual();
            InvalidateVisual();
            // ISSUE: reference to a compiler-generated field
            var trackSpanChanged = TrackSpanChanged;
            if (trackSpanChanged == null)
                return;
            _factory.GuardedOperations.RaiseEvent(this, trackSpanChanged);
        }

        private void OnContentTypeChanged(object sender, TextDataModelContentTypeChangedEventArgs e)
        {
            _tipFactories = null;
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (e != null && !e.ChangedItems.Contains("TextView Background") && (!e.ChangedItems.Contains("OverviewMarginCollapsedRegion") && !e.ChangedItems.Contains("OverviewMarginBackground")) && !e.ChangedItems.Contains("OverviewMarginVisible"))
                return;
            _backgroundBrush = _formatMap.GetBrush("TextView Background", "Background");
            var brush1 = _formatMap.GetBrush("OverviewMarginVisible", "Foreground");
            _visibleBrush = _formatMap.GetBrush("OverviewMarginVisible", "Background");
            if (_visibleBrush != null)
            {
                VisiblePen = new Pen(brush1, SystemParameters.HighContrast ? 5.0 : 1.0);
                VisiblePen.Freeze();
            }
            ElisionBrush = _formatMap.GetBrush("OverviewMarginCollapsedRegion", "Background");
            Brush brush2;
            if (SystemParameters.HighContrast && (brush2 = _formatMap.GetBrush("OverviewMarginCollapsedRegion", "Foreground")) != null)
            {
                ElisionPen = new Pen(brush2, 2.0);
                ElisionPen.Freeze();
            }
            else
                ElisionPen = null;
            Background = _formatMap.GetBrush("OverviewMarginBackground", "Background");
            _offScreenEdgeBrush = _formatMap.GetBrush("OverviewMarginBackground", "Foreground");
            _leftEdge.BorderBrush = _offScreenEdgeBrush;
            _leftEdge.BorderThickness = new Thickness(1.0, 0.0, 0.0, 0.0);
            InvalidateVisual();
            _elisionElement.InvalidateVisual();
        }

        private void OnTextViewOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            _showTips = TextViewHost.TextView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowPreviewOptionId);
            _tipSize = TextViewHost.TextView.Options.GetOptionValue(DefaultTextViewHostOptions.PreviewSizeOptionId);
            EnsureViewport();
            _elisionElement.InvalidateVisual();
            InvalidateVisual();
        }

        private void OnSourceSpansChanged(object sender, ElisionSourceSpansChangedEventArgs e)
        {
            _elisionElement.InvalidateVisual();
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            EnsureViewport();
        }

        private void OnMappingChanged(object sender, EventArgs e)
        {
            EnsureViewport();
            _elisionElement.InvalidateVisual();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = TryHandleMouseLeftButtonDown(e.GetPosition(this), e.ClickCount);
        }

        internal bool TryHandleMouseLeftButtonDown(Point pt, int clickCount = 1)
        {
            CloseTip();
            if (pt.Y < TrackSpanTop || pt.Y > TrackSpanBottom)
                return false;
            CaptureMouse();
            if (pt.Y < _viewportTop || pt.Y > _viewportBottom || clickCount == 2)
            {
                _scrollBias = 0.0;
                TextViewHost.TextView.ScrollViewToYCoordinate(this, clickCount == 2 ? _outliningManager : null, pt.Y);
            }
            else
                _scrollBias = pt.Y - (_viewportTop + _viewportBottom) * 0.5;
            return true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);
            if (position.Y >= TrackSpanTop && position.Y <= TrackSpanBottom)
                TextViewHost.TextView.ScrollViewToYCoordinate(this, null, position.Y - _scrollBias);
            ReleaseMouseCapture();
        }

        private static bool FromTouch(MouseEventArgs e)
        {
            return e.StylusDevice != null;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (!_showTips || FromTouch(e))
                return;
            _timerElapsed = false;
            _lastEventArgs = e;
            _previewTimer.Start();
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            if (IsMouseCaptured)
                return;
            _timerElapsed = true;
            _previewTimer.Stop();
            ShowTip(_lastEventArgs);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _lastEventArgs = e;
            var position = e.GetPosition(this);
            if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
            {
                TextViewHost.TextView.ScrollViewToYCoordinate(this, null, position.Y - _scrollBias);
                e.Handled = true;
            }
            else
            {
                if (!_timerElapsed || FromTouch(e))
                    return;
                ShowTip(e);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _timerElapsed = false;
            _previewTimer.Stop();
            CloseTip();
        }

        internal static void DrawRectangle(DrawingContext drawingContext, Brush brush, Pen pen, double width, double x, double yTop, double yBottom)
        {
            if (brush == null && pen == null || yBottom - 1.0 <= yTop)
                return;
            drawingContext.DrawRectangle(brush, pen, new Rect(x, yTop, width, yBottom - yTop));
        }

        internal static void DrawRoundedRectangle(DrawingContext drawingContext, Brush brush, Pen pen, double radius, double xLeft, double yTop, double xRight, double yBottom)
        {
            if (brush == null && pen == null)
                return;
            drawingContext.DrawRoundedRectangle(brush, pen, new Rect(xLeft, yTop, xRight - xLeft, yBottom - yTop), radius, radius);
        }

        private double GetCoordinateOfLineBottom(ITextViewLine firstLine, ITextViewLine lastLine)
        {
            if (lastLine.EndIncludingLineBreak.Position < TextViewHost.TextView.TextSnapshot.Length || TextViewHost.TextView.ViewportHeight == 0.0)
                return Map.GetCoordinateAtBufferPosition(lastLine.End);
            return Map.End + Math.Floor(Map.ThumbSize * Math.Max(0.0, 1.0 - (lastLine.Bottom - firstLine.Bottom) / TextViewHost.TextView.ViewportHeight));
        }

        private void EnsureViewport()
        {
            if (TextViewHost.IsClosed || TextViewHost.TextView.InLayout)
                return;
            var textViewLines = TextViewHost.TextView.TextViewLines;
            var val1 = Math.Floor(GetYCoordinateOfScrollMapPosition(Map.GetCoordinateAtBufferPosition(textViewLines.FirstVisibleLine.Start) - 0.5)) + 0.5;
            var scrollMapPosition = GetCoordinateOfLineBottom(textViewLines.FirstVisibleLine, textViewLines.LastVisibleLine) + 0.5;
            var num1 = Math.Max(val1, Math.Ceiling(GetYCoordinateOfScrollMapPosition(scrollMapPosition)) - 0.5);
            if (num1 - val1 < 6.0)
            {
                var num2 = Math.Floor((val1 + num1) * 0.5) + 0.5;
                val1 = num2 - 3.0;
                num1 = num2 + 3.0;
            }
            if (val1 == _viewportTop && num1 == _viewportBottom)
                return;
            _viewportTop = val1;
            _viewportBottom = num1;
            InvalidateVisual();
        }

        private bool UpdateTip(MouseEventArgs e)
        {
            if (_tipSize > 0 && !TextViewHost.IsClosed && !TextViewHost.TextView.InLayout)
            {
                var positionOfYcoordinate = GetScrollMapPositionOfYCoordinate(e.GetPosition(this).Y);
                if (positionOfYcoordinate <= Map.End)
                {
                    if (_tipView == null)
                    {
                        _tipView = _factory.EditorFactory.CreateTextView(new PreviewTextViewModel(TextViewHost.TextView), _factory.EditorFactory.CreateTextViewRoleSet(new string[1]
                        {
              "ENHANCED_SCROLLBAR_PREVIEW"
                        }), _factory.EditorOptionsFactoryService.GlobalOptions);
                        _tipView.Options.SetOptionValue(DefaultTextViewOptions.IsViewportLeftClippedId, false);
                        _tipView.Options.SetOptionValue(DefaultViewOptions.AppearanceCategory, TextViewHost.TextView.Options.GetOptionValue(DefaultViewOptions.AppearanceCategory));
                    }
                    var positionAtCoordinate = Map.GetBufferPositionAtCoordinate(positionOfYcoordinate);
                    var containingLine = positionAtCoordinate.GetContainingLine();
                    if (TipWindow.IsOpen)
                    {
                        var content = TipWindow.Content as FrameworkElement;
                        if (content == _tipView.VisualElement && content.Tag == containingLine)
                            return true;
                    }
                    var num1 = _tipSize * _tipView.LineHeight;
                    _tipView.DisplayTextLineContainingBufferPosition(positionAtCoordinate, num1 * 0.5, ViewRelativePosition.Bottom, new double?(), new double?(num1));
                    if (_tipView.TextViewLines[_tipView.TextViewLines.Count - 1].Bottom < _tipView.ViewportBottom)
                        _tipView.DisplayTextLineContainingBufferPosition(_tipView.TextViewLines.FormattedSpan.End, 0.0, ViewRelativePosition.Bottom, new double?(), new double?(num1));
                    var num2 = double.MaxValue;
                    using (var enumerator = _tipView.TextViewLines.GetEnumerator())
                    {
                        label_17:
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            var snapshotPoint = current.Start;
                            var position1 = snapshotPoint.Position;
                            while (true)
                            {
                                var num3 = position1;
                                snapshotPoint = current.End;
                                var position2 = snapshotPoint.Position;
                                if (num3 < position2)
                                {
                                    if (char.IsWhiteSpace(current.Snapshot[position1]))
                                        ++position1;
                                    else
                                        break;
                                }
                                else
                                    goto label_17;
                            }
                            var left = current.GetCharacterBounds(new SnapshotPoint(current.Snapshot, position1)).Left;
                            if (left < num2)
                                num2 = left;
                        }
                    }
                    _tipView.ViewportLeft = num2 == double.MaxValue ? 0.0 : num2 * 0.75;
                    TipWindow.MinWidth = TipWindow.MaxWidth = Math.Floor(Math.Max(50.0, TextViewHost.TextView.ViewportWidth * (TextViewHost.TextView.ZoomLevel / 100.0) * 0.5));
                    TipWindow.MinHeight = TipWindow.MaxHeight = 8.0 + num1;
                    TipWindow.Content = _tipView.VisualElement;
                    _tipView.VisualElement.Tag = containingLine;
                    TipWindow.IsOpen = true;
                    return true;
                }
            }
            return false;
        }

        private void ShowTip(MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            if (_showTips && position.Y >= TrackSpanTop && (position.Y <= TrackSpanBottom && !FromTouch(e)))
                OpenTip(e);
            else
                CloseTip();
        }

        internal void OpenTip(MouseEventArgs e)
        {
            EnsureTipFactories();
            if (_tipFactories.Count <= 0 && _tipSize <= 0)
                return;
            if (TipWindow == null)
            {
                TipWindow = new ToolTip
                {
                    ClipToBounds = true,
                    Padding = new Thickness(0.0, 0.0, 0.0, 0.0),
                    Background = _backgroundBrush,
                    BorderThickness = new Thickness(2.0, 2.0, 2.0, 2.0),
                    BorderBrush = TextViewHost.TextView.FormattedLineSource.DefaultTextProperties.ForegroundBrush,
                    Placement = PlacementMode.Left,
                    PlacementTarget = TextViewHost.TextView.VisualElement,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    HorizontalOffset = -TextViewHost.TextView.ViewportLeft,
                    VerticalOffset = 0.0
                };
            }
            var num = TextViewHost.TextView.ZoomLevel / 100.0;
            TipWindow.PlacementRectangle = new Rect(TextViewHost.TextView.ViewportRight, e.GetPosition(this).Y / num, 0.0, 0.0);
            foreach (var tipFactory1 in _tipFactories)
            {
                var tipFactory = tipFactory1;
                if (_factory.GuardedOperations.CallExtensionPoint(() => tipFactory.UpdateTip(this, e, TipWindow), false))
                    return;
            }
            if (UpdateTip(e))
                return;
            CloseTip();
        }

        private void EnsureTipFactories()
        {
            if (_tipFactories != null)
                return;
            _tipFactories = new List<IOverviewTipManager>();
            foreach (var tipFactoryProvider in _orderedTipFactoryProviders)
            {
                if (tipFactoryProvider.Metadata.ContentTypes.Any(contentType => TextViewHost.TextView.TextDataModel.ContentType.IsOfType(contentType)))
                {
                    var overviewTipManager = tipFactoryProvider.Value.GetOverviewTipManager(TextViewHost);
                    if (overviewTipManager != null)
                        _tipFactories.Add(overviewTipManager);
                }
            }
        }

        internal void CloseTip()
        {
            if (TipWindow == null)
                return;
            TipWindow.Visibility = Visibility.Hidden;
            TipWindow.Content = null;
            TipWindow.IsOpen = false;
            TipWindow = null;
            if (_tipView == null)
                return;
            _tipView.Close();
            _tipView = null;
        }
    }
}
