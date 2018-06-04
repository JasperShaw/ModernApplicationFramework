using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.DragDrop.Utilities;
using static ModernApplicationFramework.DragDrop.DragDrop;

namespace ModernApplicationFramework.Modules.Toolbox.Controls
{
    internal class ToolboxInsertAdorner : DropTargetAdorner
    {
        public ToolboxInsertAdorner(UIElement adornedElement) : base(adornedElement, null)
        {
        }

        public ToolboxInsertAdorner(UIElement adornedElement, DropInfo dropInfo) : base(adornedElement, dropInfo)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var dropInfo = DropInfo;

            if (dropInfo.VisualTarget is ItemsControl itemsControl)
            {
                // Get the position of the item at the insertion index. If the insertion point is
                // to be after the last item, then get the position of the last item and add an 
                // offset later to draw it at the end of the list.

                var visualTargetItem = dropInfo.VisualTargetItem;
                var itemParent = visualTargetItem != null ? ItemsControl.ItemsControlFromItemContainer(visualTargetItem) : itemsControl;

                // this could be happen with a thread scenario where items are removed very quickly
                if (itemParent == null)
                {
                    return;
                }

                var itemsCount = itemParent.Items.Count;
                var index = Math.Min(dropInfo.InsertIndex, itemsCount - 1);

                var lastItemInGroup = false;
                var targetGroup = dropInfo.TargetGroup;
                if (targetGroup != null && targetGroup.IsBottomLevel && dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem))
                {
                    var indexOf = targetGroup.Items.IndexOf(dropInfo.TargetItem);
                    lastItemInGroup = indexOf == targetGroup.ItemCount - 1;
                    if (lastItemInGroup && dropInfo.InsertIndex != itemsCount)
                    {
                        index--;
                    }
                }

                var itemContainer = (UIElement)itemParent.ItemContainerGenerator.ContainerFromIndex(index);

                var showAlwaysDropTargetAdorner = itemContainer == null && GetShowAlwaysDropTargetAdorner(itemParent);
                if (showAlwaysDropTargetAdorner)
                {
                    itemContainer = itemParent;
                }

                if (itemContainer != null)
                {
                    var itemRect = new Rect(itemContainer.TranslatePoint(new Point(), AdornedElement), itemContainer.RenderSize);
                    Point point1, point2;

                    var viewportWidth = DropInfo.TargetScrollViewer?.ViewportWidth ?? double.MaxValue;
                    var viewportHeight = DropInfo.TargetScrollViewer?.ViewportHeight ?? double.MaxValue;

                    if (dropInfo.VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (dropInfo.InsertIndex == itemsCount || lastItemInGroup)
                        {
                            if (itemsCount > 0)
                            {
                                itemRect.Y += itemContainer.RenderSize.Height;
                            }
                            else
                            {
                                if ((itemsControl as ListView)?.View is GridView)
                                {
                                    var header = itemsControl.GetVisualDescendent<GridViewHeaderRowPresenter>();
                                    if (header != null)
                                    {
                                        itemRect.Y += header.RenderSize.Height;
                                    }
                                }
                                else if (itemsControl is DataGrid)
                                {
                                    var header = itemsControl.GetVisualDescendent<DataGridColumnHeadersPresenter>();
                                    if (header != null)
                                    {
                                        itemRect.Y += header.RenderSize.Height;
                                    }
                                }
                                itemRect.Y += Pen.Thickness;
                            }
                        }

                        var itemRectRight = Math.Min(itemRect.Right, viewportWidth);
                        var itemRectLeft = itemRect.X -19 < 0 ? 0 : itemRect.X -19;
                        point1 = new Point(itemRectLeft, itemRect.Y);
                        point2 = new Point(itemRectRight, itemRect.Y);
                    }
                    else
                    {
                        if (dropInfo.VisualTargetFlowDirection == FlowDirection.LeftToRight && dropInfo.InsertIndex == itemsCount)
                        {
                            if (itemsCount > 0)
                            {
                                itemRect.X += itemContainer.RenderSize.Width;
                            }
                            else
                            {
                                itemRect.X += Pen.Thickness;
                            }
                        }
                        else if (dropInfo.VisualTargetFlowDirection == FlowDirection.RightToLeft && dropInfo.InsertIndex != itemsCount)
                        {
                            if (itemsCount > 0)
                            {
                                itemRect.X += itemContainer.RenderSize.Width;
                            }
                            else
                            {
                                itemRect.X += Pen.Thickness;
                            }
                        }

                        var itemRectTop = itemRect.Y < 0 ? 0 : itemRect.Y;
                        var itemRectBottom = Math.Min(itemRect.Bottom, viewportHeight);

                        point1 = new Point(itemRect.X, itemRectTop);
                        point2 = new Point(itemRect.X, itemRectBottom);
                    }
                    drawingContext.DrawLine(Pen, point1, point2);
                }
            }
        }
    }
}
