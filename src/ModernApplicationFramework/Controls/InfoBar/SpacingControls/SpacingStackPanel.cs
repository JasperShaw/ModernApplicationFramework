using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar.SpacingControls
{
    internal sealed class SpacingStackPanel : StackPanel
    {
        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(SpacingStackPanel), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), ValidateSpacing);

        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, Boxes.Box(value));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);
            int num = NonCollapsedChildren.Count() - 1;
            if (num > 0)
            {
                if (Orientation == Orientation.Horizontal)
                    size.Width += num * Spacing;
                else
                    size.Height += num * Spacing;
            }
            return size;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection internalChildren = InternalChildren;
            bool flag = Orientation == Orientation.Horizontal;
            Rect finalRect = new Rect(arrangeSize);
            double num1 = -Spacing;
            foreach (UIElement nonCollapsedChild in NonCollapsedChildren)
            {
                Size desiredSize;
                if (flag)
                {
                    finalRect.X += num1 + Spacing;
                    desiredSize = nonCollapsedChild.DesiredSize;
                    num1 = desiredSize.Width;
                    finalRect.Width = num1;
                    double height1 = arrangeSize.Height;
                    desiredSize = nonCollapsedChild.DesiredSize;
                    double height2 = desiredSize.Height;
                    double num2 = Math.Max(height1, height2);
                    // ISSUE: explicit reference operation
                    finalRect.Height = num2;
                }
                else
                {
                    finalRect.Y += num1 + Spacing;
                    desiredSize = nonCollapsedChild.DesiredSize;
                    num1 = desiredSize.Height;
                    finalRect.Height = num1;
                    double width1 = arrangeSize.Width;
                    desiredSize = nonCollapsedChild.DesiredSize;
                    double width2 = desiredSize.Width;
                    double num2 = Math.Max(width1, width2);
                    finalRect.Width = num2;
                }
                nonCollapsedChild.Arrange(finalRect);
            }
            return arrangeSize;
        }

        private static bool ValidateSpacing(object value)
        {
            double num = (double)value;
            if (!num.IsNonreal())
                return num >= 0.0;
            return false;
        }

        private IEnumerable<UIElement> NonCollapsedChildren
        {
            get
            {
                return InternalChildren.Cast<UIElement>().Where(e => !IsCollapsed(e));
            }
        }

        private static bool IsCollapsed(UIElement e)
        {
            if (e != null)
                return e.Visibility == Visibility.Collapsed;
            return true;
        }
    }
}
