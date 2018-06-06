using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Controls
{
    internal class ToolboxTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ToolboxTreeViewItem;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            var dragData = e.Data.GetData(DragDrop.DragDrop.DataFormat.Name);
            if (dragData is IToolboxCategory)
                return;

            if (this.IsDragElementUnderLastTreeItem(e, out var element))
            {
                if (element is TreeViewItem tvi)
                    tvi.IsExpanded = true;
            }
        }
    }
}
