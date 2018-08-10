using System;
using System.Globalization;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public struct TextBounds
    {
        private readonly double _bidiWidth;

        public double Bottom => Top + Height;

        public double Height { get; }

        public bool IsRightToLeft => _bidiWidth < 0.0;

        public double Leading { get; }

        public double Left
        {
            get
            {
                if (_bidiWidth < 0.0)
                    return Leading + _bidiWidth;
                return Leading;
            }
        }

        public double Right
        {
            get
            {
                if (_bidiWidth < 0.0)
                    return Leading;
                return Leading + _bidiWidth;
            }
        }

        public double TextBottom => TextTop + TextHeight;

        public double TextHeight { get; }

        public double TextTop { get; }

        public double Top { get; }

        public double Trailing => Leading + _bidiWidth;

        public double Width => Math.Abs(_bidiWidth);

        public TextBounds(double leading, double top, double bidiWidth, double height, double textTop,
            double textHeight)
        {
            if (double.IsNaN(leading))
                throw new ArgumentOutOfRangeException(nameof(leading));
            if (double.IsNaN(top))
                throw new ArgumentOutOfRangeException(nameof(top));
            if (double.IsNaN(bidiWidth))
                throw new ArgumentOutOfRangeException(nameof(bidiWidth));
            if (double.IsNaN(height) || height < 0.0)
                throw new ArgumentOutOfRangeException(nameof(height));
            if (double.IsNaN(textTop))
                throw new ArgumentOutOfRangeException(nameof(textTop));
            if (double.IsNaN(textHeight) || textHeight < 0.0)
                throw new ArgumentOutOfRangeException(nameof(textHeight));
            Leading = leading;
            Top = top;
            _bidiWidth = bidiWidth;
            Height = height;
            TextTop = textTop;
            TextHeight = textHeight;
        }

        public static bool operator ==(TextBounds bounds1, TextBounds bounds2)
        {
            if (bounds1.Leading == bounds2.Leading && bounds1._bidiWidth == bounds2._bidiWidth &&
                bounds1.Top == bounds2.Top && bounds1.Height == bounds2.Height && bounds1.TextTop == bounds2.TextTop)
                return bounds1.TextHeight == bounds2.TextHeight;
            return false;
        }

        public static bool operator !=(TextBounds bounds1, TextBounds bounds2)
        {
            return !(bounds1 == bounds2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TextBounds))
                return false;
            return (TextBounds) obj == this;
        }

        public override int GetHashCode()
        {
            var num1 = Leading;
            var hashCode1 = num1.GetHashCode();
            num1 = Top;
            var hashCode2 = num1.GetHashCode();
            var num2 = hashCode1 ^ hashCode2;
            num1 = _bidiWidth;
            var hashCode3 = num1.GetHashCode();
            var num3 = num2 ^ hashCode3;
            num1 = Height;
            var hashCode4 = num1.GetHashCode();
            var num4 = num3 ^ hashCode4;
            num1 = TextTop;
            var hashCode5 = num1.GetHashCode();
            var num5 = num4 ^ hashCode5;
            num1 = TextHeight;
            var hashCode6 = num1.GetHashCode();
            return num5 ^ hashCode6;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0},{1},{2},{3}]", Leading, Top, Trailing, Bottom);
        }
    }
}