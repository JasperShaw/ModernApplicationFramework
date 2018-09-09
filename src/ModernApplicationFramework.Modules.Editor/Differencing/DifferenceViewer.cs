using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Differencing;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Differencing;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class DifferenceViewer : IDifferenceViewer, IViewSynchronizationManager
    {
        private static readonly string[] LeftRolesSet = new string[2]
        {
            "DIFF",
            "LEFTDIFF"
        };

        private static readonly string[] RightRolesSet = new string[2]
        {
            "DIFF",
            "RIGHTDIFF"
        };

        private static readonly string[] InlineRolesSet = new string[2]
        {
            "DIFF",
            "INLINEDIFF"
        };
        private readonly Grid _grid = new Grid();
        private readonly Grid _leftViewGrid = new Grid();
        private readonly GridSplitter _splitter = new GridSplitter();
        private readonly ColumnDefinition _inlineColumn = new ColumnDefinition
        {
            Width = new GridLength(0.0, GridUnitType.Pixel)
        };
        private readonly ColumnDefinition _leftColumn = new ColumnDefinition
        {
            Width = new GridLength(0.0, GridUnitType.Pixel)
        };
        private readonly ColumnDefinition _rightColumn = new ColumnDefinition
        {
            Width = new GridLength(0.0, GridUnitType.Pixel)
        };
        private readonly ColumnDefinition _splitterColumn = new ColumnDefinition
        {
            Width = new GridLength(0.0, GridUnitType.Pixel)
        };
        private DifferenceViewMode _oldViewMode = ~DifferenceViewMode.Inline;
        internal ITextViewHost LastActiveHost;
        private bool _isInitialized;
        private bool _scrollToFirstChange;
        private bool _viewsLoaded;
        private DifferenceViewMode _mode;
        internal readonly DifferenceViewerFactoryService Factory;
        private IEditorOperations _leftOperations;
        private IEditorOperations _rightOperations;
        private IEditorOperations _inlineOperations;
        private ITextView _focusTarget;
        private DifferenceViewerCaretTracker _tracker;
        private IReadOnlyRegion _caretReadOnlyRegion;
        private bool _areViewsSynchronized;
        private ISnapshotDifference _differenceToUseForSynchronizedLayout;
        private DifferenceBrushManager _brushManager;
        private const double MinColumnWidth = 5.0;
        private const double SplitterWidth = 3.0;

        public DifferenceViewer(DifferenceViewerFactoryService factory)
        {
            Factory = factory;
        }

        public void Initialize(IDifferenceBuffer differenceBuffer, CreateTextViewHostCallback createTextViewHost, IEditorOptions parentOptions = null)
        {
            if (_isInitialized)
                throw new InvalidOperationException("DifferenceViewer is already initialized");
            _isInitialized = true;
            Properties = new PropertyCollection();
            Options = Factory.OptionsFactory.GetOptions(this);
            if (parentOptions != null)
                Options.Parent = parentOptions;
            DifferenceBuffer = differenceBuffer;
            Options.OptionChanged += EditorOptionsChanged;
            _mode = Options.GetOptionValue(DifferenceViewerOptions.ViewModeId);
            _areViewsSynchronized = Options.GetOptionValue(DifferenceViewerOptions.SynchronizeSideBySideViewsId);
            var desiredModel1 = new DifferenceTextViewModel(this, DifferenceViewType.RightView, DifferenceBuffer.BaseRightBuffer, DifferenceBuffer.RightBuffer);
            var desiredModel2 = new DifferenceTextViewModel(this, DifferenceViewType.LeftView, DifferenceBuffer.BaseLeftBuffer, DifferenceBuffer.LeftBuffer);
            var desiredModel3 = new DifferenceTextViewModel(this, DifferenceViewType.InlineView, DifferenceBuffer.BaseRightBuffer, DifferenceBuffer.InlineBuffer);
            var textViewRoleSet1 = Factory.EditorFactory.CreateTextViewRoleSet(LeftRolesSet);
            var textViewRoleSet2 = Factory.EditorFactory.CreateTextViewRoleSet(RightRolesSet);
            var textViewRoleSet3 = Factory.EditorFactory.CreateTextViewRoleSet(InlineRolesSet);

            //TODO: udno
            //FixUndo(Factory.UndoHistoryRegistry, Factory.TextBufferUndoManagerProvider);

            var options = Factory.OptionsFactory.CreateOptions();
            createTextViewHost(desiredModel3, textViewRoleSet3, options, out var visualElement1, out var textViewHost1);
            ValidateTextViewModel(desiredModel3, textViewHost1);
            InlineHost = textViewHost1;
            InlineVisualElement = visualElement1;
            textViewHost1.TextView.VisualElement.Loaded += OnLoaded;
            textViewHost1.TextView.Properties[typeof(IMapEditToData)] = new DifferenceMapEditToData(this);
            InlineView.Options.SetOptionValue(DefaultTextViewHostOptions.LineNumberMarginId, true);
            createTextViewHost(desiredModel2, textViewRoleSet1, options, out var visualElement2, out var textViewHost2);
            ValidateTextViewModel(desiredModel2, textViewHost2);
            LeftHost = textViewHost2;
            LeftVisualElement = visualElement2;
            textViewHost2.TextView.VisualElement.Loaded += OnLoaded;
            LeftView.Options.SetOptionValue(DefaultTextViewHostOptions.LineNumberMarginId, true);
            DisableEditingInView(LeftView);
            createTextViewHost(desiredModel1, textViewRoleSet2, options, out var visualElement3, out var textViewHost3);
            ValidateTextViewModel(desiredModel1, textViewHost3);
            RightHost = textViewHost3;
            RightVisualElement = visualElement3;
            textViewHost3.TextView.VisualElement.Loaded += OnLoaded;
            RightView.Options.SetOptionValue(DefaultTextViewHostOptions.LineNumberMarginId, true);
            if (DifferenceBuffer.IsEditingDisabled)
            {
                DisableEditingInView(RightView);
                DisableEditingInView(InlineView);
            }
            else
            {
                InlineView.Caret.PositionChanged += OnInlineCaretPositionChanged;
                InlineView.LayoutChanged += OnInlineTextViewLayoutChanged;
            }
          (textViewHost2.TextView).SynchronizationManager = this;
            (textViewHost3.TextView).SynchronizationManager = this;
            _brushManager = DifferenceBrushManager.GetBrushManager(textViewHost3.TextView, Factory.FormatMapService);
            _brushManager.BrushesChanged += OnBrushesChanged;
            _grid.ColumnDefinitions.Add(_leftColumn);
            _grid.ColumnDefinitions.Add(_rightColumn);
            _grid.ColumnDefinitions.Add(_inlineColumn);
            _leftViewGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1.0, GridUnitType.Star)
            });
            _leftViewGrid.ColumnDefinitions.Add(_splitterColumn);
            Grid.SetColumn(LeftVisualElement, 0);
            _leftViewGrid.Children.Add(LeftVisualElement);
            Grid.SetColumn(_leftViewGrid, 0);
            _grid.Children.Add(_leftViewGrid);
            _splitter.Visibility = Visibility.Hidden;
            _splitter.Background = _brushManager.ViewportBrush;
            _splitter.Width = 3.0;
            _splitter.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(_splitter, 0);
            _grid.Children.Add(_splitter);
            Grid.SetColumn(RightVisualElement, 1);
            _grid.Children.Add(RightVisualElement);
            Grid.SetColumn(InlineVisualElement, 2);
            _grid.Children.Add(InlineVisualElement);
            LastActiveHost = null;
            LeftView.GotAggregateFocus += (sender, args) =>
            {
                LastActiveHost = LeftHost;
                FocusManager.SetFocusedElement(_grid, LeftHost.HostControl);
            };
            RightView.GotAggregateFocus += (sender, args) =>
            {
                LastActiveHost = RightHost;
                FocusManager.SetFocusedElement(_grid, RightHost.HostControl);
            };
            InlineView.GotAggregateFocus += (sender, args) => FocusManager.SetFocusedElement(_grid, InlineHost.HostControl);
            LeftView.LayoutChanged += TextViewLayoutChanged;
            RightView.LayoutChanged += TextViewLayoutChanged;
            _leftOperations = Factory.OperationsFactory.GetEditorOperations(LeftView);
            _rightOperations = Factory.OperationsFactory.GetEditorOperations(RightView);
            _inlineOperations = Factory.OperationsFactory.GetEditorOperations(InlineView);
            _tracker = new DifferenceViewerCaretTracker(InlineView, DifferenceBuffer, _inlineOperations);
            DifferenceBuffer.SnapshotDifferenceChanging += SnapshotDifferenceChanging;
            DifferenceBuffer.SnapshotDifferenceChanged += SnapshotDifferenceChanged;
            UpdateVisual();
            if (Options.GetOptionValue(DifferenceViewerOptions.ScrollToFirstDiffId))
            {
                if (DifferenceBuffer.CurrentSnapshotDifference != null)
                    ScrollToFirstChange();
                else
                    _scrollToFirstChange = true;
            }
            _grid.GotKeyboardFocus += OnGotKeyboardFocus;
        }

        private static void RefreshView(ITextView view)
        {
            if (view.TextViewLines == null)
                return;
            view.DisplayTextLineContainingBufferPosition(new SnapshotPoint(), 0.0, (ViewRelativePosition)4);
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _grid.GotKeyboardFocus -= OnGotKeyboardFocus;
            if (_viewsLoaded)
                return;
            _focusTarget = (ViewMode == DifferenceViewMode.Inline ? InlineHost : (ViewMode == DifferenceViewMode.LeftViewOnly ? LeftHost : RightHost)).TextView;
        }

        private void ScrollToFirstChange()
        {
            if (DifferenceBuffer.CurrentSnapshotDifference.LineDifferences.Differences.Count <= 0)
                return;
            ScrollToChange(DifferenceBuffer.CurrentSnapshotDifference.LineDifferences.Differences[0]);
        }

        private void OnInlineTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            UpdateCaretReadOnlyRegion();
        }

        private void OnInlineCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateCaretReadOnlyRegion();
        }

        private void UpdateCaretReadOnlyRegion()
        {
            if (ViewMode != DifferenceViewMode.Inline)
                return;
            var snapshotDifference = DifferenceBuffer.CurrentSnapshotDifference;
            if (snapshotDifference == null || snapshotDifference.InlineBufferSnapshot != InlineView.TextSnapshot)
                return;
            var flag = false;
            var position1 = InlineView.Caret.Position;
            var bufferPosition = position1.BufferPosition;
            SnapshotPoint snapshotPoint;
            if (position1.VirtualSpaces == 0 && bufferPosition.Position > 0)
            {
                var sourceSnapshot = snapshotDifference.MapToSourceSnapshot(bufferPosition);
                if (sourceSnapshot.Snapshot == snapshotDifference.LeftBufferSnapshot)
                {
                    var containingLine = sourceSnapshot.GetContainingLine();
                    var position2 = sourceSnapshot.Position;
                    snapshotPoint = containingLine.Start;
                    var position3 = snapshotPoint.Position;
                    if (position2 == position3)
                    {
                        DifferenceBuffer.CurrentSnapshotDifference.FindMatchOrDifference(sourceSnapshot, out _, out var difference);
                        flag = difference != null && difference.Left.Start == containingLine.LineNumber;
                    }
                }
            }
            if (!flag && _caretReadOnlyRegion == null)
                return;
            if (flag && _caretReadOnlyRegion != null)
            {
                snapshotPoint = _caretReadOnlyRegion.Span.GetStartPoint(bufferPosition.Snapshot);
                if (snapshotPoint.Position == bufferPosition.Position)
                    return;
            }
            using (var readOnlyRegionEdit = DifferenceBuffer.InlineBuffer.CreateReadOnlyRegionEdit())
            {
                if (_caretReadOnlyRegion != null)
                {
                    readOnlyRegionEdit.RemoveReadOnlyRegion(_caretReadOnlyRegion);
                    _caretReadOnlyRegion = null;
                }
                if (flag)
                {
                    bufferPosition = bufferPosition.TranslateTo(readOnlyRegionEdit.Snapshot, PointTrackingMode.Positive);
                    _caretReadOnlyRegion = readOnlyRegionEdit.CreateReadOnlyRegion(new Span(bufferPosition, 0), SpanTrackingMode.EdgePositive, EdgeInsertionMode.Deny);
                }
                readOnlyRegionEdit.Apply();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewsLoaded = true;
            if (_focusTarget == null || _focusTarget.IsClosed || sender != _focusTarget.VisualElement)
                return;
            _focusTarget.VisualElement.Focus();
            _focusTarget = null;
        }

        private void OnBrushesChanged(object sender, EventArgs e)
        {
            _splitter.Background = _brushManager.ViewportBrush;
        }

        private static void DisableEditingInView(ITextView textView)
        {
            textView.Options.SetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId, true);
        }

        public bool IsInitialized => _isInitialized;

        private static void ValidateTextViewModel(DifferenceTextViewModel desiredModel, ITextViewHost textViewHost)
        {
            var textViewModel = textViewHost.TextView.TextViewModel as IDifferenceTextViewModel;
            if (desiredModel == textViewModel)
                return;
            if (textViewModel == null)
                throw new InvalidOperationException("The ITextViewModel for the view is not an IDifferenceTextViewModel");
            if (textViewModel.EditBuffer != desiredModel.EditBuffer)
                throw new InvalidOperationException("The ITextViewModel for the view has the wrong edit buffer");
        }

        //TODO: undo
        //private void FixUndo(ITextUndoHistoryRegistry undoHistoryRegistry, ITextBufferUndoManagerProvider bufferUndoManagerProvider)
        //{
        //    ITextUndoHistory history = undoHistoryRegistry.RegisterHistory((object)DifferenceBuffer.BaseRightBuffer);
        //    ITextBufferUndoManager bufferUndoManager = bufferUndoManagerProvider.GetTextBufferUndoManager(DifferenceBuffer.BaseRightBuffer);
        //    if (DifferenceBuffer.BaseRightBuffer != DifferenceBuffer.RightBuffer)
        //        SetUndo(history, bufferUndoManager, DifferenceBuffer.RightBuffer);
        //    SetUndo(history, bufferUndoManager, (ITextBuffer)DifferenceBuffer.InlineBuffer);
        //    SetUndo(undoHistoryRegistry.RegisterHistory((object)DifferenceBuffer.BaseLeftBuffer), bufferUndoManagerProvider.GetTextBufferUndoManager(DifferenceBuffer.BaseLeftBuffer), DifferenceBuffer.LeftBuffer);
        //}

        //private static void SetUndo(ITextUndoHistory history, ITextBufferUndoManager manager, ITextBuffer buffer)
        //{
        //    buffer.Properties[(object)typeof(ITextUndoHistory)] = (object)history;
        //    buffer.Properties[(object)typeof(ITextBufferUndoManager)] = (object)manager;
        //}

        private void EditorOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (string.Equals(e.OptionId, "Diff/View/ViewMode", StringComparison.Ordinal))
            {
                var optionValue = Options.GetOptionValue(DifferenceViewerOptions.ViewModeId);
                if (optionValue == _mode)
                    return;
                _mode = optionValue;
                // ISSUE: reference to a compiler-generated field
                var viewModeChanged = ViewModeChanged;
                viewModeChanged?.Invoke(this, EventArgs.Empty);
                UpdateVisual();
            }
            else
            {
                if (!string.Equals(e.OptionId, "Diff/View/SynchronizeSideBySideViews", StringComparison.Ordinal))
                    return;
                AreViewsSynchronized = Options.GetOptionValue(DifferenceViewerOptions.SynchronizeSideBySideViewsId);
            }
        }

        private void SnapshotDifferenceChanging(object sender, SnapshotDifferenceChangeEventArgs e)
        {
            if (ViewMode != DifferenceViewMode.Inline)
                return;
            _tracker.SaveCaretAndSelection(e.Before);
        }

        private void SnapshotDifferenceChanged(object sender, SnapshotDifferenceChangeEventArgs e)
        {
            if (ViewMode == DifferenceViewMode.Inline)
                _tracker.RestoreCaretAndSelection(e.After);
            if (_scrollToFirstChange)
            {
                _scrollToFirstChange = false;
                ScrollToFirstChange();
            }
            if (ViewMode == DifferenceViewMode.Inline)
            {
                if (DifferenceBuffer.IsEditingDisabled)
                    return;
                UpdateCaretReadOnlyRegion();
            }
            else
            {
                RefreshView(CurrentHost.TextView);
                if (ViewMode != DifferenceViewMode.SideBySide || _areViewsSynchronized)
                    return;
                RefreshView(CurrentHost == LeftHost ? RightView : LeftView);
            }
        }

        private void TextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            _differenceToUseForSynchronizedLayout = null;
        }

        private void UpdateVisual()
        {
            if (DifferenceBuffer.CurrentSnapshotDifference != null && _oldViewMode >= DifferenceViewMode.Inline)
                SynchronizeView(GetHostForViewMode(_oldViewMode).TextView, true);
            var flag = _oldViewMode >= DifferenceViewMode.Inline && (LeftView.HasAggregateFocus || RightView.HasAggregateFocus || InlineView.HasAggregateFocus);
            LastActiveHost = null;
            var textViewMargin = RightHost.GetTextViewMargin("VerticalScrollBar") as SideBySideVerticalScrollBarMargin;
            if (ViewMode == DifferenceViewMode.SideBySide)
            {
                if (textViewMargin != null)
                {
                    if (_areViewsSynchronized)
                    {
                        LeftView.Options.SetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId, false);
                        textViewMargin.UseSideBySideMapping = true;
                    }
                    else
                    {
                        LeftView.Options.SetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId, true);
                        textViewMargin.UseSideBySideMapping = false;
                    }
                }
                LeftView.Options.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId, WordWrapStyles.None);
                RightView.Options.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId, WordWrapStyles.None);
                RightView.Options.SetOptionValue(DefaultTextViewHostOptions.ZoomControlId, false);
            }
            else if (_oldViewMode == DifferenceViewMode.SideBySide)
            {
                if (textViewMargin != null)
                {
                    LeftView.Options.ClearOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId);
                    textViewMargin.UseSideBySideMapping = false;
                }
                LeftView.Options.ClearOptionValue(DefaultTextViewOptions.WordWrapStyleId);
                RightView.Options.ClearOptionValue(DefaultTextViewOptions.WordWrapStyleId);
                RightView.Options.ClearOptionValue(DefaultTextViewHostOptions.ZoomControlId);
            }
            if (ViewMode == DifferenceViewMode.Inline)
            {
                _leftColumn.Width = new GridLength(0.0, GridUnitType.Pixel);
                SetSplitterVisibility(Visibility.Hidden);
                _rightColumn.Width = new GridLength(0.0, GridUnitType.Pixel);
                _inlineColumn.Width = new GridLength(1.0, GridUnitType.Star);
                if (flag)
                    SetFocusOnHost(InlineHost);
            }
            else
            {
                if (ViewMode == DifferenceViewMode.LeftViewOnly)
                {
                    _leftColumn.Width = new GridLength(1.0, GridUnitType.Star);
                    SetSplitterVisibility(Visibility.Hidden);
                    _rightColumn.Width = new GridLength(0.0, GridUnitType.Pixel);
                    if (flag)
                        SetFocusOnHost(LeftHost);
                }
                else
                {
                    if (ViewMode == DifferenceViewMode.RightViewOnly)
                    {
                        _leftColumn.Width = new GridLength(0.0, GridUnitType.Pixel);
                        SetSplitterVisibility(Visibility.Hidden);
                    }
                    else
                    {
                        _leftColumn.Width = new GridLength(0.85, GridUnitType.Star);
                        SetSplitterVisibility(Visibility.Visible);
                    }
                    _rightColumn.Width = new GridLength(1.0, GridUnitType.Star);
                    if (flag)
                        SetFocusOnHost(RightHost);
                }
                _inlineColumn.Width = new GridLength(0.0, GridUnitType.Pixel);
            }
            UpdateCaretReadOnlyRegion();
            _oldViewMode = ViewMode;
        }

        private void SetSplitterVisibility(Visibility v)
        {
            if (v == Visibility.Visible)
            {
                _leftColumn.MinWidth = 8.0;
                _rightColumn.MinWidth = 5.0;
                _splitterColumn.Width = new GridLength(3.0, GridUnitType.Pixel);
                _splitter.Visibility = Visibility.Visible;
            }
            else
            {
                _splitter.Visibility = Visibility.Hidden;
                _leftColumn.MinWidth = 0.0;
                _rightColumn.MinWidth = 0.0;
                _splitterColumn.Width = new GridLength(0.0, GridUnitType.Pixel);
            }
        }

        private void SetFocusOnHost(ITextViewHost host)
        {
            if (_viewsLoaded)
            {
                host.TextView.VisualElement.Focus();
                _focusTarget = null;
            }
            else
                _focusTarget = host.TextView;
        }

        private void SynchronizeView(ITextView sourceView, bool updateCaret)
        {
            if (ViewMode == DifferenceViewMode.SideBySide)
            {
                if (sourceView == InlineView)
                {
                    if (DifferenceBuffer.CurrentSnapshotDifference.MapToSourceSnapshot(InlineView.TextViewLines.FirstVisibleLine.Start).Snapshot.TextBuffer == DifferenceBuffer.LeftBuffer)
                        SynchronizeView(InlineHost.TextView, true, LeftHost.TextView, _leftOperations);
                    else
                        SynchronizeView(InlineHost.TextView, true, RightHost.TextView, _rightOperations);
                }
                else
                {
                    RefreshView(CurrentHost.TextView);
                    if (_areViewsSynchronized)
                        return;
                    RefreshView(CurrentHost == LeftHost ? RightView : LeftView);
                }
            }
            else
            {
                ITextView textView;
                IEditorOperations newOperations;
                if (ViewMode == DifferenceViewMode.Inline)
                {
                    textView = InlineHost.TextView;
                    newOperations = _inlineOperations;
                }
                else if (ViewMode == DifferenceViewMode.LeftViewOnly)
                {
                    textView = LeftHost.TextView;
                    newOperations = _leftOperations;
                }
                else
                {
                    textView = RightHost.TextView;
                    newOperations = _rightOperations;
                }
                SynchronizeView(sourceView, updateCaret, textView, newOperations);
            }
        }

        private void SynchronizeView(ITextView oldView, bool updateCaret, ITextView newView, IEditorOperations newOperations)
        {
            if (oldView != newView)
            {
                if (updateCaret)
                {
                    var snapshot = DifferenceBuffer.CurrentSnapshotDifference.MapToSnapshot(oldView.Caret.Position.BufferPosition, newView.TextSnapshot, DifferenceMappingMode.LineColumn);
                    MoveCaretAndClearSelection(newOperations, snapshot);
                }
                if (newView.ZoomLevel != oldView.ZoomLevel)
                    newView.ZoomLevel = oldView.ZoomLevel;
                newView.ViewportLeft = oldView.ViewportLeft;
                RefreshView(newView);
            }
            else
            {
                if (_oldViewMode != DifferenceViewMode.SideBySide)
                    return;
                RefreshView(newView);
            }
        }

        public DifferenceViewMode ViewMode
        {
            get => _mode;
            set => Options.SetOptionValue(DifferenceViewerOptions.ViewModeId, value);
        }

        public DifferenceViewType ActiveViewType
        {
            get
            {
                if (_mode != DifferenceViewMode.SideBySide)
                    return (DifferenceViewType)_mode;
                return LastActiveHost != LeftHost ? DifferenceViewType.RightView : DifferenceViewType.LeftView;
            }
        }

        private DifferenceHighlightMode HighlightMode => Options.GetOptionValue(DifferenceViewerOptions.HighlightModeId);

        public event EventHandler<EventArgs> ViewModeChanged;

        public event EventHandler<EventArgs> Closed;

        public IDifferenceBuffer DifferenceBuffer { get; private set; }

        public ITextViewHost InlineHost { get; private set; }

        public ITextViewHost LeftHost { get; private set; }

        public ITextViewHost RightHost { get; private set; }

        public ITextView InlineView => InlineHost.TextView;

        ITextView IDifferenceViewer.InlineView => InlineHost.TextView;

        public ITextView LeftView => LeftHost.TextView;

        ITextView IDifferenceViewer.LeftView => LeftHost.TextView;

        public ITextView RightView => RightHost.TextView;

        ITextView IDifferenceViewer.RightView => RightHost.TextView;

        public FrameworkElement VisualElement => _grid;

        private FrameworkElement InlineVisualElement { get; set; }

        private FrameworkElement LeftVisualElement { get; set; }

        private FrameworkElement RightVisualElement { get; set; }

        public IEditorOptions Options { get; private set; }

        public bool AreViewsSynchronized
        {
            get => _areViewsSynchronized;
            internal set
            {
                if (_areViewsSynchronized == value)
                    return;
                _areViewsSynchronized = value;
                if (ViewMode != DifferenceViewMode.SideBySide)
                    return;
                if (RightHost.GetTextViewMargin("VerticalScrollBar") is SideBySideVerticalScrollBarMargin textViewMargin)
                {
                    if (_areViewsSynchronized)
                    {
                        LeftView.Options.SetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId, false);
                        textViewMargin.UseSideBySideMapping = true;
                    }
                    else
                    {
                        LeftView.Options.SetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId, true);
                        textViewMargin.UseSideBySideMapping = false;
                    }
                }
                RefreshView(CurrentHost.TextView);
                if (_areViewsSynchronized)
                    return;
                RefreshView(CurrentHost == LeftHost ? RightView : LeftView);
            }
        }

        public bool IsClosed { get; private set; }

        public void Close()
        {
            if (IsClosed)
                return;
            IsClosed = true;
            LeftView.LayoutChanged -= TextViewLayoutChanged;
            RightView.LayoutChanged -= TextViewLayoutChanged;
            DifferenceBuffer.SnapshotDifferenceChanging -= SnapshotDifferenceChanging;
            DifferenceBuffer.SnapshotDifferenceChanged -= SnapshotDifferenceChanged;
            FocusManager.SetFocusedElement(_grid, null);
            DifferenceBuffer.Dispose();
            if (!InlineHost.IsClosed)
            {
                InlineHost.TextView.VisualElement.Loaded -= OnLoaded;
                if (!DifferenceBuffer.IsEditingDisabled)
                {
                    InlineView.Caret.PositionChanged -= OnInlineCaretPositionChanged;
                    InlineView.LayoutChanged -= OnInlineTextViewLayoutChanged;
                }
                InlineHost.Close();
            }
            if (!LeftHost.IsClosed)
            {
                LeftHost.TextView.VisualElement.Loaded -= OnLoaded;
                LeftHost.Close();
            }
            if (!RightHost.IsClosed)
            {
                RightHost.TextView.VisualElement.Loaded -= OnLoaded;
                RightHost.Close();
            }
            Options.OptionChanged -= EditorOptionsChanged;
            _brushManager.BrushesChanged -= OnBrushesChanged;
            //TODO: undo
            //DifferenceBuffer.InlineBuffer.Properties.RemoveProperty((object)typeof(ITextUndoHistory));
            //DifferenceBuffer.InlineBuffer.Properties.RemoveProperty((object)typeof(ITextBufferUndoManager));
            InlineView.Properties.RemoveProperty(typeof(IMapEditToData));
            // ISSUE: reference to a compiler-generated field
            var closed = Closed;
            closed?.Invoke(this, EventArgs.Empty);
        }

        internal ITextViewHost CurrentHost => GetHostForViewMode(ViewMode);

        private ITextViewHost GetHostForViewMode(DifferenceViewMode mode)
        {
            switch (mode)
            {
                case DifferenceViewMode.Inline:
                    return InlineHost;
                case DifferenceViewMode.LeftViewOnly:
                    return LeftHost;
                case DifferenceViewMode.RightViewOnly:
                    return RightHost;
                default:
                    if (LastActiveHost == LeftHost)
                        return LeftHost;
                    return RightHost;
            }
        }

        public PropertyCollection Properties { get; private set; }

        internal static double GetLineGapAbove(bool isLeft, SnapshotPoint point, ISnapshotDifference snapshotDifference)
        {
            var containingLine = snapshotDifference.TranslateToSnapshot(point).GetContainingLine();
            var matchOrDifference = snapshotDifference.FindMatchOrDifference(containingLine.Start, out var match, out _);
            if (match != null && matchOrDifference > 0)
            {
                var lineNumber = containingLine.LineNumber;
                Span span;
                int start;
                if (!isLeft)
                {
                    span = match.Right;
                    start = span.Start;
                }
                else
                {
                    span = match.Left;
                    start = span.Start;
                }
                if (lineNumber == start)
                {
                    var difference2 = snapshotDifference.LineDifferences.Differences[matchOrDifference - 1];
                    int num1;
                    if (!isLeft)
                    {
                        span = difference2.Left;
                        var length1 = span.Length;
                        span = difference2.Right;
                        var length2 = span.Length;
                        num1 = length1 - length2;
                    }
                    else
                    {
                        span = difference2.Right;
                        var length1 = span.Length;
                        span = difference2.Left;
                        var length2 = span.Length;
                        num1 = length1 - length2;
                    }
                    var num2 = num1;
                    if (num2 > 0)
                        return num2;
                }
            }
            return 0.0;
        }

        public bool ScrollToNextChange(bool wrap)
        {
            return ScrollToNextChange(CurrentHost.TextView.Caret.Position.BufferPosition, wrap);
        }

        private bool IsChangeForBuffer(Difference difference)
        {
            if (difference.Left.Length == 0)
                return ViewMode != DifferenceViewMode.LeftViewOnly;
            if (difference.Right.Length == 0)
                return ViewMode != DifferenceViewMode.RightViewOnly;
            return true;
        }

        public bool ScrollToNextChange(SnapshotPoint point, bool wrap)
        {
            ThrowIfNoDiffComputed();
            if (DifferenceBuffer.CurrentSnapshotDifference.LineDifferences.Differences.Count > 0)
            {
                var matchOrDifference = DifferenceBuffer.CurrentSnapshotDifference.FindMatchOrDifference(point, out _, out var difference1);
                if (difference1 != null)
                    ++matchOrDifference;
                IList<Difference> differences;
                for (differences = DifferenceBuffer.CurrentSnapshotDifference.LineDifferences.Differences; matchOrDifference < differences.Count; ++matchOrDifference)
                {
                    var difference2 = differences[matchOrDifference];
                    if (IsChangeForBuffer(difference2))
                    {
                        ScrollToChange(difference2);
                        return true;
                    }
                }
                if (wrap)
                {
                    ScrollToChange(differences[0]);
                    return true;
                }
            }
            return false;
        }

        public bool ScrollToPreviousChange(bool wrap)
        {
            return ScrollToPreviousChange(CurrentHost.TextView.Caret.Position.BufferPosition, wrap);
        }

        public bool ScrollToPreviousChange(SnapshotPoint point, bool wrap)
        {
            ThrowIfNoDiffComputed();
            if (DifferenceBuffer.CurrentSnapshotDifference.LineDifferences.Differences.Count > 0)
            {
                point = DifferenceBuffer.CurrentSnapshotDifference.TranslateToSnapshot(point);
                var index = DifferenceBuffer.CurrentSnapshotDifference.FindMatchOrDifference(point, out var match, out _) - 1;
                var differences = DifferenceBuffer.CurrentSnapshotDifference.LineDifferences.Differences;
                if (match != null && index >= 0 && point.Snapshot != DifferenceBuffer.CurrentSnapshotDifference.InlineBufferSnapshot)
                {
                    var difference2 = differences[index];
                    var span = point.Snapshot == DifferenceBuffer.CurrentSnapshotDifference.LeftBufferSnapshot ? difference2.Left : difference2.Right;
                    if (span.Length == 0 && point == point.Snapshot.GetLineFromLineNumber(span.Start).Start)
                        --index;
                }
                for (; index >= 0; --index)
                {
                    var difference2 = differences[index];
                    if (IsChangeForBuffer(difference2))
                    {
                        ScrollToChange(difference2);
                        return true;
                    }
                }
                if (wrap)
                {
                    ScrollToChange(differences[differences.Count - 1]);
                    return true;
                }
            }
            return false;
        }

        public void ScrollToChange(Difference difference)
        {
            ThrowIfNoDiffComputed();
            var textView = CurrentHost.TextView;
            var view = textView;
            var span = DifferenceBuffer.CurrentSnapshotDifference.MapToSnapshot(difference, view.TextSnapshot);
            var start = span.Start;
            if (view == RightView && difference.Right.Length == 0 || view == LeftView && difference.Left.Length == 0)
            {
                if (_areViewsSynchronized && ViewMode == DifferenceViewMode.SideBySide && (!RightView.IsClosed && !LeftView.IsClosed))
                {
                    if (view == RightView)
                    {
                        view = LeftView;
                        span = new SnapshotSpan(DifferenceBuffer.CurrentSnapshotDifference.LeftBufferSnapshot.GetLineFromLineNumber(difference.Left.Start).End, DifferenceBuffer.CurrentSnapshotDifference.LeftBufferSnapshot.GetLineFromLineNumber(difference.Left.End - 1).End);
                    }
                    else
                    {
                        view = RightView;
                        span = new SnapshotSpan(DifferenceBuffer.CurrentSnapshotDifference.RightBufferSnapshot.GetLineFromLineNumber(difference.Right.Start).End, DifferenceBuffer.CurrentSnapshotDifference.RightBufferSnapshot.GetLineFromLineNumber(difference.Right.End - 1).End);
                    }
                    span = span.TranslateTo(view.TextSnapshot, SpanTrackingMode.EdgeExclusive);
                }
                else
                {
                    var containingLine = span.Start.GetContainingLine();
                    if (containingLine.LineNumber > 0)
                        span = new SnapshotSpan(containingLine.Snapshot.GetLineFromLineNumber(containingLine.LineNumber - 1).End, span.End);
                }
            }
            if (!IsSpanCompletelyVisible(view, span))
                view.DisplayTextLineContainingBufferPosition(span.Start, view.ViewportHeight / 4.0, ViewRelativePosition.Top);
            MoveCaretAndClearSelection(textView == InlineView ? _inlineOperations : (textView == RightView ? _rightOperations : _leftOperations), start);
        }

        public void ScrollToMatch(Match match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            ThrowIfNoDiffComputed();
            if (CurrentHost == LeftHost)
            {
                var leftBufferSnapshot = DifferenceBuffer.CurrentSnapshotDifference.LeftBufferSnapshot;
                ScrollView(_leftOperations, new SnapshotSpan(leftBufferSnapshot.GetLineFromLineNumber(match.Left.Start).Start, leftBufferSnapshot.GetLineFromLineNumber(match.Left.End - 1).End));
            }
            else
            {
                var rightBufferSnapshot = DifferenceBuffer.CurrentSnapshotDifference.RightBufferSnapshot;
                var start = rightBufferSnapshot.GetLineFromLineNumber(match.Right.Start).Start;
                var end = rightBufferSnapshot.GetLineFromLineNumber(match.Right.End - 1).End;
                if (ViewMode == DifferenceViewMode.Inline)
                    ScrollView(_inlineOperations, new SnapshotSpan(DifferenceBuffer.CurrentSnapshotDifference.MapToInlineSnapshot(start), DifferenceBuffer.CurrentSnapshotDifference.MapToInlineSnapshot(end)));
                else
                    ScrollView(_rightOperations, new SnapshotSpan(start, end));
            }
        }

        private static void ScrollView(IEditorOperations operations, SnapshotSpan span, bool scrollEvenIfVisible = false)
        {
            ScrollView(operations, span, scrollEvenIfVisible, operations.TextView.ViewportHeight / 4.0);
        }

        private static void ScrollView(IEditorOperations operations, SnapshotSpan visibleSpan, bool scrollEvenIfVisible, double offset)
        {
            visibleSpan = visibleSpan.TranslateTo(operations.TextView.TextSnapshot, SpanTrackingMode.EdgeExclusive);
            MoveCaretAndClearSelection(operations, visibleSpan.Start);
            if (!scrollEvenIfVisible && IsSpanCompletelyVisible(operations.TextView, visibleSpan))
                return;
            operations.TextView.DisplayTextLineContainingBufferPosition(visibleSpan.Start, offset, ViewRelativePosition.Top);
        }

        private static void MoveCaretAndClearSelection(IEditorOperations operations, SnapshotPoint position)
        {
            var virtualSnapshotPoint = new VirtualSnapshotPoint(position);
            operations.SelectAndMoveCaret(virtualSnapshotPoint, virtualSnapshotPoint, TextSelectionMode.Stream, new EnsureSpanVisibleOptions?());
        }

        private static bool IsSpanCompletelyVisible(ITextView view, SnapshotSpan span)
        {
            var containingBufferPosition1 = view.TextViewLines.GetTextViewLineContainingBufferPosition(span.Start);
            if (containingBufferPosition1 == null || containingBufferPosition1.VisibilityState != VisibilityState.FullyVisible)
                return false;
            var containingBufferPosition2 = view.TextViewLines.GetTextViewLineContainingBufferPosition(span.End);
            return containingBufferPosition2 != null && containingBufferPosition2.VisibilityState == VisibilityState.FullyVisible;
        }

        private void ThrowIfNoDiffComputed()
        {
            if (DifferenceBuffer.CurrentSnapshotDifference == null)
                throw new InvalidOperationException("A CurrentSnapshotDifference for the difference buffer has not been computed yet.");
        }

        public ITextView GetSubordinateView(ITextView masterView)
        {
            if (!_areViewsSynchronized || DifferenceBuffer.CurrentSnapshotDifference == null || (ViewMode != DifferenceViewMode.SideBySide || RightView.IsClosed) || LeftView.IsClosed)
                return null;
            _differenceToUseForSynchronizedLayout = DifferenceBuffer.CurrentSnapshotDifference;
            if (masterView != RightView)
                return RightHost.TextView;
            return LeftHost.TextView;
        }

        public bool TryGetAnchorPointInSubordinateView(SnapshotPoint anchorPoint, out SnapshotPoint correspondingAnchorPoint)
        {
            var flag = anchorPoint.Snapshot.TextBuffer == LeftView.TextBuffer;
            anchorPoint = anchorPoint.TranslateTo(flag ? _differenceToUseForSynchronizedLayout.LeftBufferSnapshot : _differenceToUseForSynchronizedLayout.RightBufferSnapshot, PointTrackingMode.Positive);
            _differenceToUseForSynchronizedLayout.FindMatchOrDifference(anchorPoint, out var match, out var difference);
            Span left;
            Span right;
            if (match != null)
            {
                left = match.Left;
                right = match.Right;
            }
            else
            {
                left = difference.Left;
                right = difference.Right;
            }
            var lineNumber = anchorPoint.GetContainingLine().LineNumber;
            if (flag)
            {
                var num = lineNumber - left.Start;
                if (num < right.Length)
                {
                    correspondingAnchorPoint = _differenceToUseForSynchronizedLayout.RightBufferSnapshot.GetLineFromLineNumber(right.Start + num).Start;
                    return true;
                }
            }
            else
            {
                var num = lineNumber - right.Start;
                if (num < left.Length)
                {
                    correspondingAnchorPoint = _differenceToUseForSynchronizedLayout.LeftBufferSnapshot.GetLineFromLineNumber(left.Start + num).Start;
                    return true;
                }
            }
            correspondingAnchorPoint = new SnapshotPoint();
            return false;
        }

        public SnapshotPoint GetAnchorPointAboveInSubordinateView(SnapshotPoint anchorPoint)
        {
            var flag = anchorPoint.Snapshot.TextBuffer == LeftView.TextBuffer;
            anchorPoint = anchorPoint.TranslateTo(flag ? _differenceToUseForSynchronizedLayout.LeftBufferSnapshot : _differenceToUseForSynchronizedLayout.RightBufferSnapshot, PointTrackingMode.Positive);
            _differenceToUseForSynchronizedLayout.FindMatchOrDifference(anchorPoint, out _, out var difference);
            if (!flag)
                return _differenceToUseForSynchronizedLayout.LeftBufferSnapshot.GetLineFromLineNumber(Math.Max(0, difference.Left.End - 1)).End;
            return _differenceToUseForSynchronizedLayout.RightBufferSnapshot.GetLineFromLineNumber(Math.Max(0, difference.Right.End - 1)).End;
        }

        public void WhichPairedLinesShouldBeDisplayed(SnapshotPoint masterAnchorPoint, SnapshotPoint subordinateAnchorPoint, out bool layoutMaster, out bool layoutSubordinate, bool goingUp)
        {
            var flag1 = masterAnchorPoint.Snapshot.TextBuffer == LeftView.TextBuffer;
            SnapshotPoint point1;
            SnapshotPoint point2;
            if (flag1)
            {
                point1 = masterAnchorPoint;
                point2 = subordinateAnchorPoint;
            }
            else
            {
                point1 = subordinateAnchorPoint;
                point2 = masterAnchorPoint;
            }
            point1 = point1.TranslateTo(_differenceToUseForSynchronizedLayout.LeftBufferSnapshot, PointTrackingMode.Positive);
            point2 = point2.TranslateTo(_differenceToUseForSynchronizedLayout.RightBufferSnapshot, PointTrackingMode.Positive);
            var matchOrDifference1 = _differenceToUseForSynchronizedLayout.FindMatchOrDifference(point1, out var match1, out var difference1);
            var matchOrDifference2 = _differenceToUseForSynchronizedLayout.FindMatchOrDifference(point2, out var match2, out var difference2);
            int num1;
            int num2;
            if (matchOrDifference1 == matchOrDifference2)
            {
                Span span;
                int num3;
                if (match1 == null)
                {
                    num3 = difference1.Before?.Left.Start ?? 0;
                }
                else
                {
                    span = match1.Left;
                    num3 = span.Start;
                }
                var num4 = num3;
                num1 = point1.GetContainingLine().LineNumber - num4;
                int num5;
                if (match2 == null)
                {
                    if (difference2.Before == null)
                    {
                        num5 = 0;
                    }
                    else
                    {
                        span = difference2.Before.Right;
                        num5 = span.Start;
                    }
                }
                else
                {
                    span = match2.Right;
                    num5 = span.Start;
                }
                var num6 = num5;
                num2 = point2.GetContainingLine().LineNumber - num6;
            }
            else
            {
                num1 = matchOrDifference1;
                num2 = matchOrDifference2;
            }
            if (num1 == num2)
            {
                layoutMaster = layoutSubordinate = true;
            }
            else
            {
                var flag2 = num1 > num2 == goingUp;
                layoutMaster = flag1 == flag2;
                layoutSubordinate = !layoutMaster;
            }
        }
    }
}
