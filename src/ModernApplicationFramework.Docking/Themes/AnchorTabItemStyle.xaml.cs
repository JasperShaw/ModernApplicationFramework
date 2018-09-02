using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Themes
{
    public partial class AnchorTabItemStyle
    {
        public AnchorTabItemStyle()
        {
            InitializeComponent();
        }

        private void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            UtilityMethods.OnTabItemDragOver(sender, e);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement) || e.ChangedButton != MouseButton.Middle)
                return;
            var dataContext = frameworkElement.DataContext as LayoutAnchorable;
            dataContext?.Hide();
        }

        private void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
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
