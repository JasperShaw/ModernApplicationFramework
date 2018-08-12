using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Modules.Editor.Implementation;
using ModernApplicationFramework.Modules.Editor.Utilities;
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
    internal class OverviewMargin : ContainerMargin, IVerticalScrollBar, IOverviewMarginTest
    {
        private static readonly TimeSpan TipWindowDelay = TimeSpan.FromMilliseconds(400.0);
        internal double CaretPosition = double.MinValue;
        internal double ViewportTop = double.MinValue;
        internal double ViewportBottom = double.MinValue;
        private readonly IList<Lazy<IOverviewTipManagerProvider, ITipMetadata>> _orderedTipFactoryProviders = new List<Lazy<IOverviewTipManagerProvider, ITipMetadata>>();
        internal const double ScrollBarWidth = 17.0;
        private Brush _backgroundBrush;
        private Brush _offScreenBrush;
        private Brush _visibleBrush;
        private Pen _visiblePen;
        private Pen _caretPen;
        private readonly IOutliningManager _outliningManager;
        private readonly IEditorFormatMap _formatMap;
        private readonly BackgroundBarMargin _backgroundBar;
        private readonly ScrollMapWrapper _scrollMap;
        private readonly CaretElement _caretElement;
        private readonly ElisionElement _elisionElement;
        private readonly DispatcherTimer _previewTimer;
        private bool _timerElapsed;
        private MouseEventArgs _lastEventArgs;
        internal ToolTip _tipWindow;
        private ITextView _tipView;
        private readonly OverviewMarginProvider _provider;
        private bool _useEnhancedScrollBar;
        private bool _showTips;
        private bool _showCaret;
        private int _tipSize;
        private double _scrollBias;
        private IElisionBuffer _visualBuffer;
        private IList<IOverviewTipManager> _tipFactories;
        private Point? _lastRightMouseDown;

        private OverviewMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin, OverviewMarginProvider myProvider)
            : base("VerticalScrollBar", Orientation.Vertical, textViewHost, myProvider.GuardedOperations, myProvider.MarginState)
        {
            _provider = myProvider;
            _outliningManager = myProvider.OutliningManagerService?.GetOutliningManager(textViewHost.TextView);
            _formatMap = myProvider.EditorFormatMapService.GetEditorFormatMap(textViewHost.TextView);
            _scrollMap = new ScrollMapWrapper(textViewHost.TextView, myProvider);
            _backgroundBar = new BackgroundBarMargin(textViewHost.TextView, _scrollMap, "VerticalScrollBar", myProvider.PerformanceBlockMarker);
            _previewTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher) {Interval = TipWindowDelay};
            _previewTimer.Tick += OnTimerElapsed;
            var menu = new ContextMenu {PlacementTarget = this};

            //TODO: Text

            CreateMenuItem(menu, "Scroll Here", false, false, new MenuCommand(o => ScrollViewToYCoordinate((_lastRightMouseDown ?? Mouse.GetPosition(this)).Y, false)));
            menu.Items.Add(new Separator());
            CreateMenuItem(menu, "Scroll Top", false, false, new MenuCommand(_param1 => _backgroundBar.OnVerticalScrollBarScrolled(_backgroundBar, new ScrollEventArgs(ScrollEventType.First, 0.0))));
            CreateMenuItem(menu, "Scroll Bottom", false, false, new MenuCommand(_param1 => _backgroundBar.OnVerticalScrollBarScrolled(_backgroundBar, new ScrollEventArgs(ScrollEventType.Last, _scrollMap.End))));
            menu.Items.Add(new Separator());
            CreateMenuItem(menu, "Scroll Page up", false, false, new MenuCommand(_param1 => _backgroundBar.OnVerticalScrollBarScrolled(_backgroundBar, new ScrollEventArgs(ScrollEventType.LargeDecrement, 0.0))));
            CreateMenuItem(menu, "Scroll Page down", false, false, new MenuCommand(_param1 => _backgroundBar.OnVerticalScrollBarScrolled(_backgroundBar, new ScrollEventArgs(ScrollEventType.LargeIncrement, 0.0))));
            menu.Items.Add(new Separator());
            CreateMenuItem(menu, "Scroll Up", false, false, new MenuCommand(_param1 => _backgroundBar.OnVerticalScrollBarScrolled(_backgroundBar, new ScrollEventArgs(ScrollEventType.SmallDecrement, 0.0))));
            CreateMenuItem(menu, "Scroll Down", false, false, new MenuCommand(_param1 => _backgroundBar.OnVerticalScrollBarScrolled(_backgroundBar, new ScrollEventArgs(ScrollEventType.SmallIncrement, 0.0))));
            ContextMenu = menu;
            menu.IsVisibleChanged += (s, e) => _lastRightMouseDown = ContextMenu.IsVisible ? Mouse.GetPosition(this) : new Point?();
            textViewHost.TextView.Properties.AddProperty("OverviewMarginContextMenu", menu);
            MinWidth = 17.0;
            Children.Add(_backgroundBar);
            _elisionElement = new ElisionElement(this);
            Children.Add(_elisionElement);
            _caretElement = new CaretElement(this);
            Children.Add(_caretElement);
            ClipToBounds = true;
            var roles = TextViewHost.TextView.Roles;
            foreach (var orderedTipProvider in myProvider.OrderedTipProviders)
            {
                if (roles.ContainsAny(orderedTipProvider.Metadata.TextViewRoles))
                    _orderedTipFactoryProviders.Add(orderedTipProvider);
            }
            TextViewHost.TextView.Options.OptionChanged += OnTextViewOptionsChanged;
        }

        public Brush ElisionBrush { get; private set; }

        public static OverviewMargin Create(ITextViewHost textViewHost, ITextViewMargin containerMargin, OverviewMarginProvider myProvider)
        {
            var overviewMargin = new OverviewMargin(textViewHost, containerMargin, myProvider);
            overviewMargin.Initialize();
            return overviewMargin;
        }

        public override bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return TextViewHost.TextView.Options.IsVerticalScrollBarEnabled();
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            OnTextViewOptionsChanged(null, null);
        }

        protected override void AddMargins(IList<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>> providers, List<Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin>> oldMargins)
        {
            base.AddMargins(providers, oldMargins);
            if (ColumnDefinitions.Count <= 0)
                return;
            SetColumnSpan(_backgroundBar, ColumnDefinitions.Count);
        }

        protected override bool HasVisibleChild()
        {
            return true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (ActualWidth <= 0.0 || !_useEnhancedScrollBar)
                return;
            DrawRoundedRectangle(drawingContext, _visibleBrush, _visiblePen, 2.0, 0.5, ViewportTop, ActualWidth - 0.5, ViewportBottom);
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            SizeChanged += OnContainerMarginSizeChanged;
            _formatMap.FormatMappingChanged += OnFormatMappingChanged;
            OnFormatMappingChanged(null, null);
            TextViewHost.TextView.LayoutChanged += OnLayoutChanged;
            TextViewHost.TextView.Caret.PositionChanged += OnCaretPositionChanged;
            if (TextViewHost.TextView.Roles.Contains("STRUCTURED"))
            {
                _visualBuffer = TextViewHost.TextView.VisualSnapshot.TextBuffer as IElisionBuffer;
                if (_visualBuffer != null)
                    _visualBuffer.SourceSpansChanged += OnSourceSpansChanged;
            }
            _scrollMap.MappingChanged += OnMappingChanged;
            PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            PreviewMouseRightButtonUp += OnMouseRightButtonUp;
            MouseMove += OnMouseMove;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            TextViewHost.TextView.TextDataModel.ContentTypeChanged += OnContentTypeChanged;
        }

        protected override void UnregisterEvents()
        {
            base.UnregisterEvents();
            _timerElapsed = false;
            _previewTimer.Stop();
            CloseTip();
            SizeChanged -= OnContainerMarginSizeChanged;
            _formatMap.FormatMappingChanged -= OnFormatMappingChanged;
            TextViewHost.TextView.LayoutChanged -= OnLayoutChanged;
            TextViewHost.TextView.Caret.PositionChanged -= OnCaretPositionChanged;
            if (_visualBuffer != null)
                _visualBuffer.SourceSpansChanged -= OnSourceSpansChanged;
            _scrollMap.MappingChanged -= OnMappingChanged;
            PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
            PreviewMouseRightButtonUp -= OnMouseRightButtonUp;
            MouseMove -= OnMouseMove;
            MouseEnter -= OnMouseEnter;
            MouseLeave -= OnMouseLeave;
            MouseLeftButtonUp -= OnMouseLeftButtonUp;
            TextViewHost.TextView.TextDataModel.ContentTypeChanged -= OnContentTypeChanged;
        }

        protected override void Close()
        {
            TextViewHost.TextView.Options.OptionChanged -= OnTextViewOptionsChanged;
            _previewTimer.Tick -= OnTimerElapsed;
            CloseTip();
            base.Close();
        }

        public IScrollMap Map => _scrollMap;

        public double GetYCoordinateOfBufferPosition(SnapshotPoint bufferPosition)
        {
            return GetYCoordinateOfScrollMapPosition(_scrollMap.GetCoordinateAtBufferPosition(bufferPosition));
        }

        public double GetYCoordinateOfScrollMapPosition(double scrollMapPosition)
        {
            var num1 = _scrollMap.Start - 0.5;
            var num2 = _scrollMap.End + 0.5 - num1;
            var trackSpanTop = TrackSpanTop;
            var trackSpanBottom = TrackSpanBottom;
            return trackSpanTop + (scrollMapPosition - num1) * (trackSpanBottom - trackSpanTop) / (num2 + _scrollMap.ThumbSize);
        }

        private double GetScrollMapPositionOfYCoordinate(double y)
        {
            var num1 = _scrollMap.Start - 0.5;
            var num2 = _scrollMap.End + 0.5 - num1;
            var trackSpanTop = TrackSpanTop;
            var trackSpanBottom = TrackSpanBottom;
            return num1 + (y - trackSpanTop) * (num2 + _scrollMap.ThumbSize) / (trackSpanBottom - trackSpanTop);
        }

        public SnapshotPoint GetBufferPositionOfYCoordinate(double y)
        {
            return _scrollMap.GetBufferPositionAtCoordinate(GetScrollMapPositionOfYCoordinate(y));
        }

        public double ThumbHeight => _scrollMap.ThumbSize / (_scrollMap.End - _scrollMap.Start + _scrollMap.ThumbSize) * TrackSpanHeight;

        public double TrackSpanTop
        {
            get
            {
                if (!_useEnhancedScrollBar)
                    return _backgroundBar.TrackSpanTop;
                return 0.0;
            }
        }

        public double TrackSpanBottom
        {
            get
            {
                if (!_useEnhancedScrollBar)
                    return _backgroundBar.TrackSpanBottom;
                return ActualHeight;
            }
        }

        public double TrackSpanHeight
        {
            get
            {
                if (!_useEnhancedScrollBar)
                    return _backgroundBar.TrackSpanHeight;
                return ActualHeight;
            }
        }

        public event EventHandler TrackSpanChanged;

        internal void DrawCaret(DrawingContext drawingContext)
        {
            if (!_showCaret || _caretPen == null)
                return;
            drawingContext.DrawLine(_caretPen, new Point(0.0, CaretPosition), new Point(ActualWidth, CaretPosition));
        }

        private bool UpdateTip(MouseEventArgs e)
        {
            if (_tipSize > 0)
            {
                var positionOfYcoordinate = GetScrollMapPositionOfYCoordinate(e.GetPosition(this).Y);
                if (positionOfYcoordinate <= _scrollMap.End)
                {
                    if (_tipView == null)
                    {
                        _tipView = _provider.EditorFactory.CreateTextView(new PreviewTextViewModel(TextViewHost.TextView), _provider.EditorFactory.CreateTextViewRoleSet("ENHANCED_SCROLLBAR_PREVIEW"), _provider.EditorOptionsFactoryService.GlobalOptions);
                        _tipView.Options.SetOptionValue(DefaultTextViewOptions.IsViewportLeftClippedId, false);
                        _tipView.Options.SetOptionValue(DefaultViewOptions.AppearanceCategory, TextViewHost.TextView.Options.GetOptionValue(DefaultViewOptions.AppearanceCategory));
                    }
                    var positionAtCoordinate = _scrollMap.GetBufferPositionAtCoordinate(positionOfYcoordinate);
                    var containingLine = positionAtCoordinate.GetContainingLine();
                    if (_tipWindow.IsOpen)
                    {
                        var content = _tipWindow.Content as FrameworkElement;
                        if (content == _tipView.VisualElement && content.Tag == containingLine)
                            return true;
                    }
                    var num1 = _tipSize * _tipView.LineHeight;
                    _tipView.DisplayTextLineContainingBufferPosition(positionAtCoordinate, num1 * 0.5, ViewRelativePosition.Bottom, new double?(), num1);
                    if (_tipView.TextViewLines[_tipView.TextViewLines.Count - 1].Bottom < _tipView.ViewportBottom)
                        _tipView.DisplayTextLineContainingBufferPosition(_tipView.TextViewLines.FormattedSpan.End, 0.0, ViewRelativePosition.Bottom, new double?(), num1);
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
                    _tipWindow.MinWidth = _tipWindow.MaxWidth = Math.Floor(Math.Max(50.0, TextViewHost.TextView.ViewportWidth * (TextViewHost.TextView.ZoomLevel / 100.0) * 0.5));
                    _tipWindow.MinHeight = _tipWindow.MaxHeight = 8.0 + num1;
                    _tipWindow.Content = _tipView.VisualElement;
                    _tipView.VisualElement.Tag = containingLine;
                    _tipWindow.IsOpen = true;
                    return true;
                }
            }
            return false;
        }

        private void OnContainerMarginSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (TextViewHost.IsClosed)
                return;
            EnsureCaret();
            EnsureViewport();
            _caretElement.InvalidateVisual();
            _elisionElement.InvalidateVisual();
            InvalidateVisual();
            var trackSpanChanged = TrackSpanChanged;
            if (trackSpanChanged == null)
                return;
            _provider.GuardedOperations.RaiseEvent(this, trackSpanChanged);
        }

        private void OnContentTypeChanged(object sender, TextDataModelContentTypeChangedEventArgs e)
        {
            _tipFactories = null;
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (e != null && !e.ChangedItems.Contains("TextView Background") && !e.ChangedItems.Contains("OverviewMarginCaret") && !e.ChangedItems.Contains("OverviewMarginCollapsedRegion") && (!e.ChangedItems.Contains("OverviewMarginBackground") && !e.ChangedItems.Contains("OverviewMarginVisible")))
                return;
            _backgroundBrush = GetBrush("TextView Background", "Background");
            ElisionBrush = GetBrush("OverviewMarginCollapsedRegion", "Background");
            _offScreenBrush = GetBrush("OverviewMarginBackground", "Background");
            _visibleBrush = GetBrush("OverviewMarginVisible", "Background");
            var brush1 = GetBrush("OverviewMarginVisible", "Foreground");
            if (brush1 != null)
            {
                _visiblePen = new Pen(brush1, 1.0);
                _visiblePen.Freeze();
            }
            else
                _visiblePen = null;
            var brush2 = GetBrush("OverviewMarginCaret", "Foreground");
            if (brush2 != null)
            {
                _caretPen = new Pen(brush2, 2.0);
                _caretPen.Freeze();
            }
            else
                _caretPen = null;
            Background = !_useEnhancedScrollBar || _offScreenBrush == null ? Brushes.Transparent : _offScreenBrush;
            InvalidateVisual();
            _caretElement.InvalidateVisual();
            _elisionElement.InvalidateVisual();
        }

        private void OnTextViewOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            Visibility = Enabled ? Visibility.Visible : Visibility.Hidden;
            _showTips = TextViewHost.TextView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowPreviewOptionId);
            _showCaret = TextViewHost.TextView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowCaretPositionOptionId) && TextViewHost.TextView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowScrollBarAnnotationsOptionId);
            _tipSize = TextViewHost.TextView.Options.GetOptionValue(DefaultTextViewHostOptions.PreviewSizeOptionId);
            if (e == null || e.OptionId == "OverviewMargin/ShowEnhancedScrollBar" || e.OptionId == "OverviewMargin/ShowSourceImageMargin")
            {
                var num1 = _useEnhancedScrollBar ? 1 : 0;
                _useEnhancedScrollBar = !_backgroundBar.Enabled;
                if (_useEnhancedScrollBar)
                {
                    Background = _offScreenBrush ?? Brushes.Transparent;
                    _scrollMap.AreElisionsExpanded = true;
                }
                else
                {
                    Background = Brushes.Transparent;
                    _scrollMap.AreElisionsExpanded = false;
                    CloseTip();
                }
                var num2 = _useEnhancedScrollBar ? 1 : 0;
                if (num1 != num2)
                {
                    // ISSUE: reference to a compiler-generated field
                    var trackSpanChanged = TrackSpanChanged;
                    if (trackSpanChanged != null)
                        _provider.GuardedOperations.RaiseEvent(this, trackSpanChanged);
                }
            }
            EnsureCaret();
            EnsureViewport();
            _caretElement.InvalidateVisual();
            _elisionElement.InvalidateVisual();
            InvalidateVisual();
        }

        private void OnSourceSpansChanged(object sender, ElisionSourceSpansChangedEventArgs e)
        {
            if (!_scrollMap.AreElisionsExpanded)
                return;
            _elisionElement.InvalidateVisual();
        }

        private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            EnsureCaret();
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            EnsureViewport();
        }

        private void OnMappingChanged(object sender, EventArgs e)
        {
            EnsureCaret();
            EnsureViewport();
            _elisionElement.InvalidateVisual();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_useEnhancedScrollBar)
                return;
            CloseTip();
            var position = e.GetPosition(this);
            if (position.Y < TrackSpanTop || position.Y > TrackSpanBottom)
                return;
            CaptureMouse();
            if (position.Y < ViewportTop || position.Y > ViewportBottom || e.ClickCount == 2)
            {
                _scrollBias = 0.0;
                ScrollViewToYCoordinate(position.Y, e.ClickCount == 2);
            }
            else
                _scrollBias = position.Y - (ViewportTop + ViewportBottom) * 0.5;
            e.Handled = true;
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);
            if (position.Y >= TrackSpanTop && position.Y <= TrackSpanBottom)
                ScrollViewToYCoordinate(position.Y - _scrollBias, false);
            ReleaseMouseCapture();
        }

        private bool FromTouch(MouseEventArgs e)
        {
            return e.StylusDevice != null;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (!_useEnhancedScrollBar || !_showTips || FromTouch(e))
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
                ScrollViewToYCoordinate(position.Y - _scrollBias, false);
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

        private static void DrawRectangle(DrawingContext drawingContext, Brush brush, double width, double yTop, double yBottom)
        {
            if (brush == null || yBottom - 1.0 <= yTop)
                return;
            drawingContext.DrawRectangle(brush, null, new Rect(0.0, yTop, width, yBottom - yTop));
        }

        private static void DrawRoundedRectangle(DrawingContext drawingContext, Brush brush, Pen pen, double radius, double xLeft, double yTop, double xRight, double yBottom)
        {
            if (brush == null && pen == null)
                return;
            drawingContext.DrawRoundedRectangle(brush, pen, new Rect(xLeft, yTop, xRight - xLeft, yBottom - yTop), radius, radius);
        }

        private double GetCoordinateOfLineBottom(ITextViewLine firstLine, ITextViewLine lastLine)
        {
            if (lastLine.EndIncludingLineBreak.Position < TextViewHost.TextView.TextSnapshot.Length || TextViewHost.TextView.ViewportHeight == 0.0)
                return _scrollMap.GetCoordinateAtBufferPosition(lastLine.End);
            return _scrollMap.End + Math.Floor(_scrollMap.ThumbSize * Math.Max(0.0, 1.0 - (lastLine.Bottom - firstLine.Bottom) / TextViewHost.TextView.ViewportHeight));
        }

        private Brush GetBrush(string name, string resource)
        {
            var properties = _formatMap.GetProperties(name);
            if (properties.Contains(resource))
                return properties[resource] as Brush;
            return null;
        }

        private void EnsureCaret()
        {
            if (!_showCaret)
                return;
            var num = Math.Round(GetYCoordinateOfBufferPosition(TextViewHost.TextView.Caret.Position.BufferPosition));
            if (num == CaretPosition)
                return;
            CaretPosition = num;
            if (_caretPen == null)
                return;
            _caretElement.InvalidateVisual();
        }

        private void EnsureViewport()
        {
            if (!_useEnhancedScrollBar || TextViewHost.TextView.InLayout)
                return;
            ITextViewLineCollection textViewLines = TextViewHost.TextView.TextViewLines;
            var val1 = Math.Floor(GetYCoordinateOfScrollMapPosition(_scrollMap.GetCoordinateAtBufferPosition(textViewLines.FirstVisibleLine.Start) - 0.5)) + 0.5;
            var scrollMapPosition = GetCoordinateOfLineBottom(textViewLines.FirstVisibleLine, textViewLines.LastVisibleLine) + 0.5;
            var num1 = Math.Max(val1, Math.Ceiling(GetYCoordinateOfScrollMapPosition(scrollMapPosition)) - 0.5);
            if (num1 - val1 < 6.0)
            {
                var num2 = Math.Floor((val1 + num1) * 0.5) + 0.5;
                val1 = num2 - 3.0;
                num1 = num2 + 3.0;
            }
            if (val1 == ViewportTop && num1 == ViewportBottom)
                return;
            ViewportTop = val1;
            ViewportBottom = num1;
            InvalidateVisual();
        }

        public ToolTip TipWindow => _tipWindow;

        internal void ShowTip(MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            if (_useEnhancedScrollBar && _showTips && (position.Y >= TrackSpanTop && position.Y <= TrackSpanBottom) && !FromTouch(e))
                OpenTip(e);
            else
                CloseTip();
        }

        internal void OpenTip(MouseEventArgs e)
        {
            EnsureTipFactories();
            if (_tipFactories.Count <= 0 && _tipSize <= 0)
                return;
            if (_tipWindow == null)
            {
                _tipWindow = new ToolTip
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
            _tipWindow.PlacementRectangle = new Rect(TextViewHost.TextView.ViewportRight, e.GetPosition(this).Y / num, 0.0, 0.0);
            if (_tipFactories.Any(tipFactory => _provider.GuardedOperations.CallExtensionPoint(() => tipFactory.UpdateTip(this, e, _tipWindow), false)))
            {
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
                if (!tipFactoryProvider.Metadata.ContentTypes.Any(contentType =>
                    TextViewHost.TextView.TextDataModel.ContentType.IsOfType(contentType)))
                    continue;
                var overviewTipManager = tipFactoryProvider.Value.GetOverviewTipManager(TextViewHost);
                if (overviewTipManager != null)
                {
                    _tipFactories.Add(overviewTipManager);
                }
            }
        }

        internal void CloseTip()
        {
            if (_tipWindow == null)
                return;
            _tipWindow.Visibility = Visibility.Hidden;
            _tipWindow.Content = null;
            _tipWindow.IsOpen = false;
            _tipWindow = null;
            if (_tipView == null)
                return;
            _tipView.Close();
            _tipView = null;
        }

        internal void ScrollViewToYCoordinate(double y, bool expand)
        {
            var num = TrackSpanBottom - ThumbHeight;
            if (y < num)
            {
                var positionOfYcoordinate = GetBufferPositionOfYCoordinate(y);
                if (expand)
                    Expand(positionOfYcoordinate);
                TextViewHost.TextView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(positionOfYcoordinate, 0), EnsureSpanVisibleOptions.AlwaysCenter);
            }
            else
            {
                y = Math.Min(y, num + ThumbHeight / 2.0);
                var verticalDistance = TextViewHost.TextView.ViewportHeight * (0.5 - (y - num) / ThumbHeight);
                var snapshotPoint = new SnapshotPoint(TextViewHost.TextView.TextSnapshot, TextViewHost.TextView.TextSnapshot.Length);
                if (expand)
                    Expand(snapshotPoint);
                TextViewHost.TextView.DisplayTextLineContainingBufferPosition(snapshotPoint, verticalDistance, ViewRelativePosition.Top);
            }
        }

        private void Expand(SnapshotPoint position)
        {
            _outliningManager?.ExpandAll(new SnapshotSpan(position, 0), collapsible =>
            {
                var span = (Span)collapsible.Extent.GetSpan(position.Snapshot);
                if (position > span.Start)
                    return position < span.End;
                return false;
            });
        }

        private static MenuItem CreateMenuItem(ItemsControl menu, string text, bool isCheckable, bool isChecked, ICommand command)
        {
            var menuItem = new MenuItem
            {
                Header = text,
                IsCheckable = isCheckable,
                IsChecked = isChecked,
                Command = command
            };
            menu.Items.Add(menuItem);
            return menuItem;
        }

        private class MenuCommand : ICommand
        {
            private readonly Action<object> _execute;

            public MenuCommand(Action<object> execute)
            {
                _execute = execute;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                _execute(parameter);
            }
        }

        private class ScrollMapWrapper : IScrollMap
        {
            private readonly IScrollMap _expandedScrollMap;
            private readonly IScrollMap _collapsedScrollMap;
            private readonly OverviewMarginProvider _provider;

            public ScrollMapWrapper(ITextView view, OverviewMarginProvider myProvider)
            {
                _provider = myProvider;
                ScrollMap = _expandedScrollMap = myProvider.ScrollMapFactory.Create(view, true);
                _collapsedScrollMap = myProvider.ScrollMapFactory.Create(view, false);
                ScrollMap.MappingChanged += OnMappingChanged;
            }

            private IScrollMap ScrollMap { get; set; }

            private void OnMappingChanged(object sender, EventArgs e)
            {
                var mappingChanged = MappingChanged;
                if (mappingChanged == null)
                    return;
                _provider.GuardedOperations.RaiseEvent(this, mappingChanged);
            }

            public double GetCoordinateAtBufferPosition(SnapshotPoint bufferPosition)
            {
                return ScrollMap.GetCoordinateAtBufferPosition(bufferPosition);
            }

            public bool AreElisionsExpanded
            {
                get => ScrollMap.AreElisionsExpanded;
                set
                {
                    if (value == (ScrollMap == _expandedScrollMap))
                        return;
                    ScrollMap.MappingChanged -= OnMappingChanged;
                    ScrollMap = value ? _expandedScrollMap : _collapsedScrollMap;
                    ScrollMap.MappingChanged += OnMappingChanged;
                    OnMappingChanged(this, EventArgs.Empty);
                }
            }

            public SnapshotPoint GetBufferPositionAtCoordinate(double coordinate)
            {
                return ScrollMap.GetBufferPositionAtCoordinate(coordinate);
            }

            public double Start => ScrollMap.Start;

            public double End => ScrollMap.End;

            public double ThumbSize => ScrollMap.ThumbSize;

            public ITextView TextView => ScrollMap.TextView;

            public double GetFractionAtBufferPosition(SnapshotPoint bufferPosition)
            {
                return ScrollMap.GetFractionAtBufferPosition(bufferPosition);
            }

            public SnapshotPoint GetBufferPositionAtFraction(double fraction)
            {
                return ScrollMap.GetBufferPositionAtFraction(fraction);
            }

            public event EventHandler MappingChanged;
        }

        private class ElisionElement : UIElement
        {
            private readonly OverviewMargin _margin;

            public ElisionElement(OverviewMargin margin)
            {
                _margin = margin;
                Opacity = 0.25;
                IsHitTestVisible = false;
                Focusable = false;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                if (_margin.ElisionBrush == null || !_margin.Map.AreElisionsExpanded)
                    return;
                var snapshot = _margin.TextViewHost.TextView.BufferGraph.MapDownToSnapshot(new SnapshotSpan(_margin.TextViewHost.TextView.VisualSnapshot, 0, _margin.TextViewHost.TextView.VisualSnapshot.Length), SpanTrackingMode.EdgeInclusive, _margin.TextViewHost.TextView.TextSnapshot);
                var ofBufferPosition1 = _margin.GetYCoordinateOfBufferPosition(new SnapshotPoint(_margin.TextViewHost.TextView.TextSnapshot, 0));
                foreach (var snapshotSpan in snapshot)
                {
                    var ofBufferPosition2 = _margin.GetYCoordinateOfBufferPosition(snapshotSpan.Start);
                    DrawRectangle(drawingContext, _margin.ElisionBrush, _margin.ActualWidth, ofBufferPosition1, ofBufferPosition2);
                    ofBufferPosition1 = _margin.GetYCoordinateOfBufferPosition(snapshotSpan.End);
                }
                var ofBufferPosition3 = _margin.GetYCoordinateOfBufferPosition(new SnapshotPoint(_margin.TextViewHost.TextView.TextSnapshot, _margin.TextViewHost.TextView.TextSnapshot.Length));
                DrawRectangle(drawingContext, _margin.ElisionBrush, _margin.ActualWidth, ofBufferPosition1, ofBufferPosition3);
            }
        }

        private class CaretElement : UIElement
        {
            private readonly OverviewMargin _margin;

            public CaretElement(OverviewMargin margin)
            {
                _margin = margin;
                IsHitTestVisible = false;
                Focusable = false;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);
                _margin.DrawCaret(drawingContext);
            }
        }

        private class BackgroundBarMargin : VerticalScrollBarMargin
        {
            public BackgroundBarMargin(ITextView wpfTextView, ScrollMapWrapper scrollMap, string name, PerformanceBlockMarker performanceBlockMarker)
                : base(wpfTextView, scrollMap, name, performanceBlockMarker)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch;
                Width = double.NaN;
            }

            public override bool Enabled => !TextView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowEnhancedScrollBarOptionId);
        }
    }
}