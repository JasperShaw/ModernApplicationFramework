using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class Separator: System.Windows.Controls.Separator
    {
        static Separator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Separator), new FrameworkPropertyMetadata(typeof(Separator)));
        }
    }
}
