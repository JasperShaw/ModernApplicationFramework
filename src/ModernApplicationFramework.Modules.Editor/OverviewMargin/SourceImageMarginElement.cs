using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal class SourceImageMarginElement : Canvas
    {
        private static readonly List<SourceImageMarginElement> ActiveMargins = new List<SourceImageMarginElement>(8);
        private double _currentTrackSpanHeight = -1.0;
        private double _currentScrollMapEnd = -1.0;
        private readonly List<SnapshotSpan> _reclassifiedSpans = new List<SnapshotSpan>();
        private readonly ITextView _textView;
        private readonly ITextAndAdornmentSequencer _simpleSequencer;
        private double _currentWordWrapWidth;
        private double _currentAutoIndent;
        public readonly IVerticalScrollBar ScrollBar;
        private readonly IClassifier _classifier;
        private readonly IClassificationFormatMap _classificationFormatMap;
        private readonly SourceImageMarginFactory _factory;
        internal RenderTargetBitmap Bitmap;
        private bool _isActive;
        private bool _formatQueued;
        private bool _isDisposed;
        public const int LineCost = 100;
        public const int MaxCost = 2000;
        public const double MinumumWidth = 25.0;
        public const double TargetWidth = 800.0;

        public ITextSnapshot CurrentSnapshot { get; private set; }

        public IFormattedLineSource Source { get; private set; }

        public SourceImageMarginElement(ITextView textView, SourceImageMarginFactory factory, IVerticalScrollBar verticalScrollbar)
        {
            _textView = textView;
            _factory = factory;
            ScrollBar = verticalScrollbar;
            IsHitTestVisible = false;
            Focusable = false;
            _simpleSequencer = new SimpleTextAndAdornmentSequencer(factory.BufferGraphFactoryService.CreateBufferGraph(textView.TextBuffer), textView.TextBuffer);
            _classificationFormatMap = factory.ClassificationFormatMappingService.GetClassificationFormatMap(textView);
            _classifier = factory.ClassifierAggregatorService.GetClassifier(textView.TextBuffer);
            OnOptionChanged(null, null);
            _textView.Options.OptionChanged += OnOptionChanged;
            _textView.LayoutChanged += OnLayoutChanged;
            IsVisibleChanged += (sender, e) =>
            {
                if (!(bool)e.NewValue)
                    return;
                ActivateMargin(this);
            };
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            foreach (LineTile child in Children)
                child.IsDirty = true;
            QueueFormat();
        }

        private static void ActivateMargin(SourceImageMarginElement margin)
        {
            if (margin._isActive)
            {
                ActiveMargins.Remove(margin);
            }
            else
            {
                if (ActiveMargins.Count == 8)
                    DeactivateMargin(ActiveMargins[0]);
                margin._isActive = true;
                margin.ScrollBar.Map.MappingChanged += margin.OnMappingChanged;
                margin._classifier.ClassificationChanged += margin.OnClassificationChanged;
                margin._classificationFormatMap.ClassificationFormatMappingChanged += margin.OnClassificationFormatMappingChanged;
                margin.QueueFormat();
            }
            ActiveMargins.Add(margin);
        }

        private static void DeactivateMargin(SourceImageMarginElement margin)
        {
            if (!margin._isActive)
                return;
            ActiveMargins.Remove(margin);
            margin._isActive = false;
            margin.ScrollBar.Map.MappingChanged -= margin.OnMappingChanged;
            margin._classifier.ClassificationChanged -= margin.OnClassificationChanged;
            margin._classificationFormatMap.ClassificationFormatMappingChanged -= margin.OnClassificationFormatMappingChanged;
            margin.Children.Clear();
            margin.CurrentSnapshot = null;
            margin.Source = null;
            if (margin.Bitmap == null)
                return;
            margin.Bitmap.Clear();
            margin.Bitmap.Freeze();
            margin.Bitmap = null;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            _textView.Options.OptionChanged -= OnOptionChanged;
            _textView.LayoutChanged -= OnLayoutChanged;
            DeactivateMargin(this);
            (_classifier as IDisposable)?.Dispose();
        }

        public bool Enabled
        {
            get
            {
                if (_textView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowEnhancedScrollBarOptionId) && _textView.Options.GetOptionValue(DefaultTextViewHostOptions.SourceImageMarginEnabledOptionId))
                    return _textView.Options.GetOptionValue(DefaultTextViewHostOptions.SourceImageMarginWidthOptionId) >= 25.0;
                return false;
            }
        }

        private void OnOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (!Enabled)
            {
                DeactivateMargin(this);
                Visibility = Visibility.Collapsed;
            }
            else
            {
                bool flag = Visibility == Visibility.Visible;
                Visibility = Visibility.Visible;
                double width1 = Width;
                Width = _textView.Options.GetOptionValue(DefaultTextViewHostOptions.SourceImageMarginWidthOptionId);
                double width2 = Width;
                if (width1 != width2 || !flag)
                {
                    foreach (LineTile child in Children)
                        child.IsDirty = true;
                }
                QueueFormat();
            }
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (Visibility != Visibility.Visible || !IsVisible || _textView.IsClosed)
                return;
            ActivateMargin(this);
            Source = null;
            QueueFormat();
        }

        private bool EnsureSnapshot()
        {
            if (!_isActive || ScrollBar.TrackSpanHeight <= 0.0)
                return false;
            ITextSnapshot textSnapshot = _textView.TextSnapshot;
            if (textSnapshot == CurrentSnapshot && ScrollBar.Map.End == _currentScrollMapEnd && ScrollBar.TrackSpanHeight == _currentTrackSpanHeight)
                return false;
            _currentScrollMapEnd = ScrollBar.Map.End;
            _currentTrackSpanHeight = ScrollBar.TrackSpanHeight;
            if (CurrentSnapshot != textSnapshot)
            {
                if (CurrentSnapshot != null)
                {
                    IList<Span> spanList = new List<Span>();
                    for (ITextVersion textVersion = CurrentSnapshot.Version; textSnapshot.Version != textVersion; textVersion = textVersion.Next)
                    {
                        foreach (ITextChange change in textVersion.Changes)
                        {
                            SnapshotSpan span = textVersion.CreateTrackingSpan(change.OldSpan, SpanTrackingMode.EdgeInclusive).GetSpan(CurrentSnapshot);
                            SnapshotPoint snapshotPoint = span.Start;
                            ITextSnapshotLine containingLine = snapshotPoint.GetContainingLine();
                            snapshotPoint = span.End;
                            ITextSnapshotLine textSnapshotLine1;
                            if (snapshotPoint.Position >= containingLine.EndIncludingLineBreak)
                            {
                                snapshotPoint = span.End;
                                textSnapshotLine1 = snapshotPoint.GetContainingLine();
                            }
                            else
                                textSnapshotLine1 = containingLine;
                            ITextSnapshotLine textSnapshotLine2 = textSnapshotLine1;
                            spanList.Add(Span.FromBounds(containingLine.LineNumber, textSnapshotLine2.LineNumber + 1));
                        }
                    }
                    NormalizedSpanCollection spans = new NormalizedSpanCollection(spanList);
                    int startingIndex = 0;
                    int startingLine = 0;
                    int index = 0;
                    while (index < Children.Count)
                    {
                        LineTile child = (LineTile)Children[index];
                        if (!child.IsDirty && SpanInvalidated(spans, child.LineSpan, ref startingIndex))
                            child.IsDirty = true;
                        if (child.SetSnapshot(CurrentSnapshot, textSnapshot, ref startingLine))
                        {
                            ++index;
                        }
                        else
                        {
                            if (startingLine >= textSnapshot.LineCount)
                            {
                                Children.RemoveRange(index, Children.Count - index);
                                break;
                            }
                            Children.RemoveAt(index);
                        }
                    }
                }
                else
                {
                    LineTile lineTile = new LineTile();
                    lineTile.SetLineSpan(0, textSnapshot.LineCount - 1);
                    Children.Add(lineTile);
                }
                CurrentSnapshot = textSnapshot;
                Source = null;
            }
            for (int index1 = 0; index1 < Children.Count; ++index1)
            {
                LineTile child1 = (LineTile)Children[index1];
                ITextSnapshot currentSnapshot1 = CurrentSnapshot;
                Span lineSpan = child1.LineSpan;
                int start1 = lineSpan.Start;
                ITextSnapshotLine lineFromLineNumber = currentSnapshot1.GetLineFromLineNumber(start1);
                lineSpan = child1.LineSpan;
                ITextSnapshotLine textSnapshotLine1;
                if (lineSpan.Length != 0)
                {
                    ITextSnapshot currentSnapshot2 = CurrentSnapshot;
                    lineSpan = child1.LineSpan;
                    int end = lineSpan.End;
                    textSnapshotLine1 = currentSnapshot2.GetLineFromLineNumber(end);
                }
                else
                    textSnapshotLine1 = lineFromLineNumber;
                ITextSnapshotLine textSnapshotLine2 = textSnapshotLine1;
                double ytop = GetYTop(lineFromLineNumber.Start);
                double num = GetYBottom(textSnapshotLine2.End) - ytop;
                if (num < 3.0 || num > 5.0)
                {
                    lineSpan = child1.LineSpan;
                    int end1 = lineSpan.End;
                    SnapshotPoint positionOfYcoordinate = ScrollBar.GetBufferPositionOfYCoordinate(ytop + 4.0);
                    int lineNumber = (positionOfYcoordinate.Position >= lineFromLineNumber.EndIncludingLineBreak.Position ? positionOfYcoordinate.GetContainingLine() : lineFromLineNumber).LineNumber;
                    if (lineNumber != end1)
                    {
                        LineTile lineTile1 = child1;
                        lineSpan = child1.LineSpan;
                        int start2 = lineSpan.Start;
                        int end2 = lineNumber;
                        lineTile1.SetLineSpan(start2, end2);
                        int start3 = lineNumber + 1;
                        if (lineNumber > end1)
                        {
                            if (start3 == textSnapshot.LineCount)
                            {
                                Children.RemoveRange(index1 + 1, Children.Count - index1 - 1);
                            }
                            else
                            {
                                for (int index2 = index1 + 1; index2 < Children.Count; ++index2)
                                {
                                    LineTile child2 = (LineTile)Children[index2];
                                    lineSpan = child2.LineSpan;
                                    int end3 = lineSpan.End;
                                    if (end3 > lineNumber)
                                    {
                                        Children.RemoveRange(index1 + 1, index2 - index1 - 1);
                                        child2.SetLineSpan(start3, end3);
                                        break;
                                    }
                                }
                            }
                        }
                        else if (index1 + 1 < Children.Count)
                        {
                            LineTile child2 = (LineTile)Children[index1 + 1];
                            LineTile lineTile2 = child2;
                            int start4 = start3;
                            lineSpan = child2.LineSpan;
                            int end3 = lineSpan.End;
                            lineTile2.SetLineSpan(start4, end3);
                        }
                        else
                        {
                            LineTile lineTile2 = new LineTile();
                            Children.Add(lineTile2);
                            lineTile2.SetLineSpan(start3, textSnapshot.LineCount - 1);
                        }
                    }
                }
                child1.SetPosition(this);
            }
            return true;
        }

        public static bool SpanInvalidated(NormalizedSpanCollection spans, Span testSpan, ref int startingIndex)
        {
            int num1 = startingIndex;
            int num2 = spans.Count;
            while (num1 < num2)
            {
                int index = (num1 + num2) / 2;
                if (spans[index].Start <= testSpan.End)
                    num1 = index + 1;
                else
                    num2 = index;
            }
            if (num1 == startingIndex)
                return false;
            startingIndex = num1 - 1;
            if (testSpan.Start < spans[startingIndex].End)
                return true;
            ++startingIndex;
            return false;
        }

        public bool VerifyDti()
        {
            if (Children.Count == 0)
                return false;
            Span lineSpan = ((LineTile)Children[0]).LineSpan;
            if (lineSpan.Start != 0)
                return false;
            for (int index = 1; index < Children.Count; ++index)
            {
                lineSpan = ((LineTile)Children[index - 1]).LineSpan;
                int num = lineSpan.End + 1;
                lineSpan = ((LineTile)Children[index]).LineSpan;
                int start = lineSpan.Start;
                if (num != start)
                    return false;
            }
            lineSpan = ((LineTile)Children[Children.Count - 1]).LineSpan;
            return lineSpan.End == CurrentSnapshot.LineCount - 1;
        }

        private void QueueFormat()
        {
            if (_formatQueued || !_isActive || ScrollBar.TrackSpanHeight <= 0.0)
                return;
            _formatQueued = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(DoFormat));
        }

        private void DoFormat()
        {
            _formatQueued = false;
            if (_textView.IsClosed || !_isActive || ScrollBar.TrackSpanHeight <= 0.0)
                return;
            if (EnsureSnapshot())
            {
                QueueFormat();
            }
            else
            {
                if (Source == null)
                {
                    WordWrapStyles optionValue = _textView.Options.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId);
                    if (_currentWordWrapWidth != _textView.FormattedLineSource.WordWrapWidth || _currentAutoIndent != _textView.FormattedLineSource.MaxAutoIndent)
                    {
                        foreach (LineTile child in Children)
                            child.IsDirty = true;
                        _currentWordWrapWidth = _textView.FormattedLineSource.WordWrapWidth;
                        _currentAutoIndent = _textView.FormattedLineSource.MaxAutoIndent;
                    }
                    Source = _factory.FormattedTextSourceFactoryService.Create(_textView.TextSnapshot, CurrentSnapshot, _textView.Options.GetTabSize(), _textView.FormattedLineSource.BaseIndentation, _textView.FormattedLineSource.WordWrapWidth, _textView.FormattedLineSource.MaxAutoIndent, false, _classifier, _simpleSequencer, _classificationFormatMap, (optionValue & (WordWrapStyles.WordWrap | WordWrapStyles.VisibleGlyphs)) == (WordWrapStyles.WordWrap | WordWrapStyles.VisibleGlyphs));
                }
                int num1 = 2000;
                double num2 = _currentWordWrapWidth;
                if (num2 == 0.0)
                    num2 = 800.0;
                double xScale = Math.Min(1.0, Math.Max(0.05, Width / num2));
                foreach (LineTile child in Children)
                {
                    if (!child.IsFormatted)
                    {
                        num1 -= child.DoFormat(this, xScale);
                        if (num1 <= 0)
                        {
                            QueueFormat();
                            return;
                        }
                    }
                }
                if (_reclassifiedSpans.Count > 0)
                {
                    NormalizedSpanCollection spans;
                    lock (_reclassifiedSpans)
                    {
                        List<Span> spanList = new List<Span>(_reclassifiedSpans.Count);
                        foreach (SnapshotSpan reclassifiedSpan in _reclassifiedSpans)
                        {
                            SnapshotSpan snapshotSpan = reclassifiedSpan.TranslateTo(CurrentSnapshot, SpanTrackingMode.EdgeInclusive);
                            SnapshotPoint snapshotPoint = snapshotSpan.Start;
                            ITextSnapshotLine containingLine = snapshotPoint.GetContainingLine();
                            snapshotPoint = snapshotSpan.End;
                            ITextSnapshotLine textSnapshotLine1;
                            if (snapshotPoint.Position >= containingLine.EndIncludingLineBreak)
                            {
                                snapshotPoint = snapshotSpan.End;
                                textSnapshotLine1 = snapshotPoint.GetContainingLine();
                            }
                            else
                                textSnapshotLine1 = containingLine;
                            ITextSnapshotLine textSnapshotLine2 = textSnapshotLine1;
                            spanList.Add(Span.FromBounds(containingLine.LineNumber, textSnapshotLine2.LineNumber + 1));
                        }
                        spans = new NormalizedSpanCollection(spanList);
                        _reclassifiedSpans.Clear();
                    }
                    int startingIndex = 0;
                    foreach (LineTile child in Children)
                    {
                        if (!child.IsDirty && SpanInvalidated(spans, child.LineSpan, ref startingIndex))
                            child.IsDirty = true;
                    }
                }
                foreach (LineTile child in Children)
                {
                    if (child.IsDirty)
                    {
                        num1 -= child.DoFormat(this, xScale);
                        if (num1 <= 0)
                        {
                            QueueFormat();
                            break;
                        }
                    }
                }
            }
        }

        private void OnMappingChanged(object sender, EventArgs e)
        {
            QueueFormat();
        }

        private void OnClassificationChanged(object sender, ClassificationChangedEventArgs e)
        {
            lock (_reclassifiedSpans)
                _reclassifiedSpans.Add(e.ChangeSpan);
        }

        public double GetYTop(SnapshotPoint position)
        {
            return ScrollBar.GetYCoordinateOfScrollMapPosition(ScrollBar.Map.GetCoordinateAtBufferPosition(position) - 0.5);
        }

        public double GetYBottom(SnapshotPoint position)
        {
            return ScrollBar.GetYCoordinateOfScrollMapPosition(ScrollBar.Map.GetCoordinateAtBufferPosition(position) + 0.5);
        }
    }
}
