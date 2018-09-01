using System.Windows.Data;
using ContextMenu = ModernApplicationFramework.Controls.Menu.ContextMenu;

namespace ModernApplicationFramework.Docking.Controls
{
    public class ContextMenuEx : ContextMenu
    {
        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new MenuItemEx();
        }

        protected override void OnOpened(System.Windows.RoutedEventArgs e)
        {
            var bindingExpression = BindingOperations.GetBindingExpression(this, ItemsSourceProperty);
            bindingExpression?.UpdateTarget();
            base.OnOpened(e);
        }
    }
}