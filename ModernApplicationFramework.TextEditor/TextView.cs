using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class TextView : ContentControl, ITextView
    {
        [Export]
        [Name("Text")]
        [Order(After = "SelectionAndProvisionHighlight", Before = "Caret")]
        private static readonly AdornmentLayerDefinition TextAdornmentLayer = new AdornmentLayerDefinition();


        private readonly TextEditorFactoryService _factoryService;
        private bool _hasInitializeBeenCalled;
        private bool _isClosed;
        private bool _hasAggregateFocus;
        private bool _hasKeyboardFocus;
        private Action _loadedAction;
        private bool _hasBeenLoaded;
        private Canvas _controlHostLayer;
        private Canvas _manipulationLayer;
        private ViewStack _baseLayer;
        private ViewStack _overlayLayer;
        private bool _inConstructor = true;
        private TextViewLineCollection _textViewLinesCollection;
        private double _viewportLeft;
        private double _viewportTop;
        private bool _layoutNeeded;
        private ITextViewRoleSet _roles;

        internal List<Span> _invalidatedSpans = new List<Span>();

        private ITextBuffer _textBuffer;
        private ITextBuffer _visualBuffer;

        private List<IFormattedLine> _formattedLinesInTextViewLines = new List<IFormattedLine>();

        private HwndSource _immSource;
        private HwndSourceHook _immHook;

        //private CaretElement _caretElement;
        internal TextContentLayer _contentLayer = new TextContentLayer();

        [ThreadStatic]
        private static TextView ViewWithAggregateFocus;

        private ITextSnapshot _textSnapshot;
        private ITextSnapshot _visualSnapshot;
        private bool _queuedLayout;
        private bool _inOuterLayout;
        private IFormattedLineSource _formattedLineSource;
        private IBufferGraph _bufferGraph;
        private bool _inInnerLayout;
        private IEditorOptions _editorOptions;

        public event EventHandler<BackgroundBrushChangedEventArgs> BackgroundBrushChanged;
        public event EventHandler ViewportLeftChanged;
        public event EventHandler ViewportHeightChanged;
        public event EventHandler ViewportWidthChanged;
        public event EventHandler Closed;

        public FrameworkElement VisualElement => this;

        public bool IsClosed => _isClosed;

        public double ViewportLeft
        {
            get => _viewportLeft;
            set
            {
                if (_isClosed)
                    return;
                double num1  = 0.0;


                if (_viewportLeft == num1)
                    return;
                var deltaX = num1 - _viewportLeft;
                _viewportLeft = num1;
                UpdateVisibleArea(ViewportWidth, ViewportHeight);

                Canvas.SetLeft(_baseLayer, -_viewportLeft);
                RaiseLayoutChangeEvent();
                EventHandler viewportLeftChanged = ViewportLeftChanged;
                viewportLeftChanged?.Invoke(this, new EventArgs());
            }
        }

        private void RaiseLayoutChangeEvent()
        {
            if (IsClosed)
                return;


        }

        private void UpdateVisibleArea(double viewportWidth, double viewportHeight)
        {
            var area = new Rect(ViewportLeft, _viewportTop, viewportWidth, viewportHeight);
            foreach (var line in _formattedLinesInTextViewLines)
                line.SetVisibleArea(area);
        }

        public double ViewportTop => _viewportTop;

        public double ViewportRight => ViewportLeft + ViewportWidth;
        public double ViewportBottom => ViewportTop + ViewportHeight;

        public FrameworkElement ManipulationLayer => _manipulationLayer;

        public ITextDataModel TextDataModel { get; }
        public IBufferGraph BufferGraph => _bufferGraph;
        public IEditorOptions Options => _editorOptions;

        public ITextViewModel TextViewModel { get; }

        public double ViewportHeight
        {
            get
            {
                if (!double.IsNaN(ActualHeight))
                    return Math.Ceiling(ActualHeight);
                return 120.0;
            }
        }

        public double ViewportWidth
        {
            get
            {
                if (!double.IsNaN(ActualWidth))
                    return ActualWidth;
                return 240.0;
            }
        }

        public new Brush Background
        {
            get => _controlHostLayer.Background;
            set
            {
                _controlHostLayer.Background = value;
                BackgroundBrushChanged?.Invoke(this, new BackgroundBrushChangedEventArgs(value));
            }
        }

        public bool InLayout => _inInnerLayout;

        public ITextViewRoleSet Roles => _roles;

        internal bool IsTextViewInitialized => _hasInitializeBeenCalled;

        public TextView(ITextViewModel textViewModel, ITextViewRoleSet roles, IEditorOptions parentOptions,  TextEditorFactoryService factoryService, bool initialize = true)
        {
            _roles = roles;
            _factoryService = factoryService;
            TextDataModel = textViewModel.DataModel;
            TextViewModel = textViewModel;
            _textBuffer = textViewModel.EditBuffer;
            _visualBuffer = textViewModel.VisualBuffer;
            _textSnapshot = _textBuffer.CurrentSnapshot;
            _visualSnapshot = _visualBuffer.CurrentSnapshot;
            _editorOptions = _factoryService.EditorOptionsFactoryService.GetOptions(this);
            _editorOptions.Parent = parentOptions;
            if (!initialize)
                return;
            Initialize();
        }

        internal void Initialize()
        {
            if (_hasInitializeBeenCalled)
                throw new InvalidOperationException();
            Name = nameof(TextView);
            TextOptions.SetTextHintingMode(this, TextHintingMode.Fixed);

            InputMethod.SetIsInputMethodSuspended(this, true);
            AllowDrop = true;
            _bufferGraph = _factoryService.BufferGraphFactoryService.CreateBufferGraph(TextViewModel.VisualBuffer);

            _immHook = WndProc;

            _manipulationLayer = new Canvas();
            _baseLayer = new ViewStack(_factoryService.OrderedViewLayerDefinitions, this);
            _overlayLayer = new ViewStack(_factoryService.OrderedOverlayLayerDefinitions, this, true);

            InitializeLayers();
            Loaded += OnLoaded;

            SetClearTypeHint();

            SubscribeToEvents();
            BindContentTypeSpecificAssets(null, TextViewModel.DataModel.ContentType);
            _visualBuffer.ChangedLowPriority += OnVisualBufferChanged;
            _visualBuffer.ContentTypeChanged += OnVisualBufferContentTypeChanged;
            PerformLayout(_textBuffer.CurrentSnapshot, _visualBuffer.CurrentSnapshot);
            lock (_invalidatedSpans)
                _invalidatedSpans.Add(new Span(0, _visualSnapshot.Length));

            _inConstructor = false;
            QueueLayout();
            _hasInitializeBeenCalled = true;
        }

        internal void QueueLayout()
        {
            _layoutNeeded = true;
            if (_queuedLayout || !IsVisible)
                return;
            _queuedLayout = true;
            Dispatcher.BeginInvoke((Action) (() =>
            {
                _queuedLayout = false;
                if (_isClosed || !_layoutNeeded || (!IsVisible || _textSnapshot != _textBuffer.CurrentSnapshot) || _visualSnapshot != _visualBuffer.CurrentSnapshot)
                    return;
                PerformLayout(_textSnapshot, _visualSnapshot);
            }), DispatcherPriority.DataBind, Array.Empty<object>());
        }

        private void PerformLayout(ITextSnapshot newSnapshot, ITextSnapshot newVisualSnapshot)
        {
            var verticalDistance = 0.0;
            var relativeTo = ViewRelativePosition.Top;
            SnapshotPoint anchorPosition;
            if (_textViewLinesCollection == null)
                anchorPosition = new SnapshotPoint(newSnapshot, 0);
            else
            {
                var firstVisibleLine = _textViewLinesCollection.FirstVisibleLine;
                verticalDistance = firstVisibleLine.Top - _viewportTop;
                anchorPosition = firstVisibleLine.Start.TranslateTo(newSnapshot, PointTrackingMode.Negative);
            }

            PerformLayout(newSnapshot, newVisualSnapshot, anchorPosition, verticalDistance, relativeTo, ViewportWidth,
                ViewportHeight, true, new CancellationToken?());
        }

        private void PerformLayout(ITextSnapshot newSnapshot, ITextSnapshot newVisualSnapshot, SnapshotPoint anchorPosition, double verticalDistance, ViewRelativePosition relativeTo, double viewportWidth, double viewportHeight, bool v1, CancellationToken? v2)
        {
            if (_isClosed)
                return;
            if (_inOuterLayout)
                throw new InvalidOperationException();

            _inOuterLayout = true;
            try
            {
                var useDisplayMode = ShouldUseDisplayMode();
            }
            finally
            {
                _inOuterLayout = false;
            }
        }

        private bool ShouldUseDisplayMode()
        {
            if (PresentationSource.FromVisual(this) != null || ReadLocalValue(TextOptions.TextFormattingModeProperty) !=
                DependencyProperty.UnsetValue)
                return TextOptions.GetTextFormattingMode(this) == TextFormattingMode.Display;
            if (_formattedLineSource == null)
                return true;
            return _formattedLineSource.UseDisplayMode;
        }

        private void SubscribeToEvents()
        {
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);

            SizeChanged += OnSizeChanged;
            GotKeyboardFocus += OnGotKeyboardFocus;
            LostKeyboardFocus += OnLostKeyboardFocus;
            IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;

            MouseLeave += OnMouseLeave;
        }

        private void UnsubscribeFromEvents()
        {
            MouseMove -= OnMouseMove;
            MouseLeftButtonDown -= OnMouseDown;
            MouseDown -= OnMouseDown;
            MouseRightButtonDown -= OnMouseDown;
            MouseLeave -= OnMouseLeave;

            SizeChanged -= OnSizeChanged;
            GotKeyboardFocus -= OnGotKeyboardFocus;
            LostKeyboardFocus -= OnLostKeyboardFocus;
            IsKeyboardFocusWithinChanged -= OnIsKeyboardFocusWithinChanged;
            IsVisibleChanged -= OnVisibleChanged;

            PresentationSource.RemoveSourceChangedHandler(this, OnSourceChanged);
            (PresentationSource.FromVisual(this) as HwndSource)?.RemoveHook(MouseScrollHook);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
        }

        private IntPtr MouseScrollHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        private void OnVisualBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
        }

        private void OnVisualBufferChanged(object sender, TextContentChangedEventArgs e)
        {
        }

        private void BindContentTypeSpecificAssets(IContentType beforeContentType, IContentType afterContentType)
        {
        }

        private void SetClearTypeHint()
        {

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            if (IsClosed)
                return;
            _hasBeenLoaded = true;
            if (_loadedAction == null)
                return;
            _loadedAction();
            _loadedAction = null;
        }

        public void Close()
        {
            if (_isClosed)
                throw new InvalidOperationException();
            if (_hasAggregateFocus)
            {
                ViewWithAggregateFocus = null;
                _hasAggregateFocus = false;
            }

            UnsubscribeFromEvents();


            _isClosed = true;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public ITextSnapshot TextSnapshot { get; }


        private void InitializeLayers()
        {
            Focusable = true;
            FocusVisualStyle = null;
            Cursor = Cursors.IBeam;

            _manipulationLayer.Background = Brushes.Transparent;
            _baseLayer.TryAddElement("Text", _contentLayer);
            //_baseLayer.TryAddElement("Caret", _caretElement);
            _controlHostLayer = new Canvas
            {
                Background = GetBackgroundColorFromFormatMap()
            };
            Content = _controlHostLayer;
            _controlHostLayer.SizeChanged += (sender, args) =>
            {
                _manipulationLayer.Width = args.NewSize.Width;
                _manipulationLayer.Height = args.NewSize.Height;
                _baseLayer.SetSize(args.NewSize);
                _overlayLayer.SetSize(args.NewSize);
                if (args.NewSize.Width == 0.0 || args.NewSize.Height == 0.0)
                    _controlHostLayer.Children.Clear();
                else
                {
                    if (_controlHostLayer.Children.Count != 0)
                        return;
                    _controlHostLayer.Children.Add(ManipulationLayer);
                    _controlHostLayer.Children.Add(_baseLayer);
                    _controlHostLayer.Children.Add(_overlayLayer);
                }
            };
        }

        private Brush GetBackgroundColorFromFormatMap()
        {
            return Brushes.White;
        }

        public PropertyCollection Properties { get; } = new PropertyCollection();
    }
}
