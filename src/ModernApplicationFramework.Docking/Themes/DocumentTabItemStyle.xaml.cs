using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Themes
{
    public partial class DocumentTabItemStyle
    {
        public DocumentTabItemStyle()
        {
            InitializeComponent();
        }

        private void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            UtilityMethods.OnTabItemDragOver(sender, e);
        }

        private void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement) || e.ChangedButton != MouseButton.Middle)
                return;
            var dataContext = frameworkElement.DataContext as LayoutContent;
            if (dataContext is LayoutAnchorable anchorable)
                anchorable.Hide();
            else
                dataContext?.Close();
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement) || e.ChangedButton != MouseButton.Left)
                return;
            if (!NativeMethods.NativeMethods.IsKeyPressed(17))
                return;
            var dataContext = frameworkElement.DataContext as LayoutContent;
            dataContext?.Float();
        }
    }
}
