using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal sealed class SolidColorBrushSelectionPainter : BrushSelectionPainter, ISelectionPainter
    {
        internal Dictionary<object, SelectionData> LineSpans = new Dictionary<object, SelectionData>();
        private readonly bool _isInContrastMode;
        private VirtualSnapshotSpan _oldStreamSelection;

        internal Brush Brush { get; }

        public SolidColorBrushSelectionPainter(ITextSelection selection, IAdornmentLayer adornmentLayer, Brush brush)
            : base(adornmentLayer, selection)
        {
            Brush = brush;
            _isInContrastMode = selection.TextView.Options.GetOptionValue<bool>("TextViewHost/IsInContrastMode");
        }

        public void Activate()
        {
            AdornmentLayer.Opacity = _isInContrastMode || SystemParameters.HighContrast ? 1.0 : 0.4;
            if (TextSelection.TextView.InLayout || TextSelection.TextView.TextViewLines == null)
                return;
            Update(true);
        }

        public void Clear()
        {
            LineSpans.Clear();
            _oldStreamSelection = new VirtualSnapshotSpan();
            AdornmentLayer.RemoveAllAdornments();
        }

        public void Dispose()
        {
            if (LineSpans == null)
                return;
            Clear();
            LineSpans = null;
        }

        public void RemovedCallback(object tag, UIElement element)
        {
            LineSpans.Remove(tag);
        }

        public void Update(bool selectionChanged)
        {
            if (TextSelection.TextView.InLayout)
                return;
            var streamSelectionSpan = TextSelection.StreamSelectionSpan;
            if (streamSelectionSpan.Length == 0)
            {
                Clear();
            }
            else
            {
                IList<ITextViewLine> lines;
                if (_oldStreamSelection.Snapshot != streamSelectionSpan.Snapshot)
                {
                    Clear();
                    lines = TextSelection.TextView.TextViewLines.GetTextViewLinesIntersectingSpan(streamSelectionSpan
                        .SnapshotSpan);
                }
                else if (_oldStreamSelection.Length == 0)
                {
                    lines = TextSelection.TextView.TextViewLines.GetTextViewLinesIntersectingSpan(streamSelectionSpan
                        .SnapshotSpan);
                }
                else
                {
                    IList<ITextViewLine> intersectingSpan1 =
                        TextSelection.TextView.TextViewLines.GetTextViewLinesIntersectingSpan(_oldStreamSelection
                            .SnapshotSpan);
                    IList<ITextViewLine> intersectingSpan2 =
                        TextSelection.TextView.TextViewLines.GetTextViewLinesIntersectingSpan(streamSelectionSpan
                            .SnapshotSpan);
                    lines = new List<ITextViewLine>(intersectingSpan1.Count + intersectingSpan2.Count);
                    var index1 = 0;
                    var index2 = 0;
                    while (index1 < intersectingSpan1.Count || index2 < intersectingSpan2.Count)
                    {
                        ITextViewLine textViewLine;
                        if (index1 < intersectingSpan1.Count)
                        {
                            if (index2 < intersectingSpan2.Count)
                            {
                                var start = intersectingSpan1[index1].Start;
                                var position1 = start.Position;
                                start = intersectingSpan2[index2].Start;
                                var position2 = start.Position;
                                if (position1 < position2)
                                {
                                    textViewLine = intersectingSpan1[index1++];
                                }
                                else
                                {
                                    start = intersectingSpan1[index1].Start;
                                    var position3 = start.Position;
                                    start = intersectingSpan2[index2].Start;
                                    var position4 = start.Position;
                                    if (position3 == position4)
                                        ++index1;
                                    textViewLine = intersectingSpan2[index2++];
                                }
                            }
                            else
                            {
                                textViewLine = intersectingSpan1[index1++];
                            }
                        }
                        else
                        {
                            textViewLine = intersectingSpan2[index2++];
                        }

                        lines.Add(textViewLine);
                    }
                }

                DrawSelectionOnLines(lines, streamSelectionSpan.SnapshotSpan);
            }

            _oldStreamSelection = streamSelectionSpan;
        }

        private void DrawSelectionOnLines(IList<ITextViewLine> lines, SnapshotSpan streamSelection)
        {
            var isVirtualSpaceEnabled = TextSelection.TextView.Options.IsVirtualSpaceEnabled();
            var isBoxSelection = TextSelection.Mode == TextSelectionMode.Box;
            var viewportLeft = TextSelection.TextView.ViewportLeft;
            var viewportRight = TextSelection.TextView.ViewportRight;
            var leftClip = viewportLeft - 1500.0;
            var rightClip = viewportRight + 1500.0;
            var position = TextSelection.End.Position;
            foreach (var line in lines)
            {
                var num = LineSpans.TryGetValue(line.IdentityTag, out var selectionData) ? 1 : 0;
                LineTopAndBottom(line, streamSelection, out var lineTop, out var lineBottom);
                var selectionOnTextViewLine = TextSelection.GetSelectionOnTextViewLine(line);
                if (num != 0)
                {
                    if (!selectionOnTextViewLine.HasValue || selectionData.ShouldRedraw(selectionOnTextViewLine.Value,
                            viewportLeft, viewportRight, lineTop == line.Top, lineBottom == line.Bottom))
                        AdornmentLayer.RemoveAdornmentsByTag(line.IdentityTag);
                    else
                        continue;
                }

                if (selectionOnTextViewLine.HasValue)
                {
                    var visualOverlapsForLine = CalculateVisualOverlapsForLine(line, selectionOnTextViewLine.Value,
                        position, isBoxSelection, isVirtualSpaceEnabled);
                    if (visualOverlapsForLine.Count > 0)
                    {
                        var adornmentLeft = visualOverlapsForLine[0].Item1;
                        var adornmentRight = visualOverlapsForLine[visualOverlapsForLine.Count - 1].Item2;
                        if (adornmentLeft < rightClip && adornmentRight > leftClip)
                        {
                            var path = new PathGeometry {FillRule = FillRule.Nonzero};
                            foreach (var tuple in visualOverlapsForLine)
                                AddRectangleToPath(tuple.Item1, lineTop, tuple.Item2, lineBottom, leftClip, rightClip,
                                    path);
                            if (!path.IsEmpty())
                            {
                                path.Freeze();
                                Geometry outlinedPathGeometry = path.GetOutlinedPathGeometry();
                                if (outlinedPathGeometry.CanFreeze)
                                    outlinedPathGeometry.Freeze();
                                var selectionAdornment = new SelectionAdornment(null, Brush, outlinedPathGeometry);
                                LineSpans.Add(line.IdentityTag,
                                    new SelectionData(selectionOnTextViewLine.Value, adornmentLeft, adornmentRight,
                                        leftClip, rightClip, lineTop == line.Top, lineBottom == line.Bottom));
                                AdornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, line.Extent,
                                    line.IdentityTag, selectionAdornment, RemovedCallback);
                            }
                        }
                    }
                }
            }
        }

        internal class SelectionData
        {
            public readonly bool AtBottom;
            public readonly bool AtTop;
            public readonly double LeftEdgeOfCorrectlyRenderedAdornment;
            public readonly double RightEdgeOfCorrectlyRenderedAdornment;
            public readonly VirtualSnapshotSpan Span;

            public SelectionData(VirtualSnapshotSpan span, double adornmentLeft, double adornmentRight, double leftClip,
                double rightClip, bool atTop, bool atBottom)
            {
                Span = span;
                LeftEdgeOfCorrectlyRenderedAdornment = adornmentLeft < leftClip ? leftClip + 2.0 : double.MinValue;
                RightEdgeOfCorrectlyRenderedAdornment = adornmentRight > rightClip ? rightClip - 2.0 : double.MaxValue;
                AtTop = atTop;
                AtBottom = atBottom;
            }

            public bool ShouldRedraw(VirtualSnapshotSpan span, double viewportLeft, double viewportRight, bool atTop,
                bool atBottom)
            {
                if (!(Span != span) && viewportLeft >= LeftEdgeOfCorrectlyRenderedAdornment &&
                    viewportRight <= RightEdgeOfCorrectlyRenderedAdornment && AtTop == atTop)
                    return AtBottom != atBottom;
                return true;
            }
        }
    }
}