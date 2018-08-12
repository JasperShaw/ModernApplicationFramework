using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    public class SelectionAdornment : FrameworkElement
    {
        internal DrawingVisual Child;

        public SelectionAdornment(Pen borderPen, Brush fillBrush, Geometry drawingPath)
        {
            Child = new DrawingVisual();
            var drawingContext = Child.RenderOpen();
            drawingContext.DrawGeometry(fillBrush, borderPen, drawingPath);
            drawingContext.Close();
            IsHitTestVisible = true;
            AddVisualChild(Child);
            Cursor = Cursors.Arrow;
        }

        protected override Visual GetVisualChild(int index)
        {
            return Child;
        }

        protected override int VisualChildrenCount => 1;
    }
}