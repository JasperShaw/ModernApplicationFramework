using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    public class GlyphButton : Button
    {
        public static readonly DependencyProperty GlyphDataProperty = DependencyProperty.Register(
            "GlyphData", typeof(Geometry), typeof(GlyphButton), new PropertyMetadata(default(Geometry)));

        static GlyphButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlyphButton),
                new FrameworkPropertyMetadata(typeof(GlyphButton)));
        }

        public Geometry GlyphData
        {
            get { return (Geometry) GetValue(GlyphDataProperty); }
            set { SetValue(GlyphDataProperty, value); }
        }
    }
}