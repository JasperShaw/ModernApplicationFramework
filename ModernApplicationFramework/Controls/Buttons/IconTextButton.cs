using System.Windows;

namespace ModernApplicationFramework.Controls.Buttons
{
    public class IconTextButton : Button
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(object), typeof(IconTextButton), new PropertyMetadata(default(object)));

        static IconTextButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconTextButton),
                new FrameworkPropertyMetadata(typeof(IconTextButton)));
        }

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
