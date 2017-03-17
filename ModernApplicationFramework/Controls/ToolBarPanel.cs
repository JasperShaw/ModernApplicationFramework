using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Core.Standard;

namespace ModernApplicationFramework.Controls
{
    public class ToolBarPanel : System.Windows.Controls.Primitives.ToolBarPanel
    {
        public static DependencyProperty IsStretchingProperty;

        public bool IsStretching
        {
            get => (bool)GetValue(IsStretchingProperty);
            set => SetValue(IsStretchingProperty, Boxes.Box(value));
        }

        static ToolBarPanel()
        {
            IsStretchingProperty = DependencyProperty.Register("IsStretching", typeof(bool), typeof(ToolBarPanel), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = MeasureWithCollapsePrevention(constraint);
            if (Orientation == Orientation.Horizontal && constraint.Width < double.MaxValue || Orientation == Orientation.Vertical && constraint.Height < double.MaxValue)
            {
                if (IsStretching)
                {
                    ReMeasureFirstStretchingComboBox(constraint);
                    if (Orientation == Orientation.Horizontal)
                        size.Width = constraint.Width;
                    else
                        size.Height = constraint.Height;
                }
                if (Orientation == Orientation.Horizontal)
                    size.Width += 3.0;
                else
                    size.Height += 3.0;
            }
            ToolBar templatedParent = TemplatedParent as ToolBar;
            if (templatedParent != null && templatedParent.Orientation == Orientation.Vertical)
            {
                double val2 = 0.0;
                for (int index = 0; index < templatedParent.Items.Count; ++index)
                {
                    FrameworkElement frameworkElement = templatedParent.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
                    if (frameworkElement != null && !System.Windows.Controls.ToolBar.GetIsOverflowItem(frameworkElement))
                        val2 = Math.Max(frameworkElement.DesiredSize.Width, val2);
                    else
                        break;
                }
                size.Width = val2;
            }
            return size;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Orientation == Orientation.Horizontal)
                arrangeSize.Width = Math.Max(0.0, arrangeSize.Width - 3.0);
            else
                arrangeSize.Height = Math.Max(0.0, arrangeSize.Height - 3.0);
            return BaseArrangeOverride(arrangeSize);
        }

        private Size BaseArrangeOverride(Size arrangeSize)
        {
            UIElementCollection internalChildren = InternalChildren;
            bool flag = Orientation == Orientation.Horizontal;
            Rect finalRect = new Rect(arrangeSize);
            double num = 0.0;
            int index = 0;
            for (int count = internalChildren.Count; index < count; ++index)
            {
                UIElement uiElement = internalChildren[index];
                if (uiElement != null)
                {
                    if (flag)
                    {
                        finalRect.X += num;
                        num = uiElement.DesiredSize.Width;
                        finalRect.Width = num;
                        finalRect.Height = Math.Max(arrangeSize.Height, uiElement.DesiredSize.Height);
                    }
                    else
                    {
                        finalRect.Y += num;
                        num = uiElement.DesiredSize.Height;
                        finalRect.Height = num;
                        finalRect.Width = Math.Max(arrangeSize.Width, uiElement.DesiredSize.Width);
                    }
                    uiElement.Arrange(finalRect);
                }
            }
            return arrangeSize;
        }

        private Size MeasureWithCollapsePrevention(Size constraint)
        {
            bool flag = false;
            for (int index = 0; index < VisualChildrenCount; ++index)
            {
                UIElement visualChild = (UIElement)GetVisualChild(index);
                if (visualChild != null)
                {
                    flag = visualChild.Visibility == Visibility.Collapsed;
                    if (!flag)
                        break;
                }
            }
            UIElement uiElement = null;
            Binding binding = null;
            if (flag)
            {
                for (int index = 0; index < VisualChildrenCount; ++index)
                {
                    UIElement visualChild = (UIElement)GetVisualChild(index);
                    if (visualChild != null)
                    {
                        uiElement = visualChild;
                        break;
                    }
                }
                if (uiElement != null)
                {
                    binding = BindingOperations.GetBinding(uiElement, VisibilityProperty);
                    uiElement.Visibility = Visibility.Hidden;
                }
            }
            Size size = base.MeasureOverride(constraint);
            if (uiElement != null)
            {
                uiElement.Visibility = Visibility.Collapsed;
                if (binding != null)
                    BindingOperations.SetBinding(uiElement, VisibilityProperty, binding);
            }
            return size;
        }

        private void ReMeasureFirstStretchingComboBox(Size constraint)
        {
            UIElement uiElement = null;
            double num1 = 0.0;
            for (int index = 0; index < VisualChildrenCount; ++index)
            {
                UIElement visualChild = (UIElement)GetVisualChild(index);
                if (visualChild != null)
                {
                    if (uiElement == null && IsStretchingComboBox())
                        uiElement = visualChild;
                    else if (Orientation == Orientation.Horizontal)
                        num1 += visualChild.DesiredSize.Width;
                    else
                        num1 += visualChild.DesiredSize.Height;
                }
            }
            if (uiElement == null)
                return;
            double num2 = (Orientation == Orientation.Horizontal ? constraint.Width : constraint.Height) - num1;
            uiElement.Measure(Orientation == Orientation.Horizontal
                ? new Size(num2, constraint.Height)
                : new Size(constraint.Width, num2));
        }

        private bool IsStretchingComboBox()
        {
                return false;
        }

    }
}
