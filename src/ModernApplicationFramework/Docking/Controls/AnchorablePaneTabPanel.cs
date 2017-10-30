/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class AnchorablePaneTabPanel : Panel
    {
        public AnchorablePaneTabPanel()
        {
            FlowDirection = FlowDirection.LeftToRight;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var visibleChildren = Children.Cast<UIElement>().Where(ch => ch.Visibility != Visibility.Collapsed);


            double finalWidth = finalSize.Width;
            var uiElements = visibleChildren as UIElement[] ?? visibleChildren.ToArray();
            double desideredWidth = uiElements.Sum(ch => ch.DesiredSize.Width);
            double offsetX = 0.0;

            if (finalWidth > desideredWidth)
            {
                foreach (var uiElement in uiElements)
                {
                    var child = (FrameworkElement) uiElement;
                    double childFinalWidth = child.DesiredSize.Width;
                    child.Arrange(new Rect(offsetX, 0, childFinalWidth, finalSize.Height));

                    offsetX += childFinalWidth;
                }
            }
            else
            {
                double childFinalWidth = finalWidth/uiElements.Count();
                foreach (var uiElement in uiElements)
                {
                    var child = (FrameworkElement) uiElement;
                    child.Arrange(new Rect(offsetX, 0, childFinalWidth, finalSize.Height));

                    offsetX += childFinalWidth;
                }
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double totWidth = 0;
            double maxHeight = 0;
            var visibleChildren = Children.Cast<UIElement>().Where(ch => ch.Visibility != Visibility.Collapsed);
            var uiElements = visibleChildren as UIElement[] ?? visibleChildren.ToArray();
            foreach (var child in uiElements.Cast<FrameworkElement>())
            {
                child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                totWidth += child.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            }

            if (!(totWidth > availableSize.Width))
                return new Size(Math.Min(availableSize.Width, totWidth), maxHeight);
            double childFinalDesideredWidth = availableSize.Width/uiElements.Count();
            foreach (var child in uiElements.Cast<FrameworkElement>())
            {
                child.Measure(new Size(childFinalDesideredWidth, availableSize.Height));
            }

            return new Size(Math.Min(availableSize.Width, totWidth), maxHeight);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                LayoutAnchorableTabItem.IsDraggingItem())
            {
                var contentModel = LayoutAnchorableTabItem.GetDraggingItem().Model as LayoutAnchorable;
                if (contentModel != null)
                {
                    var manager = contentModel.Root.Manager;
                    LayoutAnchorableTabItem.ResetDraggingItem();

                    manager.StartDraggingFloatingWindowForContent(contentModel);
                }
            }

            base.OnMouseLeave(e);
        }
    }
}