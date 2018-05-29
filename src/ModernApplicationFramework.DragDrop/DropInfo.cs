using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using JetBrains.Annotations;
using ModernApplicationFramework.DragDrop.Utilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.DragDrop
{
    /// <summary>
    /// Holds information about a the target of a drag drop operation.
    /// </summary>
    /// 
    /// <remarks>
    /// The <see cref="DropInfo"/> class holds all of the framework's information about the current 
    /// target of a drag. It is used by <see cref="IDropTarget.DragOver"/> method to determine whether 
    /// the current drop target is valid, and by <see cref="IDropTarget.Drop"/> to perform the drop.
    /// </remarks>
    public class DropInfo : IDropInfo
    {
        private readonly ItemsControl _itemParent;
        private readonly UIElement _item;

        /// <summary>
        /// Initializes a new instance of the DropInfo class.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The sender of the drag event.
        /// </param>
        /// 
        /// <param name="e">
        /// The drag event.
        /// </param>
        /// 
        /// <param name="dragInfo">
        /// Information about the source of the drag, if the drag came from within the framework.
        /// </param>
        public DropInfo(object sender, DragEventArgs e, [CanBeNull] IDragInfo dragInfo)
        {
            DragInfo = dragInfo;
            KeyStates = e.KeyStates;
            var dataFormat = dragInfo?.DataFormat;
            Data = dataFormat != null && e.Data.GetDataPresent(dataFormat.Name)
                ? e.Data.GetData(dataFormat.Name)
                : e.Data;

            VisualTarget = sender as UIElement;
            // if there is no drop target, find another
            if (!VisualTarget.IsDropTarget())
            {
                // try to find next element
                var element = VisualTarget.TryGetNextAncestorDropTargetElement();
                if (element != null)
                {
                    VisualTarget = element;
                }
            }

            // try find ScrollViewer
            if (VisualTarget is TabControl)
            {
                var tabPanel = VisualTarget.GetVisualDescendent<TabPanel>();
                TargetScrollViewer = tabPanel?.GetVisualAncestor<ScrollViewer>();
            }
            else
            {
                TargetScrollViewer = VisualTarget?.GetVisualDescendent<ScrollViewer>();
            }

            TargetScrollingMode =
                VisualTarget != null ? DragDrop.GetDropScrollingMode(VisualTarget) : ScrollingMode.Both;

            // visual target can be null, so give us a point...
            DropPosition = VisualTarget != null ? e.GetPosition(VisualTarget) : new Point();

            if (VisualTarget is TabControl)
            {
                if (!HitTestUtilities.HitTest4Type<TabPanel>(VisualTarget, DropPosition))
                {
                    return;
                }
            }

            if (VisualTarget is ItemsControl itemsControl)
            {
                //System.Diagnostics.Debug.WriteLine(">>> Name = {0}", itemsControl.Name);
                // get item under the mouse
                var item = itemsControl.GetItemContainerAt(DropPosition);
                var directlyOverItem = item != null;

                TargetGroup = itemsControl.FindGroup(DropPosition);
                VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();
                VisualTargetFlowDirection = itemsControl.GetItemsPanelFlowDirection();

                if (item == null)
                {
                    // ok, no item found, so maybe we can found an item at top, left, right or bottom
                    item = itemsControl.GetItemContainerAt(DropPosition, VisualTargetOrientation);
                    directlyOverItem = DropPosition.DirectlyOverElement(_item, itemsControl);
                }

                if (item == null && TargetGroup != null && TargetGroup.IsBottomLevel)
                {
                    var itemData = TargetGroup.Items.FirstOrDefault();
                    if (itemData != null)
                    {
                        item = itemsControl.ItemContainerGenerator.ContainerFromItem(itemData) as UIElement;
                        directlyOverItem = DropPosition.DirectlyOverElement(_item, itemsControl);
                    }
                }

                if (item != null)
                {
                    _itemParent = ItemsControl.ItemsControlFromItemContainer(item);
                    VisualTargetOrientation = _itemParent.GetItemsPanelOrientation();
                    VisualTargetFlowDirection = _itemParent.GetItemsPanelFlowDirection();

                    InsertIndex = _itemParent.ItemContainerGenerator.IndexFromContainer(item);
                    TargetCollection = _itemParent.ItemsSource ?? _itemParent.Items;

                    var tvItem = item as TreeViewItem;

                    if (directlyOverItem || tvItem != null)
                    {
                        VisualTargetItem = item;
                        TargetItem = _itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    }

                    var expandedTVItem = tvItem != null && tvItem.HasHeader && tvItem.HasItems && tvItem.IsExpanded;
                    var itemRenderSize = expandedTVItem ? tvItem.GetHeaderSize() : item.RenderSize;

                    if (VisualTargetOrientation == Orientation.Vertical)
                    {
                        var currentYPos = e.GetPosition(item).Y;
                        var targetHeight = itemRenderSize.Height;

                        var topGap = targetHeight * 0.25;
                        var bottomGap = targetHeight * 0.75;
                        if (currentYPos > targetHeight / 2)
                        {
                            if (expandedTVItem && (currentYPos < topGap || currentYPos > bottomGap))
                            {
                                VisualTargetItem = tvItem.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
                                TargetItem = VisualTargetItem != null
                                    ? tvItem.ItemContainerGenerator.ItemFromContainer(VisualTargetItem)
                                    : null;
                                TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                                InsertIndex = 0;
                                InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                            }
                            else
                            {
                                InsertIndex++;
                                InsertPosition = RelativeInsertPosition.AfterTargetItem;
                            }
                        }
                        else
                        {
                            InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                        }

                        if (currentYPos > topGap && currentYPos < bottomGap)
                        {
                            if (tvItem != null)
                            {
                                TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                                InsertIndex = TargetCollection?.OfType<object>().Count() ?? 0;
                            }

                            InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }

                        //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, idx={1}, Y={2}, Item={3}", this.InsertPosition, this.InsertIndex, currentYPos, item);
                    }
                    else
                    {
                        var currentXPos = e.GetPosition(item).X;
                        var targetWidth = itemRenderSize.Width;

                        if (VisualTargetFlowDirection == FlowDirection.RightToLeft)
                        {
                            if (currentXPos > targetWidth / 2)
                            {
                                InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                            }
                            else
                            {
                                InsertIndex++;
                                InsertPosition = RelativeInsertPosition.AfterTargetItem;
                            }
                        }
                        else if (VisualTargetFlowDirection == FlowDirection.LeftToRight)
                        {
                            if (currentXPos > targetWidth / 2)
                            {
                                InsertIndex++;
                                InsertPosition = RelativeInsertPosition.AfterTargetItem;
                            }
                            else
                            {
                                InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                            }
                        }

                        if (currentXPos > targetWidth * 0.25 && currentXPos < targetWidth * 0.75)
                        {
                            if (tvItem != null)
                            {
                                TargetCollection = tvItem.ItemsSource ?? tvItem.Items;
                                InsertIndex = TargetCollection?.OfType<object>().Count() ?? 0;
                            }

                            InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }

                        //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, idx={1}, X={2}, Item={3}", this.InsertPosition, this.InsertIndex, currentXPos, item);
                    }
                }
                else
                {
                    TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                    InsertIndex = itemsControl.Items.Count;
                    //System.Diagnostics.Debug.WriteLine("==> DropInfo: pos={0}, item=NULL, idx={1}", this.InsertPosition, this.InsertIndex);
                }
            }
            else
            {
                VisualTargetItem = VisualTarget;
            }
        }

        /// <summary>
        /// Gets or sets the drag data.
        /// </summary>
        /// 
        /// <remarks>
        /// If the drag came from within the framework, this will hold:
        /// 
        /// - The dragged data if a single item was dragged.
        /// - A typed IEnumerable if multiple items were dragged.
        /// </remarks>
        public object Data { get; set; }

        /// <summary>
        /// Gets a <see cref="DragInfo"/> object holding information about the source of the drag, 
        /// if the drag came from within the framework.
        /// </summary>
        public IDragInfo DragInfo { get; }

        /// <summary>
        /// Gets the mouse position relative to the VisualTarget
        /// </summary>
        public Point DropPosition { get; }

        /// <summary>
        /// Gets or sets the class of drop target to display.
        /// </summary>
        /// 
        /// <remarks>
        /// The standard drop target adorner classes are held in the <see cref="DropTargetAdorners"/>
        /// class.
        /// </remarks>
        public Type DropTargetAdorner { get; set; }

        /// <summary>
        /// Gets or sets the allowed effects for the drop.
        /// </summary>
        /// 
        /// <remarks>
        /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drop handler in order 
        /// for a drop to be possible.
        /// </remarks>
        public DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets the current insert position within <see cref="TargetCollection"/>.
        /// </summary>
        public int InsertIndex { get; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the current insert position within the source (unfiltered) <see cref="P:ModernApplicationFramework.DragDrop.DropInfo.TargetCollection" />.
        /// </summary>
        /// <remarks>
        /// This should be only used in a Drop action.
        /// This works only correct with different objects (string, int, etc won't work correct).
        /// </remarks>
        public int UnfilteredInsertIndex
        {
            get
            {
                var insertIndex = InsertIndex;
                var itemSourceAsList = _itemParent?.ItemsSource.TryGetList();
                if (itemSourceAsList != null && _itemParent.Items != null &&
                    _itemParent.Items.Count != itemSourceAsList.Count)
                {
                    if (insertIndex >= 0 && insertIndex < _itemParent.Items.Count)
                    {
                        var indexOf = itemSourceAsList.IndexOf(_itemParent.Items[insertIndex]);
                        if (indexOf >= 0)
                        {
                            return indexOf;
                        }
                    }
                    else if (_itemParent.Items.Count > 0 && insertIndex == _itemParent.Items.Count)
                    {
                        var indexOf = itemSourceAsList.IndexOf(_itemParent.Items[insertIndex - 1]);
                        if (indexOf >= 0)
                        {
                            return indexOf + 1;
                        }
                    }
                }

                return insertIndex;
            }
        }

        /// <summary>
        /// Gets the collection that the target ItemsControl is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public IEnumerable TargetCollection { get; }

        /// <summary>
        /// Gets the object that the current drop target is bound to.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public object TargetItem { get; }

        /// <summary>
        /// Gets the current group target.
        /// </summary>
        /// 
        /// <remarks>
        /// If the drag is currently over an ItemsControl with groups, describes the group that
        /// the drag is currently over.
        /// </remarks>
        public CollectionViewGroup TargetGroup { get; }

        /// <summary>
        /// Gets the ScrollViewer control for the visual target.
        /// </summary>
        public ScrollViewer TargetScrollViewer { get; }

        /// <summary>
        /// Gets or Sets the ScrollingMode for the drop action.
        /// </summary>
        public ScrollingMode TargetScrollingMode { get; set; }

        /// <summary>
        /// Gets the control that is the current drop target.
        /// </summary>
        public UIElement VisualTarget { get; }

        /// <summary>
        /// Gets the item in an ItemsControl that is the current drop target.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current drop target is unbound or not an ItemsControl, this will be null.
        /// </remarks>
        public UIElement VisualTargetItem { get; }

        /// <summary>
        /// Gets the orientation of the current drop target.
        /// </summary>
        public Orientation VisualTargetOrientation { get; }

        /// <summary>
        /// Gets the orientation of the current drop target.
        /// </summary>
        public FlowDirection VisualTargetFlowDirection { get; }

        /// <summary>
        /// Gets and sets the text displayed in the DropDropEffects adorner.
        /// </summary>
        public string DestinationText { get; set; }

        /// <summary>
        /// Gets and sets the effect text displayed in the DropDropEffects adorner.
        /// </summary>
        public string EffectText { get; set; }

        /// <summary>
        /// Gets the relative position the item will be inserted to compared to the TargetItem
        /// </summary>
        public RelativeInsertPosition InsertPosition { get; }

        /// <summary>
        /// Gets a flag enumeration indicating the current state of the SHIFT, CTRL, and ALT keys, as well as the state of the mouse buttons.
        /// </summary>
        public DragDropKeyStates KeyStates { get; }

        public bool NotHandled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the target is in the same context as the source, <see cref="ModernApplicationFramework.DragDrop.DragDropContextProperty" />.
        /// </summary>
        public bool IsSameDragDropContextAsSource
        {
            get
            {
                // Check if DragInfo stuff exists
                if (DragInfo?.VisualSource == null)
                {
                    return true;
                }

                // A target should be exists
                if (VisualTarget == null)
                {
                    return true;
                }

                // Source element has a drag context constraint, we need to check the target property matches.
                var sourceContext = DragInfo.VisualSource.GetValue(DragDrop.DragDropContextProperty) as string;
                if (String.IsNullOrEmpty(sourceContext))
                {
                    return true;
                }

                var targetContext = VisualTarget.GetValue(DragDrop.DragDropContextProperty) as string;
                return string.Equals(sourceContext, targetContext);
            }
        }
    }
}
