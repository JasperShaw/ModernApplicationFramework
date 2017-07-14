using System.Windows;

namespace ModernApplicationFramework.Controls.TextBoxes
{
    public class ModernTextBox : TextBox
    {
        static ModernTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernTextBox), new FrameworkPropertyMetadata(typeof(ModernTextBox)));
        }
    }
}
