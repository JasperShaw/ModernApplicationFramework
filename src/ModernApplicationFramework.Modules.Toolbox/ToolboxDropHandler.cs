using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Modules.Toolbox.Controls;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Imaging;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxDropHandler : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {

            dropInfo.DropTargetAdorner = null;
            var stringFlag = false;

            if (IsTextObjectInDragSource(dropInfo, out var dataObject))
            {
                dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                dropInfo.DropTargetAdorner = typeof(ToolboxInsertAdorner);
                dropInfo.Data = new ToolboxItem("Text1223", dataObject, new[] { typeof(object) }, new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFramework.Modules.Toolbox;component/text.png")));




                //FrameworkElement visual;
                //using (var stream = GetType().Assembly
                //    .GetManifestResourceStream("ModernApplicationFramework.Modules.Toolbox.TextFile_16x2.xaml"))
                //{
                //    using (var stringReader = new StreamReader(stream))
                //    {
                //        using (var xmlReader = new XmlTextReader(stringReader))
                //        {
                //            visual = (FrameworkElement) XamlReader.Load(xmlReader);
                //        }
                //    }
                //}
                //dropInfo.Data = new ToolboxItem("Text1223", dataObject, new[] { typeof(object) }, ImageUtilities.FrameworkElementToBitmapSource(visual));

                stringFlag = true;
            }

            if (dropInfo.IsSameDragDropContextAsSource && !stringFlag)
            {
                //Restore original data
                dropInfo.Data = dropInfo.DragInfo.SourceItem;
            }


            DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);

            if (!CanDropToolboxItem(dropInfo, dropInfo.Data as IToolboxItem))
                dropInfo.Effects = DragDropEffects.None;

            if (dropInfo.IsSameDragDropContextAsSource)
            {
                if (dropInfo.DragInfo != null && dropInfo.TargetItem == dropInfo.DragInfo.SourceItem)
                    dropInfo.Effects = DragDropEffects.None;
                else
                {
                    if (!stringFlag && Equals(dropInfo.TargetCollection, dropInfo.DragInfo.SourceCollection))
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
                        if (indexSource == indexTarget - 1 && dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem)
                            dropInfo.Effects = DragDropEffects.None;
                    }
                }
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
            bool stringFlag = false;
            if (dropInfo.DragInfo == null && dropInfo.Data is IToolboxItem item && dropInfo.IsSameDragDropContextAsSource)
            {
                if (!item.Data.GetDataPresent(DataFormats.Text))
                    return;
                stringFlag = true;
            }
            if (dropInfo.IsSameDragDropContextAsSource && !stringFlag)
            {
                //Restore original data
                dropInfo.Data = dropInfo.DragInfo.SourceItem;
            }
            if (stringFlag)
            {
                var destinationList = dropInfo.TargetCollection.TryGetList();
                if (destinationList != null)
                {
                    destinationList.Insert(dropInfo.InsertIndex, dropInfo.Data);

                    var selectDroppedItems = DragDrop.DragDrop.GetSelectDroppedItems(dropInfo.VisualTarget);
                    if (selectDroppedItems)
                    {
                        DefaultDropHandler.SelectDroppedItems(dropInfo, new List<object> { dropInfo.Data });
                    }
                }
            }
            else
            {
                DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }

        }

        public static bool CanDropToolboxItem(IDropInfo dropInfo, IToolboxItem item)
        {
            if (dropInfo.TargetItem == null && item != null)
                return false;
            if (dropInfo.TargetItem is IToolboxCategory && item != null &&
                !dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
                return false;
            if (dropInfo.TargetItem is IToolboxItem &&
                dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
                return false;
            return true;
        }

        public static bool IsTextObjectInDragSource(IDropInfo dropInfo, out IDataObject stringData)
        {
            stringData = null;
            if (dropInfo.DragInfo == null && dropInfo.Data is IDataObject dataObject &&
                dropInfo.IsSameDragDropContextAsSource)
            {
                if (!dataObject.GetDataPresent(DataFormats.Text))
                    return false;
                stringData = dataObject;
            }
            return true;
        }
    }
}