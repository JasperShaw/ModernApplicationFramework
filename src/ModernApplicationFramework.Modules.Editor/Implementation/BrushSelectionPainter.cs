using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal abstract class BrushSelectionPainter
    {
        internal const double ClipPadding = 1500.0;
        internal const double EdgeWidth = 2.0;
        internal const double Overlap = 0.5;
        protected readonly IAdornmentLayer AdornmentLayer;
        protected readonly ITextSelection TextSelection;

        protected BrushSelectionPainter(IAdornmentLayer adornmentLayer, ITextSelection selection)
        {
            AdornmentLayer = adornmentLayer;
            TextSelection = selection;
        }

        public static ISelectionPainter CreatePainter(ITextSelection selection, IAdornmentLayer adornmentLayer,
            ResourceDictionary dictionary, Color defaultColor)
        {
            var brush = !dictionary.Contains("Background")
                ? new SolidColorBrush(!dictionary.Contains("BackgroundColor")
                    ? defaultColor
                    : (Color) dictionary["BackgroundColor"])
                : ((Brush) dictionary["Background"]).Clone();
            if (brush.CanFreeze)
                brush.Freeze();
            if (selection.TextView.Options.IsSimpleGraphicsEnabled())
                return new SolidColorBrushSelectionPainter(selection, adornmentLayer, brush);
            Pen pen = null;
            if (dictionary.Contains("BackgroundPen"))
            {
                pen = ((Pen) dictionary["BackgroundPen"]).Clone();
                if (pen.CanFreeze)
                    pen.Freeze();
            }

            if (brush is SolidColorBrush && pen == null)
                return new SolidColorBrushSelectionPainter(selection, adornmentLayer, brush);
            return new GradientBrushSelectionPainter(selection, adornmentLayer, brush, pen);
        }

        internal static void AddRectangleToPath(double left, double top, double right, double bottom, double leftClip,
            double rightClip, PathGeometry path)
        {
            left = Math.Max(left, leftClip);
            right = Math.Min(right, rightClip);
            if (right <= left)
                return;
            var rectangleGeometry = new RectangleGeometry(new Rect(left, top - 0.5, right - left, bottom - top + 1.0));
            if (rectangleGeometry == null)
                return;
            path.AddGeometry(rectangleGeometry);
        }

        internal static IList<Tuple<double, double>> CalculateVisualOverlapsForLine(ITextViewLine line,
            VirtualSnapshotSpan span, SnapshotPoint selectionEnd, bool isBoxSelection, bool isVirtualSpaceEnabled)
        {
            var start = span.Start;
            var end = span.End;
            if (start == end)
            {
                var virtualBufferPosition = CaretElement.GetXCoordinateFromVirtualBufferPosition(line, start);
                return new FrugalList<Tuple<double, double>>
                {
                    new Tuple<double, double>(virtualBufferPosition,
                        virtualBufferPosition + SystemParameters.CaretWidth)
                };
            }

            var flag = isBoxSelection || selectionEnd < line.EndIncludingLineBreak ||
                       line.LineBreakLength == 0 && line.IsLastTextViewLineForSnapshotLine;
            IList<Tuple<double, double>> tupleList;
            if (start.Position.Position == line.End.Position)
            {
                var virtualBufferPosition = CaretElement.GetXCoordinateFromVirtualBufferPosition(line, start);
                var num = !flag
                    ? (isVirtualSpaceEnabled ? double.MaxValue : virtualBufferPosition + line.EndOfLineWidth)
                    : CaretElement.GetXCoordinateFromVirtualBufferPosition(line, end);
                tupleList = new FrugalList<Tuple<double, double>>();
                if (num > virtualBufferPosition)
                    tupleList.Add(new Tuple<double, double>(virtualBufferPosition, num));
            }
            else
            {
                var normalizedTextBounds = line.GetNormalizedTextBounds(new SnapshotSpan(start.Position, end.Position));
                tupleList = new List<Tuple<double, double>>(normalizedTextBounds.Count + 1);
                foreach (var textBounds in normalizedTextBounds)
                    tupleList.Add(new Tuple<double, double>(textBounds.Left, textBounds.Right));
                var num = double.MinValue;
                if (flag)
                {
                    if (end.IsInVirtualSpace)
                        num = CaretElement.GetXCoordinateFromVirtualBufferPosition(line, end);
                }
                else if (isVirtualSpaceEnabled)
                {
                    num = double.MaxValue;
                }

                var textRight = line.TextRight;
                if (num > textRight)
                {
                    if (tupleList.Count > 0 && tupleList[tupleList.Count - 1].Item2 >= textRight)
                    {
                        textRight = tupleList[tupleList.Count - 1].Item1;
                        tupleList.RemoveAt(tupleList.Count - 1);
                    }

                    tupleList.Add(new Tuple<double, double>(textRight, num));
                }
            }

            return tupleList;
        }

        internal static void LineTopAndBottom(ITextViewLine line, Span streamSelection, out double lineTop,
            out double lineBottom)
        {
            lineTop = line.TextTop;
            lineBottom = line.TextBottom + 1.0;
        }
    }
}