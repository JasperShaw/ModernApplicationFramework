using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls
{
    /// <summary>
    /// A custom styled expander control
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Expander" />
    public class ModernExpander : Expander
    {
        static ModernExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernExpander),
                new FrameworkPropertyMetadata(typeof(ModernExpander)));
        }
    }
}
