using System.Windows;

namespace ModernApplicationFramework.Controls.Buttons
{
    /// <inheritdoc />
    /// <summary>
    /// I button control that can have a text and an icon
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.Buttons.Button" />
    public class IconTextButton : Button
    {
        /// <summary>
        /// The icon property
        /// </summary>
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
