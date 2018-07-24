using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    internal static class Markers
    {
        public static readonly Thickness SingleLinePadding = new Thickness(0.0, 0.0, 0.0, 1.0);
        public static readonly Thickness MultiLinePadding = new Thickness(0.0);

        public static bool MarkerGeometrySpansMultipleLines(ITextViewLineCollection collection, SnapshotSpan bufferSpan)
        {
            var containingBufferPosition = collection.GetTextViewLineContainingBufferPosition(bufferSpan.Start);
            if (containingBufferPosition != null)
                return bufferSpan.End > containingBufferPosition.EndIncludingLineBreak;
            return true;
        }

        public static IList<Rect> GetRectanglesFromBounds(IList<TextBounds> bounds, Thickness padding, double leftClip, double rightClip, bool useTextBounds)
        {
            var rectList = new List<Rect>(bounds.Count);
            rectList.AddRange(from bound in bounds
                let x = Math.Max(leftClip, bound.Left - padding.Left)
                let num1 = Math.Min(rightClip, bound.Right + padding.Right)
                where x < num1
                let y = (useTextBounds ? bound.TextTop : bound.Top) - padding.Top
                let num2 = (useTextBounds ? bound.TextBottom : bound.Bottom) + padding.Bottom
                select new Rect(x, y, num1 - x, num2 - y));
            return rectList;
        }

        public static Geometry GetMarkerGeometryFromRectangles(IList<Rect> rectangles)
        {
            if (rectangles.Count == 0)
                return null;
            var pathGeometry = new PathGeometry {FillRule = FillRule.Nonzero};
            foreach (var rectangle in rectangles)
                pathGeometry.AddGeometry(new RectangleGeometry(rectangle));
            pathGeometry.Freeze();
            if (rectangles.Count > 1)
            {
                pathGeometry = pathGeometry.GetOutlinedPathGeometry();
                pathGeometry.Freeze();
            }
            return pathGeometry;
        }
    }
}
