using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Buttons
{
    /// <inheritdoc />
    /// <summary>
    /// A custom button used for window title bars
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Button" />
    public class TitleBarButton : System.Windows.Controls.Button
    {
        /// <summary>
        /// The geometry data property
        /// </summary>
        public static readonly DependencyProperty GlyphDataProperty = DependencyProperty.Register(
            "GlyphData", typeof(Geometry), typeof(TitleBarButton), new PropertyMetadata(default(Geometry)));

        static TitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBarButton),
                new FrameworkPropertyMetadata(typeof(TitleBarButton)));
        }

        public Geometry GlyphData
        {
            get => (Geometry) GetValue(GlyphDataProperty);
            set => SetValue(GlyphDataProperty, value);
        }
    }
}