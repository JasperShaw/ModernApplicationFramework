using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal sealed class GradientBrushSelectionPainter : BrushSelectionPainter
    {
        private double _leftEdgeOfCorrectlyRenderedAdornment = double.MinValue;
        private double _rightEdgeOfCorrectlyRenderedAdornment = double.MaxValue;
        internal bool SelectionRendered;
        private MultiSelectionMouseState _mouseState;

        internal Brush Brush { get; }

        internal Pen Pen { get; }

        public GradientBrushSelectionPainter(IMultiSelectionBroker broker, IAdornmentLayer adornmentLayer, Brush brush, Pen pen)
          : base(adornmentLayer, broker)
        {
            Brush = brush;
            Pen = pen;
        }

        public void Dispose()
        {
            Clear();
        }

        public override void Clear()
        {
            if (AdornmentLayer.IsEmpty)
                return;
            AdornmentLayer.RemoveAllAdornments();
            _leftEdgeOfCorrectlyRenderedAdornment = double.MinValue;
            _rightEdgeOfCorrectlyRenderedAdornment = double.MaxValue;
        }

        public override void Activate()
        {
            AdornmentLayer.Opacity = 1.0;
        }

        private MultiSelectionMouseState MouseState => _mouseState ??
                                                       (_mouseState = MultiSelectionMouseState.GetStateForView(MultiSelectionBroker.TextView));

        public override void Update(bool selectionChanged)
        {
            if (MultiSelectionBroker.TextView.IsClosed || MultiSelectionBroker.TextView.InLayout || MultiSelectionBroker.TextView.TextViewLines == null)
                return;
            var viewportLeft = MultiSelectionBroker.TextView.ViewportLeft;
            var viewportRight = MultiSelectionBroker.TextView.ViewportRight;
            if (!selectionChanged && SelectionRendered && (viewportLeft >= _leftEdgeOfCorrectlyRenderedAdornment && viewportRight <= _rightEdgeOfCorrectlyRenderedAdornment))
                return;
            Clear();
            var intersectingSpan = MultiSelectionBroker.GetSelectionsIntersectingSpan(MultiSelectionBroker.TextView.TextViewLines.FormattedSpan);
            var left = new NormalizedSnapshotSpanCollection();
            foreach (var span in intersectingSpan)
                left = NormalizedSnapshotSpanCollection.Union(left, new NormalizedSnapshotSpanCollection(span.Extent.SnapshotSpan));

            if (MouseState.ProvisionalSelection != Selection.Invalid)
                left = NormalizedSnapshotSpanCollection.Union(left, new NormalizedSnapshotSpanCollection(MouseState.ProvisionalSelection.Extent.SnapshotSpan));
            var lines = new List<ITextViewLine>();
            var textViewLines = MultiSelectionBroker.TextView.TextViewLines;
            foreach (var textViewLine in textViewLines)
            {
                if (left.IntersectsWith(textViewLine.Extent))
                    lines.Add(textViewLine);
            }
            if (lines.Count <= 0)
                return;
            DrawSelectionOnLines(lines, viewportLeft, viewportRight);
        }

        private static int CompareSelections(Selection left, Selection right)
        {
            var start1 = left.Start;
            var start2 = right.Start;
            if (start1 < start2)
                return -1;
            return !(start1 > start2) ? 0 : 1;
        }

        private void DrawSelectionOnLines(IReadOnlyCollection<ITextViewLine> lines, double viewportLeft, double viewportRight)
        {
            var flag = MultiSelectionBroker.TextView.Options.IsVirtualSpaceEnabled();
            var leftClip = viewportLeft - 1500.0;
            var rightClip = viewportRight + 1500.0;
            var path = new PathGeometry {FillRule = FillRule.Nonzero};
            var val1_1 = double.MaxValue;
            var val1_2 = double.MinValue;
            var selectionList = new List<Selection>(MultiSelectionBroker.GetSelectionsIntersectingSpans(new NormalizedSnapshotSpanCollection(lines.Select(line => line.Extent))));
            if (MouseState.ProvisionalSelection != Selection.Invalid)
            {
                for (var index = 0; index < selectionList.Count; ++index)
                {
                    if (selectionList[index].Extent.IntersectsWith(MouseState.ProvisionalSelection.Extent))
                    {
                        selectionList.RemoveAt(index);
                        --index;
                    }
                }
                selectionList.Add(MouseState.ProvisionalSelection);
                selectionList.Sort(CompareSelections);
            }
            foreach (var line1 in lines)
            {
                if (selectionList.Count > 0)
                {
                    var selection1 = selectionList[selectionList.Count - 1];
                    LineTopAndBottom(line1, out var lineTop, out var lineBottom);
                    for (var index = 0; index < selectionList.Count; ++index)
                    {
                        var selection2 = selectionList[index];
                        if (!selection2.IsEmpty)
                        {
                            selection2 = selectionList[index];
                            if (!(selection2.End.Position < line1.Start))
                            {
                                selection2 = selectionList[index];
                                if (!(selection2.Start.Position > line1.End))
                                {
                                    var line2 = line1;
                                    selection2 = selectionList[index];
                                    var extent = selection2.Extent;
                                    var position = selection1.End.Position;
                                    var num1 = MultiSelectionBroker.IsBoxSelection ? 1 : 0;
                                    var num2 = flag ? 1 : 0;
                                    var visualOverlapsForLine = CalculateVisualOverlapsForLine(line2, extent, position, num1 != 0, num2 != 0);
                                    if (visualOverlapsForLine.Count > 0)
                                    {
                                        foreach (var tuple in visualOverlapsForLine)
                                            AddRectangleToPath(tuple.Item1, lineTop, tuple.Item2, lineBottom, leftClip, rightClip, path);
                                        val1_1 = Math.Min(val1_1, visualOverlapsForLine[0].Item1);
                                        val1_2 = Math.Min(val1_2, visualOverlapsForLine[visualOverlapsForLine.Count - 1].Item2);
                                    }
                                    continue;
                                }
                                break;
                            }
                        }
                        selectionList.RemoveAt(index);
                        --index;
                    }
                }
            }
            if (path.IsEmpty())
                return;
            path.Freeze();
            var outlinedPathGeometry = (Geometry)path.GetOutlinedPathGeometry();
            if (outlinedPathGeometry.CanFreeze)
                outlinedPathGeometry.Freeze();
            var selectionAdornment = new SelectionAdornment(Pen, Brush, outlinedPathGeometry);
            SelectionRendered = true;
            _leftEdgeOfCorrectlyRenderedAdornment = val1_1 < leftClip ? leftClip + 2.0 : double.MinValue;
            _rightEdgeOfCorrectlyRenderedAdornment = val1_2 > rightClip ? rightClip - 2.0 : double.MaxValue;
            AdornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, MultiSelectionBroker.SelectionExtent.SnapshotSpan, null, selectionAdornment, RemovedCallback);
        }

        public void RemovedCallback(object tag, UIElement element)
        {
            SelectionRendered = false;
        }
    }
}