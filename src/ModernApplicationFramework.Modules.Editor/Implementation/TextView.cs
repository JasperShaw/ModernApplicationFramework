using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Modules.Editor.NativeMethods;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Attributes;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Implementation
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
        private static readonly IList<ITextViewLine> EmptyLines = new ReadOnlyCollection<ITextViewLine>(new List<ITextViewLine>(0));
        [ThreadStatic]
        private static TextView _viewWithAggregateFocus;
        private static readonly XmlLanguage EnUsLanguage = XmlLanguage.GetLanguage("en-US");
        private static bool _checkedNativeMouseWheelSupport;
        private static bool _nativeMouseWheelSupport = true;
        internal TextContentLayer ContentLayer = new TextContentLayer();
        internal List<ILineTransformSource> LineTransformSources = new List<ILineTransformSource>();
        internal IList<MouseHoverEventData> MouseHoverEvents = new List<MouseHoverEventData>();
        private List<FormattedLineGroup> _attachedLineCache = new List<FormattedLineGroup>();
        private List<FormattedLineGroup> _unattachedLineCache = new List<FormattedLineGroup>(20);
        private readonly List<IFormattedLine> _formattedLinesInTextViewLines = new List<IFormattedLine>();
        internal List<Span> InvalidatedSpans = new List<Span>();
        internal List<Span> ReclassifiedSpans = new List<Span>();
        private IntPtr _imeDefaultWnd = IntPtr.Zero;
        private IntPtr _imeContext = IntPtr.Zero;
        private IntPtr _imeOldContext = IntPtr.Zero;
        private bool _inConstructor = true;
        private bool _viewIsSynchronized = true;
        private double _currentZoomLevel = 100.0;

        [Export]
        [Name("word wrap glyph")]
        [BaseDefinition("text")]
        public ClassificationTypeDefinition WordWrapGlyphClassificationTypeDefinition;
        public const int ExpectedNumberOfVisibleLines = 100;
        private readonly ITextBuffer _visualBuffer;
        private ConnectionManager _connectionManager;
        internal readonly IGuardedOperations GuardedOperations;
        private IEditorFormatMap _editorFormatMap;
        private IClassifier _classifier;
        private ITextAndAdornmentSequencer _sequencer;
        private Canvas _controlHostLayer;
        private Canvas _manipulationLayer;
        private ViewStack _baseLayer;
        private ViewStack _overlayLayer;
        internal SpaceReservationStack SpaceReservationStack;
        private WpfTextSelection _selection;
        private ProvisionalTextHighlight _provisionalTextHighlight;
        private IFormattedLineSource _formattedLineSource;
        internal DispatcherTimer MouseHoverTimer;
        internal int MillisecondsSinceMouseMove;
        internal int? LastHoverPosition;
        private bool _hasKeyboardFocus;
        private List<Lazy<ITextViewCreationListener, IDeferrableContentTypeAndTextViewRoleMetadata>> _wpfDeferredTextViewListeners;
        private List<Lazy<ITextViewCreationListener, IDeferrableContentTypeAndTextViewRoleMetadata>> _deferredTextViewListeners;
        //private ITextUndoHistory _undoHistory;
        internal const int UnattachedCacheSize = 20;
        private TextViewLineCollection _textViewLinesCollection;
        private ViewState _oldState;
        internal LineRightCache _lineRightCache;
        private double _minMaxTextRightCoordinate;
        private double _viewportLeft;
        private CaretElement _caretElement;
        private bool _imeActive;
        private HwndSource _immSource;
        private HwndSourceHook _immHook;
        private bool _queuedLayout;
        private bool _layoutNeeded;
        private bool _adjustingSynchonizedViewportLeft;
        private IInputElement _focusedElement;
        private Action _loadedAction;
        private bool _hasBeenLoaded;
        private TextFormattingRunProperties _oldPlainTextProperties;
        private IViewScroller _viewScroller;
        private int _queuedSpaceReservationStackRefresh;
        private string _viewBackgroundId;
        private int _queuedFocusSettersCount;
        private IMultiSelectionBroker _multiSelectionBroker;
        private int _accumulatedMouseHWheel;

        public TextView(ITextViewModel textViewModel, ITextViewRoleSet roles, IEditorOptions parentOptions, TextEditorFactoryService factoryService, bool initialize = true)
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

        internal bool IsTextViewInitialized { get; private set; }

        internal void Initialize()
        {
            if (IsTextViewInitialized)
                throw new InvalidOperationException("Attempted to Initialize a TextView twice");
            Name = nameof(TextView);
            TextOptions.SetTextHintingMode(this, TextHintingMode.Fixed);
            MouseHoverTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
            InputMethod.SetIsInputMethodSuspended(this, true);
            AllowDrop = true;
            BufferGraph = ComponentContext.BufferGraphFactoryService.CreateBufferGraph(TextViewModel.VisualBuffer);
            _lineRightCache = new LineRightCache();
            _immHook = WndProc;
            _editorFormatMap = ComponentContext.EditorFormatMapService.GetEditorFormatMap(this);
            _manipulationLayer = new Canvas();
            _baseLayer = new ViewStack(ComponentContext.OrderedViewLayerDefinitions, this);
            _overlayLayer = new ViewStack(ComponentContext.OrderedOverlayLayerDefinitions, this, true);
            _selection = new WpfTextSelection(this, ComponentContext.GuardedOperations, MultiSelectionBroker);
            _caretElement = new CaretElement(this, ComponentContext.SmartIndentationService, ComponentContext.GuardedOperations, MultiSelectionBroker);
            InitializeLayers();
            Loaded += OnLoaded;
            SpaceReservationStack = new SpaceReservationStack(ComponentContext.OrderedSpaceReservationManagerDefinitions, this);
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
            PerformLayout(TextBuffer.CurrentSnapshot, _visualBuffer.CurrentSnapshot, new SnapshotPoint(TextBuffer.CurrentSnapshot, 0), 0.0, ViewRelativePosition.Top, ViewportWidth, ViewportHeight, false, new CancellationToken?());
            lock (InvalidatedSpans)
                InvalidatedSpans.Add(new Span(0, VisualSnapshot.Length));
            //TODO: undo
            //ITextUndoHistory history = (ITextUndoHistory)null;
            //if (_factoryService.UndoHistoryRegistry != null && _factoryService.UndoHistoryRegistry.TryGetHistory(TextBuffer, out history))
            //    this._undoHistory = history;
            if (Roles.Contains("ZOOMABLE"))
                ZoomLevel = Options.GetOptionValue(DefaultViewOptions.ZoomLevelId);
            _inConstructor = false;
            QueueLayout();
            IsTextViewInitialized = true;
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

        private void OnSequenceChanged(object sender, TextAndAdornmentSequenceChangedEventArgs e)
        {
            var spans = e.Span.GetSpans(TextSnapshot);
            var spanList = new List<Span>(spans.Count);
            foreach (var snapshotSpan in spans)
            {
                var inVisualSnapshot1 = TextViewModel.GetNearestPointInVisualSnapshot(snapshotSpan.Start, VisualSnapshot, PointTrackingMode.Negative);
                var inVisualSnapshot2 = TextViewModel.GetNearestPointInVisualSnapshot(snapshotSpan.End, VisualSnapshot, PointTrackingMode.Positive);
                spanList.Add(Span.FromBounds(inVisualSnapshot1, inVisualSnapshot2));
            }
            if (spanList.Count <= 0)
                return;
            lock (InvalidatedSpans)
            {
                if (_attachedLineCache.Count <= 0 && _unattachedLineCache.Count <= 0)
                    return;
                ReclassifiedSpans.AddRange(spanList);
                QueueLayout();
            }
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                MouseHoverTimer.Stop();
                LastHoverPosition = new int?();
            }
            else if (_layoutNeeded)
                QueueLayout();
            QueueSpaceReservationStackRefresh();
        }

        private void OnClassificationFormatMapChange(object sender, EventArgs e)
        {
            lock (InvalidatedSpans)
            {
                if (_attachedLineCache.Count <= 0)
                {
                    if (_unattachedLineCache.Count <= 0)
                        goto label_7;
                }
                ReclassifiedSpans.Clear();
                ReclassifiedSpans.Add(new Span(0, VisualSnapshot.Length));
                QueueLayout();
            }
            label_7:
            SetClearTypeHint();
        }

        private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            if (!_imeActive)
                return;
            PositionImmCompositionWindow();
        }

        public void DoActionThatShouldOnlyBeDoneAfterViewIsLoaded(Action action)
        {
            action();
            if (_hasBeenLoaded)
                return;
            _loadedAction = action;
        }

        public IAdornmentLayer GetAdornmentLayer(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.Equals(name, "Text", StringComparison.Ordinal))
                throw new ArgumentOutOfRangeException(nameof(name), "The Text adornment layer cannot be retrieved with this method.");
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

        public ISpaceReservationManager GetSpaceReservationManager(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return SpaceReservationStack.GetOrCreateManager(name);
        }

        public void DisplayTextLineContainingBufferPosition(SnapshotPoint bufferPosition, double verticalDistance, ViewRelativePosition relativeTo)
        {
            DisplayTextLineContainingBufferPosition(bufferPosition, verticalDistance, relativeTo, new double?(), new double?());
        }

        public void DisplayTextLineContainingBufferPosition(SnapshotPoint bufferPosition, double verticalDistance, ViewRelativePosition relativeTo, double? viewportWidthOverride, double? viewportHeightOverride)
        {
            if (relativeTo != (ViewRelativePosition)4 || bufferPosition.Snapshot != null)
                ValidateBufferPosition(bufferPosition);
            switch (relativeTo)
            {
                case ViewRelativePosition.Top:
                case ViewRelativePosition.Bottom:
                case (ViewRelativePosition)2:
                case (ViewRelativePosition)3:
                case (ViewRelativePosition)4:
                    if (viewportWidthOverride.HasValue && double.IsNaN(viewportWidthOverride.Value))
                        throw new ArgumentOutOfRangeException(nameof(viewportWidthOverride));
                    if (viewportHeightOverride.HasValue && double.IsNaN(viewportHeightOverride.Value))
                        throw new ArgumentOutOfRangeException(nameof(viewportHeightOverride));
                    var textSnapshot = TextSnapshot;
                    var visualSnapshot = VisualSnapshot;
                    var anchorPosition = bufferPosition;
                    var verticalDistance1 = verticalDistance;
                    var num1 = (int)relativeTo;
                    var nullable = viewportWidthOverride;
                    var effectiveViewportWidth = nullable ?? ViewportWidth;
                    nullable = viewportHeightOverride;
                    var effectiveViewportHeight = nullable ?? ViewportHeight;
                    PerformLayout(textSnapshot, visualSnapshot, anchorPosition, verticalDistance1,
                        (ViewRelativePosition) num1, effectiveViewportWidth, effectiveViewportHeight, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relativeTo));
            }
        }

        public void DisplayTextLineContainingBufferPosition(SnapshotPoint bufferPosition, double verticalDistance, ViewRelativePosition relativeTo, double? viewportWidthOverride, double? viewportHeightOverride, CancellationToken cancel)
        {
            if (relativeTo != (ViewRelativePosition)4 || bufferPosition.Snapshot != null)
                ValidateBufferPosition(bufferPosition);
            switch (relativeTo)
            {
                case ViewRelativePosition.Top:
                case ViewRelativePosition.Bottom:
                case (ViewRelativePosition)2:
                case (ViewRelativePosition)3:
                case (ViewRelativePosition)4:
                    if (viewportWidthOverride.HasValue && double.IsNaN(viewportWidthOverride.Value))
                        throw new ArgumentOutOfRangeException(nameof(viewportWidthOverride));
                    if (viewportHeightOverride.HasValue && double.IsNaN(viewportHeightOverride.Value))
                        throw new ArgumentOutOfRangeException(nameof(viewportHeightOverride));
                    var textSnapshot = TextSnapshot;
                    var visualSnapshot = VisualSnapshot;
                    var anchorPosition = bufferPosition;
                    var verticalDistance1 = verticalDistance;
                    var num1 = (int)relativeTo;
                    var nullable = viewportWidthOverride;
                    var effectiveViewportWidth = nullable ?? ViewportWidth;
                    nullable = viewportHeightOverride;
                    var effectiveViewportHeight = nullable ?? ViewportHeight;
                    var num2 = 0;
                    var cancel1 = new CancellationToken?(cancel);
                    PerformLayout(textSnapshot, visualSnapshot, anchorPosition, verticalDistance1, (ViewRelativePosition)num1, effectiveViewportWidth, effectiveViewportHeight, num2 != 0, cancel1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relativeTo));
            }
        }

        public SnapshotSpan GetTextElementSpan(SnapshotPoint position)
        {
            ValidateBufferPosition(position);
            return GetTextViewLineContainingBufferPosition(position).GetTextElementSpan(position);
        }

        public void Close()
        {
            if (IsClosed)
                throw new InvalidOperationException();
            if (HasAggregateFocus)
            {
                _viewWithAggregateFocus = null;
                HasAggregateFocus = false;
            }
            MouseHoverTimer.Stop();
            UpdateTrackingFocusChanges(false);
            UnsubscribeFromEvents();
            ContentLayer.SetTextViewLines(null);
            foreach (var formattedLineGroup in _attachedLineCache)
                formattedLineGroup.Dispose();
            _attachedLineCache.Clear();
            foreach (var formattedLineGroup in _unattachedLineCache)
                formattedLineGroup.Dispose();
            _unattachedLineCache.Clear();
            _connectionManager.Close();
            _caretElement.Close();
            _provisionalTextHighlight.Close();
            (_classifier as IDisposable)?.Dispose();
            DisableIme();
            TextViewModel.Dispose();
            TextViewModel = null;
            IsClosed = true;
            // ISSUE: reference to a compiler-generated field
            ComponentContext.GuardedOperations.RaiseEvent(this, Closed);
        }

        public void QueueSpaceReservationStackRefresh()
        {
            if (Interlocked.CompareExchange(ref _queuedSpaceReservationStackRefresh, 1, 0) != 0)
                return;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Interlocked.Exchange(ref _queuedSpaceReservationStackRefresh, 0);
                if (IsClosed)
                    return;
                SpaceReservationStack.Refresh();
            }), DispatcherPriority.Background, Array.Empty<object>());
        }

        public PropertyCollection Properties { get; } = new PropertyCollection();

        public LineTransform GetLineTransform(ITextViewLine formattedLine, double yPosition, ViewRelativePosition placement)
        {
            var transform1 = formattedLine.DefaultLineTransform;
            if (!_inConstructor)
            {
                foreach (var lineTransformSource in LineTransformSources)
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

        public bool IsClosed { get; private set; }

        public new Brush Background
        {
            get => _controlHostLayer.Background;
            set
            {
                if (WpfHelper.BrushesEqual(_controlHostLayer.Background, value))
                    return;
                _controlHostLayer.Background = value;
                var backgroundBrushChanged = BackgroundBrushChanged;
                backgroundBrushChanged?.Invoke(this, new BackgroundBrushChangedEventArgs(value));
            }
        }

        public event EventHandler<BackgroundBrushChangedEventArgs> BackgroundBrushChanged;

        public ITextCaret Caret => _caretElement;

        public FrameworkElement VisualElement => this;

        public ITextSelection Selection => _selection;

        public ITrackingSpan ProvisionalTextHighlight
        {
            get => _provisionalTextHighlight.ProvisionalSpan;
            set => _provisionalTextHighlight.ProvisionalSpan = value;
        }

        public ITextViewRoleSet Roles { get; }

        public ITextBuffer TextBuffer { get; }

        public IBufferGraph BufferGraph { get; private set; }

        public ITextSnapshot TextSnapshot { get; private set; }

        public ITextSnapshot VisualSnapshot { get; private set; }

        public ITextDataModel TextDataModel { get; }

        public ITextViewModel TextViewModel { get; private set; }

        public IFormattedLineSource FormattedLineSource => _formattedLineSource;

        public double LineHeight => _formattedLineSource.LineHeight;

        public ILineTransformSource LineTransformSource => this;

        public bool InLayout { get; private set; }

        public bool InOuterLayout { get; private set; }

        public ITextViewLineCollection TextViewLines
        {
            get
            {
                if (InLayout)
                    throw new InvalidOperationException();
                return _textViewLinesCollection;
            }
        }

        ITextViewLineCollection ITextView.TextViewLines
        {
            get
            {
                if (InLayout)
                    throw new InvalidOperationException();
                return _textViewLinesCollection;
            }
        }

        public ITextViewLine GetTextViewLineContainingBufferPosition(SnapshotPoint bufferPosition)
        {
            return ((ITextView)this).GetTextViewLineContainingBufferPosition(bufferPosition);
        }

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
            var containingLine = TextViewModel.GetNearestPointInVisualSnapshot(bufferPosition, VisualSnapshot, PointTrackingMode.Positive).GetContainingLine();
            var formattedLines = (IList<IFormattedLine>)_formattedLineSource.FormatLineInVisualBuffer(containingLine, new CancellationToken?());
            var formattedLineGroup1 = new FormattedLineGroup(containingLine, formattedLines) {InUse = true};
            if (_unattachedLineCache.Count >= 20)
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

        public IViewScroller ViewScroller => _viewScroller ?? (_viewScroller = new DefaultViewScroller(this, ComponentContext));

        public double MaxTextRightCoordinate => Math.Max(RawMaxTextRightCoordinate, _minMaxTextRightCoordinate);

        public double RawMaxTextRightCoordinate { get; private set; }

        public double MinMaxTextRightCoordinate
        {
            get => _minMaxTextRightCoordinate;
            set
            {
                if (IsClosed)
                    return;
                var textRightCoordinate1 = MaxTextRightCoordinate;
                _minMaxTextRightCoordinate = value;
                var textRightCoordinate2 = MaxTextRightCoordinate;
                if (textRightCoordinate1 == textRightCoordinate2)
                    return;
                var coordinateChanged = MaxTextRightCoordinateChanged;
                coordinateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler MaxTextRightCoordinateChanged;

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
                        num1 = Math.Max(0.0, value);
                }
                if (_viewportLeft == num1)
                    return;
                var deltaX = num1 - _viewportLeft;
                _viewportLeft = num1;
                UpdateVisibleArea(ViewportWidth, ViewportHeight);
                _baseLayer.SetSnapshotAndUpdate(TextSnapshot, deltaX, 0.0, EmptyLines, EmptyLines);
                Canvas.SetLeft(_baseLayer, -_viewportLeft);
                RaiseLayoutChangeEvent();
                var viewportLeftChanged = ViewportLeftChanged;
                viewportLeftChanged?.Invoke(this, EventArgs.Empty);
                QueueSpaceReservationStackRefresh();
                if (_imeActive)
                    PositionImmCompositionWindow();
                if (_adjustingSynchonizedViewportLeft)
                    return;
                try
                {
                    _adjustingSynchonizedViewportLeft = true;
                    var subordinateView = SynchronizationManager?.GetSubordinateView(this);
                    if (subordinateView == null)
                        return;
                    subordinateView.ViewportLeft = ViewportLeft;
                }
                finally
                {
                    _adjustingSynchonizedViewportLeft = false;
                }
            }
        }

        private void SetClearTypeHint()
        {
            var defaultTextProperties = ClassificationFormatMap.DefaultTextProperties;
            if (defaultTextProperties == _oldPlainTextProperties)
                return;
            _oldPlainTextProperties = defaultTextProperties;
            if (!defaultTextProperties.TypefaceEmpty && defaultTextProperties.Typeface.FontFamily.FamilyNames.TryGetValue(EnUsLanguage, out var strA) && string.Compare(strA, "Consolas", StringComparison.OrdinalIgnoreCase) == 0)
                TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            else
                TextOptions.SetTextRenderingMode(this, TextRenderingMode.Auto);
        }

        private void RaiseLayoutChangeEvent()
        {
            RaiseLayoutChangeEvent(ViewportWidth, ViewportHeight, EmptyLines, EmptyLines);
        }

        private void RaiseLayoutChangeEvent(double effectiveViewportWidth, double effectiveViewportHeight, IList<ITextViewLine> newOrReformattedLines, IList<ITextViewLine> translatedLines)
        {
            if (IsClosed)
                return;
            var newState = new ViewState(this, effectiveViewportWidth, effectiveViewportHeight);
            using (ComponentContext.PerformanceBlockMarker.CreateBlock("VsTextEditor.LayoutChangedEvent"))
            {
                if (!_inConstructor)
                {
                    var layoutChanged = LayoutChanged;
                    if (layoutChanged != null)
                        ComponentContext.GuardedOperations.RaiseEvent(this, layoutChanged, new TextViewLayoutChangedEventArgs(_oldState, newState, newOrReformattedLines, translatedLines));
                }
            }
            _oldState = newState;
            foreach (var formattedLineGroup in _attachedLineCache)
                formattedLineGroup.ClearChange();
        }

        public double ViewportTop { get; private set; }

        public double ViewportRight => ViewportLeft + ViewportWidth;

        public double ViewportBottom => ViewportTop + ViewportHeight;

        public double ViewportHeight => !double.IsNaN(ActualHeight) ? Math.Ceiling(ActualHeight) : 120.0;

        public double ViewportWidth => !double.IsNaN(ActualWidth) ? ActualWidth : 240.0;

        public IEditorOptions Options { get; }

        internal IClassificationFormatMap ClassificationFormatMap => ComponentContext.ClassificationFormatMappingService.GetClassificationFormatMap(this);

        internal IClassificationTypeRegistryService ClassificationTypeRegistry => ComponentContext.ClassificationTypeRegistryService;

        internal WordWrapStyles WordWrapStyle => Options.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId);

        public bool IsMouseOverViewOrAdornments => IsMouseOver || SpaceReservationStack.IsMouseOver;

        public bool HasAggregateFocus { get; private set; }

        public double ZoomLevel
        {
            get => _currentZoomLevel;
            set => ApplyZoom(value);
        }

        public FrameworkElement ManipulationLayer => _manipulationLayer;

        public IMultiSelectionBroker MultiSelectionBroker => _multiSelectionBroker ?? (_multiSelectionBroker =
                                                                 ComponentContext.MultiSelectionBrokerFactory.CreateBroker(this));

        public event EventHandler<TextViewLayoutChangedEventArgs> LayoutChanged;

        public event EventHandler Closed;

        public event EventHandler ViewportLeftChanged;

        public event EventHandler ViewportHeightChanged;

        public event EventHandler ViewportWidthChanged;

        public event EventHandler<MouseHoverEventArgs> MouseHover
        {
            add
            {
                lock (MouseHoverEvents)
                {
                    if (MouseHoverEvents.Count == 0)
                    {
                        MouseMove += OnMouseMove;
                        MouseLeftButtonDown += OnMouseDown;
                        MouseDown += OnMouseDown;
                        MouseRightButtonDown += OnMouseDown;
                    }
                    MouseHoverEvents.Add(new MouseHoverEventData(value));
                }
            }
            remove
            {
                lock (MouseHoverEvents)
                {
                    for (var index = MouseHoverEvents.Count - 1; index >= 0; --index)
                    {
                        if (MouseHoverEvents[index].EventHandler == value)
                        {
                            MouseHoverEvents.RemoveAt(index);
                            if (MouseHoverEvents.Count != 0)
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

        public event EventHandler LostAggregateFocus;

        public event EventHandler GotAggregateFocus;

        public event EventHandler<ZoomLevelChangedEventArgs> ZoomLevelChanged;

        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Width != double.PositiveInfinity && availableSize.Height != double.PositiveInfinity)
                return availableSize;
            var height = availableSize.Height == double.PositiveInfinity ? (double.IsNaN(Height) ? 120.0 : Height) : availableSize.Height;
            return new Size(availableSize.Width == double.PositiveInfinity ? (double.IsNaN(Width) ? 240.0 : Width) : availableSize.Width, height);
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 256:
                    if ((int)wParam == 25 && IsKorean() && !_selection.IsEmpty)
                    {
                        var position = _selection.Start.Position;
                        var ch = TextSnapshot[position];
                        if (IsHangul(ch) && WpfHelper.HanjaConversion(_imeContext, WpfHelper.GetKeyboardLayout(), ch))
                        {
                            _selection.Select(new SnapshotSpan(position, 1), false);
                            handled = true;
                        }
                    }
                    break;
                case 269:
                    if (!_imeActive)
                    {
                        _imeActive = true;
                        _caretElement.PositionChanged += OnCaretPositionChanged;
                    }
                    if (IsKorean())
                        handled = true;
                    _caretElement.EnsureVisible();
                    PositionImmCompositionWindow();
                    break;
                case 270:
                    if (_imeActive)
                    {
                        _imeActive = false;
                        _caretElement.PositionChanged -= OnCaretPositionChanged;
                    }
                    if (IsKorean())
                    {
                        RaiseTextInputEvent(true, "");
                        handled = true;
                    }
                    break;
                case 271:
                    if (!IsKorean())
                    {
                        PositionImmCompositionWindow();
                        break;
                    }
                    var dwIndex = (int)lParam & 2056;
                    if (dwIndex != 0 && _imeContext != IntPtr.Zero)
                    {
                        var compositionString = WpfHelper.GetImmCompositionString(_imeContext, dwIndex);
                        if (!string.IsNullOrEmpty(compositionString))
                            RaiseTextInputEvent((uint)(dwIndex & 2048) > 0U, compositionString);
                    }
                    handled = true;
                    break;
                case 648:
                    if (WpfHelper.ImmIsIME(WpfHelper.GetKeyboardLayout()) && !_selection.IsEmpty && _selection.Mode == TextSelectionMode.Stream)
                    {
                        var selection = _selection.StreamSelectionSpan.SnapshotSpan;
                        selection = new SnapshotSpan(selection.Start, Math.Min(selection.Length, 50));
                        if ((int)wParam == 4)
                        {
                            var num = WpfHelper.ReconvertString(lParam, selection);
                            handled = num != IntPtr.Zero;
                            return num;
                        }
                        if ((int)wParam == 5)
                        {
                            var selectionSpan = WpfHelper.ConfirmReconvertString(lParam, selection);
                            if (selectionSpan.Length != 0)
                            {
                                _caretElement.MoveTo(selectionSpan.Start);
                                _selection.Select(selectionSpan, true);
                                handled = true;
                                return new IntPtr(1);
                            }
                        }
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TextOptions.TextFormattingModeProperty)
                UpdateFormattingMode();
            base.OnPropertyChanged(e);
        }

        public bool RemoveVisualsWhenHidden
        {
            get => ContentLayer.RemoveTextVisualsWhenHidden;
            set => ContentLayer.RemoveTextVisualsWhenHidden = value;
        }

        private void SubscribeToEvents()
        {
            MouseHoverTimer.Tick += OnHoverTimer;
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

        private void UnsubscribeFromEvents()
        {
            MouseHoverTimer.Tick -= OnHoverTimer;
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

        private void OnVisualBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (IsClosed)
                return;
            var spanList = (IList<Span>)new List<Span>(e.Changes.Count);
            foreach (var change in e.Changes)
            {
                var span = change.OldSpan;
                if (change.NewLength > 0 && span.Start > 0 && (e.After[change.NewPosition] == '\n' && e.Before[span.Start - 1] == '\r'))
                    span = Span.FromBounds(span.Start - 1, span.End);
                var trackingSpan = e.Before.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                spanList.Add(trackingSpan.GetSpan(VisualSnapshot));
            }
            lock (InvalidatedSpans)
                InvalidatedSpans.AddRange(spanList);
            AdvanceSnapshotOnUiThread(e);
        }

        private void OnVisualBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            AdvanceSnapshotOnUiThread(e);
        }

        private void OnDataModelContentTypeChanged(object sender, TextDataModelContentTypeChangedEventArgs e)
        {
            BindContentTypeSpecificAssets(e.BeforeContentType, e.AfterContentType);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
        }

        private void BindContentTypeSpecificAssets(IContentType beforeContentType, IContentType afterContentType)
        {
            LineTransformSources.Clear();
            foreach (var matchingExtension in UiExtensionSelector.SelectMatchingExtensions(ComponentContext.LineTransformSourceProviders, afterContentType, null, Roles))
            {
                var lineTransformSource = ComponentContext.GuardedOperations.InstantiateExtension(matchingExtension, matchingExtension, p => p.Create(this));
                if (lineTransformSource != null)
                    LineTransformSources.Add(lineTransformSource);
            }
            foreach (var matchingExtension in UiExtensionSelector.SelectMatchingExtensions(ComponentContext.TextViewCreationListeners, afterContentType, beforeContentType, Roles))
            {
                var optionName = matchingExtension.Metadata.OptionName;
                if (!string.IsNullOrEmpty(optionName) && Options.IsOptionDefined(optionName, false))
                {
                    var optionValue = Options.GetOptionValue(optionName);
                    if (optionValue is bool b && !b)
                    {
                        if (_wpfDeferredTextViewListeners == null)
                            _wpfDeferredTextViewListeners = new List<Lazy<ITextViewCreationListener, IDeferrableContentTypeAndTextViewRoleMetadata>>();
                        _wpfDeferredTextViewListeners.Add(matchingExtension);
                        continue;
                    }
                }
                var instantiatedExtension = ComponentContext.GuardedOperations.InstantiateExtension(matchingExtension, matchingExtension);
                if (instantiatedExtension != null)
                    ComponentContext.GuardedOperations.CallExtensionPoint(instantiatedExtension, () => instantiatedExtension.TextViewCreated(this));
            }
        }

        private void AdvanceSnapshot(TextSnapshotChangedEventArgs e)
        {
            if (_visualBuffer.CurrentSnapshot != e.After)
                return;
            PerformLayout(TextBuffer.CurrentSnapshot, e.After);
        }

        private void AdvanceSnapshotOnUiThread(TextSnapshotChangedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke((() => AdvanceSnapshot(e)), DispatcherPriority.Normal);
            else if (InOuterLayout)
                Dispatcher.BeginInvoke((Action)(() => AdvanceSnapshot(e)), DispatcherPriority.Normal, Array.Empty<object>());
            else
                AdvanceSnapshot(e);
        }

        private void OnEditorOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (IsClosed)
                return;
            if (string.Equals(e.OptionId, DefaultOptions.TabSizeOptionId.Name, StringComparison.Ordinal))
                InvalidateAllLines();
            else if (string.Equals(e.OptionId, DefaultTextViewOptions.WordWrapStyleId.Name, StringComparison.Ordinal))
            {
                if ((WordWrapStyle & WordWrapStyles.WordWrap) == WordWrapStyles.WordWrap)
                    ViewportLeft = 0.0;
                InvalidateAllLines();
            }
            else if (string.Equals(e.OptionId, DefaultTextViewOptions.ViewProhibitUserInputId.Name, StringComparison.Ordinal))
            {
                if (_immSource == null)
                    return;
                DisableIme();
                EnableIme();
            }
            else if (string.Equals(e.OptionId, DefaultViewOptions.ZoomLevelId.Name, StringComparison.Ordinal) && Roles.Contains("ZOOMABLE"))
            {
                ZoomLevel = Options.GetOptionValue(DefaultViewOptions.ZoomLevelId);
            }
            else
            {
                if (_wpfDeferredTextViewListeners != null)
                {
                    for (var index = 0; index < _wpfDeferredTextViewListeners.Count; ++index)
                    {
                        var textViewListener = _wpfDeferredTextViewListeners[index];
                        if (string.Equals(textViewListener.Metadata.OptionName, e.OptionId, StringComparison.Ordinal))
                        {
                            _wpfDeferredTextViewListeners.RemoveAt(index);
                            if (_wpfDeferredTextViewListeners.Count == 0)
                                _wpfDeferredTextViewListeners = null;
                            var instantiatedExtension = ComponentContext.GuardedOperations.InstantiateExtension(textViewListener, textViewListener);
                            if (instantiatedExtension != null)
                            {
                                ComponentContext.GuardedOperations.CallExtensionPoint(instantiatedExtension, (() => instantiatedExtension.TextViewCreated(this)));
                            }
                            break;
                        }
                    }
                }
                if (_deferredTextViewListeners == null)
                    return;
                for (var index = 0; index < _deferredTextViewListeners.Count; ++index)
                {
                    var textViewListener = _deferredTextViewListeners[index];
                    if (string.Equals(textViewListener.Metadata.OptionName, e.OptionId, StringComparison.Ordinal))
                    {
                        _deferredTextViewListeners.RemoveAt(index);
                        if (_deferredTextViewListeners.Count == 0)
                            _deferredTextViewListeners = null;
                        var instantiatedExtension = ComponentContext.GuardedOperations.InstantiateExtension(textViewListener, textViewListener);
                        if (instantiatedExtension == null)
                            break;
                        ComponentContext.GuardedOperations.CallExtensionPoint(instantiatedExtension, () => instantiatedExtension.TextViewCreated(this));
                        break;
                    }
                }
            }
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (IsClosed || !e.ChangedItems.Contains(ViewBackgroundId))
                return;
            Background = GetBackgroundColorFromFormatMap();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsClosed)
                return;
            Size newSize;
            if (_controlHostLayer.Height != e.NewSize.Height)
            {
                lock (InvalidatedSpans)
                    QueueLayout();
                var controlHostLayer = _controlHostLayer;
                newSize = e.NewSize;
                var height = newSize.Height;
                controlHostLayer.Height = height;
                ViewportHeightChanged?.Invoke(this, EventArgs.Empty);
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
                lock (InvalidatedSpans)
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
            ViewportWidthChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_hasKeyboardFocus || IsClosed)
                return;
            _hasKeyboardFocus = true;
            QueueAggregateFocusCheck();
            if (MultiSelectionBroker.ActivationTracksFocus)
                MultiSelectionBroker.AreSelectionsActive = true;
            Mouse.Synchronize();
            EnableIme();
        }

        internal void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
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
            if (_selection.ActivationTracksFocus)
                _selection.IsActive = false;
            QueueAggregateFocusCheck();
        }

        private void OnFocusedElementLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdateTrackingFocusChanges(true);
            QueueAggregateFocusCheck();
        }

        private void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            UpdateTrackingFocusChanges((bool)args.NewValue);
            QueueAggregateFocusCheck();
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (IsClosed)
                return;
            (e.OldSource as HwndSource)?.RemoveHook(MouseScrollHook);
            (e.NewSource as HwndSource)?.AddHook(MouseScrollHook);
            var dependencyObject = (DependencyObject)this;
            do
            {
                if (!InputMethod.GetIsInputMethodSuspended(dependencyObject))
                    InputMethod.SetIsInputMethodSuspended(dependencyObject, true);
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            while (dependencyObject != null);
            UpdateFormattingMode();
        }

        private void OnClassificationChanged(object sender, ClassificationChangedEventArgs e)
        {
            if (IsClosed)
                return;
            var span = Span.FromBounds(
                TextViewModel.GetNearestPointInVisualSnapshot(e.ChangeSpan.Start, VisualSnapshot,
                    PointTrackingMode.Negative),
                TextViewModel.GetNearestPointInVisualSnapshot(e.ChangeSpan.End, VisualSnapshot,
                    PointTrackingMode.Positive));
            if (span.Length <= 0)
                return;
            span = new Span(span.Start, span.Length - 1);
            lock (InvalidatedSpans)
            {
                if (_attachedLineCache.Count <= 0 && _unattachedLineCache.Count <= 0)
                    return;
                ReclassifiedSpans.Add(span);
                QueueLayout();
            }
        }

        internal void OnHoverTimer(object sender, EventArgs e)
        {
            if (IsClosed)
                return;
            MillisecondsSinceMouseMove += (int)(MouseHoverTimer.Interval.Ticks / 10000L);
            if (!IsVisible || !LastHoverPosition.HasValue)
                return;
            RaiseHoverEvents();
        }

        internal void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsClosed || e.LeftButton != MouseButtonState.Released || (e.MiddleButton != MouseButtonState.Released || e.RightButton != MouseButtonState.Released))
                return;
            HandleMouseMove(e.GetPosition(this));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsClosed)
                return;
            MouseHoverTimer.Stop();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (IsClosed)
                return;
            MouseHoverTimer.Stop();
            LastHoverPosition = new int?();
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
                if (IsClosed || !_layoutNeeded || !IsVisible)
                    return;
                if (InOuterLayout)
                {
                    QueueLayout();
                }
                else
                {
                    if (TextSnapshot != TextBuffer.CurrentSnapshot || VisualSnapshot != _visualBuffer.CurrentSnapshot)
                        return;
                    PerformLayout(TextSnapshot, VisualSnapshot);
                }
            }), DispatcherPriority.DataBind, Array.Empty<object>());
        }

        internal void QueueAggregateFocusCheck(bool checkForFocus = true)
        {
            if (_queuedFocusSettersCount > 0 || IsClosed)
                return;
            var flag = false;
            if (checkForFocus)
            {
                flag = SpaceReservationStack.HasAggregateFocus;
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
                _viewWithAggregateFocus?.QueueAggregateFocusCheck(false);
                _viewWithAggregateFocus = this;
                //TODO: undo
                //if (this._undoHistory != null)
                //    this._undoHistory.Properties[(object)typeof(ITextView)] = (object)this;
            }
            else
                _viewWithAggregateFocus = null;
            ComponentContext.GuardedOperations.RaiseEvent(this, HasAggregateFocus ? GotAggregateFocus : LostAggregateFocus);
        }

        internal TextEditorFactoryService ComponentContext { get; }

        private void RaiseTextInputEvent(bool final, string text)
        {
            var composition = (TextComposition)new ImeTextComposition(InputManager.Current, Keyboard.FocusedElement, text);
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

        private static bool IsKorean()
        {
            return InputLanguageManager.Current.CurrentInputLanguage.LCID == 1042;
        }

        internal static bool IsHangul(char ch)
        {
            var flag = false;
            if (ch >= 'ᄀ')
            {
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
            }
            return flag;
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

        private static int HiWord(IntPtr wParam)
        {
            return (short)((long)wParam >> 16 & ushort.MaxValue);
        }

        private static int LoWord(IntPtr wParam)
        {
            return (short)((long)wParam &  ushort.MaxValue);
        }

        internal IntPtr MouseScrollHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (IsClosed || !HasAggregateFocus)
                return IntPtr.Zero;
            switch (msg)
            {
                case 276:
                case 277:
                    var iCode = LoWord(wParam);
                    var num1 = HiWord(wParam);
                    using (ComponentContext.PerformanceBlockMarker.CreateBlock("VsTextEditor.Scroll.MouseWheel"))
                    {
                        if (msg == 277)
                            VerticalScroll(iCode, num1);
                        else if ((WordWrapStyle & WordWrapStyles.WordWrap) == WordWrapStyles.None)
                            HorizontalScroll(iCode, num1);
                    }
                    handled = true;
                    break;
                case 526:
                    _accumulatedMouseHWheel += HiWord(wParam);
                    var num2 = _accumulatedMouseHWheel / 120;
                    if (num2 != 0)
                    {
                        using (ComponentContext.PerformanceBlockMarker.CreateBlock("VsTextEditor.Scroll.MouseWheel"))
                        {
                            _accumulatedMouseHWheel -= num2 * 120;
                            ViewScroller.ScrollViewportHorizontallyByPixels(FormattedLineSource.ColumnWidth * GetWheelScrollChars() * num2);
                        }
                    }
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private static bool NativeMouseWheelSupport
        {
            get
            {
                if (!_checkedNativeMouseWheelSupport)
                {
                    _nativeMouseWheelSupport = (uint) User32.GetSystemMetrics(75) > 0U;
                    _checkedNativeMouseWheelSupport = true;
                }
                return _nativeMouseWheelSupport;
            }
        }

        internal static int GetWheelScrollChars()
        {
            if (NativeMouseWheelSupport)
            {
                var num = 0;
                if (User32.SystemParametersInfo(108, 0, ref num, 0) == 0)
                    return num;
            }
            return 1;
        }

        private void VerticalScroll(int iCode, int lineNumber)
        {
            switch (iCode)
            {
                case 0:
                    ViewScroller.ScrollViewportVerticallyByPixels(LineHeight);
                    break;
                case 1:
                    ViewScroller.ScrollViewportVerticallyByPixels(-LineHeight);
                    break;
                case 2:
                    ViewScroller.ScrollViewportVerticallyByPage(ScrollDirection.Up);
                    break;
                case 3:
                    ViewScroller.ScrollViewportVerticallyByPage(ScrollDirection.Down);
                    break;
                case 4:
                case 5:
                    if (lineNumber < 0)
                        lineNumber = 0;
                    else if (lineNumber >= VisualSnapshot.LineCount)
                        lineNumber = Math.Max(0, VisualSnapshot.LineCount - 1);
                    DisplayTextLineContainingBufferPosition(MappingHelper.MapDownToBufferNoTrack(VisualSnapshot.GetLineFromLineNumber(lineNumber).Start, TextBuffer).Value, 0.0, ViewRelativePosition.Top);
                    break;
                case 6:
                    DisplayTextLineContainingBufferPosition(new SnapshotPoint(TextSnapshot, 0), 0.0, ViewRelativePosition.Top);
                    break;
                case 7:
                    DisplayTextLineContainingBufferPosition(new SnapshotPoint(TextSnapshot, TextSnapshot.Length), 0.0, ViewRelativePosition.Bottom);
                    break;
            }
        }

        private void HorizontalScroll(int iCode, int columnNumber)
        {
            var viewportWidth = ViewportWidth;
            var num = Math.Max(MaxTextRightCoordinate, _caretElement.Right) + 200.0;
            switch (iCode)
            {
                case 0:
                    ViewScroller.ScrollViewportHorizontallyByPixels(-FormattedLineSource.ColumnWidth);
                    break;
                case 1:
                    ViewScroller.ScrollViewportHorizontallyByPixels(FormattedLineSource.ColumnWidth);
                    break;
                case 2:
                    ViewScroller.ScrollViewportHorizontallyByPixels(-viewportWidth / 4.0);
                    break;
                case 3:
                    ViewScroller.ScrollViewportHorizontallyByPixels(viewportWidth / 4.0);
                    break;
                case 4:
                case 5:
                    if (columnNumber < 0)
                        columnNumber = 0;
                    ViewportLeft = columnNumber * FormattedLineSource.ColumnWidth;
                    break;
                case 6:
                    ViewportLeft = 0.0;
                    break;
                case 7:
                    ViewportLeft = num;
                    break;
            }
        }

        private void InitializeLayers()
        {
            Focusable = true;
            FocusVisualStyle = null;
            Cursor = Cursors.IBeam;
            _provisionalTextHighlight = new ProvisionalTextHighlight(this);
            _manipulationLayer.Background = Brushes.Transparent;
            _baseLayer.TryAddElement("Text", ContentLayer);
            _controlHostLayer = new Canvas {Background = GetBackgroundColorFromFormatMap()};
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

        internal void PerformLayout(ITextSnapshot newSnapshot, ITextSnapshot newVisualSnapshot)
        {
            PerformLayout(newSnapshot, newVisualSnapshot, new SnapshotPoint(), 0.0, (ViewRelativePosition)4, ViewportWidth, ViewportHeight, true, new CancellationToken?());
        }

        private List<FormattedLineGroup> PreLayout(LayoutLineCache lineCache)
        {
            var useDisplayMode = ShouldUseDisplayMode();
            NormalizedSpanCollection invalidSpans1;
            NormalizedSpanCollection invalidSpans2;
            lock (InvalidatedSpans)
            {
                _layoutNeeded = false;
                var flag = (WordWrapStyle & WordWrapStyles.WordWrap) == WordWrapStyles.WordWrap && _oldState.ViewportWidth != lineCache.Width;
                invalidSpans1 = ((_formattedLineSource == null ? 1 : (useDisplayMode != _formattedLineSource.UseDisplayMode ? 1 : 0)) | (flag ? 1 : 0)) == 0 ? new NormalizedSpanCollection((IEnumerable<Span>)InvalidatedSpans) : new NormalizedSpanCollection(new Span(0, VisualSnapshot.Length));
                InvalidatedSpans.Clear();
                invalidSpans2 = new NormalizedSpanCollection(ReclassifiedSpans);
                ReclassifiedSpans.Clear();
            }
            var formattedLineGroupList1 = new List<FormattedLineGroup>(_attachedLineCache.Count);
            var formattedLineGroupList2 = new List<FormattedLineGroup>(_attachedLineCache.Count);
            foreach (var formattedLineGroup in _attachedLineCache)
            {
                if (IsLineInvalid(formattedLineGroup.Line, invalidSpans1))
                {
                    formattedLineGroupList1.Add(formattedLineGroup);
                }
                else
                {
                    formattedLineGroup.InUse = false;
                    formattedLineGroupList2.Add(formattedLineGroup);
                    formattedLineGroup.Reclassified = IsLineInvalid(formattedLineGroup.Line, invalidSpans2);
                }
            }
            _attachedLineCache = formattedLineGroupList2;
            var formattedLineGroupList3 = new List<FormattedLineGroup>(20);
            foreach (var formattedLineGroup in _unattachedLineCache)
            {
                if (formattedLineGroup.InUse && !IsLineInvalid(formattedLineGroup.Line, invalidSpans1) && !IsLineInvalid(formattedLineGroup.Line, invalidSpans2))
                {
                    formattedLineGroup.InUse = false;
                    formattedLineGroupList3.Add(formattedLineGroup);
                }
                else
                    formattedLineGroup.Dispose();
            }
            _unattachedLineCache = formattedLineGroupList3;
            _lineRightCache.InvalidateSpans(invalidSpans1);
            if (TextSnapshot != lineCache.TextSnapshot || VisualSnapshot != lineCache.VisualSnapshot)
            {
                TextSnapshot = lineCache.TextSnapshot;
                VisualSnapshot = lineCache.VisualSnapshot;
                foreach (var formattedLineGroup in _attachedLineCache)
                    formattedLineGroup.SetSnapshotAndChange(VisualSnapshot, TextSnapshot);
                foreach (var formattedLineGroup in _unattachedLineCache)
                    formattedLineGroup.SetSnapshotAndChange(VisualSnapshot, TextSnapshot);
                _lineRightCache.SetSnapshot(VisualSnapshot);
            }
            else
            {
                foreach (var formattedLineGroup in _attachedLineCache)
                    formattedLineGroup.ResetChange();
            }
            foreach (var linesInTextViewLine in _formattedLinesInTextViewLines)
                linesInTextViewLine.SetChange(TextViewLineChange.None);
            _formattedLineSource = ComponentContext.FormattedTextSourceFactoryService.Create(TextSnapshot, VisualSnapshot, Options.GetTabSize(), 2.0, (WordWrapStyle & WordWrapStyles.WordWrap) != WordWrapStyles.None ? lineCache.Width : 0.0, (WordWrapStyle & WordWrapStyles.AutoIndent) != WordWrapStyles.None ? lineCache.Width * 0.25 : 0.0, useDisplayMode, _classifier, _sequencer, ClassificationFormatMap, IsViewWrapEnabled);
            return formattedLineGroupList1;
        }

        private void PerformLayout(ITextSnapshot newSnapshot, ITextSnapshot newVisualSnapshot, SnapshotPoint anchorPosition, double verticalDistance, ViewRelativePosition relativeTo, double effectiveViewportWidth, double effectiveViewportHeight, bool preserveViewportTop, CancellationToken? cancel = null)
        {
            if (IsClosed)
                return;
            if (InOuterLayout)
                throw new InvalidOperationException();
            using (ComponentContext.PerformanceBlockMarker.CreateBlock("VsTextEditor.PerformLayout"))
            {
                var layout = new LayoutDescription(this, SynchronizationManager, effectiveViewportWidth, effectiveViewportHeight, newSnapshot, newVisualSnapshot, anchorPosition.Snapshot == null && !_viewIsSynchronized);
                var view1 = layout.MasterLineCache.View;
                var view2 = layout.SubordinateLineCache?.View;
                var topToBaselineDistance = 0.0;
                if (relativeTo == (ViewRelativePosition)4)
                {
                    var textViewLine = anchorPosition.Snapshot != null ? view1.TextViewLines.GetTextViewLineContainingBufferPosition(anchorPosition) : null;
                    if (textViewLine == null)
                    {
                        textViewLine = verticalDistance >= 0.0 ? view1.TextViewLines.FirstVisibleLine : view1.TextViewLines.LastVisibleLine;
                        anchorPosition = textViewLine.Start.TranslateTo(layout.MasterLineCache.TextSnapshot, PointTrackingMode.Negative);
                    }
                    topToBaselineDistance = textViewLine.Baseline + (textViewLine.TextTop - textViewLine.Top);
                    verticalDistance += textViewLine.Top + topToBaselineDistance - view1.ViewportTop;
                }
                try
                {
                    view1.SetOuterLayout(true);
                    view2?.SetOuterLayout(true);
                    var groupsToBeDisposedOf1 = view1.PreLayout(layout.MasterLineCache);
                    var groupsToBeDisposedOf2 = view2?.PreLayout(layout.SubordinateLineCache);
                    try
                    {
                        view1.SetInnerLayout(true);
                        view2?.SetInnerLayout(true);
                        view1.DoAnchorLayout(layout, anchorPosition, verticalDistance, topToBaselineDistance, relativeTo, cancel);
                        LayoutRemainingLines(layout, cancel, true);
                        layout.MasterLineCache.AttachedLineCache.Reverse();
                        layout.SubordinateLineCache?.AttachedLineCache.Reverse();
                        layout.LinePairs?.Reverse();
                        LayoutRemainingLines(layout, cancel, false);
                        var padding1 = 0.0;
                        var padding2 = 0.0;
                        if (view2 != null)
                        {
                            if (layout.SubordinateLineCache.AnchorLine == null)
                            {
                                var inSubordinateView = layout.Manager.GetAnchorPointAboveInSubordinateView(anchorPosition);
                                inSubordinateView = inSubordinateView.TranslateTo(layout.SubordinateLineCache.View.TextSnapshot, PointTrackingMode.Positive);
                                var group = layout.SubordinateLineCache.DoAnchorFormat(inSubordinateView, cancel);
                                layout.SubordinateLineCache.CalculateAndSetTransforms(@group, layout.ReferenceLine, layout.DistanceAboveReferenceLine, layout.DistanceBelowReferenceLine, @group.FormattedLines.Count);
                                var index = @group.FormattedLines.Count - 1;
                                layout.SubordinateLineCache.AnchorLine = @group.FormattedLines[index];
                                layout.LinePairs.Insert(0, new ValueTuple<FormattedLineGroup, FormattedLineGroup>((FormattedLineGroup)null, @group));
                                padding2 = -@group.TotalHeight;
                                view2._viewIsSynchronized = false;
                            }
                            else
                                view2._viewIsSynchronized = true;
                            var offset = 0.0;
                            var myOffset1 = 0.0;
                            var myOffset2 = 0.0;
                            var firstLine1 = true;
                            var firstLine2 = true;
                            for (var index = 0; index < layout.LinePairs.Count; ++index)
                            {
                                var linePair = layout.LinePairs[index];
                                var val1 = AdjustGroup(linePair.Item1, offset, ref myOffset1, ref firstLine1, ref padding1);
                                var val2 = AdjustGroup(linePair.Item2, offset, ref myOffset2, ref firstLine2, ref padding2);
                                if (index != 0 || view2._viewIsSynchronized)
                                {
                                    offset += Math.Max(val1, val2);
                                    myOffset1 += val1;
                                    myOffset2 += val2;
                                }
                            }
                        }
                        view1.UpdateAttachedLineCache(layout, layout.MasterLineCache, preserveViewportTop, padding1);
                        view2?.UpdateAttachedLineCache(layout, layout.SubordinateLineCache, preserveViewportTop, padding2);
                    }
                    finally
                    {
                        view1.SetInnerLayout(false);
                        view2?.SetInnerLayout(false);
                    }
                    view1.PostLayout(effectiveViewportWidth, effectiveViewportHeight, groupsToBeDisposedOf1);
                    view2?.PostLayout(view2.ViewportWidth, effectiveViewportHeight, groupsToBeDisposedOf2);
                }
                finally
                {
                    view1.SetOuterLayout(false);
                    view2?.SetOuterLayout(false);
                }
                if (view2 != null)
                    view1.MinMaxTextRightCoordinate = view2.MinMaxTextRightCoordinate = Math.Max(view1.RawMaxTextRightCoordinate, view2.RawMaxTextRightCoordinate);
                else
                    view1.MinMaxTextRightCoordinate = 0.0;
            }
        }

        private static double AdjustGroup(FormattedLineGroup group, double offset, ref double myOffset, ref bool firstLine, ref double padding)
        {
            var num1 = 0.0;
            if (group != null)
            {
                foreach (var formattedLine in group.FormattedLines)
                {
                    num1 += formattedLine.Height;
                }
                var num2 = offset - myOffset;
                if (num2 > 0.0)
                {
                    if (firstLine)
                    {
                        padding = num2;
                    }
                    else
                    {
                        var formattedLine1 = group.FormattedLines[0];
                        var formattedLine2 = formattedLine1;
                        var topSpace = formattedLine1.LineTransform.TopSpace + num2;
                        var lineTransform = formattedLine1.LineTransform;
                        var bottomSpace = lineTransform.BottomSpace;
                        lineTransform = formattedLine1.LineTransform;
                        var verticalScale = lineTransform.VerticalScale;
                        var right = formattedLine1.Right;
                        var transform = new LineTransform(topSpace, bottomSpace, verticalScale, right);
                        formattedLine2.SetLineTransform(transform);
                        formattedLine1.SetChange(TextViewLineChange.NewOrReformatted);
                    }
                    myOffset = offset;
                }
                firstLine = false;
            }
            return num1;
        }

        private void SetOuterLayout(bool isIn)
        {
            InOuterLayout = isIn;
        }

        private void SetInnerLayout(bool isIn)
        {
            InLayout = isIn;
        }

        private void PostLayout(double effectiveViewportWidth, double effectiveViewportHeight, List<FormattedLineGroup> groupsToBeDisposedOf)
        {
            foreach (var formattedLineGroup in groupsToBeDisposedOf)
                formattedLineGroup.Dispose();
            RawMaxTextRightCoordinate = _lineRightCache.MaxRight;
            var textViewLineList1 = new List<ITextViewLine>();
            var textViewLineList2 = new List<ITextViewLine>();
            foreach (var textViewLines in _textViewLinesCollection)
            {
                if (textViewLines.Change == TextViewLineChange.NewOrReformatted)
                    textViewLineList1.Add(textViewLines);
                else if (textViewLines.Change == TextViewLineChange.Translated)
                    textViewLineList2.Add(textViewLines);
            }
            _baseLayer.SetSnapshotAndUpdate(TextSnapshot, 0.0, ViewportTop - _oldState.ViewportTop, textViewLineList1, textViewLineList2);
            _overlayLayer.SetSnapshotAndUpdate(TextSnapshot, 0.0, 0.0, textViewLineList1, EmptyLines);
            RaiseLayoutChangeEvent(effectiveViewportWidth, effectiveViewportHeight, textViewLineList1, textViewLineList2);
            if (LastHoverPosition.HasValue)
            {
                MouseHoverTimer.Stop();
                LastHoverPosition = new int?();
            }
            QueueSpaceReservationStackRefresh();
            if (!_imeActive)
                return;
            PositionImmCompositionWindow();
        }

        private void UpdateAttachedLineCache(LayoutDescription layout, LayoutLineCache lineCache, bool preserveViewportTop, double padding)
        {
            lineCache.OldAttachedLineCache = _attachedLineCache;
            _attachedLineCache = lineCache.AttachedLineCache;
            _formattedLinesInTextViewLines.Clear();
            var formattedLine1 = lineCache.AnchorLine.Change != TextViewLineChange.NewOrReformatted ? lineCache.AnchorLine : null;
            foreach (var group in _attachedLineCache)
            {
                group.InUse = true;
                UpdateLineRightCache(group);
                var formattedLines = group.FormattedLines;
                foreach (var formattedLine2 in formattedLines)
                {
                    _formattedLinesInTextViewLines.Add(formattedLine2);
                    if (formattedLine1 == null && formattedLine2.Change != TextViewLineChange.NewOrReformatted)
                        formattedLine1 = formattedLine2;
                }
            }
            if (formattedLine1 != null)
            {
                var num1 = 0.0;
                for (var index = 0; _formattedLinesInTextViewLines[index] != formattedLine1; ++index)
                    num1 += _formattedLinesInTextViewLines[index].Height;
                lineCache.YTopInBaseLayerCoordinates = formattedLine1.Top - num1;
                var num2 = layout.ReferenceLine - layout.DistanceAboveReferenceLine;
                lineCache.NewViewportTop = lineCache.YTopInBaseLayerCoordinates - num2;
            }
            else
            {
                lineCache.NewViewportTop = preserveViewportTop ? ViewportTop : 0.0;
                lineCache.YTopInBaseLayerCoordinates = lineCache.NewViewportTop + layout.ReferenceLine - layout.DistanceAboveReferenceLine;
            }
            var top = lineCache.YTopInBaseLayerCoordinates + padding;
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
            }
            if (ViewportTop != lineCache.NewViewportTop)
            {
                ViewportTop = lineCache.NewViewportTop;
                Canvas.SetTop(_baseLayer, -ViewportTop);
            }
            UpdateVisibleArea(lineCache.Width, layout.EffectiveHeight);
            TrimHiddenLines();
            _textViewLinesCollection?.Invalidate();
            _textViewLinesCollection = new TextViewLineCollection(this, _formattedLinesInTextViewLines);
            ContentLayer.SetTextViewLines(_formattedLinesInTextViewLines);
            foreach (var formattedLineGroup in lineCache.OldAttachedLineCache)
            {
                if (!formattedLineGroup.InUse)
                    formattedLineGroup.Dispose();
                else
                    formattedLineGroup.Reclassified = false;
            }
        }

        private void DoAnchorLayout(LayoutDescription layout, SnapshotPoint anchorPosition, double verticalDistance, double topToBaselineDistance, ViewRelativePosition relativeTo, CancellationToken? cancel)
        {
            var formattedLineGroup1 = layout.MasterLineCache.DoAnchorFormat(anchorPosition, cancel);
            var anchorIndex = formattedLineGroup1.IndexOfLineContaining(anchorPosition);
            layout.MasterLineCache.AnchorLine = formattedLineGroup1.FormattedLines[anchorIndex];
            layout.ReferenceLine = relativeTo == ViewRelativePosition.Bottom || relativeTo == (ViewRelativePosition)3 ? layout.EffectiveHeight - verticalDistance : verticalDistance;
            CalculateAndSetTransform(layout.MasterLineCache.AnchorLine, layout.ReferenceLine, relativeTo & ViewRelativePosition.Bottom);
            double distanceAboveReferenceLine;
            double distanceBelowReferenceLine;
            switch (relativeTo)
            {
                case ViewRelativePosition.Top:
                    distanceAboveReferenceLine = 0.0;
                    distanceBelowReferenceLine = layout.MasterLineCache.AnchorLine.Height;
                    break;
                case (ViewRelativePosition)2:
                    distanceAboveReferenceLine = layout.MasterLineCache.AnchorLine.TextTop - layout.MasterLineCache.AnchorLine.Top;
                    distanceBelowReferenceLine = layout.MasterLineCache.AnchorLine.Height - distanceAboveReferenceLine;
                    break;
                case (ViewRelativePosition)3:
                    distanceBelowReferenceLine = layout.MasterLineCache.AnchorLine.Bottom - layout.MasterLineCache.AnchorLine.TextBottom;
                    distanceAboveReferenceLine = layout.MasterLineCache.AnchorLine.Height - distanceBelowReferenceLine;
                    break;
                case (ViewRelativePosition)4:
                    distanceAboveReferenceLine = topToBaselineDistance;
                    distanceBelowReferenceLine = layout.MasterLineCache.AnchorLine.Height - distanceAboveReferenceLine;
                    break;
                default:
                    distanceAboveReferenceLine = layout.MasterLineCache.AnchorLine.Height;
                    distanceBelowReferenceLine = 0.0;
                    break;
            }
            var andSetTransforms = layout.MasterLineCache.CalculateAndSetTransforms(formattedLineGroup1, layout.ReferenceLine, distanceAboveReferenceLine, distanceBelowReferenceLine, anchorIndex);
            var formattedLineGroup2 = (FormattedLineGroup)null;
            if (layout.TryLayoutSubordinateAnchorFormat(formattedLineGroup1, cancel))
            {
                formattedLineGroup2 = layout.SubordinateLineCache.AttachedLineCache[0];
                layout.SubordinateLineCache.CalculateAndSetTransforms(formattedLineGroup2, layout.ReferenceLine - andSetTransforms, 0.0, 0.0, -1);
                LayoutDescription.AdjustGroupLineHeights(formattedLineGroup1, formattedLineGroup2);
            }
            layout.LinePairs?.Add(new ValueTuple<FormattedLineGroup, FormattedLineGroup>(formattedLineGroup1, formattedLineGroup2));
            switch (relativeTo)
            {
                case ViewRelativePosition.Top:
                    layout.DistanceAboveReferenceLine = 0.0;
                    layout.DistanceBelowReferenceLine = layout.MasterLineCache.AnchorLine.Height;
                    break;
                case (ViewRelativePosition)2:
                    layout.DistanceAboveReferenceLine = layout.MasterLineCache.AnchorLine.TextTop - layout.MasterLineCache.AnchorLine.Top;
                    layout.DistanceBelowReferenceLine = layout.MasterLineCache.AnchorLine.Height - layout.DistanceAboveReferenceLine;
                    break;
                case (ViewRelativePosition)3:
                    layout.DistanceBelowReferenceLine = layout.MasterLineCache.AnchorLine.Bottom - layout.MasterLineCache.AnchorLine.TextBottom;
                    layout.DistanceAboveReferenceLine = layout.MasterLineCache.AnchorLine.Height - layout.DistanceBelowReferenceLine;
                    break;
                case (ViewRelativePosition)4:
                    layout.DistanceAboveReferenceLine = layout.MasterLineCache.AnchorLine.Baseline + layout.MasterLineCache.AnchorLine.TextTop - layout.MasterLineCache.AnchorLine.Top;
                    layout.DistanceBelowReferenceLine = layout.MasterLineCache.AnchorLine.Height - layout.DistanceAboveReferenceLine;
                    break;
                default:
                    layout.DistanceAboveReferenceLine = layout.MasterLineCache.AnchorLine.Height;
                    layout.DistanceBelowReferenceLine = 0.0;
                    break;
            }
            for (var index = anchorIndex - 1; index >= 0; --index)
            {
                var formattedLine = formattedLineGroup1.FormattedLines[index];
                layout.DistanceAboveReferenceLine += formattedLine.Height;
            }
            for (var index = anchorIndex + 1; index < formattedLineGroup1.FormattedLines.Count; ++index)
            {
                var formattedLine = formattedLineGroup1.FormattedLines[index];
                layout.DistanceBelowReferenceLine += formattedLine.Height;
            }
            if (formattedLineGroup2 == null)
                return;
            for (var count = formattedLineGroup1.FormattedLines.Count; count < formattedLineGroup2.FormattedLines.Count; ++count)
            {
                var formattedLine = formattedLineGroup2.FormattedLines[count];
                layout.DistanceBelowReferenceLine += formattedLine.Height;
            }
        }

        private static void LayoutRemainingLines(LayoutDescription layout, CancellationToken? cancel, bool goingUp)
        {
            var textSnapshotLine1 = layout.MasterLineCache.VisualAnchorLine;
            var textSnapshotLine2 = layout.SubordinateLineCache?.VisualAnchorLine;
            var layoutMaster = true;
            var layoutSubordinate = textSnapshotLine2 != null;
            bool flag;
            do
            {
                flag = goingUp ? layout.ReferenceLine - layout.DistanceAboveReferenceLine <= 0.0 : layout.DistanceBelowReferenceLine + layout.ReferenceLine >= layout.EffectiveHeight;
                if (layoutMaster)
                    textSnapshotLine1 = NextLine(textSnapshotLine1, goingUp);
                if (layoutSubordinate)
                    textSnapshotLine2 = NextLine(textSnapshotLine2, goingUp);
                layoutMaster = layoutSubordinate = false;
                if (textSnapshotLine1 != null)
                {
                    if (textSnapshotLine2 != null)
                    {
                        var bufferNoTrack = MappingHelper.MapDownToBufferNoTrack(textSnapshotLine1.Start, layout.MasterLineCache.View.TextBuffer, PositionAffinity.Successor);
                        var masterAnchorPoint = bufferNoTrack.Value;
                        bufferNoTrack = MappingHelper.MapDownToBufferNoTrack(textSnapshotLine2.Start, layout.SubordinateLineCache.View.TextBuffer, PositionAffinity.Successor);
                        var subordinateAnchorPoint = bufferNoTrack.Value;
                        layout.Manager.WhichPairedLinesShouldBeDisplayed(masterAnchorPoint, subordinateAnchorPoint, out layoutMaster, out layoutSubordinate, goingUp);
                    }
                    else
                        layoutMaster = true;
                }
                else if (textSnapshotLine2 != null)
                {
                    layoutSubordinate = true;
                }
                else
                {
                    ShiftIfNeeded(layout, goingUp);
                    break;
                }
                var formattedLineGroup = (FormattedLineGroup)null;
                if (layoutSubordinate)
                    formattedLineGroup = layout.SubordinateLineCache.FormatLineAndSetTransforms(layout, textSnapshotLine2, goingUp, cancel);
                var masterGroup = (FormattedLineGroup)null;
                if (layoutMaster)
                {
                    masterGroup = layout.MasterLineCache.FormatLineAndSetTransforms(layout, textSnapshotLine1, goingUp, cancel);
                    if (layout.TryLayoutSubordinateAnchorFormat(masterGroup, cancel))
                    {
                        formattedLineGroup = layout.SubordinateLineCache.AttachedLineCache[0];
                        textSnapshotLine2 = layout.SubordinateLineCache.VisualAnchorLine;
                        layout.SubordinateLineCache.CalculateAndSetTransforms(formattedLineGroup, layout.ReferenceLine, layout.DistanceAboveReferenceLine, layout.DistanceBelowReferenceLine, goingUp ? formattedLineGroup.FormattedLines.Count : -1);
                        layoutSubordinate = true;
                    }
                }
                layout.AdjustLayout(masterGroup, formattedLineGroup, goingUp);
            }
            while (!flag);
        }

        private static ITextSnapshotLine NextLine(ITextSnapshotLine line, bool goingUp)
        {
            if (goingUp)
            {
                if (line.Start > 0)
                    return (line.Start - 1).GetContainingLine();
            }
            else if (line.LineBreakLength != 0)
                return line.EndIncludingLineBreak.GetContainingLine();
            return null;
        }

        private static void ShiftIfNeeded(LayoutDescription layout, bool goUp)
        {
            if (goUp)
            {
                var num = layout.ReferenceLine - layout.DistanceAboveReferenceLine;
                if (num <= 0.0)
                    return;
                layout.ReferenceLine -= num;
            }
            else
            {
                var val2 = Math.Min(layout.MasterLineCache.LineHeight, layout.EffectiveHeight);
                if (layout.SubordinateLineCache != null)
                    val2 = Math.Min(layout.SubordinateLineCache.LineHeight, val2);
                var num = layout.ReferenceLine + layout.DistanceBelowReferenceLine - val2;
                if (num >= 0.0)
                    return;
                layout.ReferenceLine -= num;
            }
        }

        private void CalculateAndSetTransform(IFormattedLine formattedLine, double yPosition, ViewRelativePosition placement)
        {
            var lineTransform = GetLineTransform(formattedLine, yPosition, placement);
            if (!(lineTransform != formattedLine.LineTransform))
                return;
            formattedLine.SetLineTransform(lineTransform);
            formattedLine.SetChange(TextViewLineChange.NewOrReformatted);
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
                        var formattedLineList = (IList<IFormattedLine>)_formattedLineSource.FormatLineInVisualBufferIfChanged(visualLine, formattedLineGroup.FormattedLines, cancel);
                        if (formattedLineList != null)
                            formattedLineGroup.FormattedLines = formattedLineList;
                        formattedLineGroup.Reclassified = false;
                    }
                    return formattedLineGroup;
                }
            }
            var formattedLines = (IList<IFormattedLine>)_formattedLineSource.FormatLineInVisualBuffer(visualLine, cancel);
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

        private void UpdateVisibleArea(double effectiveViewportWidth, double effectiveViewportHeight)
        {
            var visibleArea = new Rect(ViewportLeft, ViewportTop, effectiveViewportWidth, effectiveViewportHeight);
            foreach (var linesInTextViewLine in _formattedLinesInTextViewLines)
                linesInTextViewLine.SetVisibleArea(visibleArea);
        }

        private void TrimHiddenLines()
        {
            for (var index = _formattedLinesInTextViewLines.Count - 2; index >= 0; --index)
            {
                if (_formattedLinesInTextViewLines[index].VisibilityState != VisibilityState.Hidden)
                {
                    TrimList(_formattedLinesInTextViewLines, index + 2, _formattedLinesInTextViewLines.Count);
                    break;
                }
            }
            for (var index = 1; index < _formattedLinesInTextViewLines.Count; ++index)
            {
                if (_formattedLinesInTextViewLines[index].VisibilityState != VisibilityState.Hidden)
                {
                    TrimList(_formattedLinesInTextViewLines, 0, index - 1);
                    break;
                }
            }
        }

        private static void TrimList(List<IFormattedLine> lines, int start, int end)
        {
            var count = end - start;
            if (count <= 0)
                return;
            lines.RemoveRange(start, count);
        }

        internal void InvalidateAllLines()
        {
            lock (InvalidatedSpans)
            {
                if (_attachedLineCache.Count <= 0 && _unattachedLineCache.Count <= 0)
                    return;
                InvalidatedSpans.Clear();
                InvalidatedSpans.Add(new Span(0, VisualSnapshot.Length));
                QueueLayout();
            }
        }

        internal void RaiseHoverEvents()
        {
            var mouseHoverEventData1 = (MouseHoverEventData)null;
            var mouseHoverEventDataList = (IList<MouseHoverEventData>)new List<MouseHoverEventData>();
            lock (MouseHoverEvents)
            {
                foreach (var mouseHoverEvent in MouseHoverEvents)
                {
                    if (!mouseHoverEvent.Fired)
                    {
                        if (mouseHoverEvent.Attribute.Delay <= MillisecondsSinceMouseMove)
                            mouseHoverEventDataList.Add(mouseHoverEvent);
                        else if (mouseHoverEventData1 == null || mouseHoverEvent.Attribute.Delay < mouseHoverEventData1.Attribute.Delay)
                            mouseHoverEventData1 = mouseHoverEvent;
                    }
                }
            }
            if (mouseHoverEventDataList.Count > 0)
            {
                var e = new MouseHoverEventArgs(this, LastHoverPosition.Value, BufferGraph.CreateMappingPoint(new SnapshotPoint(TextSnapshot, LastHoverPosition.Value), PointTrackingMode.Positive));
                foreach (var mouseHoverEventData2 in mouseHoverEventDataList)
                {
                    mouseHoverEventData2.Fired = true;
                    try
                    {
                        mouseHoverEventData2.EventHandler(this, e);
                    }
                    catch (Exception ex)
                    {
                        ComponentContext.GuardedOperations.HandleException(mouseHoverEventData2.EventHandler, ex);
                    }
                }
            }
            if (mouseHoverEventData1 == null)
                MouseHoverTimer.Stop();
            else
                MouseHoverTimer.Interval = new TimeSpan(Math.Max(50, mouseHoverEventData1.Attribute.Delay - MillisecondsSinceMouseMove) * 10000L);
        }

        internal void HandleMouseMove(Point pt)
        {
            if (MouseHoverEvents.Count <= 0)
                return;
            var nullable1 = new int?();
            if (pt.X >= 0.0 && pt.X < ViewportWidth && (pt.Y >= 0.0 && pt.Y < ViewportHeight))
            {
                var y = pt.Y + ViewportTop;
                var containingYcoordinate = _textViewLinesCollection.GetTextViewLineContainingYCoordinate(y);
                if (containingYcoordinate != null && y >= containingYcoordinate.TextTop && y <= containingYcoordinate.TextBottom)
                {
                    var xCoordinate = pt.X + ViewportLeft;
                    var positionFromXcoordinate = containingYcoordinate.GetBufferPositionFromXCoordinate(xCoordinate, true);
                    nullable1 = positionFromXcoordinate;
                    if (!nullable1.HasValue && containingYcoordinate.LineBreakLength == 0 && (containingYcoordinate.IsLastTextViewLineForSnapshotLine && containingYcoordinate.TextRight <= xCoordinate) && xCoordinate < containingYcoordinate.TextRight + containingYcoordinate.EndOfLineWidth)
                        nullable1 = containingYcoordinate.End;
                }
            }
            var nullable2 = nullable1;
            var lastHoverPosition = LastHoverPosition;
            if ((nullable2.GetValueOrDefault() == lastHoverPosition.GetValueOrDefault() ? (nullable2.HasValue != lastHoverPosition.HasValue ? 1 : 0) : 1) == 0)
                return;
            LastHoverPosition = nullable1;
            MouseHoverTimer.Stop();
            if (!nullable1.HasValue)
                return;
            var val2 = int.MaxValue;
            lock (MouseHoverEvents)
            {
                foreach (var mouseHoverEvent in MouseHoverEvents)
                {
                    mouseHoverEvent.Fired = false;
                    if (mouseHoverEvent.Attribute.Delay < val2)
                        val2 = mouseHoverEvent.Attribute.Delay;
                }
            }
            if (val2 == int.MaxValue)
                return;
            MillisecondsSinceMouseMove = 0;
            MouseHoverTimer.Interval = new TimeSpan(Math.Max(50, val2) * 10000L);
            MouseHoverTimer.Start();
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
                var includingLineBreak = (int)line.EndIncludingLineBreak;
                invalidSpan = invalidSpans[index1];
                var start = invalidSpan.Start;
                return includingLineBreak > start;
            }
            var includingLineBreak1 = (int)line.EndIncludingLineBreak;
            invalidSpan = invalidSpans[index1];
            var start1 = invalidSpan.Start;
            return includingLineBreak1 >= start1;
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

        private void ValidateBufferPosition(SnapshotPoint bufferPosition)
        {
            if (bufferPosition.Snapshot != TextSnapshot)
                throw new ArgumentException();
        }

        private string ViewBackgroundId
        {
            get
            {
                _viewBackgroundId = _viewBackgroundId ?? (this.IsEmbeddedTextView() ? "Peek Background" : "TextView Background");
                return _viewBackgroundId;
            }
        }

        private Brush GetBackgroundColorFromFormatMap()
        {
            var brush = (Brush)null;
            if (string.Equals(ViewBackgroundId, "Peek Background", StringComparison.Ordinal))
            {
                brush = Brushes.Transparent;
            }
            else
            {
                var properties = _editorFormatMap.GetProperties(ViewBackgroundId);
                if (properties.Contains("Background"))
                    brush = properties["Background"] as Brush;
                else if (properties.Contains("BackgroundColor"))
                {
                    if (properties["BackgroundColor"] is Color nullable)
                    {
                        brush = new SolidColorBrush(nullable);
                        brush.Freeze();
                    }
                }
                if (brush == null)
                    brush = SystemColors.WindowBrush;
            }
            if (brush.CanFreeze)
                brush.Freeze();
            return brush;
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            if (condition || !Debugger.IsAttached)
                return;
            Debugger.Break();
        }

        private void ApplyZoom(double zoomLevel)
        {
            if (double.IsNaN(zoomLevel))
                return;
            if (Math.Abs(zoomLevel - 100.0) < 0.5)
            {
                zoomLevel = 100.0;
            }
            else
            {
                zoomLevel = Math.Max(20.0, zoomLevel);
                zoomLevel = Math.Min(400.0, zoomLevel);
            }
            if (Math.Abs(_currentZoomLevel - zoomLevel) < 1E-05)
                return;
            _currentZoomLevel = zoomLevel;
            if (_currentZoomLevel == 100.0)
                ClearValue(TextOptions.TextFormattingModeProperty);
            else
                TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);
            var num = _currentZoomLevel / 100.0;
            var scaleTransform = new ScaleTransform(num, num);
            scaleTransform.Freeze();
            LayoutTransform = scaleTransform;
            var zoomLevelChanged = ZoomLevelChanged;
            zoomLevelChanged?.Invoke(this, new ZoomLevelChangedEventArgs(_currentZoomLevel, scaleTransform));
        }

        private bool ShouldUseDisplayMode()
        {
            if (PresentationSource.FromVisual(this) != null || ReadLocalValue(TextOptions.TextFormattingModeProperty) != DependencyProperty.UnsetValue)
                return TextOptions.GetTextFormattingMode(this) == TextFormattingMode.Display;
            if (_formattedLineSource == null)
                return true;
            return _formattedLineSource.UseDisplayMode;
        }

        private void UpdateFormattingMode()
        {
            if (ShouldUseDisplayMode() == _formattedLineSource.UseDisplayMode)
                return;
            InvalidateAllLines();
        }

        public void QueueSetFocus(Action focusSetter)
        {
            Interlocked.Increment(ref _queuedFocusSettersCount);
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
            {
                focusSetter();
                if (Interlocked.Decrement(ref _queuedFocusSettersCount) != 0)
                    return;
                QueueAggregateFocusCheck();
            }));
        }

        public IViewSynchronizationManager SynchronizationManager { get; set; }

        [Export(typeof(EditorFormatDefinition))]
        [Name("TextView Background")]
        [UserVisible(false)]
        internal sealed class TextViewBackgroundProperties : EditorFormatDefinition
        {
            public TextViewBackgroundProperties()
            {
                BackgroundBrush = SystemColors.WindowBrush;
            }
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

        internal class LineRightCache
        {
            private readonly LineRight[] _cachedLines = new LineRight[50];
            private const int CacheSize = 50;
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
                        for (var index = 1; index < _cachedLinesCount; ++index)
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
                {
                    _cachedLines[_cachedLinesCount++] = new LineRight(line, right);
                }
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
                public LineRight(ITextSnapshotLine line, double right)
                {
                    Line = line;
                    Right = right;
                }

                public double Right { get; set; }

                public ITextSnapshotLine Line { get; private set; }

                public void SetSnapshot(ITextSnapshot snapshot)
                {
                    var position = (int)new SnapshotPoint(Line.Snapshot, Line.Start).TranslateTo(snapshot, PointTrackingMode.Negative);
                    Line = snapshot.GetLineFromPosition(position);
                }
            }
        }

        private class FormattedLineGroup
        {
            public double Right = -1.0;
            public bool InUse;
            public bool Reclassified;

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

            public ITextSnapshotLine Line { get; private set; }

            public void SetSnapshotAndChange(ITextSnapshot visualSnapshot, ITextSnapshot textSnapshot)
            {
                Line = Line.Start.TranslateTo(visualSnapshot, PointTrackingMode.Negative).GetContainingLine();
                foreach (var formattedline in FormattedLines)
                {
                    formattedline.SetSnapshot(visualSnapshot, textSnapshot);
                    formattedline.SetChange(TextViewLineChange.NewOrReformatted);
                }
            }

            public void ClearChange()
            {
                foreach (var formattedline in FormattedLines)
                {
                    formattedline.SetChange(TextViewLineChange.None);
                    formattedline.SetDeltaY(0.0);
                }
            }

            public void ResetChange()
            {
                foreach (var line in FormattedLines)
                    line.SetChange(TextViewLineChange.NewOrReformatted);
            }

            public IList<IFormattedLine> FormattedLines { get; set; }

            public int IndexOfLineContaining(int position)
            {
                var count = FormattedLines.Count;
                do
                {
                } while (count > 0 && FormattedLines[--count].Start > position);
                return count;
            }

            public double TotalHeight
            {
                get
                {
                    return FormattedLines.Sum(line => line.Height);
                }
            }

            public override string ToString()
            {
                return Line.ExtentIncludingLineBreak.ToString();
            }
        }

        private class LayoutDescription
        {
            public readonly IViewSynchronizationManager Manager;
            public readonly LayoutLineCache MasterLineCache;
            public readonly LayoutLineCache SubordinateLineCache;
            public readonly double EffectiveHeight;
            public double ReferenceLine;
            public double DistanceAboveReferenceLine;
            public double DistanceBelowReferenceLine;
            public readonly List<ValueTuple<FormattedLineGroup, FormattedLineGroup>> LinePairs;

            public LayoutDescription(TextView masterView, IViewSynchronizationManager manager, double width, double height, ITextSnapshot masterTextSnapshot, ITextSnapshot masterVisualSnapshot, bool swapViews)
            {
                Manager = manager;
                EffectiveHeight = height;
                MasterLineCache = new LayoutLineCache(masterView, width, masterTextSnapshot, masterVisualSnapshot);
                var subordinateView = manager?.GetSubordinateView(masterView) as TextView;
                if (subordinateView == null || subordinateView.IsClosed || subordinateView.InOuterLayout)
                    return;
                LinePairs = new List<ValueTuple<FormattedLineGroup, FormattedLineGroup>>(100);
                var layoutLineCache = new LayoutLineCache(subordinateView, subordinateView.ViewportWidth, subordinateView.TextSnapshot, subordinateView.VisualSnapshot);
                if (swapViews)
                {
                    SubordinateLineCache = MasterLineCache;
                    MasterLineCache = layoutLineCache;
                }
                else
                    SubordinateLineCache = layoutLineCache;
            }

            public bool TryLayoutSubordinateAnchorFormat(FormattedLineGroup masterGroup, CancellationToken? cancel)
            {
                var formattedLineGroup = (FormattedLineGroup)null;
                if (SubordinateLineCache != null && SubordinateLineCache.AnchorLine == null && Manager.TryGetAnchorPointInSubordinateView(masterGroup.FormattedLines[0].Start, out var correspondingAnchorPoint))
                {
                    var anchorPosition = correspondingAnchorPoint.TranslateTo(SubordinateLineCache.View.TextSnapshot, PointTrackingMode.Positive);
                    formattedLineGroup = SubordinateLineCache.DoAnchorFormat(anchorPosition, cancel);
                    var index = formattedLineGroup.IndexOfLineContaining(anchorPosition);
                    SubordinateLineCache.AnchorLine = formattedLineGroup.FormattedLines[index];
                }
                return formattedLineGroup != null;
            }

            public static void AdjustGroupLineHeights(FormattedLineGroup masterGroup, FormattedLineGroup subordinateGroup)
            {
                if (masterGroup == null || subordinateGroup == null)
                    return;
                var num1 = Math.Min(masterGroup.FormattedLines.Count, subordinateGroup.FormattedLines.Count);
                for (var index = 0; index < num1; ++index)
                {
                    var formattedLine1 = masterGroup.FormattedLines[index];
                    var formattedLine2 = subordinateGroup.FormattedLines[index];
                    var num2 = formattedLine1.TextTop - formattedLine1.Top + formattedLine1.Baseline - (formattedLine2.TextTop - formattedLine2.Top + formattedLine2.Baseline);
                    if (num2 > 0.0)
                    {
                        var lineTransform = formattedLine2.LineTransform;
                        formattedLine2.SetLineTransform(new LineTransform(lineTransform.TopSpace + num2, lineTransform.BottomSpace, lineTransform.VerticalScale, lineTransform.Right));
                        formattedLine2.SetChange(TextViewLineChange.NewOrReformatted);
                    }
                    else if (num2 < 0.0)
                    {
                        var lineTransform = formattedLine1.LineTransform;
                        formattedLine1.SetLineTransform(new LineTransform(lineTransform.TopSpace - num2, lineTransform.BottomSpace, lineTransform.VerticalScale, lineTransform.Right));
                        formattedLine1.SetChange(TextViewLineChange.NewOrReformatted);
                    }
                    var num3 = formattedLine1.Bottom - formattedLine1.TextBottom + (formattedLine1.TextHeight - formattedLine1.Baseline) - (formattedLine2.Bottom - formattedLine2.TextBottom + (formattedLine2.TextHeight - formattedLine2.Baseline));
                    if (num3 > 0.0)
                    {
                        var lineTransform = formattedLine2.LineTransform;
                        formattedLine2.SetLineTransform(new LineTransform(lineTransform.TopSpace, lineTransform.BottomSpace + num3, lineTransform.VerticalScale, lineTransform.Right));
                        formattedLine2.SetChange(TextViewLineChange.NewOrReformatted);
                    }
                    else if (num3 < 0.0)
                    {
                        var lineTransform = formattedLine1.LineTransform;
                        formattedLine1.SetLineTransform(new LineTransform(lineTransform.TopSpace, lineTransform.BottomSpace - num3, lineTransform.VerticalScale, lineTransform.Right));
                        formattedLine1.SetChange(TextViewLineChange.NewOrReformatted);
                    }
                }
            }

            public void AdjustLayout(FormattedLineGroup masterGroup, FormattedLineGroup subordinateGroup, bool goingUp)
            {
                AdjustGroupLineHeights(masterGroup, subordinateGroup);
                LinePairs?.Add(new ValueTuple<FormattedLineGroup, FormattedLineGroup>(masterGroup, subordinateGroup));
                var num = Math.Max(masterGroup?.TotalHeight ?? 0.0, subordinateGroup?.TotalHeight ?? 0.0);
                if (goingUp)
                    DistanceAboveReferenceLine += num;
                else
                    DistanceBelowReferenceLine += num;
            }
        }

        private class LayoutLineCache
        {
            public readonly TextView View;
            public readonly List<FormattedLineGroup> AttachedLineCache;
            public readonly double Width;
            public readonly ITextSnapshot TextSnapshot;
            public readonly ITextSnapshot VisualSnapshot;
            public ITextSnapshotLine VisualAnchorLine;
            public IFormattedLine AnchorLine;
            public double YTopInBaseLayerCoordinates;
            public double NewViewportTop;
            public List<FormattedLineGroup> OldAttachedLineCache;

            public LayoutLineCache(TextView view, double width, ITextSnapshot textSnapshot, ITextSnapshot visualSnapshot)
            {
                View = view;
                Width = width;
                TextSnapshot = textSnapshot;
                VisualSnapshot = visualSnapshot;
                AttachedLineCache = new List<FormattedLineGroup>(100);
            }

            public double LineHeight
            {
                get
                {
                    var val1 = View.LineHeight;
                    if (AttachedLineCache.Count > 0)
                    {
                        var formattedLineGroup = AttachedLineCache[AttachedLineCache.Count - 1];
                        var formattedLine = formattedLineGroup.FormattedLines[formattedLineGroup.FormattedLines.Count - 1];
                        val1 = Math.Min(val1, formattedLine.Height);
                    }
                    return val1;
                }
            }

            public FormattedLineGroup DoAnchorFormat(SnapshotPoint anchorPosition, CancellationToken? cancel)
            {
                VisualAnchorLine = View.TextViewModel.GetNearestPointInVisualSnapshot(anchorPosition, View.VisualSnapshot, PointTrackingMode.Positive).GetContainingLine();
                var formattedLineGroup = View.FormatSnapshotLine(VisualAnchorLine, cancel);
                AttachedLineCache.Add(formattedLineGroup);
                return formattedLineGroup;
            }

            public FormattedLineGroup FormatLineAndSetTransforms(LayoutDescription layout, ITextSnapshotLine visualLine, bool goingUp, CancellationToken? cancel)
            {
                var group = View.FormatSnapshotLine(visualLine, cancel);
                AttachedLineCache.Add(group);
                CalculateAndSetTransforms(group, layout.ReferenceLine, layout.DistanceAboveReferenceLine, layout.DistanceBelowReferenceLine, goingUp ? group.FormattedLines.Count : -1);
                return group;
            }

            public double CalculateAndSetTransforms(FormattedLineGroup group, double referenceLine, double distanceAboveReferenceLine, double distanceBelowReferenceLine, int anchorIndex)
            {
                for (var index = anchorIndex - 1; index >= 0; --index)
                {
                    var formattedLine = group.FormattedLines[index];
                    View.CalculateAndSetTransform(formattedLine, referenceLine - distanceAboveReferenceLine, ViewRelativePosition.Bottom);
                    distanceAboveReferenceLine += formattedLine.Height;
                }
                for (var index = anchorIndex + 1; index < group.FormattedLines.Count; ++index)
                {
                    var formattedLine = group.FormattedLines[index];
                    View.CalculateAndSetTransform(formattedLine, referenceLine + distanceBelowReferenceLine, ViewRelativePosition.Top);
                    distanceBelowReferenceLine += formattedLine.Height;
                }
                return distanceAboveReferenceLine;
            }
        }
    }
}