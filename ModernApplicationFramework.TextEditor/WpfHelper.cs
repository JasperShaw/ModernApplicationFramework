using System;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    public static class WpfHelper
    {
        public static bool TypefacesEqual(Typeface typeface, Typeface other)
        {
            if (typeface == null)
                return other == null;
            return typeface.Equals(other);
        }

        public static bool BrushesEqual(Brush brush, Brush other)
        {
            if (brush == null || other == null)
                return brush == other;
            if (brush.Opacity == 0.0 && other.Opacity == 0.0)
                return true;
            SolidColorBrush solidColorBrush1 = brush as SolidColorBrush;
            SolidColorBrush solidColorBrush2 = other as SolidColorBrush;
            if (solidColorBrush1 == null || solidColorBrush2 == null)
                return brush.Equals((object)other);
            Color color = solidColorBrush1.Color;
            if (color.A == (byte)0)
            {
                color = solidColorBrush2.Color;
                if (color.A == (byte)0)
                    return true;
            }
            if (solidColorBrush1.Color == solidColorBrush2.Color)
                return Math.Abs(solidColorBrush1.Opacity - solidColorBrush2.Opacity) < 0.01;
            return false;
        }
    }
}