using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    public class TitleBarButton : System.Windows.Controls.Button
    {

        public static readonly DependencyProperty GlyphDataProperty = DependencyProperty.Register(
            "GlyphData", typeof (Geometry), typeof (TitleBarButton), new PropertyMetadata(default(Geometry)));

        public Geometry GlyphData
        {
            get { return (Geometry) GetValue(GlyphDataProperty); }
            set { SetValue(GlyphDataProperty, value); }
        }
        static TitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBarButton), new FrameworkPropertyMetadata(typeof(TitleBarButton)));
        }
    }
}
