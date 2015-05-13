using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    public class ContextMenuItem : MenuItem
    {
        static ContextMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenuItem),
                new FrameworkPropertyMetadata(typeof(ContextMenuItem)));
        }
    }

    public class ContextMenuGlyphItem : ContextMenuItem
    {
        public static readonly DependencyProperty IconForegroundProperty = DependencyProperty.Register(
            "IconForeground", typeof (Brush), typeof (ContextMenuItem), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty IconGeometryProperty = DependencyProperty.Register("IconGeometry",
            typeof (Geometry), typeof (ContextMenuItem), new PropertyMetadata(default(Geometry)));

        public Geometry IconGeometry
        {
            get { return (Geometry) GetValue(IconGeometryProperty); }
            set { SetValue(IconGeometryProperty, value); }
        }

        public Brush IconForeground
        {
            get { return (Brush) GetValue(IconForegroundProperty); }
            set { SetValue(IconForegroundProperty, value); }
        }

        static ContextMenuGlyphItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ContextMenuGlyphItem),
                new FrameworkPropertyMetadata(typeof (ContextMenuGlyphItem)));
        }
    }
}
