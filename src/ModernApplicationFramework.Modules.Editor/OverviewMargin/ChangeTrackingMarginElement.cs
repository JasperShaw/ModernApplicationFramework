using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Document;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal class ChangeTrackingMarginElement : FrameworkElement
    {
        internal ITagAggregator<ChangeTag> ChangeTagAggregator;
        private readonly Brush[] _brushes = new Brush[4];
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly IVerticalScrollBar _scrollBar;
        private readonly IViewTagAggregatorFactoryService _tagAggregatorFactoryService;
        private readonly ITextView _textView;

        public bool Enabled =>
            _textView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowChangeTrackingMarginOptionId) &&
            _textView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowScrollBarAnnotationsOptionId);

        internal bool TrackChanges
        {
            get => ChangeTagAggregator != null;
            set
            {
                if (value == TrackChanges)
                    return;
                if (value)
                {
                    ChangeTagAggregator = _tagAggregatorFactoryService.CreateTagAggregator<ChangeTag>(_textView);
                    ChangeTagAggregator.TagsChanged += OnTagsChanged;
                }
                else
                {
                    ChangeTagAggregator.TagsChanged -= OnTagsChanged;
                    ChangeTagAggregator.Dispose();
                    ChangeTagAggregator = null;
                }

                InvalidateVisual();
            }
        }

        public ChangeTrackingMarginElement(ITextView textView, IVerticalScrollBar verticalScrollbar,
            OverviewChangeTrackingMarginProvider provider)
        {
            _textView = textView;
            _scrollBar = verticalScrollbar;
            IsHitTestVisible = false;
            Focusable = false;
            _editorFormatMap = provider.EditorFormatMapService.GetEditorFormatMap(textView);
            _tagAggregatorFactoryService = provider.TagAggregatorFactoryService;
            _textView.Options.OptionChanged += OnEditorOptionChanged;
            OnEditorOptionChanged(null, null);
            IsVisibleChanged += (sender, e) =>
            {
                if ((bool) e.NewValue)
                {
                    _scrollBar.Map.MappingChanged += OnMappingChanged;
                    _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
                    _textView.LayoutChanged += OnLayoutChanged;
                    UpdateBrushes();
                    InvalidateVisual();
                }
                else
                {
                    _scrollBar.Map.MappingChanged -= OnMappingChanged;
                    _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
                    _textView.LayoutChanged -= OnLayoutChanged;
                }
            };
        }

        public void Dispose()
        {
            _textView.Options.OptionChanged -= OnEditorOptionChanged;
            TrackChanges = false;
        }

        internal static NormalizedSnapshotSpanCollection[] GetUnifiedChanges(ITextSnapshot snapshot,
            IEnumerable<IMappingTagSpan<ChangeTag>> tags)
        {
            var snapshotSpanListArray = new[]
            {
                null,
                new List<SnapshotSpan>(),
                new List<SnapshotSpan>(),
                new List<SnapshotSpan>()
            };
            foreach (var tag in tags)
                snapshotSpanListArray[(int) tag.Tag.ChangeTypes].AddRange(tag.Span.GetSpans(snapshot));
            var snapshotSpanCollectionArray = new NormalizedSnapshotSpanCollection[4];
            for (var index = 1; index <= 3; ++index)
                snapshotSpanCollectionArray[index] = new NormalizedSnapshotSpanCollection(snapshotSpanListArray[index]);
            return snapshotSpanCollectionArray;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (ChangeTagAggregator == null)
                return;
            var unifiedChanges = GetUnifiedChanges(_textView.TextSnapshot,
                ChangeTagAggregator.GetTags(new SnapshotSpan(_textView.TextSnapshot, 0,
                    _textView.TextSnapshot.Length)));
            DrawChange(drawingContext, ChangeTypes.ChangedSinceOpened, unifiedChanges);
            DrawChange(drawingContext, ChangeTypes.ChangedSinceSaved, unifiedChanges);
            DrawChange(drawingContext, ChangeTypes.ChangedSinceOpened | ChangeTypes.ChangedSinceSaved, unifiedChanges);
        }

        private static bool AnyTextChanges(ITextVersion oldVersion, ITextVersion currentVersion)
        {
            for (; oldVersion != currentVersion; oldVersion = oldVersion.Next)
                if (oldVersion.Changes.Count > 0)
                    return true;
            return false;
        }

        private void DrawChange(DrawingContext drawingContext, ChangeTypes type,
            IReadOnlyList<NormalizedSnapshotSpanCollection> allChanges)
        {
            var allChange = allChanges[(int) type];
            if (allChange.Count <= 0)
                return;
            MapSpanToPixels(allChange[0], out var yTop1, out var yBottom1);
            for (var index = 1; index < allChange.Count; ++index)
            {
                MapSpanToPixels(allChange[index], out var yTop2, out var yBottom2);
                if (yBottom1 < yTop2 - 2.0)
                {
                    drawingContext.DrawRectangle(_brushes[(int) type], null,
                        new Rect(1.0, yTop1, Width - 1.0, yBottom1 - yTop1));
                    yTop1 = yTop2;
                }

                yBottom1 = yBottom2;
            }

            drawingContext.DrawRectangle(_brushes[(int) type], null,
                new Rect(1.0, yTop1, Width - 1.0, yBottom1 - yTop1));
        }

        private void MapSpanToPixels(SnapshotSpan span, out double yTop, out double yBottom)
        {
            var scrollMapPosition1 = _scrollBar.Map.GetCoordinateAtBufferPosition(span.Start) - 0.5;
            var scrollMapPosition2 = _scrollBar.Map.GetCoordinateAtBufferPosition(span.End) + 0.5;
            yTop = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(scrollMapPosition1)) - 2.0;
            yBottom = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(scrollMapPosition2)) + 2.0;
        }

        private void OnEditorOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            TrackChanges = _textView.Options.IsChangeTrackingEnabled() && Enabled && !_textView.IsClosed;
            Width = _textView.Options.GetOptionValue(DefaultTextViewHostOptions.ChangeTrackingMarginWidthOptionId);
            Visibility = Enabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (!e.ChangedItems.Contains("Track Changes before save") &&
                !e.ChangedItems.Contains("Track Changes after save") &&
                !e.ChangedItems.Contains("Track reverted changes"))
                return;
            UpdateBrushes();
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (!AnyTextChanges(e.OldViewState.EditSnapshot.Version, e.NewViewState.EditSnapshot.Version))
                return;
            InvalidateVisual();
        }

        private void OnMappingChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private void OnTagsChanged(object sender, TagsChangedEventArgs e)
        {
            if (!IsVisible)
                return;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action) InvalidateVisual, null);
        }

        private void UpdateBrushes()
        {
            var properties1 = _editorFormatMap.GetProperties("Track Changes before save");
            if (properties1.Contains("Background"))
                _brushes[3] = (Brush) properties1["Background"];
            var properties2 = _editorFormatMap.GetProperties("Track Changes after save");
            if (properties2.Contains("Background"))
                _brushes[1] = (Brush) properties2["Background"];
            var properties3 = _editorFormatMap.GetProperties("Track reverted changes");
            if (!properties3.Contains("Background"))
                return;
            _brushes[2] = (Brush) properties3["Background"];
        }
    }
}