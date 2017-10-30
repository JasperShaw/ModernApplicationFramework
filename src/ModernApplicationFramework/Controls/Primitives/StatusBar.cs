using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Primitives
{
    /// <inheritdoc cref="System.Windows.Controls.Primitives.StatusBar" />
    /// <summary>
    /// A custom styled <see cref="T:System.Windows.Controls.Primitives.StatusBar" /> 
    /// implementing the <see cref="T:ModernApplicationFramework.Interfaces.Controls.IStatusBar" /> interface which is used for exporting an instance
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Primitives.StatusBar" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Controls.IStatusBar" />
    [Export(typeof(IStatusBar))]
    public class BasicStatusBar : System.Windows.Controls.Primitives.StatusBar, IStatusBar
    {
        static BasicStatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BasicStatusBar), new FrameworkPropertyMetadata(typeof(BasicStatusBar)));
        }
    }
}