using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class Caret : UIElement
    {
        internal readonly CaretAdornmentLayer Layer;
        private readonly IMultiSelectionBroker _broker;
        private readonly Selection _region;
        internal readonly Geometry CaretGeometry;

        public Caret(CaretAdornmentLayer layer, Selection region, AbstractSelectionPresentationProperties properties)
        {
            Layer = layer;
            _broker = layer.TextView.GetMultiSelectionBroker();
            _region = region;
            Opacity = properties.IsOverwriteMode ? 0.5 : 1.0;
            var caretBounds = properties.CaretBounds;
            if (InputLanguageManager.Current.CurrentInputLanguage.TextInfo.IsRightToLeft)
            {
                var pathGeometry = new PathGeometry();
                var rectangleGeometry = new RectangleGeometry(new Rect(0.0, 0.0, caretBounds.Width, caretBounds.TextHeight));
                rectangleGeometry.Freeze();
                pathGeometry.AddGeometry(rectangleGeometry);
                pathGeometry.Figures.Add(new PathFigure
                {
                    StartPoint = new Point(0.0, 0.0),
                    Segments = {
            new LineSegment(new Point(-(caretBounds.Height / 6.0), 0.0), true),
            new LineSegment(new Point(0.0, caretBounds.Height / 6.0), true)
          },
                    IsClosed = true
                });
                pathGeometry.Transform = new TranslateTransform(caretBounds.Left, caretBounds.TextTop);
                pathGeometry.Transform.Freeze();
                CaretGeometry = pathGeometry;
            }
            else
            {
                var rectangleGeometry = new RectangleGeometry(new Rect(caretBounds.Left, caretBounds.TextTop, caretBounds.Width, caretBounds.TextHeight));
                rectangleGeometry.Freeze();
                CaretGeometry = rectangleGeometry;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var brush = !_broker.HasMultipleSelections ? Layer.TextBrush : (_region == _broker.PrimarySelection ? Layer.PrimaryCaretBrush : Layer.SecondaryCaretBrush);
            drawingContext.DrawGeometry(brush, null, CaretGeometry);
        }
    }
}
