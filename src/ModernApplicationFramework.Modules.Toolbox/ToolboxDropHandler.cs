using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Modules.Toolbox.Controls;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxDropHandler : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.IsSameDragDropContextAsSource)
            {
                //Restore original data
                dropInfo.Data = dropInfo.DragInfo.SourceItem;
            }
            DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);

            if (dropInfo.IsSameDragDropContextAsSource)
            {
                if (dropInfo.TargetItem == dropInfo.DragInfo.SourceItem)
                    dropInfo.Effects = DragDropEffects.None;
                else
                {
                    if (Equals(dropInfo.TargetCollection, dropInfo.DragInfo.SourceCollection))
                    {
                        var targetSource = dropInfo.TargetCollection.Cast<object>().ToList();
                        var indexTarget = targetSource.IndexOf(dropInfo.TargetItem);
                        var indexSource = targetSource.IndexOf(dropInfo.DragInfo.SourceItem);

                        if (indexTarget == -1)
                        {
                            if (indexSource == dropInfo.InsertIndex - 1 && (dropInfo.InsertPosition & RelativeInsertPosition.TargetItemCenter) == 0)
                            dropInfo.Effects = DragDropEffects.None;
                        }
                           

                        if (indexSource == indexTarget + 1 && dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem && (dropInfo.InsertPosition & RelativeInsertPosition.TargetItemCenter) == 0)
                            dropInfo.Effects = DragDropEffects.None;
                        if (indexSource == indexTarget -1 && dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem)
                            dropInfo.Effects = DragDropEffects.None;
                    }
                }
            }

            if (dropInfo.TargetItem is IToolboxItem &&
                dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
            {
                dropInfo.Effects = DragDropEffects.None;
                dropInfo.DropTargetAdorner = null;
            }
            if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
                dropInfo.DropTargetAdorner = null;
            else
            {
                dropInfo.DropTargetAdorner = dropInfo.Effects == DragDropEffects.None ? null : typeof(ToolboxInsertAdorner);
            }

        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.IsSameDragDropContextAsSource)
            {
                //Restore original data
                dropInfo.Data = dropInfo.DragInfo.SourceItem;
            }

            DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            //FocusMovedElement(dropInfo);
        }

        //private void FocusMovedElement(IDropInfo dropInfo)
        //{
        //    var targetSource = dropInfo.TargetCollection.Cast<object>().ToList();
        //    try
        //    {
        //        var insertIndex = dropInfo.InsertIndex;
        //        if (dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem &&
        //            Equals(dropInfo.TargetCollection, dropInfo.DragInfo.SourceCollection))
        //            insertIndex = --insertIndex;

        //        var itemModel = targetSource.ElementAt(insertIndex);

        //        if (!(dropInfo.VisualTarget is ToolboxTreeView tv))
        //            return;
        //        var item = ContainerFromItem(tv.ItemContainerGenerator, itemModel);
        //        if (item is UIElement uiElement)
        //            uiElement.Focus();
        //    }
        //    catch
        //    {
        //    }
        //}

        //private static TreeViewItem ContainerFromItem(ItemContainerGenerator containerGenerator, object item)
        //{
        //    var container = (TreeViewItem)containerGenerator.ContainerFromItem(item);
        //    if (container != null)
        //        return container;
        //    foreach (var childItem in containerGenerator.Items)
        //    {
        //        var parent = containerGenerator.ContainerFromItem(childItem) as TreeViewItem;
        //        if (parent == null)
        //            continue;
        //        container = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
        //        if (container != null)
        //            return container;
        //        container = ContainerFromItem(parent.ItemContainerGenerator, item);
        //        if (container != null)
        //            return container;
        //    }
        //    return null;
        //}
    }
}