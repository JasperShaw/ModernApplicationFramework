using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.MVVM.Demo.Modules.StatusBar
{
    [Export(typeof(IStatusBar))]
    public class ExtendedStatusBar : System.Windows.Controls.Primitives.StatusBar, IStatusBar
    {
        static ExtendedStatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedStatusBar), new FrameworkPropertyMetadata(typeof(ExtendedStatusBar)));
        }
    }
}
