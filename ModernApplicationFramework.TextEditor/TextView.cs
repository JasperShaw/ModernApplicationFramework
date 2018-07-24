using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
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

        private ITextBuffer _textBuffer;
        private ITextBuffer _visualBuffer;

        private List<IFormattedLine> _formattedLinesInTextViewLines = new List<IFormattedLine>();

        private HwndSource _immSource;
        private HwndSourceHook _immHook;

        //private CaretElement _caretElement;
        internal TextContentLayer _contentLayer = new TextContentLayer();

        [ThreadStatic]
        private static TextView ViewWithAggregateFocus;

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

        internal bool IsTextViewInitialized => _hasInitializeBeenCalled;

        public TextView(ITextViewModel textViewModel, TextEditorFactoryService factoryService, bool initialize = true)
        {
            _factoryService = factoryService;
            TextDataModel = textViewModel.DataModel;
            TextViewModel = textViewModel;
            _textBuffer = textViewModel.EditBuffer;
            _visualBuffer = textViewModel.VisualBuffer;
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

            _immHook = WndProc;

            _manipulationLayer = new Canvas();
            _baseLayer = new ViewStack(_factoryService.OrderedViewLayerDefinitions, this);
            _overlayLayer = new ViewStack(_factoryService.OrderedOverlayLayerDefinitions, this, true);

            InitializeLayers();
            Loaded += OnLoaded;

            SubscribeToEvents();

            PerformLayout(_textBuffer.CurrentSnapshot, _visualBuffer.CurrentSnapshot);

            _inConstructor = false;

            _hasInitializeBeenCalled = true;
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
    }
}
