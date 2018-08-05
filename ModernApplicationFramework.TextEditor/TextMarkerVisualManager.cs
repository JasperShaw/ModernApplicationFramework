using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    internal class TextMarkerVisualManager
    {
        internal double Left = double.MaxValue;
        internal double Right = double.MinValue;
        private readonly ITextView _textView;
        private readonly IAdornmentLayer _textMarkerLayer;
        private readonly IAdornmentLayer _negativeTextMarkerLayer;
        internal ITagAggregator<ITextMarkerTag> TagAggregator;
        private readonly IEditorFormatMap _editorFormatMap;

        internal TextMarkerVisualManager(ITextView textView, IViewTagAggregatorFactoryService tagAggregatorFactoryService, IEditorFormatMapService editorFormatMapService)
        {
            _textView = textView;
            TagAggregator = tagAggregatorFactoryService.CreateTagAggregator<ITextMarkerTag>(textView, (TagAggregatorOptions)2);
            _editorFormatMap = editorFormatMapService.GetEditorFormatMap(textView);
            _textView.LayoutChanged += OnLayoutChanged;
            _textView.Closed += OnTextViewClosed;
            TagAggregator.BatchedTagsChanged += OnBatchedTagsChanged;
            _textMarkerLayer = _textView.GetAdornmentLayer("TextMarker");
            _negativeTextMarkerLayer = _textView.GetAdornmentLayer("negativetextmarkerlayer");
            _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            Left = double.MaxValue;
            Right = double.MinValue;
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            if (TagAggregator == null)
                return;
            TagAggregator.BatchedTagsChanged -= OnBatchedTagsChanged;
            _textView.LayoutChanged -= OnLayoutChanged;
            _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
            _textView.Closed -= OnTextViewClosed;
            TagAggregator.Dispose();
            TagAggregator = null;
        }

        private void OnBatchedTagsChanged(object sender, BatchedTagsChangedEventArgs e)
        {
            if (_textView.IsClosed)
                return;
            var snapshotSpanList = new List<SnapshotSpan>();
            var formattedSpan = _textView.TextViewLines.FormattedSpan;
            foreach (var span1 in e.Spans)
            {
                foreach (var span2 in span1.GetSpans(_textView.TextSnapshot))
                {
                    var nullable = span2.Intersection(formattedSpan);
                    if (nullable.HasValue)
                        snapshotSpanList.Add(nullable.Value);
                }
            }
            if (snapshotSpanList.Count <= 0)
                return;
            foreach (var visualSpan in new NormalizedSnapshotSpanCollection(snapshotSpanList))
            {
                _textMarkerLayer.RemoveAdornmentsByVisualSpan(visualSpan);
                _negativeTextMarkerLayer.RemoveAdornmentsByVisualSpan(visualSpan);
            }
            AddAdornments(new NormalizedSnapshotSpanCollection(snapshotSpanList), _textView.ViewportLeft, _textView.ViewportRight);
        }

        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            AddAdornments(e.NewOrReformattedSpans, e.NewViewState.ViewportLeft, e.NewViewState.ViewportRight);
        }

        private void AddAdornments(NormalizedSnapshotSpanCollection spans, double left, double right)
        {
            if (left - 2.0 < Left || right + 2.0 > Right)
            {
                Left = left - 200.0;
                Right = right + 200.0;
                _textMarkerLayer.RemoveAllAdornments();
                _negativeTextMarkerLayer.RemoveAllAdornments();
                spans = new NormalizedSnapshotSpanCollection(_textView.TextViewLines.FormattedSpan);
            }
            if (spans.Count <= 0)
                return;
            IList<SnapshotSpan> snapshotSpanList = new List<SnapshotSpan>();
            foreach (var span in spans)
                MappingSpanSnapshot.MapUpToSnapshotNoTrack(_textView.VisualSnapshot, span, snapshotSpanList);
            var lineCache = new Dictionary<ITextViewLine, Span>();
            foreach (var tag in TagAggregator.GetTags(new NormalizedSnapshotSpanCollection(snapshotSpanList)))
                AddAdornment(tag, lineCache, spans, _textView.TextViewLines.FormattedSpan);
        }

        private void AddAdornment(IMappingTagSpan<ITextMarkerTag> tag, Dictionary<ITextViewLine, Span> lineCache, NormalizedSnapshotSpanCollection applicabilitySpans, SnapshotSpan formattedSpan)
        {
            foreach (var span in tag.Span.GetSpans(_textView.TextSnapshot))
            {
                var nullable = formattedSpan.Intersection(span);
                if (nullable.HasValue && (applicabilitySpans == null || applicabilitySpans.OverlapsWith(nullable.Value)))
                    AddAdornment(nullable.Value, span, tag, lineCache);
            }
        }

        private static SnapshotPoint GetBufferPositionFromXCoordinate(ITextViewLine line, double x)
        {
            if (x <= line.TextLeft)
                return line.Start;
            if (x >= line.TextRight)
                return line.EndIncludingLineBreak;
            return line.GetBufferPositionFromXCoordinate(x).Value;
        }

        private void AddAdornment(SnapshotSpan span, SnapshotSpan sourceSpan, IMappingTagSpan<ITextMarkerTag> tag, Dictionary<ITextViewLine, Span> lineCache)
        {
            var containingBufferPosition = _textView.TextViewLines.GetTextViewLineContainingBufferPosition(span.Start);
            var useTextBounds = containingBufferPosition != null && span.End.Position <= containingBufferPosition.EndIncludingLineBreak;
            if (useTextBounds)
            {
                if (!lineCache.TryGetValue(containingBufferPosition, out var span1))
                {
                    span1 = Span.FromBounds(GetBufferPositionFromXCoordinate(containingBufferPosition, Left - 1000.0), GetBufferPositionFromXCoordinate(containingBufferPosition, Right + 1000.0));
                    lineCache.Add(containingBufferPosition, span1);
                }
                if (!span1.OverlapsWith(span))
                    return;
            }
            var rectanglesFromBounds = Markers.GetRectanglesFromBounds(_textView.TextViewLines.GetNormalizedTextBounds(span), useTextBounds ? Markers.SingleLinePadding : Markers.MultiLinePadding, Left, Right, useTextBounds);
            if (rectanglesFromBounds.Count <= 0)
                return;
            var adornmentForGeometry = GetAdornmentForGeometry(Markers.GetMarkerGeometryFromRectangles(rectanglesFromBounds), tag);
            if (Panel.GetZIndex(adornmentForGeometry) >= 0)
                _textMarkerLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, sourceSpan, tag, adornmentForGeometry, null);
            else
                _negativeTextMarkerLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, sourceSpan, tag, adornmentForGeometry, null);
        }

        private UIElement GetAdornmentForGeometry(Geometry geometry, IMappingTagSpan<ITextMarkerTag> tag)
        {
            var properties = _editorFormatMap.GetProperties(tag.Tag.Type);
            Brush fillBrush = null;
            if (properties.Contains("BackgroundColor"))
            {
                if (properties["BackgroundColor"] is Color nullable)
                {
                    fillBrush = new SolidColorBrush(nullable) {Opacity = 0.8};
                }
            }
            else if (properties.Contains("MarkerFormatDefinition/FillId"))
                fillBrush = properties["MarkerFormatDefinition/FillId"] as Brush;
            if (fillBrush == null)
                fillBrush = new SolidColorBrush(Color.FromArgb(32, Colors.DarkGray.R, Colors.DarkGray.G, Colors.DarkGray.B));
            else if (SystemParameters.HighContrast)
            {
                fillBrush = fillBrush.Clone();
                fillBrush.Opacity = 0.5;
            }
            if (fillBrush.CanFreeze)
                fillBrush.Freeze();
            var flag = true;
            Brush brush = null;
            if (properties.Contains("ForegroundColor"))
            {
                if (properties["ForegroundColor"] is Color nullable)
                {
                    if (nullable.A != 0)
                    {
                        brush = new SolidColorBrush(nullable);
                        brush.Freeze();
                    }
                    else
                        flag = false;
                }
            }
            Pen borderPen = null;
            if (flag)
            {
                if (properties.Contains("MarkerFormatDefinition/BorderId"))
                {
                    borderPen = properties["MarkerFormatDefinition/BorderId"] as Pen;
                    if (borderPen != null)
                    {
                        borderPen = borderPen.Clone();
                        if (brush != null)
                            borderPen.Brush = brush;
                    }
                }
                else if (brush != null)
                    borderPen = new Pen(brush, 0.5);
            }
            if (borderPen != null && borderPen.CanFreeze)
                borderPen.Freeze();
            UIElement element = new GeometryAdornment(borderPen, fillBrush, geometry);
            var nullable1 = new int?();
            if (properties.Contains("MarkerFormatDefinition/ZOrderId"))
                nullable1 = properties["MarkerFormatDefinition/ZOrderId"] as int?;
            if (nullable1.HasValue)
                Panel.SetZIndex(element, nullable1.Value);
            return element;
        }
    }
}