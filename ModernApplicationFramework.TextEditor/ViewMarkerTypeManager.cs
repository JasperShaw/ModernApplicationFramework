using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    internal class ViewMarkerTypeManager
    {
        internal readonly List<int> IgnoredMarkerTypes = new List<int>();
        internal readonly List<int> ViewOnlyMarkerTypes = new List<int>();
        private ITagAggregator<VsTextMarkerTag> _tagAggregator;
        private IEditorFormatMap _editorFormatMap;
        internal IAdornmentLayer Layer;
        private bool _updateVisibleMarkersOnNextLayout;
        private bool _useReducedOpacityForHighContrast;

        public void AttachToView(ITextView view)
        {
            if (View != null)
                OnClosed(null, null);
            View = view;
            View.Properties.AddProperty(typeof(ViewMarkerTypeManager), this);
            _tagAggregator = EditorParts.ViewTagAggregatorFactoryService.CreateTagAggregator<VsTextMarkerTag>(View);
            _editorFormatMap = EditorParts.EditorFormatMapService.GetEditorFormatMap(View);
            Layer = View.GetAdornmentLayer("TextMarker");
            _tagAggregator.BatchedTagsChanged += OnBatchedTagsChanged;
            View.LayoutChanged += OnViewLayoutChanged;
            _editorFormatMap.FormatMappingChanged += OnFormatMapChanged;
            _useReducedOpacityForHighContrast = View.Options.GetOptionValue(DefaultViewOptions.UseReducedOpacityForHighContrastOptionId);
            View.Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs args)
        {
            _editorFormatMap.FormatMappingChanged -= OnFormatMapChanged;
            if (_tagAggregator != null)
            {
                _tagAggregator.BatchedTagsChanged -= OnBatchedTagsChanged;
                _tagAggregator.Dispose();
                _tagAggregator = null;
            }
            if (View == null)
                return;
            View.LayoutChanged -= OnViewLayoutChanged;
            View.Closed -= OnClosed;
            View = null;
        }

        private void OnBatchedTagsChanged(object sender, BatchedTagsChangedEventArgs e)
        {
            if (View == null || View.IsClosed)
                return;
            var snapshotSpanList = new List<SnapshotSpan>();
            foreach (var span in e.Spans)
                snapshotSpanList.AddRange(span.GetSpans(View.TextSnapshot));
            DrawMarkersOnSpans(new NormalizedSnapshotSpanCollection(snapshotSpanList));
        }

        private void OnViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var spansInEditBuffer = _updateVisibleMarkersOnNextLayout ? new NormalizedSnapshotSpanCollection(View.TextViewLines.FormattedSpan) : e.NewOrReformattedSpans;
            _updateVisibleMarkersOnNextLayout = false;
            DrawMarkersOnSpans(spansInEditBuffer);
        }

        private void OnFormatMapChanged(object sender, FormatItemsEventArgs e)
        {
            _updateVisibleMarkersOnNextLayout = true;
        }

        public bool IsMarkerVisibleInView(VsTextMarkerAdapter marker)
        {
            if (View == null || View.Roles.Contains("EMBEDDED_PEEK_TEXT_VIEW") && IgnoreMarkerInEmbeddedView(marker) || IgnoredMarkerTypes.Contains(marker.Type))
                return false;
            if (((int)marker.Behavior & 128) != 0)
                return ViewOnlyMarkerTypes.Contains(marker.Type);
            return true;
        }

        private static bool IgnoreMarkerInEmbeddedView(VsTextMarkerAdapter marker)
        {
            return MarkerManager.OverviewTaggerImplementation.MapMarkerToOverviewMarkType(marker.MarkerType) != null;
        }

        public ITextView View { get; private set; }

        public bool VisibleGlyphMargin => View.Options.GetOptionValue(DefaultTextViewHostOptions.GlyphMarginId);

        public bool ShowColorForMarker(VsTextMarkerAdapter marker)
        {
            if (((int)marker.VisualStyle & 4) != 0)
                return !VisibleGlyphMargin;
            return (marker.VisualStyle & 2U) > 0U;
        }

        public void SetIgnoredMarkerTypes(int iCountMarkerTypes, uint[] rgIgnoreMarkerTypes)
        {
            IgnoredMarkerTypes.Clear();
            for (var index = 0; index < iCountMarkerTypes; ++index)
                IgnoredMarkerTypes.Add((int)rgIgnoreMarkerTypes[index]);
            UpdateVisibleMarkers();
        }

        public void AppendViewOnlyMarkerTypes(uint iCountViewMarkerOnly, uint[] rgViewMarkerOnly)
        {
            for (var index = 0; index < iCountViewMarkerOnly; ++index)
            {
                var num = (int)rgViewMarkerOnly[index];
                if (!ViewOnlyMarkerTypes.Contains(num))
                    ViewOnlyMarkerTypes.Add(num);
            }
            UpdateVisibleMarkers();
        }

        public void RemoveViewOnlyMarkerTypes(uint iCountViewMarkerOnly, uint[] rgViewMarkerOnly)
        {
            for (var index = 0; index < iCountViewMarkerOnly; ++index)
                ViewOnlyMarkerTypes.Remove((int)rgViewMarkerOnly[index]);
            UpdateVisibleMarkers();
        }

        private void AddExclusiveAdornment(SnapshotSpan markerSpan, VsTextMarkerAdapter marker)
        {
            if (markerSpan.Length == 0)
                return;
            var markerGeometry = View.TextViewLines.GetMarkerGeometry(markerSpan);
            if (markerGeometry == null)
                return;
            var markerBackgroundColor = GetMarkerBackgroundColor(marker.MarkerType.MergeName);
            if (!markerBackgroundColor.HasValue)
                return;
            Brush brush1 = new SolidColorBrush(markerBackgroundColor.Value);
            brush1.Opacity = !_useReducedOpacityForHighContrast || !SystemParameters.HighContrast ? 0.8 : 0.5;
            brush1.Freeze();
            Brush brush2 = null;
            Pen pen = null;
            if (ShowColorForMarker(marker) && ((int)marker.Behavior & 64) == 0)
                brush2 = brush1;
            if (pen == null && brush2 == null)
                return;
            markerGeometry.Freeze();
            var geometryDrawing = new GeometryDrawing(brush2, pen, markerGeometry);
            geometryDrawing.Freeze();
            var drawingImage = new DrawingImage(geometryDrawing);
            drawingImage.Freeze();
            var image = new Image {Source = drawingImage};
            Canvas.SetLeft(image, markerGeometry.Bounds.Left);
            Canvas.SetTop(image, markerGeometry.Bounds.Top);
            Layer.AddAdornment(markerSpan, null, image);
        }

        private void AddNonExclusiveAdornment(SnapshotSpan markerSpan, VsTextMarkerAdapter marker)
        {
            Geometry geometry = null;
            var nullable = new Color?();
            var lineStyle = marker.LineStyle;
            if (lineStyle >= (Linestyle.LiSolid | Linestyle.LiDotted))
            {
                SnapshotPoint bufferPosition;
                if (lineStyle == (Linestyle.LiSolid | Linestyle.LiDotted))
                {
                    bufferPosition = markerSpan.Start;
                    nullable = Colors.Blue;
                }
                else
                {
                    bufferPosition = markerSpan.End - 1;
                    nullable = Colors.DarkRed;
                }
                var characterBounds = View.TextViewLines.GetCharacterBounds(bufferPosition);
                geometry = new RectangleGeometry(new Rect(characterBounds.Left, characterBounds.TextBottom - 2.0, characterBounds.Width, 2.0));
            }
            else if (((int)marker.VisualStyle & 256) != 0 && lineStyle != Linestyle.LiSquiggly)
            {
                if (((int)marker.VisualStyle & 4194304) != 0)
                {
                    var containingBufferPosition = View.TextViewLines.GetTextViewLineContainingBufferPosition(markerSpan.IsEmpty ? markerSpan.Start : markerSpan.End - 1);
                    geometry = new LineGeometry(new Point(containingBufferPosition.TextLeft, containingBufferPosition.TextBottom), new Point(View.ViewportRight, containingBufferPosition.TextBottom));
                }
                else
                    geometry = View.TextViewLines.GetMarkerGeometry(markerSpan);
                nullable = GetMarkerForegroundColor(marker.MarkerType.MergeName);
            }
            if (geometry == null)
                return;
            geometry.Freeze();
            if (!nullable.HasValue)
                return;
            var solidColorBrush = new SolidColorBrush(nullable.Value);
            solidColorBrush.Freeze();
            var pen = new Pen(solidColorBrush, 1.0);
            if (lineStyle == Linestyle.LiDotted)
            {
                pen.DashCap = PenLineCap.Round;
                pen.DashStyle = new DashStyle(new List<double>(2)
                {
                    0.0,
                    2.0
                }, 0.0);
                pen.EndLineCap = PenLineCap.Round;
            }
            pen.Freeze();
            var geometryDrawing = new GeometryDrawing(null, pen, geometry);
            geometryDrawing.Freeze();
            var drawingImage = new DrawingImage(geometryDrawing);
            drawingImage.Freeze();
            var image = new Image {Source = drawingImage};
            Canvas.SetLeft(image, geometry.Bounds.Left);
            Canvas.SetTop(image, geometry.Bounds.Top);
            Layer.AddAdornment(markerSpan, null, image);
        }

        private void UpdateVisibleMarkers()
        {
            if (View == null)
                return;
            if (View.TextSnapshot == View.TextBuffer.CurrentSnapshot && !View.InLayout && View.VisualElement.Dispatcher.CheckAccess())
                DrawMarkersOnSpans(new NormalizedSnapshotSpanCollection(View.TextViewLines.FormattedSpan));
            else
                _updateVisibleMarkersOnNextLayout = true;
        }

        private void DrawMarkersOnSpans(NormalizedSnapshotSpanCollection spansInEditBuffer)
        {
            if (spansInEditBuffer.Count <= 0)
                return;
            var snapshotSpan1 = new SnapshotSpan(View.TextViewModel.GetNearestPointInVisualBuffer(View.TextViewLines.FormattedSpan.Start).TranslateTo(View.VisualSnapshot, PointTrackingMode.Negative), View.TextViewModel.GetNearestPointInVisualBuffer(View.TextViewLines.FormattedSpan.End).TranslateTo(View.VisualSnapshot, PointTrackingMode.Positive));
            var snapshotSpanList = (from snapshotSpan2 in spansInEditBuffer
                select new SnapshotSpan(
                    View.TextViewModel.GetNearestPointInVisualBuffer(snapshotSpan2.Start)
                        .TranslateTo(View.VisualSnapshot, PointTrackingMode.Negative),
                    View.TextViewModel.GetNearestPointInVisualBuffer(snapshotSpan2.End)
                        .TranslateTo(View.VisualSnapshot, PointTrackingMode.Positive))
                into snapshotSpan3
                select snapshotSpan1.Overlap(snapshotSpan3)
                into nullable
                where nullable.HasValue
                select nullable.Value).ToList();
            var snapshotSpanCollection1 = new NormalizedSnapshotSpanCollection(snapshotSpanList);
            if (snapshotSpanCollection1.Count <= 0)
                return;
            IList<Tuple<Span, VsTextMarkerAdapter>> unsortedMarkers;
            IList<Tuple<Span, VsTextMarkerAdapter>> tupleList;
            while (true)
            {
                unsortedMarkers = new List<Tuple<Span, VsTextMarkerAdapter>>();
                tupleList = new List<Tuple<Span, VsTextMarkerAdapter>>();
                IList<SnapshotSpan> snapshotSpans = new List<SnapshotSpan>();
                foreach (var tag in _tagAggregator.GetTags(snapshotSpanCollection1))
                {
                    if (tag.Tag.TextMarker.IsVisible && IsMarkerVisibleInView(tag.Tag.TextMarker))
                    {
                        foreach (var span in tag.Span.GetSpans(View.VisualSnapshot))
                        {
                            var nullable = snapshotSpan1.Overlap(span);
                            if (nullable.HasValue)
                            {
                                snapshotSpans.Add(nullable.Value);
                                var tuple = new Tuple<Span, VsTextMarkerAdapter>(nullable.Value.Span, tag.Tag.TextMarker);
                                if (tag.Tag.TextMarker.IsExclusive(this))
                                    unsortedMarkers.Add(tuple);
                                else
                                    tupleList.Add(tuple);
                            }
                        }
                    }
                }
                var snapshotSpanCollection2 = NormalizedSnapshotSpanCollection.Union(snapshotSpanCollection1, new NormalizedSnapshotSpanCollection(snapshotSpans));
                if (!(snapshotSpanCollection2 == snapshotSpanCollection1))
                    snapshotSpanCollection1 = snapshotSpanCollection2;
                else
                    break;
            }
            foreach (var span in snapshotSpanCollection1)
            {
                foreach (var visualSpan in View.BufferGraph.MapDownToSnapshot(span, SpanTrackingMode.EdgeExclusive, View.TextSnapshot))
                    Layer.RemoveAdornmentsByVisualSpan(visualSpan);
            }
            foreach (var prioritizedMarkerSpan in MarkerManager.GetPrioritizedMarkerSpans(unsortedMarkers))
            {
                foreach (var markerSpan in View.BufferGraph.MapDownToSnapshot(new SnapshotSpan(View.VisualSnapshot, prioritizedMarkerSpan.Item1), SpanTrackingMode.EdgeExclusive, View.TextSnapshot))
                    AddExclusiveAdornment(markerSpan, prioritizedMarkerSpan.Item2);
            }
            foreach (var tuple in tupleList)
            {
                foreach (var markerSpan in View.BufferGraph.MapDownToSnapshot(new SnapshotSpan(View.VisualSnapshot, tuple.Item1), SpanTrackingMode.EdgeExclusive, View.TextSnapshot))
                    AddNonExclusiveAdornment(markerSpan, tuple.Item2);
            }
        }

        private Color? GetMarkerForegroundColor(string canonicalName)
        {
            var properties = _editorFormatMap.GetProperties(canonicalName);
            var nullable = new Color?();
            if (properties.Contains("ForegroundColor"))
                nullable = properties["ForegroundColor"] as Color?;
            if (!nullable.HasValue && properties.Contains("Foreground"))
            {
                if (properties["Foreground"] is SolidColorBrush solidColorBrush)
                    nullable = solidColorBrush.Color;
            }
            return nullable;
        }

        private Color? GetMarkerBackgroundColor(string canonicalName)
        {
            var properties = _editorFormatMap.GetProperties(canonicalName);
            var nullable = new Color?();
            if (properties.Contains("BackgroundColor"))
                nullable = properties["BackgroundColor"] as Color?;
            if (!nullable.HasValue && properties.Contains("Background"))
            {
                if (properties["Background"] is SolidColorBrush solidColorBrush)
                    nullable = solidColorBrush.Color;
            }
            return nullable;
        }
    }
}