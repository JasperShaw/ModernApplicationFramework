using System.Windows;

namespace ModernApplicationFramework.Controls.Menu
{
    /// <summary>
    /// A custom styled <see cref="ScrollViewer"/> for menus
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Controls.ScrollViewer" />
    public class MenuScrollViewer : ScrollViewer
    {
        static MenuScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuScrollViewer), new FrameworkPropertyMetadata(typeof(MenuScrollViewer)));
        }
    }
}
