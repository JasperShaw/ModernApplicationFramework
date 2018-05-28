using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ModernApplicationFramework.DragDrop.Utilities;

namespace ModernApplicationFramework.DragDrop
{
    /// <summary>
    /// Holds information about a the source of a drag drop operation.
    /// </summary>
    /// 
    /// <remarks>
    /// The <see cref="DragInfo"/> class holds all of the framework's information about the source
    /// of a drag. It is used by <see cref="IDragSource.StartDrag"/> to determine whether a drag 
    /// can start, and what the dragged data should be.
    /// </remarks>
    public class DragInfo : IDragInfo
    {
        /// <summary>
        /// Initializes a new instance of the DragInfo class.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The sender of the mouse event that initiated the drag.
        /// </param>
        /// 
        /// <param name="e">
        /// The mouse event that initiated the drag.
        /// </param>
        public DragInfo(object sender, MouseButtonEventArgs e)
        {
            Effects = DragDropEffects.None;
            MouseButton = e.ChangedButton;
            VisualSource = sender as UIElement;
            DragStartPosition = e.GetPosition(VisualSource);
            DragDropCopyKeyState = DragDrop.GetDragDropCopyKeyState(VisualSource);

            var dataFormat = DragDrop.GetDataFormat(VisualSource);
            if (dataFormat != null)
            {
                DataFormat = dataFormat;
            }

            var sourceElement = e.OriginalSource as UIElement;
            // If we can't cast object as a UIElement it might be a FrameworkContentElement, if so try and use its parent.
            if (sourceElement == null && e.OriginalSource is FrameworkContentElement element)
            {
                sourceElement = element.Parent as UIElement;
            }

            if (sender is ItemsControl itemsControl)
            {
                SourceGroup = itemsControl.FindGroup(DragStartPosition);
                VisualSourceFlowDirection = itemsControl.GetItemsPanelFlowDirection();

                UIElement item = null;
                if (sourceElement != null)
                {
                    item = itemsControl.GetItemContainer(sourceElement);
                }

                if (item == null)
                {
                    item = DragDrop.GetDragDirectlySelectedOnly(VisualSource) ? itemsControl.GetItemContainerAt(e.GetPosition(itemsControl)) : itemsControl.GetItemContainerAt(e.GetPosition(itemsControl), itemsControl.GetItemsPanelOrientation());
                }

                if (item != null)
                {
                    // Remember the relative position of the item being dragged
                    PositionInDraggedItem = e.GetPosition(item);

                    var itemParent = ItemsControl.ItemsControlFromItemContainer(item);

                    if (itemParent != null)
                    {
                        SourceCollection = itemParent.ItemsSource ?? itemParent.Items;
                        if (itemParent != itemsControl)
                        {
                            if (item is TreeViewItem tvItem)
                            {
                                var tv = tvItem.GetVisualAncestor<TreeView>();
                                if (tv != null && !Equals(tv, itemsControl) && !tv.IsDragSource())
                                {
                                    return;
                                }
                            }
                            else if (itemsControl.ItemContainerGenerator.IndexFromContainer(itemParent) < 0 && !itemParent.IsDragSource())
                            {
                                return;
                            }
                        }
                        SourceIndex = itemParent.ItemContainerGenerator.IndexFromContainer(item);
                        SourceItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    }
                    else
                    {
                        SourceIndex = -1;
                    }

                    var selectedItems = itemsControl.GetSelectedItems().OfType<object>().Where(i => i != CollectionView.NewItemPlaceholder).ToList();
                    SourceItems = selectedItems;

                    // Some controls (I'm looking at you TreeView!) haven't updated their
                    // SelectedItem by this point. Check to see if there 1 or less item in 
                    // the SourceItems collection, and if so, override the control's SelectedItems with the clicked item.
                    //
                    // The control has still the old selected items at the mouse down event, so we should check this and give only the real selected item to the user.
                    if (selectedItems.Count <= 1 || SourceItem != null && !selectedItems.Contains(SourceItem))
                    {
                        SourceItems = Enumerable.Repeat(SourceItem, 1);
                    }

                    VisualSourceItem = item;
                }
                else
                {
                    SourceCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                }
            }
            else
            {
                SourceItem = (sender as FrameworkElement)?.DataContext;
                if (SourceItem != null)
                {
                    SourceItems = Enumerable.Repeat(SourceItem, 1);
                }
                VisualSourceItem = sourceElement;
                PositionInDraggedItem = sourceElement != null ? e.GetPosition(sourceElement) : DragStartPosition;
            }

            if (SourceItems == null)
            {
                SourceItems = Enumerable.Empty<object>();
            }
        }

        internal void RefreshSelectedItems(object sender, MouseEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                var selectedItems = itemsControl.GetSelectedItems().OfType<object>().Where(i => i != CollectionView.NewItemPlaceholder).ToList();
                SourceItems = selectedItems;

                // Some controls (I'm looking at you TreeView!) haven't updated their
                // SelectedItem by this point. Check to see if there 1 or less item in 
                // the SourceItems collection, and if so, override the control's SelectedItems with the clicked item.
                //
                // The control has still the old selected items at the mouse down event, so we should check this and give only the real selected item to the user.
                if (selectedItems.Count <= 1 || SourceItem != null && !selectedItems.Contains(SourceItem))
                {
                    SourceItems = Enumerable.Repeat(SourceItem, 1);
                }
            }
        }

        /// <summary>
        /// Gets or sets the data format which will be used for the drag and drop actions.
        /// </summary>
        /// <value>The data format.</value>
        public DataFormat DataFormat { get; set; } = DragDrop.DataFormat;

        /// <summary>
        /// Gets or sets the drag data.
        /// </summary>
        /// 
        /// <remarks>
        /// This must be set by a drag handler in order for a drag to start.
        /// </remarks>
        public object Data { get; set; }

        /// <summary>
        /// Gets the position of the click that initiated the drag, relative to <see cref="VisualSource"/>.
        /// </summary>
        public Point DragStartPosition { get; }

        /// <summary>
        /// Gets the point where the cursor was relative to the item being dragged when the drag was started.
        /// </summary>
        public Point PositionInDraggedItem { get; }

        /// <summary>
        /// Gets or sets the allowed effects for the drag.
        /// </summary>
        /// 
        /// <remarks>
        /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drag handler in order 
        /// for a drag to start.
        /// </remarks>
        public DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets the mouse button that initiated the drag.
        /// </summary>
        public MouseButton MouseButton { get; }

        /// <summary>
        /// Gets the collection that the source ItemsControl is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public IEnumerable SourceCollection { get; }

        /// <summary>
        /// Gets the position from where the item was dragged.
        /// </summary>
        /// <value>The index of the source.</value>
        public int SourceIndex { get; }

        /// <summary>
        /// Gets the object that a dragged item is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initated the drag is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public object SourceItem { get; }

        /// <summary>
        /// Gets a collection of objects that the selected items in an ItemsControl are bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initated the drag is unbound or not an ItemsControl, this will be empty.
        /// </remarks>
        public IEnumerable SourceItems { get; private set; }

        /// <summary>
        /// Gets the group from a dragged item if the drag is currently from an ItemsControl with groups.
        /// </summary>
        public CollectionViewGroup SourceGroup { get; }

        /// <summary>
        /// Gets the control that initiated the drag.
        /// </summary>
        public UIElement VisualSource { get; }

        /// <summary>
        /// Gets the item in an ItemsControl that started the drag.
        /// </summary>
        /// 
        /// <remarks>
        /// If the control that initiated the drag is an ItemsControl, this property will hold the item
        /// container of the clicked item. For example, if <see cref="VisualSource"/> is a ListBox this
        /// will hold a ListBoxItem.
        /// </remarks>
        public UIElement VisualSourceItem { get; }

        /// <summary>
        /// Gets the FlowDirection of the current drag source.
        /// </summary>
        public FlowDirection VisualSourceFlowDirection { get; }

        /// <summary>
        /// Gets the <see cref="IDataObject"/> which is used by the drag and drop operation. Set it to
        /// a custom instance if custom drag and drop behavior is needed.
        /// </summary>
        public IDataObject DataObject { get; set; }

        /// <summary>
        /// Gets the drag drop copy key state indicating the effect of the drag drop operation.
        /// </summary>
        public DragDropKeyStates DragDropCopyKeyState { get; }
    }
}
