﻿using System;
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
        public const int LineCost = 100;
        public const int MaxCost = 2000;
        public const double MinumumWidth = 25.0;
        public const double TargetWidth = 800.0;
        private static readonly List<SourceImageMarginElement> ActiveMargins = new List<SourceImageMarginElement>(8);
        public readonly IVerticalScrollBar ScrollBar;
        internal RenderTargetBitmap Bitmap;
        private readonly IClassificationFormatMap _classificationFormatMap;
        private readonly IClassifier _classifier;
        private readonly SourceImageMarginFactory _factory;
        private readonly List<SnapshotSpan> _reclassifiedSpans = new List<SnapshotSpan>();
        private readonly ITextAndAdornmentSequencer _simpleSequencer;
        private readonly ITextView _textView;
        private double _currentAutoIndent;
        private double _currentScrollMapEnd = -1.0;
        private double _currentTrackSpanHeight = -1.0;
        private double _currentWordWrapWidth;
        private bool _formatQueued;
        private bool _isActive;
        private bool _isDisposed;

        public ITextSnapshot CurrentSnapshot { get; private set; }

        public bool Enabled
        {
            get
            {
                if (_textView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowEnhancedScrollBarOptionId) &&
                    _textView.Options.GetOptionValue(DefaultTextViewHostOptions.SourceImageMarginEnabledOptionId))
                    return _textView.Options.GetOptionValue(DefaultTextViewHostOptions
                               .SourceImageMarginWidthOptionId) >= 25.0;
                return false;
            }
        }

        public IFormattedLineSource Source { get; private set; }

        public SourceImageMarginElement(ITextView textView, SourceImageMarginFactory factory,
            IVerticalScrollBar verticalScrollbar)
        {
            _textView = textView;
            _factory = factory;
            ScrollBar = verticalScrollbar;
            IsHitTestVisible = false;
            Focusable = false;
            _simpleSequencer = new SimpleTextAndAdornmentSequencer(
                factory.BufferGraphFactoryService.CreateBufferGraph(textView.TextBuffer), textView.TextBuffer);
            _classificationFormatMap = factory.ClassificationFormatMappingService.GetClassificationFormatMap(textView);
            _classifier = factory.ClassifierAggregatorService.GetClassifier(textView.TextBuffer);
            OnOptionChanged(null, null);
            _textView.Options.OptionChanged += OnOptionChanged;
            _textView.LayoutChanged += OnLayoutChanged;
            IsVisibleChanged += (sender, e) =>
            {
                if (!(bool) e.NewValue)
                    return;
                ActivateMargin(this);
            };
        }

        public static bool SpanInvalidated(NormalizedSpanCollection spans, Span testSpan, ref int startingIndex)
        {
            var num1 = startingIndex;
            var num2 = spans.Count;
            while (num1 < num2)
            {
                var index = (num1 + num2) / 2;
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

        public double GetYBottom(SnapshotPoint position)
        {
            return ScrollBar.GetYCoordinateOfScrollMapPosition(
                ScrollBar.Map.GetCoordinateAtBufferPosition(position) + 0.5);
        }

        public double GetYTop(SnapshotPoint position)
        {
            return ScrollBar.GetYCoordinateOfScrollMapPosition(
                ScrollBar.Map.GetCoordinateAtBufferPosition(position) - 0.5);
        }

        public bool VerifyDti()
        {
            if (Children.Count == 0)
                return false;
            var lineSpan = ((LineTile) Children[0]).LineSpan;
            if (lineSpan.Start != 0)
                return false;
            for (var index = 1; index < Children.Count; ++index)
            {
                lineSpan = ((LineTile) Children[index - 1]).LineSpan;
                var num = lineSpan.End + 1;
                lineSpan = ((LineTile) Children[index]).LineSpan;
                var start = lineSpan.Start;
                if (num != start)
                    return false;
            }

            lineSpan = ((LineTile) Children[Children.Count - 1]).LineSpan;
            return lineSpan.End == CurrentSnapshot.LineCount - 1;
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
                margin._classificationFormatMap.ClassificationFormatMappingChanged +=
                    margin.OnClassificationFormatMappingChanged;
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
            margin._classificationFormatMap.ClassificationFormatMappingChanged -=
                margin.OnClassificationFormatMappingChanged;
            margin.Children.Clear();
            margin.CurrentSnapshot = null;
            margin.Source = null;
            if (margin.Bitmap == null)
                return;
            margin.Bitmap.Clear();
            margin.Bitmap.Freeze();
            margin.Bitmap = null;
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
                    var optionValue = _textView.Options.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId);
                    if (_currentWordWrapWidth != _textView.FormattedLineSource.WordWrapWidth ||
                        _currentAutoIndent != _textView.FormattedLineSource.MaxAutoIndent)
                    {
                        foreach (LineTile child in Children)
                            child.IsDirty = true;
                        _currentWordWrapWidth = _textView.FormattedLineSource.WordWrapWidth;
                        _currentAutoIndent = _textView.FormattedLineSource.MaxAutoIndent;
                    }

                    Source = _factory.FormattedTextSourceFactoryService.Create(_textView.TextSnapshot, CurrentSnapshot,
                        _textView.Options.GetTabSize(), _textView.FormattedLineSource.BaseIndentation,
                        _textView.FormattedLineSource.WordWrapWidth, _textView.FormattedLineSource.MaxAutoIndent, false,
                        _classifier, _simpleSequencer, _classificationFormatMap,
                        (optionValue & (WordWrapStyles.WordWrap | WordWrapStyles.VisibleGlyphs)) ==
                        (WordWrapStyles.WordWrap | WordWrapStyles.VisibleGlyphs));
                }

                var num1 = 2000;
                var num2 = _currentWordWrapWidth;
                if (num2 == 0.0)
                    num2 = 800.0;
                var xScale = Math.Min(1.0, Math.Max(0.05, Width / num2));
                foreach (LineTile child in Children)
                    if (!child.IsFormatted)
                    {
                        num1 -= child.DoFormat(this, xScale);
                        if (num1 <= 0)
                        {
                            QueueFormat();
                            return;
                        }
                    }

                if (_reclassifiedSpans.Count > 0)
                {
                    NormalizedSpanCollection spans;
                    lock (_reclassifiedSpans)
                    {
                        var spanList = new List<Span>(_reclassifiedSpans.Count);
                        foreach (var reclassifiedSpan in _reclassifiedSpans)
                        {
                            var snapshotSpan =
                                reclassifiedSpan.TranslateTo(CurrentSnapshot, SpanTrackingMode.EdgeInclusive);
                            var snapshotPoint = snapshotSpan.Start;
                            var containingLine = snapshotPoint.GetContainingLine();
                            snapshotPoint = snapshotSpan.End;
                            ITextSnapshotLine textSnapshotLine1;
                            if (snapshotPoint.Position >= containingLine.EndIncludingLineBreak)
                            {
                                snapshotPoint = snapshotSpan.End;
                                textSnapshotLine1 = snapshotPoint.GetContainingLine();
                            }
                            else
                            {
                                textSnapshotLine1 = containingLine;
                            }

                            var textSnapshotLine2 = textSnapshotLine1;
                            spanList.Add(Span.FromBounds(containingLine.LineNumber, textSnapshotLine2.LineNumber + 1));
                        }

                        spans = new NormalizedSpanCollection(spanList);
                        _reclassifiedSpans.Clear();
                    }

                    var startingIndex = 0;
                    foreach (LineTile child in Children)
                        if (!child.IsDirty && SpanInvalidated(spans, child.LineSpan, ref startingIndex))
                            child.IsDirty = true;
                }

                foreach (LineTile child in Children)
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

        private bool EnsureSnapshot()
        {
            if (!_isActive || ScrollBar.TrackSpanHeight <= 0.0)
                return false;
            var textSnapshot = _textView.TextSnapshot;
            if (textSnapshot == CurrentSnapshot && ScrollBar.Map.End == _currentScrollMapEnd &&
                ScrollBar.TrackSpanHeight == _currentTrackSpanHeight)
                return false;
            _currentScrollMapEnd = ScrollBar.Map.End;
            _currentTrackSpanHeight = ScrollBar.TrackSpanHeight;
            if (CurrentSnapshot != textSnapshot)
            {
                if (CurrentSnapshot != null)
                {
                    IList<Span> spanList = new List<Span>();
                    for (var textVersion = CurrentSnapshot.Version;
                        textSnapshot.Version != textVersion;
                        textVersion = textVersion.Next)
                        foreach (var change in textVersion.Changes)
                        {
                            var span = textVersion.CreateTrackingSpan(change.OldSpan, SpanTrackingMode.EdgeInclusive)
                                .GetSpan(CurrentSnapshot);
                            var snapshotPoint = span.Start;
                            var containingLine = snapshotPoint.GetContainingLine();
                            snapshotPoint = span.End;
                            ITextSnapshotLine textSnapshotLine1;
                            if (snapshotPoint.Position >= containingLine.EndIncludingLineBreak)
                            {
                                snapshotPoint = span.End;
                                textSnapshotLine1 = snapshotPoint.GetContainingLine();
                            }
                            else
                            {
                                textSnapshotLine1 = containingLine;
                            }

                            var textSnapshotLine2 = textSnapshotLine1;
                            spanList.Add(Span.FromBounds(containingLine.LineNumber, textSnapshotLine2.LineNumber + 1));
                        }

                    var spans = new NormalizedSpanCollection(spanList);
                    var startingIndex = 0;
                    var startingLine = 0;
                    var index = 0;
                    while (index < Children.Count)
                    {
                        var child = (LineTile) Children[index];
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
                    var lineTile = new LineTile();
                    lineTile.SetLineSpan(0, textSnapshot.LineCount - 1);
                    Children.Add(lineTile);
                }

                CurrentSnapshot = textSnapshot;
                Source = null;
            }

            for (var index1 = 0; index1 < Children.Count; ++index1)
            {
                var child1 = (LineTile) Children[index1];
                var currentSnapshot1 = CurrentSnapshot;
                var lineSpan = child1.LineSpan;
                var start1 = lineSpan.Start;
                var lineFromLineNumber = currentSnapshot1.GetLineFromLineNumber(start1);
                lineSpan = child1.LineSpan;
                ITextSnapshotLine textSnapshotLine1;
                if (lineSpan.Length != 0)
                {
                    var currentSnapshot2 = CurrentSnapshot;
                    lineSpan = child1.LineSpan;
                    var end = lineSpan.End;
                    textSnapshotLine1 = currentSnapshot2.GetLineFromLineNumber(end);
                }
                else
                {
                    textSnapshotLine1 = lineFromLineNumber;
                }

                var textSnapshotLine2 = textSnapshotLine1;
                var ytop = GetYTop(lineFromLineNumber.Start);
                var num = GetYBottom(textSnapshotLine2.End) - ytop;
                if (num < 3.0 || num > 5.0)
                {
                    lineSpan = child1.LineSpan;
                    var end1 = lineSpan.End;
                    var positionOfYcoordinate = ScrollBar.GetBufferPositionOfYCoordinate(ytop + 4.0);
                    var lineNumber =
                        (positionOfYcoordinate.Position >= lineFromLineNumber.EndIncludingLineBreak.Position
                            ? positionOfYcoordinate.GetContainingLine()
                            : lineFromLineNumber).LineNumber;
                    if (lineNumber != end1)
                    {
                        var lineTile1 = child1;
                        lineSpan = child1.LineSpan;
                        var start2 = lineSpan.Start;
                        var end2 = lineNumber;
                        lineTile1.SetLineSpan(start2, end2);
                        var start3 = lineNumber + 1;
                        if (lineNumber > end1)
                        {
                            if (start3 == textSnapshot.LineCount)
                                Children.RemoveRange(index1 + 1, Children.Count - index1 - 1);
                            else
                                for (var index2 = index1 + 1; index2 < Children.Count; ++index2)
                                {
                                    var child2 = (LineTile) Children[index2];
                                    lineSpan = child2.LineSpan;
                                    var end3 = lineSpan.End;
                                    if (end3 > lineNumber)
                                    {
                                        Children.RemoveRange(index1 + 1, index2 - index1 - 1);
                                        child2.SetLineSpan(start3, end3);
                                        break;
                                    }
                                }
                        }
                        else if (index1 + 1 < Children.Count)
                        {
                            var child2 = (LineTile) Children[index1 + 1];
                            var lineTile2 = child2;
                            var start4 = start3;
                            lineSpan = child2.LineSpan;
                            var end3 = lineSpan.End;
                            lineTile2.SetLineSpan(start4, end3);
                        }
                        else
                        {
                            var lineTile2 = new LineTile();
                            Children.Add(lineTile2);
                            lineTile2.SetLineSpan(start3, textSnapshot.LineCount - 1);
                        }
                    }
                }

                child1.SetPosition(this);
            }

            return true;
        }

        private void OnClassificationChanged(object sender, ClassificationChangedEventArgs e)
        {
            lock (_reclassifiedSpans)
            {
                _reclassifiedSpans.Add(e.ChangeSpan);
            }
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            foreach (LineTile child in Children)
                child.IsDirty = true;
            QueueFormat();
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (Visibility != Visibility.Visible || !IsVisible || _textView.IsClosed)
                return;
            ActivateMargin(this);
            Source = null;
            QueueFormat();
        }

        private void OnMappingChanged(object sender, EventArgs e)
        {
            QueueFormat();
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
                var flag = Visibility == Visibility.Visible;
                Visibility = Visibility.Visible;
                var width1 = Width;
                Width = _textView.Options.GetOptionValue(DefaultTextViewHostOptions.SourceImageMarginWidthOptionId);
                var width2 = Width;
                if (width1 != width2 || !flag)
                    foreach (LineTile child in Children)
                        child.IsDirty = true;
                QueueFormat();
            }
        }

        private void QueueFormat()
        {
            if (_formatQueued || !_isActive || ScrollBar.TrackSpanHeight <= 0.0)
                return;
            _formatQueued = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(DoFormat));
        }
    }
}