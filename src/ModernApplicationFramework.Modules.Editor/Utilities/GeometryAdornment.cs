using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public class GeometryAdornment : UIElement
    {
        internal readonly DrawingVisual Child;
        internal readonly Brush FillBrush;
        internal readonly Pen BorderPen;
        internal readonly Geometry Geometry;

        public GeometryAdornment(Pen borderPen, Brush fillBrush, double left, double top, double width, double height, bool isHitTestVisible = false, bool snapsToDevicePixels = false)
            : this(borderPen, fillBrush, new Rect(left, top, width, height), isHitTestVisible, snapsToDevicePixels)
        {
        }

        public GeometryAdornment(Pen borderPen, Brush fillBrush, Rect rect, bool isHitTestVisible = false, bool snapsToDevicePixels = false)
            : this(borderPen, fillBrush, new RectangleGeometry(rect), isHitTestVisible, snapsToDevicePixels)
        {
        }

        public GeometryAdornment(Pen borderPen, Brush fillBrush, Geometry geometry, bool isHitTestVisible = false, bool snapsToDevicePixels = false)
        {
            BorderPen = borderPen;
            FillBrush = fillBrush;
            Geometry = geometry;
            IsHitTestVisible = isHitTestVisible;
            SnapsToDevicePixels = snapsToDevicePixels;
            Child = new DrawingVisual();
            DrawingContext drawingContext = Child.RenderOpen();
            drawingContext.DrawGeometry(fillBrush, borderPen, geometry);
            drawingContext.Close();
            AddVisualChild(Child);
        }

        protected override Visual GetVisualChild(int index)
        {
            return Child;
        }

        protected override int VisualChildrenCount => 1;
    }
}