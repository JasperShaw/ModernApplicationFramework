using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace ModernApplicationFramework.DragDrop.Utilities
{
    public static class ItemsControlExtensions
    {
        public static CollectionViewGroup FindGroup(this ItemsControl itemsControl, Point position)
        {
            if (itemsControl.Items.Groups == null || itemsControl.Items.Groups.Count == 0)
            {
                return null;
            }

            if (!(itemsControl.InputHitTest(position) is DependencyObject element))
                return null;
            var groupItem = element.GetVisualAncestor<GroupItem>();

            // drag after last item - get group of it
            if (groupItem == null && itemsControl.Items.Count > 0)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(itemsControl.Items.GetItemAt(itemsControl.Items.Count - 1)) is FrameworkElement lastItem)
                {
                    var itemEndpoint = lastItem.PointToScreen(new Point(lastItem.ActualWidth, lastItem.ActualHeight));
                    var positionToScreen = itemsControl.PointToScreen(position);
                    switch (itemsControl.GetItemsPanelOrientation())
                    {
                        case Orientation.Horizontal:
                            // assume LeftToRight
                            groupItem = itemEndpoint.X <= positionToScreen.X ? lastItem.GetVisualAncestor<GroupItem>() : null;
                            break;
                        case Orientation.Vertical:
                            groupItem = itemEndpoint.Y <= positionToScreen.Y ? lastItem.GetVisualAncestor<GroupItem>() : null;
                            break;
                    }
                }
            }
            return groupItem?.Content as CollectionViewGroup;
        }

        public static bool CanSelectMultipleItems(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector)
            {
                // The CanSelectMultipleItems property is protected. Use reflection to
                // get its value anyway.
                return (bool)itemsControl.GetType()
                                         .GetProperty("CanSelectMultipleItems", BindingFlags.Instance | BindingFlags.NonPublic)
                                         .GetValue(itemsControl, null);
            }
            if (itemsControl is ListBox box)
            {
                return box.SelectionMode != SelectionMode.Single;
            }
            return false;
        }

        public static UIElement GetItemContainer(this ItemsControl itemsControl, DependencyObject child)
        {
            var itemType = GetItemContainerType(itemsControl, out var isItemContainer);

            if (itemType != null)
            {
                return isItemContainer
                    ? (UIElement)child.GetVisualAncestor(itemType, itemsControl)
                    : (UIElement)child.GetVisualAncestor(itemType, itemsControl, itemsControl.GetType());
            }

            return null;
        }

        public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position)
        {
            var inputElement = itemsControl.InputHitTest(position);

            if (inputElement is UIElement uiElement)
            {
                return GetItemContainer(itemsControl, uiElement);
            }

            // ContentElement's such as Run's within TextBlock's could not be used as drop target items, because they are not UIElement's.
            if (inputElement is ContentElement contentElement)
            {
                return GetItemContainer(itemsControl, contentElement);
            }

            return null;
        }

        public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position, Orientation searchDirection)
        {
            var itemContainerType = GetItemContainerType(itemsControl, out var isItemContainer);

            Geometry hitTestGeometry;

            if (typeof(TreeViewItem).IsAssignableFrom(itemContainerType))
            {
                hitTestGeometry = new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y));
            }
            else
            {
                var geometryGroup = new GeometryGroup();
                geometryGroup.Children.Add(new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y)));
                geometryGroup.Children.Add(new LineGeometry(new Point(position.X, 0), new Point(position.X, itemsControl.RenderSize.Height)));
                hitTestGeometry = geometryGroup;
            }

            var hits = new HashSet<DependencyObject>();

            VisualTreeHelper.HitTest(itemsControl,
                                     obj =>
                                     {
                                         // Viewport3D is not good for us
                                         // Stop on ScrollBar to improve performance (e.g. at DataGrid)
                                         if (obj is Viewport3D || (itemsControl is DataGrid && obj is ScrollBar))
                                         {
                                             return HitTestFilterBehavior.Stop;
                                         }
                                         return HitTestFilterBehavior.Continue;
                                     },
                                     result =>
                                     {
                                         var itemContainer = isItemContainer
                                             ? result.VisualHit.GetVisualAncestor(itemContainerType, itemsControl)
                                             : result.VisualHit.GetVisualAncestor(itemContainerType, itemsControl, itemsControl.GetType());
                                         if (itemContainer != null && ((UIElement)itemContainer).IsVisible)
                                         {
                                             if (itemContainer is TreeViewItem tvItem)
                                             {
                                                 var tv = tvItem.GetVisualAncestor<TreeView>();
                                                 if (tv == itemsControl)
                                                 {
                                                     hits.Add(itemContainer);
                                                 }
                                             }
                                             else
                                             {
                                                 if (itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) >= 0)
                                                 {
                                                     hits.Add(itemContainer);
                                                 }
                                             }
                                         }
                                         return HitTestResultBehavior.Continue;
                                     },
                                     new GeometryHitTestParameters(hitTestGeometry));

            return GetClosest(itemsControl, hits, position, searchDirection);
        }

        public static Type GetItemContainerType(this ItemsControl itemsControl, out bool isItemContainer)
        {
            // determines if the itemsControl is not a ListView, ListBox or TreeView
            isItemContainer = false;

            if (itemsControl is TabControl)
            {
                return typeof(TabItem);
            }

            if (itemsControl is DataGrid)
            {
                return typeof(DataGridRow);
            }

            // There is no safe way to get the item container type for an ItemsControl. 
            // First hard-code the types for the common ItemsControls.
            if (itemsControl is ListView)
            {
                return typeof(ListViewItem);
            }
            if (itemsControl is ListBox)
            {
                return typeof(ListBoxItem);
            }
            if (itemsControl is TreeView)
            {
                return typeof(TreeViewItem);
            }

            // Otherwise look for the control's ItemsPresenter, get it's child panel and the first 
            // child of that *should* be an item container.
            //
            // If the control currently has no items, we're out of luck.
            if (itemsControl.Items.Count > 0)
            {
                var itemsPresenters = itemsControl.GetVisualDescendents<ItemsPresenter>();

                foreach (var itemsPresenter in itemsPresenters)
                {
                    if (VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
                    {
                        var panel = VisualTreeHelper.GetChild(itemsPresenter, 0);
                        var itemContainer = VisualTreeHelper.GetChildrenCount(panel) > 0
                            ? VisualTreeHelper.GetChild(panel, 0)
                            : null;

                        // Ensure that this actually *is* an item container by checking it with
                        // ItemContainerGenerator.
                        if (itemContainer != null &&
                            !(itemContainer is GroupItem) &&
                            itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1)
                        {
                            isItemContainer = true;
                            return itemContainer.GetType();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the Orientation which will be used for the drag drop action.
        /// Normally it will be look up to find the correct orientaion of the inner ItemsPanel,
        /// but sometimes it's necessary to force the oreintation, if the look up is wrong.
        /// If so, the ItemsPanelOrientation value is taken.
        /// </summary>
        /// <param name="itemsControl">The ItemsControl for the look up.</param>
        /// <returns>Orientation for the given ItemsControl.</returns>
        public static Orientation GetItemsPanelOrientation(this ItemsControl itemsControl)
        {
            var itemsPanelOrientation = DragDrop.GetItemsPanelOrientation(itemsControl);
            if (itemsPanelOrientation.HasValue)
            {
                return itemsPanelOrientation.Value;
            }

            if (itemsControl is TabControl tabControl)
            {
                //HitTestUtilities.HitTest4Type<TabPanel>(sender, elementPosition)
                //var tabPanel = itemsControl.GetVisualDescendent<TabPanel>();
                return tabControl.TabStripPlacement == Dock.Left || tabControl.TabStripPlacement == Dock.Right ? Orientation.Vertical : Orientation.Horizontal;
            }

            var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>() ?? itemsControl.GetVisualDescendent<ScrollContentPresenter>() as UIElement;
            if (itemsPresenter != null && VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
            {
                var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
                var orientationProperty = itemsPanel.GetType().GetProperty("Orientation", typeof(Orientation));
                if (orientationProperty != null)
                {
                    return (Orientation)orientationProperty.GetValue(itemsPanel, null);
                }
            }

            // Make a guess!
            return Orientation.Vertical;
        }

        /// <summary>
        /// Gets the FlowDirection which will be used for the drag drop action.
        /// </summary>
        /// <param name="itemsControl">The ItemsControl for the look up.</param>
        /// <returns>FlowDirection for the given ItemsControl.</returns>
        public static FlowDirection GetItemsPanelFlowDirection(this ItemsControl itemsControl)
        {
            var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>() ?? itemsControl.GetVisualDescendent<ScrollContentPresenter>() as UIElement;
            if (itemsPresenter != null && VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
            {
                var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
                var flowDirectionProperty = itemsPanel.GetType().GetProperty("FlowDirection", typeof(FlowDirection));
                if (flowDirectionProperty != null)
                {
                    return (FlowDirection)flowDirectionProperty.GetValue(itemsPanel, null);
                }
            }

            // Make a guess!
            return FlowDirection.LeftToRight;
        }

        /// <summary>
        /// Sets the given object as selected item at the ItemsControl.
        /// </summary>
        /// <param name="itemsControl">The ItemsControl which contains the item.</param>
        /// <param name="item">The object which should be selected.</param>
        public static void SetSelectedItem(this ItemsControl itemsControl, object item)
        {
            if (itemsControl is MultiSelector selector)
            {
                selector.SelectedItem = null;
                selector.SelectedItem = item;
            }
            else if (itemsControl is ListBox box)
            {
                var selectionMode = box.SelectionMode;
                try
                {
                    // change SelectionMode for UpdateAnchorAndActionItem
                    box.SelectionMode = SelectionMode.Single;
                    box.SelectedItem = null;
                    box.SelectedItem = item;
                }
                finally
                {
                    box.SelectionMode = selectionMode;
                }
            }
            else if (itemsControl is TreeViewItem viewItem)
            {
                // clear old selected item
                var treeView = ItemsControl.ItemsControlFromItemContainer(viewItem);
                var prevSelectedItem = treeView?.GetValue(TreeView.SelectedItemProperty);
                if (prevSelectedItem != null)
                {
                    if (treeView.ItemContainerGenerator.ContainerFromItem(prevSelectedItem) is TreeViewItem prevSelectedTreeViewItem)
                    {
                        prevSelectedTreeViewItem.IsSelected = false;
                    }
                }
                // set new selected item
                // TreeView.SelectedItemProperty is a read only property, so we must set the selection on the TreeViewItem itself
                if (viewItem.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem)
                {
                    treeViewItem.IsSelected = true;
                }
            }
            else if (itemsControl is TreeView view)
            {
                // clear old selected item
                var prevSelectedItem = view.GetValue(TreeView.SelectedItemProperty);
                if (prevSelectedItem != null)
                {
                    if (view.ItemContainerGenerator.ContainerFromItem(prevSelectedItem) is TreeViewItem prevSelectedTreeViewItem)
                    {
                        prevSelectedTreeViewItem.IsSelected = false;
                    }
                }
                // set new selected item
                // TreeView.SelectedItemProperty is a read only property, so we must set the selection on the TreeViewItem itself
                if (view.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem)
                {
                    treeViewItem.IsSelected = true;
                }
            }
            else if (itemsControl is Selector selector1)
            {
                selector1.SelectedItem = null;
                selector1.SelectedItem = item;
            }
        }

        /// <summary>
        /// Clears the selected items.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        public static void ClearSelectedItems(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector selector)
            {
                if (selector.CanSelectMultipleItems())
                {
                    selector.SelectedItems.Clear();
                }
                selector.SelectedItem = null;
            }
            else if (itemsControl is ListBox box)
            {
                box.SelectedItems.Clear();
                box.SelectedItem = null;
            }
            else if (itemsControl is TreeViewItem item)
            {
                var treeView = ItemsControl.ItemsControlFromItemContainer(item);
                treeView?.ClearSelectedItems();
            }
            else if (itemsControl is TreeView view)
            {
                // clear old selected item
                var prevSelectedItem = view.GetValue(TreeView.SelectedItemProperty);
                if (prevSelectedItem != null)
                {
                    if (view.ItemContainerGenerator.ContainerFromItem(prevSelectedItem) is TreeViewItem prevSelectedTreeViewItem)
                    {
                        prevSelectedTreeViewItem.IsSelected = false;
                    }
                }
            }
            else if (itemsControl is Selector selector1)
            {
                selector1.SelectedItem = null;
            }
        }

        public static object GetSelectedItem(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector selector)
            {
                return selector.SelectedItem;
            }
            if (itemsControl is ListBox box)
            {
                return box.SelectedItem;
            }
            if (itemsControl is TreeView view)
            {
                return view.GetValue(TreeView.SelectedItemProperty);
            }
            if (itemsControl is Selector selector1)
            {
                return selector1.SelectedItem;
            }

            return null;
        }

        public static IEnumerable GetSelectedItems(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector selector)
            {
                return selector.SelectedItems;
            }

            if (itemsControl is ListBox listBox)
            {
                if (listBox.SelectionMode == SelectionMode.Single)
                {
                    return Enumerable.Repeat(listBox.SelectedItem, 1);
                }

                return listBox.SelectedItems;
            }
            //else if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))

            if (itemsControl is TreeView view)
            {
                return Enumerable.Repeat(view.SelectedItem, 1);
            }
            //else if (itemsControl.GetType().IsAssignableFrom(typeof(Selector)))
            if (itemsControl is Selector selector1)
            {
                return Enumerable.Repeat(selector1.SelectedItem, 1);
            }
            return Enumerable.Empty<object>();
        }

        public static bool GetItemSelected(this ItemsControl itemsControl, object item)
        {
            if (itemsControl is MultiSelector selector)
            {
                return selector.SelectedItems.Contains(item);
            }
            if (itemsControl is ListBox box)
            {
                return box.SelectedItems.Contains(item);
            }
            if (itemsControl is TreeView view)
            {
                return view.SelectedItem == item;
            }
            if (itemsControl is Selector selector1)
            {
                return selector1.SelectedItem == item;
            }
            return false;
        }

        public static void SetItemSelected(this ItemsControl itemsControl, object item, bool itemSelected)
        {
            if (itemsControl is MultiSelector multiSelector)
            {
                if (multiSelector.CanSelectMultipleItems())
                {
                    if (itemSelected)
                    {
                        multiSelector.SelectedItems.Add(item);
                    }
                    else
                    {
                        multiSelector.SelectedItems.Remove(item);
                    }
                }
                else
                {
                    multiSelector.SelectedItem = null;
                    if (itemSelected)
                    {
                        multiSelector.SelectedItem = item;
                    }
                }
            }
            else if (itemsControl is ListBox listBox)
            {
                if (listBox.SelectionMode != SelectionMode.Single)
                {
                    if (itemSelected)
                    {
                        listBox.SelectedItems.Add(item);
                    }
                    else
                    {
                        listBox.SelectedItems.Remove(item);
                    }
                }
                else
                {
                    listBox.SelectedItem = null;
                    if (itemSelected)
                    {
                        listBox.SelectedItem = item;
                    }
                }
            }
            else
            {
                itemsControl.SetSelectedItem(itemSelected ? item : null);
            }
        }

        private static UIElement GetClosest(ItemsControl itemsControl, IEnumerable<DependencyObject> items,
                                            Point position, Orientation searchDirection)
        {
            //Console.WriteLine("GetClosest - {0}", itemsControl.ToString());

            UIElement closest = null;
            var closestDistance = double.MaxValue;

            foreach (var i in items)
            {
                if (i is UIElement uiElement)
                {
                    var p = uiElement.TransformToAncestor(itemsControl).Transform(new Point(0, 0));
                    var distance = double.MaxValue;

                    if (itemsControl is TreeView)
                    {
                        var xDiff = position.X - p.X;
                        var yDiff = position.Y - p.Y;
                        var hyp = Math.Sqrt(Math.Pow(xDiff, 2d) + Math.Pow(yDiff, 2d));
                        distance = Math.Abs(hyp);
                    }
                    else
                    {
                        var itemParent = ItemsControl.ItemsControlFromItemContainer(uiElement);
                        if (itemParent != null && itemParent != itemsControl)
                        {
                            searchDirection = itemParent.GetItemsPanelOrientation();
                        }
                        switch (searchDirection)
                        {
                            case Orientation.Horizontal:
                                distance = position.X <= p.X ? p.X - position.X : position.X - uiElement.RenderSize.Width - p.X;
                                break;
                            case Orientation.Vertical:
                                distance = position.Y <= p.Y ? p.Y - position.Y : position.Y - uiElement.RenderSize.Height - p.Y;
                                break;
                        }
                    }

                    if (distance < closestDistance)
                    {
                        closest = uiElement;
                        closestDistance = distance;
                    }
                }
            }

            return closest;
        }
    }
}
