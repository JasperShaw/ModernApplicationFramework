using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.DragDrop.Utilities
{
    public static class RootElementFinder
    {
        public static UIElement FindRoot(DependencyObject visual)
        {
            var parentWindow = Window.GetWindow(visual);
            var rootElement = parentWindow?.Content as UIElement;
            if (rootElement == null)
            {
                if (Application.Current != null && Application.Current.MainWindow != null)
                {
                    rootElement = Application.Current.MainWindow.Content as UIElement;
                }
                if (rootElement == null)
                {
                    rootElement = visual.GetVisualAncestor<Page>() ?? visual.GetVisualAncestor<UserControl>() as UIElement;
                }
            }
            return rootElement;
        }
    }
}
