using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    public class ColorInput : DynamicElement
    {
        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                UpdatePreviewImage();
                NotifyOfPropertyChange(() => Color);
            }
        }

        public ColorInput()
        {
            Color = Colors.Red;
            UpdatePreviewImage();
        }

        protected override void Draw(DrawingContext drawingContext, Rect bounds)
        {
            drawingContext.DrawRectangle(new SolidColorBrush(Color), null, bounds);
        }
    }
}
