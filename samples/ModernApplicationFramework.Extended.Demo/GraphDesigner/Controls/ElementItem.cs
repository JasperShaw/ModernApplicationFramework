using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.GraphDesigner.Controls
{
    public class ElementItem : ListBoxItem
    {
        private bool _justSelected;

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
            e.Handled = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_justSelected)
            {
                _justSelected = false;
                return;
            }

            var parentGraphControl = ParentGraphControl;
            if (parentGraphControl == null)
                return;

            if (IsSelected && parentGraphControl.SelectedElementItems.Count() > 1 && !ParentGraphControl.WasDragged)
            {
                var other =  parentGraphControl.SelectedElementItems.Where(x => !Equals(x, this));
                other.ForEach(x => x.IsSelected = false);
                e.Handled = true;
            }
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

            if (IsSelected)
                return;

            if (!parentGraphControl.MultiselectEnabled)
            {
                parentGraphControl.SelectedElements.Clear();
            }
            if (parentGraphControl.MultiselectEnabled && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                parentGraphControl.SelectedElements.Clear();
            }
            IsSelected = true;
            _justSelected = true;
        }
    }
}
