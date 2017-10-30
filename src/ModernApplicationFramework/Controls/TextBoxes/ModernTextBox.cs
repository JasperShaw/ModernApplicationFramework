using System.Windows;

namespace ModernApplicationFramework.Controls.TextBoxes
{
    /// <summary>
    /// Custom styled <see cref="TextBox"/>
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Controls.TextBoxes.TextBox" />
    public class ModernTextBox : TextBox
    {
        static ModernTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernTextBox), new FrameworkPropertyMetadata(typeof(ModernTextBox)));
        }
    }
}
