using System.ComponentModel;
using System.Windows;
using ModernApplicationFramework.DragDrop;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxDropHandler : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {
            DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
            if (dropInfo.TargetItem is IToolboxItem &&
                dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
                dropInfo.Effects = DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            if (dropInfo.TargetCollection is ICollectionView view)
                view.Refresh();
        }
    }
}