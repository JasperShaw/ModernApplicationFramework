using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Primitives
{
    [Export(typeof(IStatusBar))]
    public class BasicStatusBar : System.Windows.Controls.Primitives.StatusBar, IStatusBar
    {
        static BasicStatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BasicStatusBar), new FrameworkPropertyMetadata(typeof(BasicStatusBar)));
        }
    }
}