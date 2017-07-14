using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Buttons
{
    public class TitleBarButton : System.Windows.Controls.Button
    {
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