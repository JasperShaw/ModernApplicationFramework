using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Controls
{
    internal class ToolboxTreeViewItem : TreeViewItem
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
                return;
            switch (e.Key)
            {
                case Key.Right:
                case Key.Left:
                    if (!IsLogicalLeft(e.Key))
                    {
                        if (DataContext is IToolboxItem)
                            break;
                        if (!IsExpanded)
                        {
                            IsExpanded = true;
                            e.Handled = true;
                            break;
                        }
                        HandleLogicalRight();
                        e.Handled = true;
                        break;
                    }
                    if (DataContext is IToolboxItem)
                        break;
                    if (IsExpanded)
                        IsExpanded = false;
                    else
                        HandleLogicalLeft();
                    e.Handled = true;
                    break;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ToolboxTreeViewItem;
        }

        private bool IsLogicalLeft(Key key)
        {
            if (FlowDirection != FlowDirection.LeftToRight)
                return key == Key.Right;
            return key == Key.Left;
        }

        private void HandleLogicalRight()
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
        }

        private void HandleLogicalLeft()
        {
            var parent = this.FindAncestorOrSelf<TreeView>();
            ((UIElement)parent?.ItemContainerGenerator.ContainerFromItem(parent))?.Focus();
        }
    }
}