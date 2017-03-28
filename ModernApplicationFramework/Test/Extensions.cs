using System.Windows.Media;

namespace ModernApplicationFramework.Test
{
    public static class Extensions
    {
        public static Color ToColorFromRgba(this uint colorValue)
        {
            return Color.FromArgb((byte)(colorValue >> 24), (byte)colorValue, (byte)(colorValue >> 8), (byte)(colorValue >> 16));
        }

        public static uint ToRgba(this Color color)
        {
            return (uint)(color.A << 24 | color.B << 16 | color.G << 8) | color.R;
        }
    }
}
