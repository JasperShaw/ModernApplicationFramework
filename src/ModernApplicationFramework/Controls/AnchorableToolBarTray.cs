using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class AnchorableToolBarTray : ToolBarTray
    {
        static AnchorableToolBarTray()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorableToolBarTray),
                new FrameworkPropertyMetadata(typeof(AnchorableToolBarTray)));
        }
    }
}
