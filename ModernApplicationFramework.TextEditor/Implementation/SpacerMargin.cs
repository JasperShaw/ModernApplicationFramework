using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.TextEditor.Text.Document;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal sealed class SpacerMargin : ITextViewMargin
    {
        internal bool IsDisposed;
        internal SpacerMarginElement spacerMarginElement;

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return spacerMarginElement;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return spacerMarginElement.ActualWidth;
            }
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return spacerMarginElement.Enabled;
            }
        }

        public SpacerMargin(ITextView textView, IViewTagAggregatorFactoryService tagAggregatorFactoryService, IEditorFormatMapService editorFormatMapService)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (tagAggregatorFactoryService == null)
                throw new ArgumentNullException(nameof(tagAggregatorFactoryService));
            spacerMarginElement = new SpacerMarginElement(textView, tagAggregatorFactoryService, editorFormatMapService.GetEditorFormatMap(textView));
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (string.Compare(marginName, "Spacer", StringComparison.OrdinalIgnoreCase) != 0)
                return null;
            return this;
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Spacer");
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            spacerMarginElement.Dispose();
            IsDisposed = true;
        }

        internal class SpacerMarginElement : FrameworkElement
        {
            private readonly ITextView _textView;
            private readonly IViewTagAggregatorFactoryService _tagAggregatorFactoryService;
            private readonly IEditorFormatMap _editorFormatMap;
            private ITagAggregator<ChangeTag> _changeTagAggregator;

            private readonly Brush[] _brushes = new Brush[4];

            internal bool TrackChanges
            {
                get => _changeTagAggregator != null;
                set
                {
                    if (value == TrackChanges)
                        return;
                    if (value)
                    {
                        _changeTagAggregator = _tagAggregatorFactoryService.CreateTagAggregator<ChangeTag>(_textView);
                        _changeTagAggregator.TagsChanged += OnTagsChanged;
                    }
                    else
                    {
                        _changeTagAggregator.TagsChanged -= OnTagsChanged;
                        _changeTagAggregator.Dispose();
                        _changeTagAggregator = null;
                    }
                    InvalidateVisual();
                }
            }

            public bool Enabled => _textView.Options.IsSelectionMarginEnabled();

            public SpacerMarginElement(ITextView textView,
                IViewTagAggregatorFactoryService tagAggregatorFactoryService, IEditorFormatMap editorFormatMap)
            {
                _textView = textView;
                _tagAggregatorFactoryService = tagAggregatorFactoryService;
                _editorFormatMap = editorFormatMap;
                _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
                _textView.Closed += (sender, args) => _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
                UpdateBrushes();
                ClipToBounds = true;
                Width = 10.0;
                OnOptionsChanged(null, null);
                _textView.Options.OptionChanged += OnOptionsChanged;
                IsVisibleChanged += (sender, e) =>
                {
                    if ((bool)e.NewValue)
                        _textView.LayoutChanged += OnLayoutChanged;
                    else
                        _textView.LayoutChanged -= OnLayoutChanged;
                };
                IsHitTestVisible = false;
            }

            public void Dispose()
            {
                TrackChanges = false;
                _textView.Options.OptionChanged -= OnOptionsChanged;
            }

            internal static bool ContainsChanges(ITextViewLine line, NormalizedSnapshotSpanCollection invalidSpans)
            {
                var index1 = 0;
                var num = invalidSpans.Count;
                SnapshotSpan invalidSpan;
                while (index1 < num)
                {
                    var index2 = (index1 + num) / 2;
                    int start = line.Start;
                    invalidSpan = invalidSpans[index2];
                    int end = invalidSpan.End;
                    if (start <= end)
                        num = index2;
                    else
                        index1 = index2 + 1;
                }
                if (index1 >= invalidSpans.Count)
                    return false;
                if (line.EndIncludingLineBreak != line.Snapshot.Length || line.LineBreakLength != 0)
                {
                    var includingLineBreak = line.EndIncludingLineBreak;
                    invalidSpan = invalidSpans[index1];
                    var start = invalidSpan.Start;
                    return includingLineBreak > start;
                }
                int includingLineBreak1 = line.EndIncludingLineBreak;
                invalidSpan = invalidSpans[index1];
                int start1 = invalidSpan.Start;
                return includingLineBreak1 >= start1;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                if (!TrackChanges || _textView.InLayout || _textView.IsClosed)
                    return;
                var unifiedChanges = ChangeBrushes.GetUnifiedChanges(_textView.TextSnapshot, _changeTagAggregator.GetTags(_textView.TextViewLines.FormattedSpan));
                foreach (var textViewLine in _textView.TextViewLines)
                {
                    var changeTypes = ChangeTypes.None;
                    if (ContainsChanges(textViewLine, unifiedChanges[3]))
                    {
                        changeTypes = ChangeTypes.ChangedSinceOpened | ChangeTypes.ChangedSinceSaved;
                    }
                    else
                    {
                        if (ContainsChanges(textViewLine, unifiedChanges[1]))
                            changeTypes = ChangeTypes.ChangedSinceOpened;
                        if (ContainsChanges(textViewLine, unifiedChanges[2]))
                            changeTypes |= ChangeTypes.ChangedSinceSaved;
                    }
                    if (changeTypes != ChangeTypes.None)
                        drawingContext.DrawRectangle(_brushes[(int)changeTypes], null, new Rect(2.5, textViewLine.Top - _textView.ViewportTop, 5.0, textViewLine.Height));
                }
            }

            private void OnTagsChanged(object sender, TagsChangedEventArgs e)
            {
                if (!IsVisible)
                    return;
                Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action) InvalidateVisual, null);
            }

            private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
            {
                if (!e.ChangedItems.Contains("Track Changes before save") && !e.ChangedItems.Contains("Track Changes after save") && !e.ChangedItems.Contains("Track reverted changes"))
                    return;
                UpdateBrushes();
            }

            private void OnOptionsChanged(object sender, EventArgs e)
            {
                TrackChanges = _textView.Options.IsChangeTrackingEnabled() && !_textView.IsClosed;
            }

            private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
            {
                if (!TrackChanges)
                    return;
                InvalidateVisual();
            }

            private void UpdateBrushes()
            {
                var properties1 = _editorFormatMap.GetProperties("Track Changes before save");
                if (properties1.Contains("BackgroundColor"))
                {
                    _brushes[3] = new SolidColorBrush((Color)properties1["BackgroundColor"]);
                    if (_brushes[3].CanFreeze)
                        _brushes[3].Freeze();
                }
                else if (properties1.Contains("Background"))
                {
                    _brushes[3] = (Brush)properties1["Background"];
                    if (_brushes[3].CanFreeze)
                        _brushes[3].Freeze();
                }
                var properties2 = _editorFormatMap.GetProperties("Track Changes after save");
                if (properties2.Contains("BackgroundColor"))
                {
                    _brushes[1] = new SolidColorBrush((Color)properties2["BackgroundColor"]);
                    if (_brushes[1].CanFreeze)
                        _brushes[1].Freeze();
                }
                else if (properties2.Contains("Background"))
                {
                    _brushes[1] = (Brush)properties2["Background"];
                    if (_brushes[1].CanFreeze)
                        _brushes[1].Freeze();
                }
                var properties3 = _editorFormatMap.GetProperties("Track reverted changes");
                if (properties3.Contains("BackgroundColor"))
                {
                    _brushes[2] = new SolidColorBrush((Color)properties3["BackgroundColor"]);
                    if (!_brushes[2].CanFreeze)
                        return;
                    _brushes[2].Freeze();
                }
                else
                {
                    if (!properties3.Contains("Background"))
                        return;
                    _brushes[2] = (Brush)properties3["Background"];
                    if (!_brushes[2].CanFreeze)
                        return;
                    _brushes[2].Freeze();
                }
            }
        }
    }
}