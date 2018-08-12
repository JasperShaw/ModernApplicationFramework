using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal abstract class BaseMarginElement : FrameworkElement
    {
        internal const int MaxTags = 5000;
        internal readonly string MarginName;
        protected readonly Dictionary<string, Brush> CachedBrushes = new Dictionary<string, Brush>();
        protected readonly IEditorFormatMap EditorFormatMap;
        protected readonly string FormatMapKey;
        protected readonly EditorOptionKey<bool> MarginEnabledKey;
        protected readonly EditorOptionKey<double> MarginWidthKey;
        protected readonly IVerticalScrollBar ScrollBar;
        protected readonly ITextView TextView;
        private readonly List<string> _orderedErrorTypes;
        private List<MarkData> _marks;
        private bool _updateQueued;

        internal bool Enabled => TextView.Options.GetOptionValue(MarginEnabledKey) &&
                                 TextView.Options.GetOptionValue(DefaultTextViewHostOptions
                                     .ShowScrollBarAnnotationsOptionId);

        protected BaseMarginElement(ITextView textView, IVerticalScrollBar scrollbar,
            EditorOptionKey<bool> marginEnabledKey, EditorOptionKey<double> marginWidthKey, string marginName,
            string formatMapKey, MarginProvider provider, List<string> orderedErrorTypes)
        {
            TextView = textView;
            ScrollBar = scrollbar;
            MarginEnabledKey = marginEnabledKey;
            MarginWidthKey = marginWidthKey;
            MarginName = marginName;
            FormatMapKey = formatMapKey;
            IsHitTestVisible = false;
            Focusable = false;
            EditorFormatMap = provider.EditorFormatMapService.GetEditorFormatMap(TextView);
            _orderedErrorTypes = orderedErrorTypes;
        }

        public static Span Extend(ITextSnapshot snapshot, Span span)
        {
            var lineFromPosition1 = snapshot.GetLineFromPosition(span.Start);
            if (span.End <= lineFromPosition1.EndIncludingLineBreak.Position)
                return lineFromPosition1.ExtentIncludingLineBreak;
            var lineFromPosition2 = snapshot.GetLineFromPosition(span.End);
            return Span.FromBounds(lineFromPosition1.Start,
                lineFromPosition2.Start.Position == span.End
                    ? lineFromPosition2.Start
                    : lineFromPosition2.EndIncludingLineBreak);
        }

        public abstract void Dispose();

        internal void ClearMarks()
        {
            _marks = null;
        }

        internal IList<Tuple<string, NormalizedSnapshotSpanCollection, int>> GetMarks()
        {
            var marks = _marks;
            if (marks == null)
                return new List<Tuple<string, NormalizedSnapshotSpanCollection, int>>(0);
            var tupleList = new List<Tuple<string, NormalizedSnapshotSpanCollection, int>>(marks.Count);
            foreach (var markData in marks)
                tupleList.Add(new Tuple<string, NormalizedSnapshotSpanCollection, int>(markData.Name, markData.Spans,
                    markData.Priority));
            return tupleList;
        }

        internal void InvalidateMarks()
        {
            if (_updateQueued)
                return;
            _updateQueued = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(UpdateMarks));
        }

        internal void UpdateMarks()
        {
            _updateQueued = false;
            var markDataList = new List<MarkData>();
            if (IsVisible)
            {
                var dictionary = new Dictionary<string, List<SnapshotSpan>>();
                var textSnapshot = TextView.TextSnapshot;
                var num1 = 0;
                foreach (var tuple in GetMarksFromTagger(new SnapshotSpan(textSnapshot, 0, textSnapshot.Length)))
                {
                    if (!dictionary.TryGetValue(tuple.Item1, out var snapshotSpanList))
                    {
                        snapshotSpanList = new List<SnapshotSpan>();
                        dictionary.Add(tuple.Item1, snapshotSpanList);
                    }

                    foreach (var span in tuple.Item2.GetSpans(textSnapshot))
                        snapshotSpanList.Add(new SnapshotSpan(span.Start, 0));
                    if (++num1 > 5000)
                        break;
                }

                foreach (var keyValuePair in dictionary)
                    if (keyValuePair.Value.Count > 0)
                    {
                        var num2 = _orderedErrorTypes.IndexOf(keyValuePair.Key);
                        markDataList.Add(new MarkData(keyValuePair.Key, num2 == -1 ? int.MaxValue : num2,
                            new NormalizedSnapshotSpanCollection(keyValuePair.Value)));
                    }

                markDataList.Sort(CompareMarks);
            }

            _marks = markDataList;
            InvalidateVisual();
        }

        protected abstract IEnumerable<Tuple<string, IMappingSpan>> GetMarksFromTagger(SnapshotSpan span);

        protected void OnBatchedTagsChanged(object sender, BatchedTagsChangedEventArgs e)
        {
            InvalidateMarks();
        }

        protected void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            CachedBrushes.Clear();
            InvalidateVisual();
        }

        protected void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var marks = _marks;
            if (marks == null || marks.Count <= 0)
                return;
            var textSnapshot = TextView.TextSnapshot;
            var snapshot = marks[0].Spans[0].Snapshot;
            if (snapshot == textSnapshot)
                return;
            var spanList = new List<Span>();
            for (var textVersion = snapshot.Version;
                textVersion != textSnapshot.Version;
                textVersion = textVersion.Next)
                spanList.AddRange(textVersion.Changes.Select(change => Extend(textSnapshot,
                    Tracking.TrackSpanForwardInTime(SpanTrackingMode.EdgeExclusive, change.NewSpan, textVersion.Next,
                        textSnapshot.Version))));
            var normalizedSpanCollection = new NormalizedSpanCollection(spanList);
            var flag = false;
            var markDataList = new List<MarkData>(marks.Count);
            foreach (var markData in marks)
            {
                var snapshotSpanList = new List<SnapshotSpan>(markData.Spans.Count);
                foreach (var span in markData.Spans)
                {
                    var snapshotSpan = span.TranslateTo(TextView.TextSnapshot, SpanTrackingMode.EdgeExclusive);
                    snapshotSpanList.Add(snapshotSpan);
                }

                if (snapshotSpanList.Count > 0)
                {
                    var spans = new NormalizedSnapshotSpanCollection(snapshotSpanList);
                    flag = flag || normalizedSpanCollection.IntersectsWith(spans);
                    markDataList.Add(new MarkData(markData.Name, markData.Priority, spans));
                }
            }

            _marks = markDataList;
            if (!flag)
                return;
            InvalidateMarks();
        }

        protected void OnMappingChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected void OnOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            Width = TextView.Options.GetOptionValue(MarginWidthKey);
            Visibility = Enabled ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var marks = _marks;
            if (marks == null || marks.Count <= 0)
                return;
            foreach (var markData in marks)
                DrawMark(drawingContext, 1.0, Width - 1.0, GetBrush(markData.Name, FormatMapKey), markData.Spans);
        }

        private static int CompareMarks(MarkData left, MarkData right)
        {
            if (right.Priority != left.Priority || right.Priority != int.MaxValue)
                return right.Priority - left.Priority;
            return string.Compare(left.Name, right.Name);
        }

        private void DrawMark(DrawingContext drawingContext, double left, double width, Brush brush,
            NormalizedSnapshotSpanCollection spans)
        {
            if (brush == null)
                return;
            var y = double.MaxValue;
            var num1 = double.MinValue;
            foreach (var span in spans)
            {
                var num2 = Math.Round(ScrollBar.GetYCoordinateOfBufferPosition(span
                               .TranslateTo(TextView.TextSnapshot, SpanTrackingMode.EdgeExclusive).Start)) - 3.0;
                var num3 = num2 + 6.0;
                if (num2 <= num1 + 1.0)
                {
                    num1 = num3;
                }
                else
                {
                    if (num1 != double.MinValue)
                        drawingContext.DrawRectangle(brush, null, new Rect(left, y, width, num1 - y));
                    y = num2;
                    num1 = num3;
                }
            }

            if (num1 == double.MinValue)
                return;
            drawingContext.DrawRectangle(brush, null, new Rect(left, y, width, num1 - y));
        }

        private Brush GetBrush(string type, string key)
        {
            if (!CachedBrushes.TryGetValue(type, out var brush))
            {
                var properties = EditorFormatMap.GetProperties(type);
                brush = !properties.Contains(key) ? null : properties[key] as Brush;
                CachedBrushes.Add(type, brush);
            }

            return brush;
        }

        private class MarkData
        {
            public readonly string Name;
            public readonly int Priority;
            public readonly NormalizedSnapshotSpanCollection Spans;

            public MarkData(string name, int priority, NormalizedSnapshotSpanCollection spans)
            {
                Name = name;
                Priority = priority;
                Spans = spans;
            }
        }
    }

    internal abstract class BaseMarginElement<T> : BaseMarginElement where T : ITag
    {
        protected ITagAggregator<T> Aggregator { get; }

        protected BaseMarginElement(ITextView textView, IVerticalScrollBar scrollbar,
            EditorOptionKey<bool> marginEnabledKey, EditorOptionKey<double> marginWidthKey, string marginName,
            string formatMapKey, MarginProvider provider, List<string> orderedErrorTypes)
            : base(textView, scrollbar, marginEnabledKey, marginWidthKey, marginName, formatMapKey, provider,
                orderedErrorTypes)
        {
            Aggregator =
                provider.ViewTagAggregatorFactoryService.CreateTagAggregator<T>(textView, (TagAggregatorOptions) 2);
            IsVisibleChanged += (sender, e) =>
            {
                if ((bool) e.NewValue)
                {
                    ScrollBar.Map.MappingChanged += OnMappingChanged;
                    EditorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
                    TextView.LayoutChanged += OnLayoutChanged;
                    CachedBrushes.Clear();
                    Aggregator.BatchedTagsChanged += OnBatchedTagsChanged;
                    InvalidateMarks();
                }
                else
                {
                    ScrollBar.Map.MappingChanged -= OnMappingChanged;
                    EditorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
                    TextView.LayoutChanged -= OnLayoutChanged;
                    Aggregator.BatchedTagsChanged -= OnBatchedTagsChanged;
                    ClearMarks();
                }
            };
            TextView.Options.OptionChanged += OnOptionChanged;
            OnOptionChanged(null, null);
        }

        public override void Dispose()
        {
            Aggregator.Dispose();
            TextView.Options.OptionChanged -= OnOptionChanged;
        }
    }
}