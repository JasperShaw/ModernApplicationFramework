using System.Windows;

namespace ModernApplicationFramework.Controls.Menu
{
    /// <summary>
    /// Custom context menu item
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Controls.Menu.MenuItem" />
    public class ContextMenuItem : MenuItem
    {
        static ContextMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenuItem),
                new FrameworkPropertyMetadata(typeof(ContextMenuItem)));
        }
    }
}