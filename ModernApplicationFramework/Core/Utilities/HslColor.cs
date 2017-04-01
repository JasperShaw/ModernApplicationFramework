using System;
using System.Windows.Media;

namespace ModernApplicationFramework.Core.Utilities
{
    public struct HslColor
    {
        private double _hue;
        private double _saturation;
        private double _luminosity;
        private double _alpha;

        public double Hue
        {
            get => _hue;
            set => _hue = LimitRange(value, 0.0, 360.0);
        }

        public double Saturation
        {
            get => _saturation;
            set => _saturation = LimitRange(value, 0.0, 1.0);
        }

        public double Luminosity
        {
            get => _luminosity;
            set => _luminosity = LimitRange(value, 0.0, 1.0);
        }

        public double Alpha
        {
            get => _alpha;
            set => _alpha = LimitRange(value, 0.0, 1.0);
        }

        public HslColor(double hue, double saturation, double luminosity)
        {
            this = new HslColor(hue, saturation, luminosity, 1.0);
        }

        public HslColor(double hue, double saturation, double luminosity, double alpha)
        {
            _hue = LimitRange(hue, 0.0, 360.0);
            _saturation = LimitRange(saturation, 0.0, 1.0);
            _luminosity = LimitRange(luminosity, 0.0, 1.0);
            _alpha = LimitRange(alpha, 0.0, 1.0);
        }

        public static HslColor FromColor(Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte num1 = Math.Max(r, Math.Max(g, b));
            byte num2 = Math.Min(r, Math.Min(g, b));
            double num3 = num1 - num2;
            double num4 = num1 / (double)byte.MaxValue;
            double num5 = num2 / (double)byte.MaxValue;
            double hue = num1 != num2 ? num1 != r ? num1 != g ? 60.0 * (r - g) / num3 + 240.0 : 60.0 * (b - r) / num3 + 120.0 : (int)(60.0 * (g - b) / num3 + 360.0) % 360 : 0.0;
            double alpha = color.A / (double)byte.MaxValue;
            double luminosity = 0.5 * (num4 + num5);
            double saturation = num1 != num2 ? (luminosity > 0.5 ? (num4 - num5) / (2.0 - 2.0 * luminosity) : (num4 - num5) / (2.0 * luminosity)) : 0.0;
            return new HslColor(hue, saturation, luminosity, alpha);
        }

        public Color ToColor()
        {
            double q = Luminosity < 0.5 ? Luminosity * (1.0 + Saturation) : Luminosity + Saturation - Luminosity * Saturation;
            double p = 2.0 * Luminosity - q;
            double num = Hue / 360.0;
            double tC1 = ModOne(num + 1.0 / 3.0);
            double tC2 = num;
            double tC3 = ModOne(num - 1.0 / 3.0);
            return Color.FromArgb((byte)(Alpha * byte.MaxValue), (byte)(ComputeRgbComponent(p, q, tC1) * byte.MaxValue), (byte)(ComputeRgbComponent(p, q, tC2) * byte.MaxValue), (byte)(ComputeRgbComponent(p, q, tC3) * byte.MaxValue));
        }

        private static double ModOne(double value)
        {
            if (value < 0.0)
                return value + 1.0;
            if (value > 1.0)
                return value - 1.0;
            return value;
        }

        private static double ComputeRgbComponent(double p, double q, double tC)
        {
            if (tC < 1.0 / 6.0)
                return p + (q - p) * 6.0 * tC;
            if (tC < 0.5)
                return q;
            if (tC < 2.0 / 3.0)
                return p + (q - p) * 6.0 * (2.0 / 3.0 - tC);
            return p;
        }

        private static double LimitRange(double value, double min, double max)
        {
            value = Math.Max(min, value);
            value = Math.Min(value, max);
            return value;
        }
    }
}
