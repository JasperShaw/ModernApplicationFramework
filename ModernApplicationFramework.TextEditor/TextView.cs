using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Text.Formatting;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class TextView : ContentControl, ITextView, ILineTransformSource
    {
        [Export]
        [Name("Text")]
        [Order(After = "SelectionAndProvisionHighlight", Before = "Caret")]
        private static readonly AdornmentLayerDefinition TextAdornmentLayer = new AdornmentLayerDefinition();
        [Export]
        [Name("SelectionAndProvisionHighlight")]
        [Order(Before = "Text")]
        private static readonly AdornmentLayerDefinition SelectionAndProvisionalHighlightAdornmentLayer = new AdornmentLayerDefinition();
        [Export]
        [Name("Caret")]
        [Order(After = "Text")]
        private static readonly AdornmentLayerDefinition CaretAdornmentLayer = new AdornmentLayerDefinition();

        [ThreadStatic] private static TextView ViewWithAggregateFocus;

        //private CaretElement _caretElement;
        internal TextContentLayer _contentLayer = new TextContentLayer();

        internal List<Span> _invalidatedSpans = new List<Span>();
        internal List<Span> _reclassifiedSpans = new List<Span>();


        private List<FormattedLineGroup> _attachedLineCache = new List<FormattedLineGroup>();
        private List<FormattedLineGroup> _unattachedLineCache = new List<FormattedLineGroup>(8);
        private ViewStack _baseLayer;
        private Canvas _controlHostLayer;
        private readonly double _currentZoomLevel = 100.0;
        private TextSelection _selection;
        private CaretElement _caretElement;

        private ConnectionManager _connectionManager;
        private readonly List<IFormattedLine> _formattedLinesInTextViewLines = new List<IFormattedLine>();
        private bool _hasBeenLoaded;
        private bool _hasKeyboardFocus;
        private HwndSourceHook _immHook;

        private IClassifier _classifier;
        private ITextAndAdornmentSequencer _sequencer;

        private HwndSource _immSource;
        private bool _inConstructor = true;
        private bool _layoutNeeded;
        private Action _loadedAction;
        private Canvas _manipulationLayer;
        private ViewState _oldState;
        private ViewStack _overlayLayer;
        private bool _queuedLayout;
        private static Exception _lastPerformLayoutException = null;
        private static string _lastPerformLayoutExceptionStackTrace = null;

        private List<Lazy<ITextViewCreationListener, IDeferrableContentTypeAndTextViewRoleMetadata>> _deferredTextViewListeners;

        internal readonly IGuardedOperations GuardedOperations;

        private static readonly XmlLanguage EnUsLanguage = XmlLanguage.GetLanguage("en-US");

        private TextViewLineCollection _textViewLinesCollection;
        private double _viewportLeft;
        private readonly ITextBuffer _visualBuffer;
        internal LineRightCache _lineRightCache;
        private ITextSnapshot _visualSnapshot;
        private IInputElement _focusedElement;

        private IntPtr _imeDefaultWnd = IntPtr.Zero;
        private IntPtr _imeContext = IntPtr.Zero;
        private IntPtr _imeOldContext = IntPtr.Zero;

        internal SpaceReservationStack _spaceReservationStack;

        private IViewScroller _viewScroller;

        private static readonly IList<ITextViewLine> _emptyLines =
            new ReadOnlyCollection<ITextViewLine>(new List<ITextViewLine>(0));

        private int? _lastHoverPosition;
        internal DispatcherTimer _mouseHoverTimer;
        private bool _imeActive;
        private double _minMaxTextRightCoordinate;
        internal List<ILineTransformSource> _lineTransformSources = new List<ILineTransformSource>();
        private IEditorFormatMap _editorFormatMap;
        private TextFormattingRunProperties _oldPlainTextProperties;
        private int _queuedFocusSettersCount;
        private ProvisionalTextHighlight _provisionalTextHighlight;
        private int _queuedSpaceReservationStackRefresh;

        public event EventHandler<BackgroundBrushChangedEventArgs> BackgroundBrushChanged;
        public event EventHandler Closed;
        public event EventHandler LostAggregateFocus;
        public event EventHandler GotAggregateFocus;
        public event EventHandler ViewportHeightChanged;
        public event EventHandler ViewportLeftChanged;
        public event EventHandler ViewportWidthChanged;

        public event EventHandler<MouseHoverEventArgs> MouseHover
        {
            add
            {
                lock (_mouseHoverEvents)
                {
                    if (_mouseHoverEvents.Count == 0)
                    {
                        MouseMove += OnMouseMove;
                        MouseLeftButtonDown += OnMouseDown;
                        MouseDown += OnMouseDown;
                        MouseRightButtonDown += OnMouseDown;
                    }
                    _mouseHoverEvents.Add(new MouseHoverEventData(value));
                }
            }
            remove
            {
                lock (_mouseHoverEvents)
                {
                    for (var index = _mouseHoverEvents.Count - 1; index >= 0; --index)
                    {
                        if (_mouseHoverEvents[index].EventHandler == value)
                        {
                            _mouseHoverEvents.RemoveAt(index);
                            if (_mouseHoverEvents.Count != 0)
                                break;
                            MouseMove -= OnMouseMove;
                            MouseLeftButtonDown -= OnMouseDown;
                            MouseDown -= OnMouseDown;
                            MouseRightButtonDown -= OnMouseDown;
                            break;
                        }
                    }
                }
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

        internal IClassificationFormatMap ClassificationFormatMap => ComponentContext.ClassificationFormatMappingService.GetClassificationFormatMap(this);

        public IBufferGraph BufferGraph { get; private set; }

        public bool InLayout { get; private set; }

        public bool IsClosed { get; private set; }

        public FrameworkElement ManipulationLayer => _manipulationLayer;
        public IEditorOptions Options { get; }

        public IViewScroller ViewScroller => _viewScroller ?? (_viewScroller = new DefaultViewScroller(this));

        public PropertyCollection Properties { get; } = new PropertyCollection();

        public ITextViewRoleSet Roles { get; }
        public ITextBuffer TextBuffer { get; }

        public ITextDataModel TextDataModel { get; }

        public ITextSnapshot TextSnapshot { get; private set; }

        public IFormattedLineSource FormattedLineSource { get; private set; }

        public ITextViewModel TextViewModel { get; private set; }
        public double ViewportBottom => ViewportTop + ViewportHeight;

        public ITextSelection Selection => _selection;

        public double ViewportHeight
        {
            get
            {
                if (!double.IsNaN(ActualHeight))
                    return Math.Ceiling(ActualHeight);
                return 120.0;
            }
        }

        public double ViewportLeft
        {
            get => _viewportLeft;
            set
            {
                if (IsClosed)
                    return;
                double num1;
                if ((WordWrapStyle & WordWrapStyles.WordWrap) == WordWrapStyles.WordWrap)
                {
                    num1 = 0.0;
                }
                else
                {
                    if (double.IsNaN(value))
                        throw new ArgumentOutOfRangeException(nameof(value));
                    if (Options.IsViewportLeftClipped())
                    {
                        var num2 = Math.Max(MaxTextRightCoordinate, _caretElement.Right) + 200.0;
                        num1 = Math.Max(0.0, Math.Min(value, num2 - ViewportWidth));
                    }
                    else
                        num1 = Math.Max(0, value);
                }
                if (_viewportLeft == num1)
                    return;
                var deltaX = num1 - _viewportLeft;
                _viewportLeft = num1;
                UpdateVisibleArea(ViewportWidth, ViewportHeight);
                _baseLayer.SetSnapshotAndUpdate(TextSnapshot, deltaX, 0.0, _emptyLines, _emptyLines);
                Canvas.SetLeft(_baseLayer, -_viewportLeft);
                RaiseLayoutChangeEvent();
                var viewportLeftChanged = ViewportLeftChanged;
                viewportLeftChanged?.Invoke(this, new EventArgs());
                QueueSpaceReservationStackRefresh();
                if (!_imeActive)
                    return;
                PositionImmCompositionWindow();
            }
        }

        public event EventHandler MaxTextRightCoordinateChanged;

        public double MinMaxTextRightCoordinate
        {
            get => _minMaxTextRightCoordinate;
            set
            {
                if (IsClosed)
                    return;
                var coordinate1 = MaxTextRightCoordinate;
                _minMaxTextRightCoordinate = value;
                var coordinate2 = MaxTextRightCoordinate;
                if (coordinate1 == coordinate2)
                    return;
                var changed = MaxTextRightCoordinateChanged;
                changed?.Invoke(this, new EventArgs());
            }
        }

        public double ViewportRight => ViewportLeft + ViewportWidth;

        public double ViewportTop { get; private set; }

        public double MaxTextRightCoordinate => Math.Max(RawMaxTextRightCoordinate, _minMaxTextRightCoordinate);

        public double LineHeight => FormattedLineSource.LineHeight;

        public double ViewportWidth
        {
            get
            {
                if (!double.IsNaN(ActualWidth))
                    return ActualWidth;
                return 240.0;
            }
        }

        internal bool IsViewWrapEnabled
        {
            get
            {
                if ((WordWrapStyle & WordWrapStyles.WordWrap) != WordWrapStyles.None)
                    return (uint)(WordWrapStyle & WordWrapStyles.VisibleGlyphs) > 0U;
                return false;
            }
        }

        public FrameworkElement VisualElement => this;

        public ITextViewLineCollection TextViewLines
        {
            get
            {
                if (InLayout)
                    throw new InvalidOperationException();
                return _textViewLinesCollection;
            }
        }

        internal TextEditorFactoryService ComponentContext { get; }

        public bool IsMouseOverViewOrAdornments
        {
            get
            {
                if (!IsMouseOver)
                    return _spaceReservationStack.IsMouseOver;
                return true;
            }
        }

        public ITextSnapshot VisualSnapshot { get; }

        public double ZoomLevel
        {
            get => _currentZoomLevel;
            set => ApplyZoom(value);
        }

        public event EventHandler<ZoomLevelChangedEventArgs> ZoomLevelChanged;

        public event EventHandler<TextViewLayoutChangedEventArgs> LayoutChanged;

        internal bool IsTextViewInitialized { get; private set; }

        public bool InOuterLayout { get; private set; }

        internal WordWrapStyles WordWrapStyle =>
            Options.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId);

        public TextView(ITextViewModel textViewModel, ITextViewRoleSet roles, IEditorOptions parentOptions,
            TextEditorFactoryService factoryService, bool initialize = true)
        {
            Roles = roles;
            ComponentContext = factoryService;
            GuardedOperations = ComponentContext.GuardedOperations;
            TextDataModel = textViewModel.DataModel;
            TextViewModel = textViewModel;
            TextBuffer = textViewModel.EditBuffer;
            _visualBuffer = textViewModel.VisualBuffer;
            TextSnapshot = TextBuffer.CurrentSnapshot;
            VisualSnapshot = _visualBuffer.CurrentSnapshot;
            Options = ComponentContext.EditorOptionsFactoryService.GetOptions(this);
            Options.Parent = parentOptions;
            if (!initialize)
                return;
            Initialize();
        }

        public void Close()
        {
            if (IsClosed)
                throw new InvalidOperationException();
            if (HasAggregateFocus)
            {
                ViewWithAggregateFocus = null;
                HasAggregateFocus = false;
            }
            _mouseHoverTimer.Stop();
            UpdateTrackingFocusChanges(false);
            UnsubscribeFromEvents();
            _contentLayer.SetTextViewLines(null);
            foreach (var formattedLineGroup in _attachedLineCache)
                formattedLineGroup.Dispose();
            _attachedLineCache.Clear();
            foreach(var formmatedLineGroup in _unattachedLineCache)
                formmatedLineGroup.Dispose();
            _unattachedLineCache.Clear();
            _connectionManager.Close();
            _caretElement.Close();
            _provisionalTextHighlight.Close();
            (_classifier as IDisposable)?.Dispose();
            DisableIme();
            TextViewModel.Dispose();
            TextViewModel = null;        
            IsClosed = true;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void QueueSpaceReservationStackRefresh()
        {
            if (Interlocked.CompareExchange(ref _queuedSpaceReservationStackRefresh, 1,0) != 0)
                return;
            Dispatcher.BeginInvoke((Action) (() =>
            {
                if (IsClosed)
                    return;
                _spaceReservationStack.Refresh();
            }), DispatcherPriority.Background, Array.Empty<object>());
        }

        internal void Initialize()
        {
            if (IsTextViewInitialized)
                throw new InvalidOperationException();
            Name = nameof(TextView);
            TextOptions.SetTextHintingMode(this, TextHintingMode.Fixed);
            _mouseHoverTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
            InputMethod.SetIsInputMethodSuspended(this, true);
            AllowDrop = true;
            BufferGraph = ComponentContext.BufferGraphFactoryService.CreateBufferGraph(TextViewModel.VisualBuffer);
            _lineRightCache = new LineRightCache();
            _immHook = WndProc;
            _editorFormatMap = ComponentContext.EditorFormatMapService.GetEditorFormatMap(this);
            _manipulationLayer = new Canvas();
            _baseLayer = new ViewStack(ComponentContext.OrderedViewLayerDefinitions, this);
            _overlayLayer = new ViewStack(ComponentContext.OrderedOverlayLayerDefinitions, this, true);
            _selection = new TextSelection(this, _editorFormatMap, ComponentContext.GuardedOperations);
            _caretElement = new CaretElement(this, _selection, ComponentContext.SmartIndentationService,
                _editorFormatMap, ClassificationFormatMap, ComponentContext.GuardedOperations);
            InitializeLayers();
            Loaded += OnLoaded;
            _caretElement.Visibility = Visibility.Hidden;
            _spaceReservationStack = new SpaceReservationStack(ComponentContext.OrderedSpaceReservationManagerDefinitions, this);
            SetClearTypeHint();
            _oldState = new ViewState(this);
            _classifier = ComponentContext.ClassifierAggregatorService.GetClassifier(this);
            _sequencer = ComponentContext.TextAndAdornmentSequencerFactoryService.Create(this);
            _connectionManager = new ConnectionManager(this, ComponentContext.TextViewConnectionListeners,
                ComponentContext.GuardedOperations);
            SubscribeToEvents();
            BindContentTypeSpecificAssets(null, TextViewModel.DataModel.ContentType);
            _visualBuffer.ChangedLowPriority += OnVisualBufferChanged;
            _visualBuffer.ContentTypeChanged += OnVisualBufferContentTypeChanged;
            PerformLayout(TextBuffer.CurrentSnapshot, _visualBuffer.CurrentSnapshot);
            lock (_invalidatedSpans)
                _invalidatedSpans.Add(new Span(0, VisualSnapshot.Length));

            if (Roles.Contains("ZOOMABLE"))
                ZoomLevel = Options.GetOptionValue(DefaultViewOptions.ZoomLevelId);
            _inConstructor = false;
            QueueLayout();
            IsTextViewInitialized = true;
        }

        internal void QueueLayout()
        {
            _layoutNeeded = true;
            if (_queuedLayout || !IsVisible)
                return;
            _queuedLayout = true;
            Dispatcher.BeginInvoke((Action)(() =>
           {
               _queuedLayout = false;
               if (IsClosed || !_layoutNeeded || !IsVisible || TextSnapshot != TextBuffer.CurrentSnapshot ||
                   VisualSnapshot != _visualBuffer.CurrentSnapshot)
                   return;
               PerformLayout(TextSnapshot, VisualSnapshot);
           }), DispatcherPriority.DataBind, Array.Empty<object>());
        }

        internal void InvalidateAllLines()
        {
            lock (_invalidatedSpans)
            {
                if (_attachedLineCache.Count <= 0 && _unattachedLineCache.Count <= 0)
                    return;
                _invalidatedSpans.Clear();
                _invalidatedSpans.Add(new Span(0, _visualSnapshot.Length));
                QueueLayout();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Width != double.PositiveInfinity && availableSize.Height != double.PositiveInfinity)
                return availableSize;
            var height = availableSize.Height == double.PositiveInfinity ? (double.IsNaN(Height) ? 120.0 : Height) : availableSize.Height;
            return new Size(availableSize.Width == double.PositiveInfinity ? (double.IsNaN(Width) ? 240.0 : Width) : availableSize.Width, height);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TextOptions.TextFormattingModeProperty)
                UpdateFormattingMode();
            base.OnPropertyChanged(e);
        }

        private void UpdateFormattingMode()
        {
            if (ShouldUseDisplayMode() == FormattedLineSource.UseDisplayMode)
                return;
            InvalidateAllLines();
        }

        private void ApplyZoom(double zoomLevel)
        {
        }

        private void BindContentTypeSpecificAssets(IContentType beforeContentType, IContentType afterContentType)
        {
            _lineTransformSources.Clear();
            foreach (var matchingExtension in 
                UiExtensionSelector.SelectMatchingExtensions(ComponentContext.LineTransformSourceProviders, afterContentType, null, Roles))
            {
                var lineTransformSource = ComponentContext.GuardedOperations.InstantiateExtension(matchingExtension, matchingExtension, p => p.Create(this));
                if (lineTransformSource != null)
                    _lineTransformSources.Add(lineTransformSource);
            }
            foreach (var matchingExtension in 
                UiExtensionSelector.SelectMatchingExtensions(ComponentContext.TextViewCreationListeners, afterContentType, beforeContentType, Roles))
            {
                var optionName = matchingExtension.Metadata.OptionName;
                if (!string.IsNullOrEmpty(optionName) && Options.IsOptionDefined(optionName, false))
                {
                    var optionValue = Options.GetOptionValue(optionName);
                    if (optionValue is bool b && !b)
                    {
                        if (_deferredTextViewListeners == null)
                            _deferredTextViewListeners = new List<Lazy<ITextViewCreationListener, IDeferrableContentTypeAndTextViewRoleMetadata>>();
                        _deferredTextViewListeners.Add(matchingExtension);
                        continue;
                    }
                }
                var instantiatedExtension = ComponentContext.GuardedOperations.InstantiateExtension(matchingExtension, matchingExtension);
                if (instantiatedExtension != null)
                    ComponentContext.GuardedOperations.CallExtensionPoint(instantiatedExtension, () => instantiatedExtension.TextViewCreated(this));
            }
        }

        private string _viewBackgroundId;
        private int _millisecondsSinceMouseMove;
        internal IList<MouseHoverEventData> _mouseHoverEvents = new List<MouseHoverEventData>();

        private string ViewBackgroundId
        {
            get
            {
                _viewBackgroundId = _viewBackgroundId ?? (Roles.Contains("EMBEDDED_PEEK_TEXT_VIEW") ? "Peek Background" : "TextView Background");
                return _viewBackgroundId;
            }
        }

        private Brush GetBackgroundColorFromFormatMap()
        {
            return SystemColors.WindowBrush;
        }


        private void InitializeLayers()
        {
            Focusable = true;
            FocusVisualStyle = null;
            Cursor = Cursors.IBeam;
            _provisionalTextHighlight = new ProvisionalTextHighlight(this);
            _manipulationLayer.Background = Brushes.Transparent;
            _baseLayer.TryAddElement("Text", _contentLayer);
            _baseLayer.TryAddElement("Caret", _caretElement);
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
                {
                    _controlHostLayer.Children.Clear();
                }
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

        private IntPtr MouseScrollHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_hasKeyboardFocus || IsClosed)
                return;
            _hasKeyboardFocus = true;
            QueueAggregateFocusCheck();
            _caretElement.Visibility = Visibility.Visible;
            if (_selection.ActivationTracksFocus)
                _selection.IsActive = true;
            Mouse.Synchronize();
            EnableIme();
        }

        private void EnableIme()
        {
            _immSource = (HwndSource)PresentationSource.FromVisual(this);
            if (_immSource == null)
                return;
            if (Options.GetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId))
            {
                _imeDefaultWnd = IntPtr.Zero;
                _imeContext = IntPtr.Zero;
            }
            else
            {
                _imeDefaultWnd = WpfHelper.GetDefaultImeWnd();
                _imeContext = WpfHelper.GetImmContext(_imeDefaultWnd);
            }

            _imeOldContext = WpfHelper.AttachContext(_immSource, _imeContext);
            _immSource.AddHook(_immHook);
            WpfHelper.EnableImmComposition();
        }

        private void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateTrackingFocusChanges((bool)e.NewValue);
            QueueAggregateFocusCheck();
        }

        private void UpdateTrackingFocusChanges(bool track)
        {
            if (_focusedElement != null)
            {
                _focusedElement.LostKeyboardFocus -= OnFocusedElementLostKeyboardFocus;
                _focusedElement = null;
            }
            if (!track || !IsKeyboardFocusWithin)
                return;
            _focusedElement = Keyboard.FocusedElement;
            if (_focusedElement == null)
                return;
            _focusedElement.LostKeyboardFocus += OnFocusedElementLostKeyboardFocus;
        }

        private void OnFocusedElementLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdateTrackingFocusChanges(true);
            QueueAggregateFocusCheck();
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

        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!_hasKeyboardFocus || IsClosed)
                return;
            _hasKeyboardFocus = false;
            if (_imeContext != IntPtr.Zero)
            {
                var compositionString = WpfHelper.GetImmCompositionString(_imeContext, 8);
                if (!string.IsNullOrEmpty(compositionString))
                    RaiseTextInputEvent(true, compositionString);
                WpfHelper.ImmNotifyIme(_imeContext, 21, 4, 0);
            }

            DisableIme();
            _caretElement.Visibility = Visibility.Hidden;
            if (_selection.ActivationTracksFocus)
                _selection.IsActive = false;
            QueueAggregateFocusCheck();
        }

        private void RaiseTextInputEvent(bool final, string text)
        {
            TextComposition composition = new ImeTextComposition(InputManager.Current, Keyboard.FocusedElement, text);
            var compositionEventArgs1 = new TextCompositionEventArgs(Keyboard.PrimaryDevice, composition);
            var compositionEventArgs2 = new TextCompositionEventArgs(Keyboard.PrimaryDevice, composition);
            if (final)
            {
                compositionEventArgs1.RoutedEvent = TextCompositionManager.PreviewTextInputEvent;
                compositionEventArgs2.RoutedEvent = TextCompositionManager.TextInputEvent;
            }
            else if (_provisionalTextHighlight.ProvisionalSpan == null)
            {
                compositionEventArgs1.RoutedEvent = TextCompositionManager.PreviewTextInputStartEvent;
                compositionEventArgs2.RoutedEvent = TextCompositionManager.TextInputStartEvent;
            }
            else
            {
                compositionEventArgs1.RoutedEvent = TextCompositionManager.PreviewTextInputUpdateEvent;
                compositionEventArgs2.RoutedEvent = TextCompositionManager.TextInputUpdateEvent;
            }
            RaiseEvent(compositionEventArgs1);
            RaiseEvent(compositionEventArgs2);
        }

        internal void QueueAggregateFocusCheck(bool checkForFocus = true)
        {
            if (_queuedFocusSettersCount > 0 || IsClosed)
                return;
            var flag = false;
            if (checkForFocus)
            {
                flag = _spaceReservationStack.HasAggregateFocus;
                if (!flag && IsKeyboardFocusWithin)
                {
                    var focusedElement = Keyboard.FocusedElement as DependencyObject;
                    if (focusedElement == this || !AggregateFocusInterceptor.GetInterceptsAggregateFocus(focusedElement))
                        flag = true;
                }
            }
            if (HasAggregateFocus == flag)
                return;
            HasAggregateFocus = flag;
            if (HasAggregateFocus)
            {
                ViewWithAggregateFocus?.QueueAggregateFocusCheck(false);
                ViewWithAggregateFocus = this;
                //if (this._undoHistory != null)
                //    this._undoHistory.Properties[(object)typeof(ITextView)] = (object)this;
            }
            else
                ViewWithAggregateFocus = null;
            ComponentContext.GuardedOperations.RaiseEvent(this, HasAggregateFocus ? GotAggregateFocus : LostAggregateFocus);
        }

        private void DisableIme()
        {
            if (_immSource == null)
                return;
            _immSource.RemoveHook(_immHook);
            WpfHelper.AttachContext(_immSource, _imeOldContext);
            _imeOldContext = IntPtr.Zero;
            WpfHelper.ReleaseContext(_imeDefaultWnd, _imeContext);
            _immSource = null;
            _imeContext = IntPtr.Zero;
            _imeDefaultWnd = IntPtr.Zero;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsClosed)
                return;
            _mouseHoverTimer.Stop();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (IsClosed)
                return;
            _mouseHoverTimer.Stop();
            _lastHoverPosition = new int?();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsClosed || e.LeftButton != MouseButtonState.Released || (e.MiddleButton != MouseButtonState.Released || e.RightButton != MouseButtonState.Released))
                return;
            HandleMouseMove(e.GetPosition(this));
        }

        private void HandleMouseMove(Point pt)
        {
            
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsClosed)
                return;
            Size newSize;
            if (_controlHostLayer.Height != e.NewSize.Height)
            {
                lock (_invalidatedSpans)
                    QueueLayout();
                var controlHostLayer = _controlHostLayer;
                newSize = e.NewSize;
                var height = newSize.Height;
                controlHostLayer.Height = height;
                ViewportHeightChanged?.Invoke(this, new EventArgs());
            }
            var width1 = _controlHostLayer.Width;
            newSize = e.NewSize;
            var width2 = newSize.Width;
            if (width1 == width2)
                return;
            var controlHostLayer1 = _controlHostLayer;
            newSize = e.NewSize;
            var width3 = newSize.Width;
            controlHostLayer1.Width = width3;
            if ((WordWrapStyle & WordWrapStyles.WordWrap) == WordWrapStyles.WordWrap)
            {
                lock (_invalidatedSpans)
                    QueueLayout();
            }
            else if (!_layoutNeeded)
            {
                newSize = e.NewSize;
                var width4 = newSize.Width;
                newSize = e.NewSize;
                var height = newSize.Height;
                UpdateVisibleArea(width4, height);
                RaiseLayoutChangeEvent();
            }
            ViewportWidthChanged?.Invoke(this, new EventArgs());
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (IsClosed)
                return;
            (e.OldSource as HwndSource)?.RemoveHook(MouseScrollHook);
            (e.NewSource as HwndSource)?.AddHook(MouseScrollHook);
            var d = this as DependencyObject;
            do
            {
                if (!InputMethod.GetIsInputMethodSuspended(d))
                    InputMethod.SetIsInputMethodSuspended(d, true);
                d = VisualTreeHelper.GetParent(d);
            } while (d != null);

            UpdateFormattingMode();
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                _mouseHoverTimer.Stop();
                _lastHoverPosition = new int?();
            }
            else if (_layoutNeeded)
                QueueLayout();
            QueueSpaceReservationStackRefresh();
        }

        private void OnVisualBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (IsClosed)
                return;
            var spanList = new List<Span>(e.Changes.Count) as IList<Span>;
            foreach (var change in e.Changes)
            {
                var span = change.OldSpan;
                if (change.NewLength > 0 && span.Start > 0 && (e.After[change.NewPosition] == '\n' && e.Before[span.Start - 1] == '\r'))
                    span = Span.FromBounds(span.Start - 1, span.End);
                var trackingSpan = e.Before.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                spanList.Add(trackingSpan.GetSpan(_visualSnapshot));
            }
            lock (_invalidatedSpans)
                _invalidatedSpans.AddRange(spanList);
            AdvanceSnapshotOnUiThread(e);
        }

        private void AdvanceSnapshotOnUiThread(TextSnapshotChangedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(() => AdvanceSnapshot(e), DispatcherPriority.Normal);
            else if (InOuterLayout)
                Dispatcher.BeginInvoke((Action)(() => AdvanceSnapshot(e)), DispatcherPriority.Normal, Array.Empty<object>());
            else
                AdvanceSnapshot(e);
        }

        private void AdvanceSnapshot(TextSnapshotChangedEventArgs e)
        {
            if (_visualBuffer.CurrentSnapshot != e.After)
                return;
            PerformLayout(TextBuffer.CurrentSnapshot, e.After);
        }

        private void OnVisualBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            AdvanceSnapshotOnUiThread(e);
        }

        public SnapshotSpan GetTextElementSpan(SnapshotPoint position)
        {
            ValidateBufferPosition(position);
            return GetTextViewLineContainingBufferPosition(position).GetTextElementSpan(position);
        }

        private void PerformLayout(ITextSnapshot newSnapshot, ITextSnapshot newVisualSnapshot)
        {
            var verticalDistance = 0.0;
            SnapshotPoint anchorPosition;
            if (_textViewLinesCollection == null)
                anchorPosition = new SnapshotPoint(newSnapshot, 0);
            else
            {
                var firstVisibleLine = _textViewLinesCollection.FirstVisibleLine;
                verticalDistance = firstVisibleLine.Top - ViewportTop;
                anchorPosition = firstVisibleLine.Start.TranslateTo(newSnapshot, PointTrackingMode.Negative);
            }

            PerformLayout(newSnapshot, newVisualSnapshot, anchorPosition, verticalDistance, ViewRelativePosition.Top, ViewportWidth,
                ViewportHeight, true, new CancellationToken?());
        }

        private void PerformLayout(ITextSnapshot newSnapshot, ITextSnapshot newVisualSnapshot,
            SnapshotPoint anchorPosition, double verticalDistance, ViewRelativePosition relativeTo,
            double effectiveViewportWidth, double effectiveViewportHeight, bool preserveViewportTop,
            CancellationToken? cancel = null)
        {
            if (IsClosed)
                return;
            if (InOuterLayout)
                throw new InvalidOperationException();

            InOuterLayout = true;
            try
            {
                var useDisplayMode = ShouldUseDisplayMode();
                NormalizedSpanCollection invalidSpans1;
                NormalizedSpanCollection invalidSpans2;
                lock (_invalidatedSpans)
                {
                    _layoutNeeded = false;
                    var flag = (WordWrapStyle & WordWrapStyles.WordWrap) == WordWrapStyles.WordWrap &&
                               _oldState.ViewportWidth != effectiveViewportWidth;
                    invalidSpans1 =
                        (FormattedLineSource == null
                            ? 1
                            : (useDisplayMode != FormattedLineSource.UseDisplayMode ? 1 : 0)) != 0 || flag
                            ? new NormalizedSpanCollection(new Span(0, VisualSnapshot.Length))
                            : new NormalizedSpanCollection(_invalidatedSpans);
                    _invalidatedSpans.Clear();
                    invalidSpans2 = new NormalizedSpanCollection(_reclassifiedSpans);
                    _reclassifiedSpans.Clear();
                }

                var formattedLineGroupList1 = new List<FormattedLineGroup>(_attachedLineCache.Count);
                var formattedLineGroupList2 = new List<FormattedLineGroup>(_attachedLineCache.Count);
                foreach (var lineGroup in _attachedLineCache)
                {
                    if (IsLineInvalid(lineGroup.Line, invalidSpans1))
                        formattedLineGroupList1.Add(lineGroup);
                    else
                    {
                        lineGroup.InUse = false;
                        formattedLineGroupList2.Add(lineGroup);
                        lineGroup.Reclassified = IsLineInvalid(lineGroup.Line, invalidSpans2);
                    }
                }

                _attachedLineCache = formattedLineGroupList2;
                var formattedLineGroupList3 = new List<FormattedLineGroup>(8);
                foreach (var formattedLineGroup in _unattachedLineCache)
                {
                    if (formattedLineGroup.InUse && !IsLineInvalid(formattedLineGroup.Line, invalidSpans1) &&
                        !IsLineInvalid(formattedLineGroup.Line, invalidSpans2))
                    {
                        formattedLineGroup.InUse = false;
                        formattedLineGroupList3.Add(formattedLineGroup);
                    }
                    else
                        formattedLineGroup.Dispose();
                }

                _unattachedLineCache = formattedLineGroupList3;
                _lineRightCache.InvalidateSpans(invalidSpans1);
                try
                {
                    if (TextSnapshot != newSnapshot || _visualSnapshot != newVisualSnapshot)
                    {
                        TextSnapshot = newSnapshot;
                        _visualSnapshot = newVisualSnapshot;
                        foreach (var formattedLineGroup in _attachedLineCache)
                            formattedLineGroup.SetSnapshotAndChange(_visualSnapshot, TextSnapshot);
                        foreach (var formattedLineGroup in _unattachedLineCache)
                            formattedLineGroup.SetSnapshotAndChange(_visualSnapshot, TextSnapshot);
                        _lineRightCache.SetSnapshot(_visualSnapshot);
                    }
                    else
                    {
                        foreach (var formattedLineGroup in _attachedLineCache)
                            formattedLineGroup.ResetChange();
                    }
                    foreach (var linesInTextViewLine in _formattedLinesInTextViewLines)
                        linesInTextViewLine.SetChange(TextViewLineChange.None);
                    FormattedLineSource = ComponentContext.FormattedTextSourceFactoryService.Create(
                            TextSnapshot, _visualSnapshot, Options.GetTabSize(), 2.0,
                            (WordWrapStyle & WordWrapStyles.WordWrap) != WordWrapStyles.None
                                ? effectiveViewportWidth
                                : 0.0,
                            (WordWrapStyle & WordWrapStyles.AutoIndent) != WordWrapStyles.None
                                ? effectiveViewportWidth * 0.25
                                : 0.0, useDisplayMode, _classifier, _sequencer, ClassificationFormatMap, IsViewWrapEnabled);
                    InLayout = true;
                    try
                    {
                        InnerPerformLayout(anchorPosition, verticalDistance, relativeTo, effectiveViewportWidth,
                            effectiveViewportHeight, preserveViewportTop, cancel);
                        foreach (var formattedLindGroup in formattedLineGroupList1)
                            formattedLindGroup.Dispose();
                    }
                    finally
                    {
                        InLayout = false;
                    }

                    RawMaxTextRightCoordinate = _lineRightCache.MaxRight;
                }
                catch (Exception e)
                {
                    _lastPerformLayoutException = e;
                    _lastPerformLayoutExceptionStackTrace = e.StackTrace;
                    throw;
                }
                var textViewLineList1 = new List<ITextViewLine>();
                var textViewLineList2 = new List<ITextViewLine>();
                foreach (var textViewLines in _textViewLinesCollection)
                {
                    switch (textViewLines.Change)
                    {
                        case TextViewLineChange.NewOrReformatted:
                            textViewLineList1.Add(textViewLines);
                            break;
                        case TextViewLineChange.Translated:
                            textViewLineList2.Add(textViewLines);
                            break;
                    }
                }

                _baseLayer.SetSnapshotAndUpdate(TextSnapshot, 0.0, ViewportTop - _oldState.ViewportTop,
                    textViewLineList1, textViewLineList2);
                _overlayLayer.SetSnapshotAndUpdate(TextSnapshot, 0.0, 0.0, textViewLineList1, _emptyLines);
                RaiseLayoutChangeEvent(effectiveViewportWidth, effectiveViewportHeight, textViewLineList1, textViewLineList2);
                if (_lastHoverPosition.HasValue)
                {
                    _mouseHoverTimer.Stop();
                    _lastHoverPosition = new int?();
                }
                QueueSpaceReservationStackRefresh();
                if (!_imeActive)
                    return;
                PositionImmCompositionWindow();
            }
            finally
            {
                InOuterLayout = false;
            }
        }

        private void InnerPerformLayout(SnapshotPoint anchorPosition, double verticalDistance, ViewRelativePosition relativeTo, double effectiveViewportWidth, double effectiveViewportHeight, bool preserveViewportTop, CancellationToken? cancel)
        {
            var attachedLineCache = _attachedLineCache;
            var formattedLineGroupList = DoCompleteLayout(anchorPosition, verticalDistance, relativeTo, effectiveViewportHeight, out var referenceLine, out var distanceAboveReferenceLine, out var distanceBelowReferenceLine, cancel);
            if (referenceLine - distanceAboveReferenceLine > 0.0)
            {
                formattedLineGroupList = DoCompleteLayout(_attachedLineCache[0].FormattedLines[0].Start, 0.0, ViewRelativePosition.Top, effectiveViewportHeight, out referenceLine, out distanceAboveReferenceLine, out distanceBelowReferenceLine, cancel);
            }
            else
            {
                var val1 = Math.Min(LineHeight, effectiveViewportHeight);
                if (referenceLine + distanceBelowReferenceLine < val1)
                {
                    var formattedLineGroup = formattedLineGroupList[formattedLineGroupList.Count - 1];
                    var formattedLine = formattedLineGroup.FormattedLines[formattedLineGroup.FormattedLines.Count - 1];
                    var num = Math.Min(val1, formattedLine.Height);
                    if (referenceLine + distanceBelowReferenceLine < num)
                        formattedLineGroupList = DoCompleteLayout(formattedLine.Start, num - formattedLine.Height, ViewRelativePosition.Top, effectiveViewportHeight, out referenceLine, out distanceAboveReferenceLine, out distanceBelowReferenceLine, cancel);
                }
            }

            _attachedLineCache = formattedLineGroupList;
            _formattedLinesInTextViewLines.Clear();
            IFormattedLine formattedLine1 = null;
            IFormattedLine formattedLine2 = null;
            foreach (var formattedLineGroup in _attachedLineCache)
            {
                formattedLineGroup.InUse = true;
                var formattedLines = formattedLineGroup.FormattedLines;
                foreach (var formattedline3 in formattedLines)
                {
                    _formattedLinesInTextViewLines.Add(formattedline3);
                    if (formattedline3.Change != TextViewLineChange.NewOrReformatted && formattedLine1 == null)
                    {
                        if (formattedline3.ExtentIncludingLineBreak.Contains(anchorPosition))
                            formattedLine1 = formattedline3;
                        else if (formattedLine2 == null)
                            formattedLine2 = formattedline3;
                    }
                }
            }

            if (formattedLine1 == null)
                formattedLine1 = formattedLine2;

            double num1;
            double num2;
            if (formattedLine1 != null)
            {
                var num3 = 0.0;
                for (var index = 0; _formattedLinesInTextViewLines[index] != formattedLine1; ++index)
                    num3 += _formattedLinesInTextViewLines[index].Height;
                num1 = formattedLine1.Top - num3;
                var num4 = referenceLine - distanceAboveReferenceLine;
                num2 = num1 - num4;
            }
            else
            {
                num2 = preserveViewportTop ? ViewportTop : 0.0;
                num1 = num2 + referenceLine - distanceAboveReferenceLine;
            }

            var top = num1;
            foreach (var linesInTextViewLine in _formattedLinesInTextViewLines)
            {
                if (top != linesInTextViewLine.Top)
                {
                    if (linesInTextViewLine.Change == TextViewLineChange.None)
                    {
                        linesInTextViewLine.SetChange(TextViewLineChange.Translated);
                        linesInTextViewLine.SetDeltaY(top - linesInTextViewLine.Top);
                    }
                    linesInTextViewLine.SetTop(top);
                }

                top += linesInTextViewLine.Height;
                if (ViewportTop != num2)
                {
                    ViewportTop = num2;
                    Canvas.SetTop(_baseLayer, -ViewportTop);
                }
                UpdateVisibleArea(effectiveViewportWidth, effectiveViewportHeight);
                TrimHiddenLines();
                _textViewLinesCollection?.Invalidate();
                _textViewLinesCollection = new TextViewLineCollection(this, _formattedLinesInTextViewLines);
                _contentLayer.SetTextViewLines(_formattedLinesInTextViewLines);
                foreach (var formattedLineGroup in attachedLineCache)
                {
                    if (!formattedLineGroup.InUse)
                        formattedLineGroup.Dispose();
                    else
                        formattedLineGroup.Reclassified = false;
                }
            }

        }

        private void TrimHiddenLines()
        {
            for (var index = _formattedLinesInTextViewLines.Count - 2; index >= 0; --index)
            {
                if (_formattedLinesInTextViewLines[index].VisibilityState == VisibilityState.Hidden)
                    continue;
                TrimList(_formattedLinesInTextViewLines, index + 2, _formattedLinesInTextViewLines.Count);
                break;
            }
            for (var index = 1; index < _formattedLinesInTextViewLines.Count; ++index)
            {
                if (_formattedLinesInTextViewLines[index].VisibilityState == VisibilityState.Hidden)
                    continue;
                TrimList(_formattedLinesInTextViewLines, 0, index - 1);
                break;
            }
        }

        private static void TrimList(List<IFormattedLine> lines, int start, int end)
        {
            var count = end - start;
            if (count <= 0)
                return;
            lines.RemoveRange(start, count);
        }

        private List<FormattedLineGroup> DoCompleteLayout(SnapshotPoint anchorPosition,
            double verticalDistance, ViewRelativePosition relativeTo, double effectiveViewportHeight,
            out double referenceLine, out double distanceAboveReferenceLine, out double distanceBelowReferenceLine,
            CancellationToken? cancel)
        {
            var containingLine = TextViewModel.GetNearestPointInVisualSnapshot(anchorPosition, _visualSnapshot, PointTrackingMode.Positive).GetContainingLine();
            referenceLine = relativeTo == ViewRelativePosition.Top || relativeTo == (ViewRelativePosition)2 ? verticalDistance : effectiveViewportHeight - verticalDistance;
            var formattedLineGroup = DoAnchorLayout(containingLine, anchorPosition, referenceLine, relativeTo, out distanceAboveReferenceLine, out distanceBelowReferenceLine, cancel);
            var formattedLineGroupList1 = DoLayoutUp(containingLine, referenceLine, ref distanceAboveReferenceLine, cancel);
            var formattedLineGroupList2 = DoLayoutDown(containingLine, referenceLine, effectiveViewportHeight, ref distanceBelowReferenceLine, cancel);
            var formattedLineGroupList3 = new List<FormattedLineGroup>(formattedLineGroupList1.Count + formattedLineGroupList2.Count + 1);
            formattedLineGroupList3.AddRange(formattedLineGroupList1);
            formattedLineGroupList3.Add(formattedLineGroup);
            formattedLineGroupList3.AddRange(formattedLineGroupList2);
            return formattedLineGroupList3;
        }

        private FormattedLineGroup DoAnchorLayout(ITextSnapshotLine visualLine, SnapshotPoint anchorPosition, double referenceLine, ViewRelativePosition relativeTo, out double distanceAboveReferenceLine, out double distanceBelowReferenceLine, CancellationToken? cancel)
        {
            var group = FormatSnapshotLine(visualLine, cancel);
            var count = group.FormattedLines.Count;
            do
            {
            }
            while (count > 0 && group.FormattedLines[--count].Start > anchorPosition.Position);
            var formattedLine1 = group.FormattedLines[count];
            CalculateAndSetTransform(formattedLine1, referenceLine, relativeTo & ViewRelativePosition.Bottom);
            switch (relativeTo)
            {
                case ViewRelativePosition.Top:
                    distanceAboveReferenceLine = 0.0;
                    distanceBelowReferenceLine = formattedLine1.Height;
                    break;
                case (ViewRelativePosition)2:
                    distanceAboveReferenceLine = formattedLine1.TextTop - formattedLine1.Top;
                    distanceBelowReferenceLine = formattedLine1.Height - distanceAboveReferenceLine;
                    break;
                case (ViewRelativePosition)3:
                    distanceBelowReferenceLine = formattedLine1.Bottom - formattedLine1.TextBottom;
                    distanceAboveReferenceLine = formattedLine1.Height - distanceBelowReferenceLine;
                    break;
                default:
                    distanceAboveReferenceLine = formattedLine1.Height;
                    distanceBelowReferenceLine = 0.0;
                    break;
            }
            for (var index = count - 1; index >= 0; --index)
            {
                var formattedLine2 = group.FormattedLines[index];
                CalculateAndSetTransform(formattedLine2, referenceLine - distanceAboveReferenceLine, ViewRelativePosition.Bottom);
                distanceAboveReferenceLine += formattedLine2.Height;
            }
            for (var index = count + 1; index < group.FormattedLines.Count; ++index)
            {
                var formattedLine2 = group.FormattedLines[index];
                CalculateAndSetTransform(formattedLine2, referenceLine + distanceBelowReferenceLine, ViewRelativePosition.Top);
                distanceBelowReferenceLine += formattedLine2.Height;
            }
            UpdateLineRightCache(group);
            return group;
        }

        private FormattedLineGroup FormatSnapshotLine(ITextSnapshotLine visualLine, CancellationToken? cancel)
        {
            var position1 = visualLine.Start.Position;
            var index1 = 0;
            var num = _attachedLineCache.Count;
            while (index1 < num)
            {
                var index2 = (index1 + num) / 2;
                var formattedLineGroup = _attachedLineCache[index2];
                var position2 = formattedLineGroup.Line.Start.Position;
                if (position1 < position2)
                    num = index2;
                else if (position1 > position2)
                {
                    index1 = index2 + 1;
                }
                else
                {
                    if (formattedLineGroup.Reclassified)
                    {
                        IList<IFormattedLine> formattedLineList = FormattedLineSource.FormatLineInVisualBufferIfChanged(visualLine, formattedLineGroup.FormattedLines, cancel);
                        if (formattedLineList != null)
                            formattedLineGroup.FormattedLines = formattedLineList;
                        formattedLineGroup.Reclassified = false;
                    }
                    return formattedLineGroup;
                }
            }
            IList<IFormattedLine> formattedLines = FormattedLineSource.FormatLineInVisualBuffer(visualLine, cancel);
            var formattedLineGroup1 = new FormattedLineGroup(visualLine, formattedLines);
            _attachedLineCache.Insert(index1, formattedLineGroup1);
            return formattedLineGroup1;
        }

        private void UpdateLineRightCache(FormattedLineGroup group)
        {
            var right = group.FormattedLines[0].LineTransform.Right;
            for (var index = 1; index < group.FormattedLines.Count; ++index)
            {
                var formattedLine = group.FormattedLines[index];
                var lineTransform = formattedLine.LineTransform;
                if (lineTransform.Right > right)
                {
                    lineTransform = formattedLine.LineTransform;
                    right = lineTransform.Right;
                }
            }
            if (right == group.Right)
                return;
            group.Right = right;
            _lineRightCache.AddLine(group.Line, right);
        }

        private IList<FormattedLineGroup> DoLayoutUp(ITextSnapshotLine visualLine, double referenceLine, ref double distanceAboveReferenceLine, CancellationToken? cancel)
        {
            var formattedLineGroupList = new List<FormattedLineGroup>();
            while (visualLine.Start != 0)
            {
                var num = distanceAboveReferenceLine;
                visualLine = visualLine.Snapshot.GetLineFromPosition(visualLine.Start - 1);
                var group = FormatSnapshotLine(visualLine, cancel);
                formattedLineGroupList.Add(group);
                for (var index = group.FormattedLines.Count - 1; index >= 0; --index)
                {
                    var formattedLine = group.FormattedLines[index];
                    CalculateAndSetTransform(formattedLine, referenceLine - distanceAboveReferenceLine, ViewRelativePosition.Bottom);
                    distanceAboveReferenceLine += formattedLine.Height;
                }
                UpdateLineRightCache(group);
                if (referenceLine - num <= 0.0)
                    break;
            }
            formattedLineGroupList.Reverse();
            return formattedLineGroupList;
        }

        private IList<FormattedLineGroup> DoLayoutDown(ITextSnapshotLine visualLine, double referenceLine, double effectiveViewportHeight, ref double distanceBelowReferenceLine, CancellationToken? cancel)
        {
            var formattedLineGroupList = new List<FormattedLineGroup>();
            while (visualLine.LineBreakLength != 0)
            {
                var num = distanceBelowReferenceLine;
                visualLine = visualLine.Snapshot.GetLineFromPosition(visualLine.EndIncludingLineBreak);
                var group = FormatSnapshotLine(visualLine, cancel);
                formattedLineGroupList.Add(group);
                foreach (var formattedLine in group.FormattedLines)
                {
                    CalculateAndSetTransform(formattedLine, referenceLine + distanceBelowReferenceLine, ViewRelativePosition.Top);
                    distanceBelowReferenceLine += formattedLine.Height;
                }
                UpdateLineRightCache(group);
                if (num + referenceLine >= effectiveViewportHeight)
                    break;
            }
            return formattedLineGroupList;
        }

        private void CalculateAndSetTransform(IFormattedLine formattedLine, double yPosition, ViewRelativePosition placement)
        {
            var lineTransform = GetLineTransform(formattedLine, yPosition, placement);
            if (!(lineTransform != formattedLine.LineTransform))
                return;
            formattedLine.SetLineTransform(lineTransform);
            formattedLine.SetChange(TextViewLineChange.NewOrReformatted);
        }

        public LineTransform GetLineTransform(ITextViewLine formattedLine, double yPosition, ViewRelativePosition placement)
        {
            var transform1 = formattedLine.DefaultLineTransform;
            if (!_inConstructor)
            {
                foreach (var lineTransformSource in _lineTransformSources)
                {
                    try
                    {
                        transform1 = LineTransform.Combine(transform1, lineTransformSource.GetLineTransform(formattedLine, yPosition, placement));
                    }
                    catch (Exception ex)
                    {
                        ComponentContext.GuardedOperations.HandleException(lineTransformSource, ex);
                    }
                }
            }
            return transform1;
        }


        public double RawMaxTextRightCoordinate { get; private set; }

        private void RaiseLayoutChangeEvent()
        {
            RaiseLayoutChangeEvent(ViewportWidth, ViewportHeight, _emptyLines, _emptyLines);
        }

        private void RaiseLayoutChangeEvent(double effectiveViewportWidth, double effectiveViewportHeight,
            IList<ITextViewLine> newOrReformattedLines, IList<ITextViewLine> translatedLines)
        {
            if (IsClosed)
                return;
            var newState = new ViewState(this, effectiveViewportWidth, effectiveViewportHeight);
            using (ComponentContext.PerformanceBlockMarker.CreateBlock("VsTextEditor.LayoutChangedEvent"))
            {
                _caretElement.LayoutChanged(_oldState.EditSnapshot, TextSnapshot);
                _selection.LayoutChanged(_oldState.VisualSnapshot != _visualSnapshot, TextSnapshot);
                if (!_inConstructor)
                {
                    var layoutChanged = LayoutChanged;
                    if (layoutChanged != null)
                        ComponentContext.GuardedOperations.RaiseEvent(this, layoutChanged,
                            new TextViewLayoutChangedEventArgs(_oldState, newState, newOrReformattedLines,
                                translatedLines));
                }

                _oldState = newState;
                foreach (var formattedLineGroup in _attachedLineCache)
                    formattedLineGroup.ClearChange();
            }
        }

        private void SetClearTypeHint()
        {
            var defaultTextProperties = ClassificationFormatMap.DefaultTextProperties;
            if (defaultTextProperties == _oldPlainTextProperties)
                return;
            _oldPlainTextProperties = defaultTextProperties;
            if (!defaultTextProperties.TypefaceEmpty && defaultTextProperties.Typeface.FontFamily.FamilyNames.TryGetValue(EnUsLanguage, out var str) &&
                string.Compare(str, "Consolas", StringComparison.OrdinalIgnoreCase) == 0)
                TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            else
                TextOptions.SetTextRenderingMode(this, TextRenderingMode.Auto);
        }

        private void PositionImmCompositionWindow()
        {
            if (!(_imeContext != IntPtr.Zero) || IsKorean())
                return;
            var containingTextViewLine = _caretElement.ContainingTextViewLine;
            if (containingTextViewLine.VisibilityState == VisibilityState.Unattached)
                return;
            var virtualBufferPosition = _caretElement.Position.VirtualBufferPosition;
            var left = containingTextViewLine.GetExtendedCharacterBounds(virtualBufferPosition).Left;
            var fontFamily = ClassificationFormatMap.DefaultTextProperties.Typeface.FontFamily;
            if (!fontFamily.FamilyNames.TryGetValue(EnUsLanguage, out var baseFont))
                baseFont = fontFamily.Source ?? string.Empty;
            WpfHelper.SetCompositionPositionAndHeight(_immSource, _imeContext, baseFont,
                Options.GetOptionValue(ImeCompositionWindowFont.KeyId),
                Options.GetOptionValue(ImeCompositionWindowTopOffset.KeyId),
                Options.GetOptionValue(ImeCompositionWindowBottomOffset.KeyId),
                Options.GetOptionValue(ImeCompositionWindowHeightOffset.KeyId),
                new Point(left - ViewportLeft, containingTextViewLine.TextTop - ViewportTop),
                containingTextViewLine.TextHeight, this, new Point(0.0, 0.0),
                new Point(ViewportWidth, ViewportHeight));
        }

        private static bool IsKorean()
        {
            return InputLanguageManager.Current.CurrentInputLanguage.LCID == 1042;
        }

        private bool ShouldUseDisplayMode()
        {
            if (PresentationSource.FromVisual(this) != null || ReadLocalValue(TextOptions.TextFormattingModeProperty) !=
                DependencyProperty.UnsetValue)
                return TextOptions.GetTextFormattingMode(this) == TextFormattingMode.Display;
            if (FormattedLineSource == null)
                return true;
            return FormattedLineSource.UseDisplayMode;
        }

        private void SubscribeToEvents()
        {
            _mouseHoverTimer.Tick += OnHoverTimer;
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);
            Options.OptionChanged += OnEditorOptionChanged;
            _sequencer.SequenceChanged += OnSequenceChanged;
            _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
            _classifier.ClassificationChanged += OnClassificationChanged;
            ClassificationFormatMap.ClassificationFormatMappingChanged += OnClassificationFormatMapChange;
            TextViewModel.DataModel.ContentTypeChanged += OnDataModelContentTypeChanged;
            SizeChanged += OnSizeChanged;
            GotKeyboardFocus += OnGotKeyboardFocus;
            LostKeyboardFocus += OnLostKeyboardFocus;
            IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
            IsVisibleChanged += OnVisibleChanged;
            Selection.SelectionChanged += OnSelectionChanged;
            MouseLeave += OnMouseLeave;
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void OnClassificationFormatMapChange(object sender, EventArgs e)
        {
            lock (_invalidatedSpans)
            {
                if (_attachedLineCache.Count <= 0)
                {
                    if (_unattachedLineCache.Count <= 0)
                    {
                        SetClearTypeHint();
                        return;
                    }
                }
                _reclassifiedSpans.Clear();
                _reclassifiedSpans.Add(new Span(0, _visualSnapshot.Length));
                QueueLayout();
            }
            SetClearTypeHint();
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (IsClosed || !e.ChangedItems.Contains(ViewBackgroundId))
                return;
            Background = GetBackgroundColorFromFormatMap();
        }

        private void OnDataModelContentTypeChanged(object sender, TextDataModelContentTypeChangedEventArgs e)
        {
            BindContentTypeSpecificAssets(e.BeforeContentType, e.AfterContentType);
        }

        private void OnClassificationChanged(object sender, ClassificationChangedEventArgs e)
        {
            if (IsClosed)
                return;
            var span = Span.FromBounds(TextViewModel.GetNearestPointInVisualSnapshot(e.ChangeSpan.Start, _visualSnapshot, PointTrackingMode.Negative), TextViewModel.GetNearestPointInVisualSnapshot(e.ChangeSpan.End, _visualSnapshot, PointTrackingMode.Positive));
            if (span.Length <= 0)
                return;
            span = new Span(span.Start, span.Length - 1);
            lock (_invalidatedSpans)
            {
                if (_attachedLineCache.Count <= 0 && _unattachedLineCache.Count <= 0)
                    return;
                _reclassifiedSpans.Add(span);
                QueueLayout();
            }
        }

        private void OnSequenceChanged(object sender, TextAndAdornmentSequenceChangedEventArgs e)
        {
            var spans = e.Span.GetSpans(TextSnapshot);
            var spanList = new List<Span>(spans.Count);
            foreach (var snapshotSpan in spans)
            {
                var inVisualSnapshot1 = TextViewModel.GetNearestPointInVisualSnapshot(snapshotSpan.Start, _visualSnapshot, PointTrackingMode.Negative);
                var inVisualSnapshot2 = TextViewModel.GetNearestPointInVisualSnapshot(snapshotSpan.End, _visualSnapshot, PointTrackingMode.Positive);
                spanList.Add(Span.FromBounds(inVisualSnapshot1, inVisualSnapshot2));
            }
            if (spanList.Count <= 0)
                return;
            lock (_invalidatedSpans)
            {
                if (_attachedLineCache.Count <= 0 && _unattachedLineCache.Count <= 0)
                    return;
                _reclassifiedSpans.AddRange(spanList);
                QueueLayout();
            }
        }

        private void OnEditorOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (IsClosed)
                return;
            if (e.OptionId == DefaultOptions.TabSizeOptionId.Name)
                InvalidateAllLines();
            else if (e.OptionId == DefaultTextViewOptions.WordWrapStyleId.Name)
            {
                if ((WordWrapStyle & WordWrapStyles.WordWrap) == WordWrapStyles.WordWrap)
                    ViewportLeft = 0.0;
                InvalidateAllLines();
            }
            else if (e.OptionId == DefaultTextViewOptions.ViewProhibitUserInputId.Name)
            {
                if (_immSource == null)
                    return;
                DisableIme();
                EnableIme();
            }
            else if (e.OptionId == DefaultViewOptions.ZoomLevelId.Name && Roles.Contains("ZOOMABLE"))
            {
                ZoomLevel = Options.GetOptionValue(DefaultViewOptions.ZoomLevelId);
            }
            else
            {
                if (_deferredTextViewListeners == null)
                    return;
                for (var index = 0; index < _deferredTextViewListeners.Count; ++index)
                {
                    var textViewListener = _deferredTextViewListeners[index];
                    if (textViewListener.Metadata.OptionName == e.OptionId)
                    {
                        _deferredTextViewListeners.RemoveAt(index);
                        if (_deferredTextViewListeners.Count == 0)
                            _deferredTextViewListeners = null;
                        var instantiatedExtension = ComponentContext.GuardedOperations.InstantiateExtension(textViewListener, textViewListener);
                        if (instantiatedExtension != null)
                            ComponentContext.GuardedOperations.CallExtensionPoint(instantiatedExtension, () => instantiatedExtension.TextViewCreated(this));
                        break;
                    }
                }
            }
        }

        private void OnHoverTimer(object sender, EventArgs e)
        {
            if(IsClosed)
                return;
            _millisecondsSinceMouseMove += (int) (_mouseHoverTimer.Interval.Ticks / 10000L);
            if (!IsVisible || !_lastHoverPosition.HasValue)
                return;
            RaiseHoverEvents();
        }

        private void RaiseHoverEvents()
        {
        }

        private void UnsubscribeFromEvents()
        {
            _mouseHoverTimer.Tick -= OnHoverTimer;
            MouseMove -= OnMouseMove;
            MouseLeftButtonDown -= OnMouseDown;
            MouseDown -= OnMouseDown;
            MouseRightButtonDown -= OnMouseDown;
            MouseLeave -= OnMouseLeave;
            Options.OptionChanged -= OnEditorOptionChanged;
            _sequencer.SequenceChanged -= OnSequenceChanged;
            _classifier.ClassificationChanged -= OnClassificationChanged;
            ClassificationFormatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMapChange;
            _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
            SizeChanged -= OnSizeChanged;
            GotKeyboardFocus -= OnGotKeyboardFocus;
            LostKeyboardFocus -= OnLostKeyboardFocus;
            IsKeyboardFocusWithinChanged -= OnIsKeyboardFocusWithinChanged;
            IsVisibleChanged -= OnVisibleChanged;
            Selection.SelectionChanged -= OnSelectionChanged;
            _visualBuffer.ChangedLowPriority -= OnVisualBufferChanged;
            _visualBuffer.ContentTypeChanged -= OnVisualBufferContentTypeChanged;
            TextViewModel.DataModel.ContentTypeChanged -= OnDataModelContentTypeChanged;
            PresentationSource.RemoveSourceChangedHandler(this, OnSourceChanged);
            (PresentationSource.FromVisual(this) as HwndSource)?.RemoveHook(MouseScrollHook);
        }

        private void UpdateVisibleArea(double viewportWidth, double viewportHeight)
        {
            var area = new Rect(ViewportLeft, ViewportTop, viewportWidth, viewportHeight);
            foreach (var line in _formattedLinesInTextViewLines)
                line.SetVisibleArea(area);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch (msg)
            {
                case 61:
                    break;
                case 256:
                    if ((int) wparam == 25 && IsKorean() && !_selection.IsEmpty)
                    {
                        SnapshotPoint position = _selection.Start.Position;
                        char ch = TextSnapshot[position];
                        if (IsHangul(ch) && WpfHelper.HanjaConversion(_imeContext, WpfHelper.GetKeyboardLayout(), ch))
                        {
                            _selection.Select(new SnapshotSpan(position, 1), false);
                            handled = true;
                        }
                    }
                    break;
                case 269:
                    break;
                case 270:
                    break;
                case 271:
                    break;
                case 648:
                    break;
            }
            return IntPtr.Zero;
        }

        internal static bool IsHangul(char ch)
        {
            var flag = false;
            if (ch < 'ᄀ') return flag;
            if (ch <= 'ᇿ')
                flag = true;
            else if (ch >= '\x3130')
            {
                if (ch <= '\x318F')
                    flag = true;
                else if (ch >= '가')
                {
                    if (ch <= '힣')
                        flag = true;
                    else if (ch >= 'ﾠ' && ch <= '\xFFDF')
                        flag = true;
                }
            }
            return flag;
        }

        private static bool IsLineInvalid(ITextSnapshotLine line, NormalizedSpanCollection invalidSpans)
        {
            var index1 = 0;
            var num = invalidSpans.Count;
            Span invalidSpan;
            while (index1 < num)
            {
                var index2 = (index1 + num) / 2;
                var start = (int)line.Start;
                invalidSpan = invalidSpans[index2];
                var end = invalidSpan.End;
                if (start <= end)
                    num = index2;
                else
                    index1 = index2 + 1;
            }

            if (index1 >= invalidSpans.Count)
                return false;
            if (line.LineBreakLength != 0)
            {
                var includingLineBreak = line.EndIncludingLineBreak;
                invalidSpan = invalidSpans[index1];
                var start = invalidSpan.Start;
                return includingLineBreak > start;
            }

            var includingLineBreak1 = line.EndIncludingLineBreak;
            invalidSpan = invalidSpans[index1];
            var start1 = invalidSpan.Start;
            return includingLineBreak1 >= start1;
        }

        private class FormattedLineGroup
        {
            public bool InUse;
            public bool Reclassified;
            public double Right = -1.0;

            public ITextSnapshotLine Line { get; private set; }

            public IList<IFormattedLine> FormattedLines { get; set; }

            public FormattedLineGroup(ITextSnapshotLine visualLine, IList<IFormattedLine> formattedLines)
            {
                Line = visualLine;
                FormattedLines = formattedLines;
            }

            public void Dispose()
            {
                foreach (var line in FormattedLines)
                    line.Dispose();
            }

            public void SetSnapshotAndChange(ITextSnapshot visualSnapshot, ITextSnapshot textSnapshot)
            {
                Line = Line.Start.TranslateTo(visualSnapshot, PointTrackingMode.Negative)
                    .GetContainingLine();
                foreach (var line in FormattedLines)
                {
                    line.SetSnapshot(visualSnapshot, textSnapshot);
                    line.SetChange(TextViewLineChange.NewOrReformatted);
                }
            }

            public void ClearChange()
            {
                foreach (var line in FormattedLines)
                {
                    line.SetChange(TextViewLineChange.None);
                    line.SetDeltaY(0.0);
                }
            }

            public void ResetChange()
            {
                foreach (var formattedLine in FormattedLines)
                    formattedLine.SetChange(TextViewLineChange.NewOrReformatted);
            }

            public override string ToString()
            {
                return Line.ExtentIncludingLineBreak.ToString();
            }
        }

        internal class LineRightCache
        {
            private readonly LineRight[] _cachedLines = new LineRight[50];
            private double _maxRight;
            private int _cachedLinesCount;

            public double MaxRight
            {
                get
                {
                    if (_maxRight == double.MaxValue)
                    {
                        if (_cachedLinesCount == 0)
                            return 0.0;
                        _maxRight = _cachedLines[0].Right;
                        for (var index = 0; index < _cachedLinesCount; index++)
                        {
                            var cachedLine = _cachedLines[index];
                            if (cachedLine.Right > _maxRight)
                                _maxRight = cachedLine.Right;
                        }
                    }
                    return _maxRight;
                }
            }

            public void AddLine(ITextSnapshotLine line, double right)
            {
                if (right > _maxRight)
                    _maxRight = right;
                for (var index = 0; index < _cachedLinesCount; ++index)
                {
                    var cachedLine = _cachedLines[index];
                    if (cachedLine.Line.Start == line.Start)
                    {
                        if (cachedLine.Right == _maxRight && right < _maxRight)
                            _maxRight = double.MaxValue;
                        cachedLine.Right = right;
                        return;
                    }
                }
                if (_cachedLinesCount < 50)
                    _cachedLines[_cachedLinesCount++] = new LineRight(line, right);
                else
                {
                    var index1 = 0;
                    for (var index2 = 1; index2 < _cachedLinesCount; ++index2)
                    {
                        if (_cachedLines[index2].Right < _cachedLines[index1].Right)
                            index1 = index2;
                    }
                    if (right <= _cachedLines[index1].Right)
                        return;
                    _cachedLines[index1] = new LineRight(line, right);
                }
            }

            public void SetSnapshot(ITextSnapshot snapshot)
            {
                for (var index = 0; index < _cachedLinesCount; ++index)
                    _cachedLines[index].SetSnapshot(snapshot);
            }

            public void InvalidateSpans(NormalizedSpanCollection invalidSpans)
            {
                var index = 0;
                while (index < _cachedLinesCount)
                {
                    var cachedLine = _cachedLines[index];
                    if (IsLineInvalid(cachedLine.Line, invalidSpans))
                    {
                        if (cachedLine.Right == _maxRight)
                            _maxRight = double.MaxValue;
                        _cachedLines[index] = _cachedLines[--_cachedLinesCount];
                        _cachedLines[_cachedLinesCount] = null;
                    }
                    else
                        ++index;
                }
            }

            private class LineRight
            {
                public double Right { get; set; }

                public ITextSnapshotLine Line { get; private set; }

                public LineRight(ITextSnapshotLine line, double right)
                {
                    Line = line;
                    Right = right;
                }

                public void SetSnapshot(ITextSnapshot snapshot)
                {
                    var position =
                        new SnapshotPoint(Line.Snapshot, Line.Start).TranslateTo(snapshot,
                            PointTrackingMode.Negative);
                    Line = snapshot.GetLineFromPosition(position);
                }
            }
        }

        public void DoActionThatShouldOnlyBeDoneAfterViewIsLoaded(Action action)
        {
            action();
            if (_hasBeenLoaded)
                return;
            _loadedAction = action;
        }

        public void DisplayTextLineContainingBufferPosition(SnapshotPoint bufferPosition, double verticalDistance, ViewRelativePosition relativeTo)
        {
            DisplayTextLineContainingBufferPosition(bufferPosition, verticalDistance, relativeTo, new double?(), new double?());
        }

        public void DisplayTextLineContainingBufferPosition(SnapshotPoint bufferPosition, double verticalDistance, ViewRelativePosition relativeTo, double? viewportWidthOverride, double? viewportHeightOverride)
        {
            ValidateBufferPosition(bufferPosition);
            switch (relativeTo)
            {
                case ViewRelativePosition.Top:
                case ViewRelativePosition.Bottom:
                case (ViewRelativePosition)2:
                case (ViewRelativePosition)3:
                    if (viewportWidthOverride.HasValue && double.IsNaN(viewportWidthOverride.Value))
                        throw new ArgumentOutOfRangeException(nameof(viewportWidthOverride));
                    if (viewportHeightOverride.HasValue && double.IsNaN(viewportHeightOverride.Value))
                        throw new ArgumentOutOfRangeException(nameof(viewportHeightOverride));
                    var textSnapshot = TextSnapshot;
                    var visualSnapshot = _visualSnapshot;
                    var anchorPosition = bufferPosition;
                    var verticalDistance1 = verticalDistance;
                    var num1 = (int)relativeTo;
                    var nullable = viewportWidthOverride;
                    var effectiveViewportWidth = nullable ?? ViewportWidth;
                    nullable = viewportHeightOverride;
                    var effectiveViewportHeight = nullable ?? ViewportHeight;
                    var num2 = 0;
                    var cancel = new CancellationToken?();
                    PerformLayout(textSnapshot, visualSnapshot, anchorPosition, verticalDistance1, (ViewRelativePosition)num1, effectiveViewportWidth, effectiveViewportHeight, num2 != 0, cancel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relativeTo), relativeTo, null);
            }
        }

        private void ValidateBufferPosition(SnapshotPoint bufferPosition)
        {
            if (bufferPosition.Snapshot != TextSnapshot)
                throw new ArgumentException();
        }

        public ITextViewLine GetTextViewLineContainingBufferPosition(SnapshotPoint bufferPosition)
        {
            return ((ITextView)this).GetTextViewLineContainingBufferPosition(bufferPosition);
        }

        public IAdornmentLayer GetAdornmentLayer(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name == "Text" || name == "Caret")
                throw new ArgumentOutOfRangeException(nameof(name), "The Text and Caret adornment layers cannot be retrieved with this method.");
            var isOverlayLayer = ComponentContext.OrderedOverlayLayerDefinitions.ContainsKey(name);
            var viewStack = isOverlayLayer ? _overlayLayer : _baseLayer;
            var element = viewStack.GetElement(name);
            if (element != null)
                return (IAdornmentLayer)element;
            var adornmentLayer = new AdornmentLayer(this, isOverlayLayer);
            if (!viewStack.TryAddElement(name, adornmentLayer))
                throw new ArgumentOutOfRangeException(nameof(name), "Could not find a matching AdornmentLayerDefinition export for the requested adornment layer name.");
            return adornmentLayer;
        }

        public bool HasAggregateFocus { get; private set; }

        public ITextCaret Caret => _caretElement;

        ITextViewLine ITextView.GetTextViewLineContainingBufferPosition(SnapshotPoint bufferPosition)
        {
            if (_textViewLinesCollection == null)
                throw new InvalidOperationException("GetTextViewLineContainingBufferPosition called before view is fully initialized.");
            if (IsClosed)
                throw new InvalidOperationException("GetTextViewLineContainingBufferPosition called after the view is closed");
            ValidateBufferPosition(bufferPosition);
            if (!InLayout)
            {
                var containingBufferPosition = _textViewLinesCollection.GetTextViewLineContainingBufferPosition(bufferPosition);
                if (containingBufferPosition != null)
                    return containingBufferPosition;
            }
            for (var index = _unattachedLineCache.Count - 1; index >= 0; --index)
            {
                var formattedLineGroup = _unattachedLineCache[index];
                foreach (var formattedLine in formattedLineGroup.FormattedLines)
                {
                    if (formattedLine.ContainsBufferPosition(bufferPosition))
                    {
                        formattedLineGroup.InUse = true;
                        if (index != _unattachedLineCache.Count - 1)
                        {
                            _unattachedLineCache.RemoveAt(index);
                            _unattachedLineCache.Add(formattedLineGroup);
                        }
                        return formattedLine;
                    }
                }
            }
            var containingLine = TextViewModel.GetNearestPointInVisualSnapshot(bufferPosition, _visualSnapshot, PointTrackingMode.Positive).GetContainingLine();
            IList<IFormattedLine> formattedLines = FormattedLineSource.FormatLineInVisualBuffer(containingLine, new CancellationToken?());
            var formattedLineGroup1 =
                new FormattedLineGroup(containingLine, formattedLines) {InUse = true};
            if (_unattachedLineCache.Count >= 8)
            {
                _unattachedLineCache[0].Dispose();
                _unattachedLineCache.RemoveAt(0);
            }
            _unattachedLineCache.Add(formattedLineGroup1);
            var index1 = formattedLineGroup1.FormattedLines.Count - 1;
            while (index1 > 0 && !formattedLineGroup1.FormattedLines[index1].ContainsBufferPosition(bufferPosition))
                --index1;
            return formattedLineGroup1.FormattedLines[index1];
        }

        internal class MouseHoverEventData
        {
            public readonly MouseHoverAttribute Attribute;
            public readonly EventHandler<MouseHoverEventArgs> EventHandler;
            public bool Fired;

            public MouseHoverEventData(EventHandler<MouseHoverEventArgs> eventHandler)
            {
                Attribute = GetMouseHoverAttribute(eventHandler);
                EventHandler = eventHandler;
                Fired = false;
            }

            private static MouseHoverAttribute GetMouseHoverAttribute(EventHandler<MouseHoverEventArgs> client)
            {
                foreach (var customAttribute in client.Method.GetCustomAttributes(typeof(MouseHoverAttribute), false))
                {
                    if (customAttribute is MouseHoverAttribute mouseHoverAttribute)
                        return mouseHoverAttribute;
                }
                return new MouseHoverAttribute(150);
            }
        }
    }
}