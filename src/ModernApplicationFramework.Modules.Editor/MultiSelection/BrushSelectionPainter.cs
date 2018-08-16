using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal abstract class BrushSelectionPainter
    {
        protected readonly IMultiSelectionBroker MultiSelectionBroker;
        protected readonly IAdornmentLayer AdornmentLayer;
        internal const double ClipPadding = 1500.0;
        internal const double EdgeWidth = 2.0;
        internal const double Overlap = 0.5;

        protected BrushSelectionPainter(IAdornmentLayer adornmentLayer, IMultiSelectionBroker multiSelectionBroker)
        {
            AdornmentLayer = adornmentLayer;
            MultiSelectionBroker = multiSelectionBroker;
        }

        public abstract void Clear();

        public abstract void Activate();

        public abstract void Update(bool selectionChanged);

        public static BrushSelectionPainter CreatePainter(IMultiSelectionBroker broker, IAdornmentLayer adornmentLayer, ResourceDictionary dictionary, Color defaultColor)
        {
            Brush brush = !dictionary.Contains("Background") ? new SolidColorBrush(!dictionary.Contains("BackgroundColor") ? defaultColor : (Color)dictionary["BackgroundColor"]) : ((Brush)dictionary["Background"]).Clone();
            if (brush.CanFreeze)
                brush.Freeze();
            Pen pen = null;
            if (dictionary.Contains("BackgroundPen"))
            {
                pen = ((Pen)dictionary["BackgroundPen"]).Clone();
                if (pen.CanFreeze)
                    pen.Freeze();
            }
            return new GradientBrushSelectionPainter(broker, adornmentLayer, brush, pen);
        }

        internal static void LineTopAndBottom(ITextViewLine line, out double lineTop, out double lineBottom)
        {
            lineTop = line.TextTop;
            lineBottom = line.TextBottom + 1.0;
        }

        internal static IList<Tuple<double, double>> CalculateVisualOverlapsForLine(ITextViewLine line, VirtualSnapshotSpan span, SnapshotPoint selectionEnd, bool isBoxSelection, bool isVirtualSpaceEnabled)
        {
            VirtualSnapshotPoint start = span.Start;
            VirtualSnapshotPoint bufferPosition = span.End;
            if (bufferPosition.Position > line.End)
                bufferPosition = new VirtualSnapshotPoint(line.EndIncludingLineBreak);
            if (start == bufferPosition)
            {
                double virtualBufferPosition = TextViewLineHelpers.GetXCoordinateFromVirtualBufferPosition(line, start);
                return new FrugalList<Tuple<double, double>>()
                {
                    new Tuple<double, double>(virtualBufferPosition, virtualBufferPosition + SystemParameters.CaretWidth)
                };
            }
            bool flag = isBoxSelection || selectionEnd < line.EndIncludingLineBreak || line.LineBreakLength == 0 && line.IsLastTextViewLineForSnapshotLine;
            IList<Tuple<double, double>> tupleList;
            if (start.Position.Position == line.End.Position)
            {
                double virtualBufferPosition = TextViewLineHelpers.GetXCoordinateFromVirtualBufferPosition(line, start);
                double num = !flag ? (isVirtualSpaceEnabled ? double.MaxValue : virtualBufferPosition + line.EndOfLineWidth) : TextViewLineHelpers.GetXCoordinateFromVirtualBufferPosition(line, bufferPosition);
                tupleList = new FrugalList<Tuple<double, double>>();
                if (num > virtualBufferPosition)
                    tupleList.Add(new Tuple<double, double>(virtualBufferPosition, num));
            }
            else
            {
                Collection<TextBounds> normalizedTextBounds = line.GetNormalizedTextBounds(new SnapshotSpan(start.Position, bufferPosition.Position));
                tupleList = new List<Tuple<double, double>>(normalizedTextBounds.Count + 1);
                foreach (TextBounds textBounds in normalizedTextBounds)
                    tupleList.Add(new Tuple<double, double>(textBounds.Left, textBounds.Right));
                double num = double.MinValue;
                if (flag)
                {
                    if (bufferPosition.IsInVirtualSpace)
                        num = TextViewLineHelpers.GetXCoordinateFromVirtualBufferPosition(line, bufferPosition);
                }
                else if (isVirtualSpaceEnabled)
                    num = double.MaxValue;
                double textRight = line.TextRight;
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

        internal static void AddRectangleToPath(double left, double top, double right, double bottom, double leftClip, double rightClip, PathGeometry path)
        {
            left = Math.Max(left, leftClip);
            right = Math.Min(right, rightClip);
            if (right <= left)
                return;
            var rectangleGeometry = new RectangleGeometry(new Rect(left, top - 0.5, right - left, bottom - top + 1.0));
            path.AddGeometry(rectangleGeometry);
        }
    }
}