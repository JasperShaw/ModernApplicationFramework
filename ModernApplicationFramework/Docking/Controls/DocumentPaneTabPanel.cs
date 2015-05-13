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
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class DocumentPaneTabPanel : Panel
    {
        public DocumentPaneTabPanel()
        {
            FlowDirection = FlowDirection.LeftToRight;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
	        Size desideredSize = new Size();
            foreach (FrameworkElement child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    desideredSize.Width += child.DesiredSize.Width;

                desideredSize.Height = Math.Max(desideredSize.Height, child.DesiredSize.Height);
            }

            return new Size(Math.Min(desideredSize.Width, availableSize.Width), desideredSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            restart:
            var visibleChildren = Children.Cast<UIElement>().Where(ch => ch.Visibility != Visibility.Collapsed);
            double offset = 0.0;
            bool skipAllOthers = false;
            foreach (var uiElement in visibleChildren)
            {
	            var doc = (TabItem) uiElement;
	            var layoutContent = doc.Content as LayoutContent;
                if (skipAllOthers || offset + doc.DesiredSize.Width > finalSize.Width)
                {
                    if (layoutContent != null && layoutContent.IsSelected)
                    {
                        var parentContainer = layoutContent.Parent;
                        var parentSelector = layoutContent.Parent as ILayoutContentSelector;
                        var parentPane = layoutContent.Parent as ILayoutPane;
	                    if (parentSelector != null)
	                    {
		                    int contentIndex = parentSelector.IndexOf(layoutContent);
		                    if (contentIndex > 0 &&
		                        parentContainer.ChildrenCount > 1)
		                    {
			                    parentPane?.MoveChild(contentIndex, 0);
			                    parentSelector.SelectedContentIndex = 0;
			                    goto restart;
		                    }
	                    }
                    }
                    doc.Visibility = Visibility.Hidden;
                    skipAllOthers = true;
                }
                else
                {
                    doc.Visibility = Visibility.Visible;
                    doc.Arrange(new Rect(offset, 0.0, doc.DesiredSize.Width, finalSize.Height));
                    offset += doc.ActualWidth + doc.Margin.Left + doc.Margin.Right;
                }
            }

            return finalSize;

        }
    }
}
