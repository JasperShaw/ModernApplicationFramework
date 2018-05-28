using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ModernApplicationFramework.DragDrop
{
    internal class DragAdorner : Adorner
    {
        public DragAdorner(UIElement adornedElement, UIElement adornment, Point translation, DragDropEffects effects = DragDropEffects.None)
            : base(adornedElement)
        {
            _translation = translation;
            _mAdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            _mAdornerLayer.Add(this);
            _mAdornment = adornment;
            IsHitTestVisible = false;
            Effects = effects;
        }

        public DragDropEffects Effects { get; }

        public Point MousePosition
        {
            get => _mMousePosition;
            set
            {
                if (_mMousePosition != value)
                {
                    _mMousePosition = value;
                    _mAdornerLayer.Update(AdornedElement);
                }
            }
        }

        public void Detatch()
        {
            _mAdornerLayer.Remove(this);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _mAdornment.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(MousePosition.X + _translation.X, MousePosition.Y + _translation.Y));

            return result;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _mAdornment;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _mAdornment.Measure(constraint);
            return _mAdornment.DesiredSize;
        }

        protected override int VisualChildrenCount => 1;

        private readonly AdornerLayer _mAdornerLayer;
        private readonly UIElement _mAdornment;
        private Point _mMousePosition;
        private readonly Point _translation;
    }
}
