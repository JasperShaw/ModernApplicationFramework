using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.CommandBar;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Controls
{
    internal class ToolboxTreeView : TreeView
    {
        public ToolboxTreeView()
        {
            RegisterCommandHandlers(typeof(ToolboxTreeView));
        }

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
                if (element is TreeViewItem tvi)
                    tvi.IsExpanded = true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            switch (e.Key)
            {
                case Key.Delete:
                    //Required as the Edit Command is not used for Categories
                    if (SelectedItem is IToolboxCategory)
                    {
                        OnDelete();
                        e.Handled = true;
                    }      
                    break;
            }
            
        }

        private static void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
                e.CanExecute = tv.CanDelete();
        }

        private static void OnDelete(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is ToolboxTreeView tv)
            {
                tv.OnDelete();
                e.Handled = true;
            }
        }

        private static void RegisterCommandHandlers(Type controlType)
        {
            CommandHelpers.RegisterCommandHandler(controlType, EditingCommands.Delete, OnDelete, CanDelete, Key.Delete);
        }

        private bool CanDelete()
        {
            return SelectedItem is IToolboxItem;
        }

        private void OnDelete()
        {
            if (SelectedItem is IToolboxCategory)
                IoC.Get<DeleteActiveToolbarCategoryCommandDefinition>().Command.Execute(null);
            if (SelectedItem is IToolboxItem item)
                item.Parent?.RemoveItem(item);
        }
    }
}