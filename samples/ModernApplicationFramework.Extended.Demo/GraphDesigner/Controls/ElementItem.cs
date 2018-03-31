using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls
{
    public class ElementItem : ListBoxItem
    {
        private Point _lastMousePosition;
        private bool _isLeftMouseButtonDown;
        private bool _isDragging;

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X", typeof(double), typeof(ElementItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y", typeof(double), typeof(ElementItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ZIndexProperty = DependencyProperty.Register(
            "ZIndex", typeof(int), typeof(ElementItem),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public int ZIndex
        {
            get => (int)GetValue(ZIndexProperty);
            set => SetValue(ZIndexProperty, value);
        }

        private GraphControl ParentGraphControl => this.FindAncestor<GraphControl>();

        static ElementItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ElementItem),
                new FrameworkPropertyMetadata(typeof(ElementItem)));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            BringToFront();
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DoSelection();

            var parentGraphControl = ParentGraphControl;
            if (parentGraphControl != null)
                _lastMousePosition = e.GetPosition(parentGraphControl);

            _isLeftMouseButtonDown = true;

            e.Handled = true;

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging)
            {
                var newMousePosition = e.GetPosition(ParentGraphControl);
                var delta = newMousePosition - _lastMousePosition;

                X += delta.X;
                Y += delta.Y;

                _lastMousePosition = newMousePosition;
            }
            if (_isLeftMouseButtonDown)
            {
                _isDragging = true;
                CaptureMouse();
            }

            e.Handled = true;

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isLeftMouseButtonDown)
            {
                _isLeftMouseButtonDown = false;
                if (_isDragging)
                {
                    ReleaseMouseCapture();
                    _isDragging = false;
                }
            }
            e.Handled = true;
            base.OnMouseLeftButtonUp(e);
        }

        internal void BringToFront()
        {
            var parentGraphControl = ParentGraphControl;
            if (parentGraphControl == null)
                return;

            var maxZ = parentGraphControl.GetMaxZIndex();
            ZIndex = maxZ + 1;
        }


        private void DoSelection()
        {
            var parentGraphControl = ParentGraphControl;
            if (parentGraphControl == null)
                return;

            parentGraphControl.SelectedElements.Clear();
            IsSelected = true;
        }
    }
}
