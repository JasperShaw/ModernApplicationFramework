using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal sealed class GradientBrushSelectionPainter : BrushSelectionPainter, ISelectionPainter
    {
        internal bool SelectionRendered;
        private double _leftEdgeOfCorrectlyRenderedAdornment = double.MinValue;
        private double _rightEdgeOfCorrectlyRenderedAdornment = double.MaxValue;

        internal Brush Brush { get; }

        internal Pen Pen { get; }

        public GradientBrushSelectionPainter(ITextSelection selection, IAdornmentLayer adornmentLayer, Brush brush,
            Pen pen)
            : base(adornmentLayer, selection)
        {
            Brush = brush;
            Pen = pen;
        }

        public void Activate()
        {
            AdornmentLayer.Opacity = 1.0;
            if (TextSelection.TextView.InLayout || TextSelection.TextView.TextViewLines == null)
                return;
            Update(true);
        }

        public void Clear()
        {
            AdornmentLayer.RemoveAllAdornments();
            _leftEdgeOfCorrectlyRenderedAdornment = double.MinValue;
            _rightEdgeOfCorrectlyRenderedAdornment = double.MaxValue;
        }

        public void Dispose()
        {
            Clear();
        }

        public void RemovedCallback(object tag, UIElement element)
        {
            SelectionRendered = false;
        }

        public void Update(bool selectionChanged)
        {
            var viewportLeft = TextSelection.TextView.ViewportLeft;
            var viewportRight = TextSelection.TextView.ViewportRight;
            if (!selectionChanged && SelectionRendered && viewportLeft >= _leftEdgeOfCorrectlyRenderedAdornment &&
                viewportRight <= _rightEdgeOfCorrectlyRenderedAdornment)
                return;
            Clear();
            var streamSelectionSpan = TextSelection.StreamSelectionSpan;
            if (streamSelectionSpan.Length <= 0)
                return;
            DrawSelectionOnLines(
                TextSelection.TextView.TextViewLines.GetTextViewLinesIntersectingSpan(streamSelectionSpan.SnapshotSpan),
                streamSelectionSpan.SnapshotSpan, viewportLeft, viewportRight);
        }

        private void DrawSelectionOnLines(IList<ITextViewLine> lines, SnapshotSpan streamSelection, double viewportLeft,
            double viewportRight)
        {
            var isVirtualSpaceEnabled = TextSelection.TextView.Options.IsVirtualSpaceEnabled();
            var isBoxSelection = TextSelection.Mode == TextSelectionMode.Box;
            var leftClip = viewportLeft - 1500.0;
            var rightClip = viewportRight + 1500.0;
            var position = TextSelection.End.Position;
            var path = new PathGeometry {FillRule = FillRule.Nonzero};
            var val11 = double.MaxValue;
            var val12 = double.MinValue;
            foreach (var line in lines)
            {
                var selectionOnTextViewLine = TextSelection.GetSelectionOnTextViewLine(line);
                LineTopAndBottom(line, streamSelection, out var lineTop, out var lineBottom);
                if (selectionOnTextViewLine.HasValue)
                {
                    var visualOverlapsForLine = CalculateVisualOverlapsForLine(line, selectionOnTextViewLine.Value,
                        position, isBoxSelection, isVirtualSpaceEnabled);
                    if (visualOverlapsForLine.Count > 0)
                    {
                        foreach (var tuple in visualOverlapsForLine)
                            AddRectangleToPath(tuple.Item1, lineTop, tuple.Item2, lineBottom, leftClip, rightClip,
                                path);
                        val11 = Math.Min(val11, visualOverlapsForLine[0].Item1);
                        val12 = Math.Min(val12, visualOverlapsForLine[visualOverlapsForLine.Count - 1].Item2);
                    }
                }
            }

            if (path.IsEmpty())
                return;
            path.Freeze();
            Geometry outlinedPathGeometry = path.GetOutlinedPathGeometry();
            if (outlinedPathGeometry.CanFreeze)
                outlinedPathGeometry.Freeze();
            var selectionAdornment = new SelectionAdornment(Pen, Brush, outlinedPathGeometry);
            SelectionRendered = true;
            _leftEdgeOfCorrectlyRenderedAdornment = val11 < leftClip ? leftClip + 2.0 : double.MinValue;
            _rightEdgeOfCorrectlyRenderedAdornment = val12 > rightClip ? rightClip - 2.0 : double.MaxValue;
            AdornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, streamSelection, null,
                selectionAdornment, RemovedCallback);
        }
    }
}