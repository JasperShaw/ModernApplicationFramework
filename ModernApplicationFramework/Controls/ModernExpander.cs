using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls
{
    //From Gemini
    public class ModernExpander : Expander
    {
        static ModernExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernExpander),
                new FrameworkPropertyMetadata(typeof(ModernExpander)));
        }
    }
}
