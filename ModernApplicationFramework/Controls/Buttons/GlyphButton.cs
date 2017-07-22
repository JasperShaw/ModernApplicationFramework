using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Buttons
{
    /// <summary>
    /// A simple button control that has a geometry icon
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Controls.Buttons.Button" />
    public class GlyphButton : Button
    {
        /// <summary>
        /// The geometry data property
        /// </summary>
        public static readonly DependencyProperty GlyphDataProperty = DependencyProperty.Register(
            "GlyphData", typeof(Geometry), typeof(GlyphButton), new PropertyMetadata(default(Geometry)));

        static GlyphButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlyphButton),
                new FrameworkPropertyMetadata(typeof(GlyphButton)));
        }

        public Geometry GlyphData
        {
            get => (Geometry) GetValue(GlyphDataProperty);
            set => SetValue(GlyphDataProperty, value);
        }
    }
}