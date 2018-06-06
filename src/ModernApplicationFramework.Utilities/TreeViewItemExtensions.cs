using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApplicationFramework.Utilities
{
	public static class TreeViewItemExtensions
	{
		public static int GetDepth(this TreeViewItem item)
		{
			TreeViewItem parent;
			while ((parent = GetParent(item)) != null)
			{
				return GetDepth(parent) + 1;
			}
			return 0;
		}

		private static TreeViewItem GetParent(DependencyObject item)
		{
			var parent = VisualTreeHelper.GetParent(item);
			while (!(parent is TreeViewItem || parent is TreeView))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}
			return parent as TreeViewItem;
		}


	    public static bool IsDragElementUnderLastTreeItem(this ItemsControl control, DragEventArgs e, out FrameworkElement element)
	    {
	        element = null;
	        if (!control.HasItems)
	            return true;
	        var i = control.Items.GetItemAt(control.Items.Count - 1);
	        element = control.ItemContainerGenerator.ContainerFromItem(i) as FrameworkElement;
	        if (element == null)
	            return false;
	        var mp = e.GetPosition(element);
	        return mp.Y > element.ActualHeight * 1.25;
	    }
    }
}
