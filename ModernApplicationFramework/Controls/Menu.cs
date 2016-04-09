using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class Menu : System.Windows.Controls.Menu
    {
        static Menu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Menu), new FrameworkPropertyMetadata(typeof(Menu)));
        }
    }
}