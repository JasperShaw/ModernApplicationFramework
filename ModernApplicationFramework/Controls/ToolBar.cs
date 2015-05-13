using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class ToolBar : System.Windows.Controls.ToolBar
    {
        public static readonly DependencyProperty IdentifierNameProperty = DependencyProperty.Register(
            "IdentifierName", typeof (string), typeof (ToolBar), new PropertyMetadata(default(string)));

        public string IdentifierName
        {
            get { return (string) GetValue(IdentifierNameProperty); }
            set { SetValue(IdentifierNameProperty, value); }
        }

        static ToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(typeof(ToolBar)));
        }
    }
}
