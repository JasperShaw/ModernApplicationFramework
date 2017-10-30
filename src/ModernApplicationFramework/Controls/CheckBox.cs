using System.Windows;

namespace ModernApplicationFramework.Controls
{
    /// <summary>
    /// A restyled check box control
    /// </summary>
    /// <seealso cref="System.Windows.Controls.CheckBox" />
    public class CheckBox : System.Windows.Controls.CheckBox
    {
        static CheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBox), new FrameworkPropertyMetadata(typeof(CheckBox)));
        }
    }
}