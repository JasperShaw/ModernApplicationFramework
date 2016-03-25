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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class DocumentPaneTabPanel : Panel
    {
        private int _indexOfFirstVisibleElement;
        private int _indexOfLastVisibleElement;
        private Size _oldFinalSize;
        private int _selectedIndex;

        public DocumentPaneTabPanel()
        {
            FlowDirection = FlowDirection.LeftToRight;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var tabs = Children.Cast<TabItem>().ToList();
            var offset = 0.0;

            var selectedItem = tabs.Where(item => item.Content is LayoutContent).FirstOrDefault(item => item.IsSelected);
            var selectedIndex = Children.IndexOf(selectedItem);

            var indexOfFirstVisibleElement = 0;
            var indexOfLastVisibleElement = 0;

            // check visible range of tab items
            foreach (var tab in tabs)
            {
                var desiredWidth = Math.Min(tab.ActualWidth, tab.DesiredSize.Width) + tab.Margin.Left + tab.Margin.Right;
                if (offset + desiredWidth > finalSize.Width)
                {
                    if (indexOfLastVisibleElement > selectedIndex)
                    {
                        indexOfLastVisibleElement--;
                        break;
                    }
                }

                //remove leading elements until current element comes to view
                while (offset + desiredWidth > finalSize.Width)
                {
                    offset -=
                        Math.Min(tabs[indexOfFirstVisibleElement].ActualWidth,
                            tabs[indexOfFirstVisibleElement].DesiredSize.Width) -
                        tabs[indexOfFirstVisibleElement].Margin.Left - tabs[indexOfFirstVisibleElement].Margin.Right;

                    if (indexOfFirstVisibleElement <= selectedIndex && indexOfLastVisibleElement >= selectedIndex)
                        break;
                }
                offset += desiredWidth;
                indexOfLastVisibleElement++;
            }

            // don't move the visible range, if not necessary. User can select a item in the middle
            if (selectedIndex >= _indexOfFirstVisibleElement && selectedIndex <= _indexOfLastVisibleElement &&
                _oldFinalSize.Width == finalSize.Width && _selectedIndex != selectedIndex)
            {
                indexOfFirstVisibleElement = _indexOfFirstVisibleElement;
                indexOfLastVisibleElement = _indexOfLastVisibleElement;
            }

            ShowHideTabs(tabs, finalSize, indexOfFirstVisibleElement, indexOfLastVisibleElement, selectedIndex);

            _indexOfFirstVisibleElement = indexOfFirstVisibleElement;
            _indexOfLastVisibleElement = indexOfLastVisibleElement;
            _oldFinalSize = finalSize;
            _selectedIndex = selectedIndex;

            return finalSize;
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

        private static void ShowHideTabs(IList<TabItem> tabs, Size finalSize, int indexOfFirstVisibleElement,
            int indexOfLastVisibleElement, int selectedIndex)
        {
            var offset = 0.0;
            for (var i = 0; i < tabs.Count; i++)
            {
                if (i < indexOfFirstVisibleElement || i > indexOfLastVisibleElement)
                {
                    // have to be hidden, not collapsed. If collapsed the ArrangeOverride function will be called endless!
                    tabs[i].Visibility = Visibility.Hidden;
                    continue;
                }
                if (indexOfFirstVisibleElement != indexOfLastVisibleElement)
                {
                    tabs[i].Visibility = Visibility.Visible;
                    tabs[i].Arrange(new Rect(offset, 0.0, tabs[i].DesiredSize.Width, finalSize.Height));
                    offset += Math.Min(tabs[i].ActualWidth, tabs[i].DesiredSize.Width) + tabs[i].Margin.Left +
                              tabs[i].Margin.Right;
                }
            }

            //Make visible of selected tab as first item if the space is less instead of hiding.            
            var chkHideAllTab = tabs.Any(x => x.Visibility == Visibility.Visible);
            var tab = tabs.FirstOrDefault(x => x.IsSelected);
            if (!chkHideAllTab && tabs.Count >= 1 || indexOfFirstVisibleElement == indexOfLastVisibleElement ||
                tabs[selectedIndex].DesiredSize.Width >= finalSize.Width)
            {
                if (tab != null)
                {
                    tabs[selectedIndex].Arrange(new Rect(0.0, 0.0, tabs[selectedIndex].DesiredSize.Width,
                        finalSize.Height));
                    tabs[selectedIndex].Visibility = Visibility.Visible;
                }
            }
        }
    }
}