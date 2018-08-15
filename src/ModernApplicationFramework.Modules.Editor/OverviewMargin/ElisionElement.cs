using System;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal sealed class ElisionElement : UIElement
    {
        private readonly OverviewElement _element;

        public ElisionElement(OverviewElement element)
        {
            _element = element;
            IsHitTestVisible = false;
            Focusable = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Opacity = SystemParameters.HighContrast ? 1.0 : 0.25;
            ValueTuple<double, double> horizontalDimensions = GetSourceImageMarginHorizontalDimensions();
            double x = horizontalDimensions.Item1;
            double num1 = horizontalDimensions.Item2;
            if (_element.ElisionBrush == null && _element.ElisionPen == null || !_element.Map.AreElisionsExpanded)
                return;
            NormalizedSnapshotSpanCollection snapshot = _element.TextViewHost.TextView.BufferGraph.MapDownToSnapshot(new SnapshotSpan(_element.TextViewHost.TextView.VisualSnapshot, 0, _element.TextViewHost.TextView.VisualSnapshot.Length), SpanTrackingMode.EdgeInclusive, _element.TextViewHost.TextView.TextSnapshot);
            double num2 = -_element.VisiblePen.Thickness;
            double ofBufferPosition1 = _element.GetYCoordinateOfBufferPosition(new SnapshotPoint(_element.TextViewHost.TextView.TextSnapshot, 0));
            foreach (SnapshotSpan snapshotSpan in snapshot)
            {
                double ofBufferPosition2 = _element.GetYCoordinateOfBufferPosition(snapshotSpan.Start);
                OverviewElement.DrawRectangle(drawingContext, _element.ElisionBrush, _element.ElisionPen, num1 - num2, x, ofBufferPosition1, ofBufferPosition2);
                ofBufferPosition1 = _element.GetYCoordinateOfBufferPosition(snapshotSpan.End);
            }
            double ofBufferPosition3 = _element.GetYCoordinateOfBufferPosition(new SnapshotPoint(_element.TextViewHost.TextView.TextSnapshot, _element.TextViewHost.TextView.TextSnapshot.Length));
            OverviewElement.DrawRectangle(drawingContext, _element.ElisionBrush, _element.ElisionPen, num1 - num2, x, ofBufferPosition1, ofBufferPosition3);
        }

        private ValueTuple<double, double> GetSourceImageMarginHorizontalDimensions()
        {
            if (!SystemParameters.HighContrast)
                return new ValueTuple<double, double>(0.0, _element.ActualWidth);
            ITextViewMargin textViewMargin = _element.TextViewHost.GetTextViewMargin("OverviewSourceImageMargin");
            double num1;
            double num2;
            if (textViewMargin != null)
            {
                num1 = textViewMargin.VisualElement.TranslatePoint(new Point(0.0, 0.0), this).X;
                num2 = textViewMargin.VisualElement.ActualWidth;
            }
            else
            {
                num1 = 0.0;
                num2 = _element.Width;
            }
            return new ValueTuple<double, double>(num1, num2);
        }
    }
}