using System.ComponentModel;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxDropHandler : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
            if (dropInfo.TargetItem is IToolboxItem &&
                dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
                dropInfo.Effects = DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            if (dropInfo.TargetCollection is ICollectionView view)
                view.Refresh();
        }
    }
}