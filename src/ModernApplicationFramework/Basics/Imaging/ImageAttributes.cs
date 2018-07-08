using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Imaging;

namespace ModernApplicationFramework.Basics.Imaging
{
    public struct ImageAttributes : IEquatable<ImageAttributes>, IFormattable
    {
        private byte _packedFields;
        public Int16Size DeviceSize;
        public Color? Background;
        private Color? _grayscaleBiasColor;

        public bool Grayscale
        {
            get => Convert.ToBoolean(GetPackedValue(2, 1));
            set => SetPackedValue(2, 1, Convert.ToByte(value));
        }

        public bool HasLightBackground
        {
            get
            {
                if (Background.HasValue)
                    return Background.Value.IsLight();
                return false;
            }
        }

        public bool HasDarkBackground
        {
            get
            {
                if (Background.HasValue)
                    return Background.Value.IsDark();
                return false;
            }
        }

        public Color GrayscaleBiasColor
        {
            get
            {
                if (_grayscaleBiasColor.HasValue)
                    return _grayscaleBiasColor.Value;
                if (HighContrast)
                    return ImageLibrary.HighContrastGrayscaleBiasColor;
                return ImageLibrary.DefaultGrayscaleBiasColor;
            }
            set => _grayscaleBiasColor = value;
        }

        public bool HighContrast
        {
            get => Convert.ToBoolean(GetPackedValue(1, 0));
            set => SetPackedValue(1, 0, Convert.ToByte(value));
        }

        public override string ToString()
        {
            return ToString("g", null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Empty;
        }

        public override int GetHashCode()
        {
            return HashHelpers.CombineHashes(HashHelpers.CombineHashes(HashHelpers.CombineHashes(_packedFields, Background.GetHashCode()), GrayscaleBiasColor.GetHashCode()), DeviceSize.GetHashCode());
        }

        public static bool operator ==(ImageAttributes attr1, ImageAttributes attr2)
        {
            return attr1.Equals(attr2);
        }

        public static bool operator !=(ImageAttributes attr1, ImageAttributes attr2)
        {
            return !attr1.Equals(attr2);
        }

        public override bool Equals(object other)
        {
            if (!(other is ImageAttributes))
                return false;
            return Equals((ImageAttributes)other);
        }

        public bool Equals(ImageAttributes other)
        {
            if (_packedFields != other._packedFields)
                return false;
            Color? background1 = Background;
            Color? background2 = other.Background;
            return (background1.HasValue == background2.HasValue
                       ? (background1.HasValue
                           ? (background1.GetValueOrDefault() != background2.GetValueOrDefault() ? 1 : 0)
                           : 0)
                       : 1) == 0 && !(GrayscaleBiasColor != other.GrayscaleBiasColor) &&
                   !(DeviceSize != other.DeviceSize);
        }

        private int GetPackedValue(byte mask, int shift)
        {
            return (_packedFields & mask) >> shift;
        }

        private void SetPackedValue(byte mask, int shift, byte value)
        {
            _packedFields = (byte)(_packedFields & ~mask | (byte)((uint)value << shift) & mask);
        }
    }

    public struct Int16Size : IEquatable<Int16Size>, IFormattable
    {
        private short _width;
        private short _height;

        public Int16Size(short width, short height)
        {
            Validate.IsWithinRange(width, 0, short.MaxValue, nameof(width));
            Validate.IsWithinRange(height, 0, short.MaxValue, nameof(height));
            _width = width;
            _height = height;
        }

        public Int16Size(int width, int height)
        {
            Validate.IsWithinRange(width, 0, short.MaxValue, nameof(width));
            Validate.IsWithinRange(height, 0, short.MaxValue, nameof(height));
            _width = (short)width;
            _height = (short)height;
        }

        public Int16Size(Size size)
        {
            if (size.IsEmpty)
            {
                _width = short.MinValue;
                _height = short.MinValue;
            }
            else
            {
                var num1 = (long)Math.Round(size.Width);
                var num2 = (long)Math.Round(size.Height);
                _width = (short)num1;
                _height = (short)num2;
            }
        }

        public short Width
        {
            get => _width;
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException();
                Validate.IsWithinRange(value, 0, short.MaxValue, nameof(value));
                _width = value;
            }
        }

        public short Height
        {
            get => _height;
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException();
                Validate.IsWithinRange(value, 0, short.MaxValue, nameof(value));
                _height = value;
            }
        }

        public int Area
        {
            get
            {
                if (!IsEmpty)
                    return _width * _height;
                return 0;
            }
        }

        public static Int16Size Empty => CreateEmptySize();

        public bool IsEmpty => _width < 0;

        public override bool Equals(object other)
        {
            if (!(other is Int16Size))
                return false;
            return Equals((Int16Size)other);
        }

        public override int GetHashCode()
        {
            return _width << 16 | (ushort)_height;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public bool Equals(Int16Size other)
        {
            if (_width == other._width)
                return _height == other._height;
            return false;
        }

        public static bool operator ==(Int16Size size1, Int16Size size2)
        {
            return size1.Equals(size2);
        }

        public static bool operator !=(Int16Size size1, Int16Size size2)
        {
            return !size1.Equals(size2);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (IsEmpty)
                return "Empty";
            string format1 = "{1:" + format + "}{0}{2:" + format + "}";
            char numericListSeparator = GetNumericListSeparator(formatProvider);
            return string.Format(formatProvider, format1, numericListSeparator, _width, _height);
        }

        public Size ToWindowsSize()
        {
            return new Size(_width, _height);
        }

        public string ToDimensionString()
        {
            if (IsEmpty)
                return ToString();
            return _width.ToString() + "x" + _height.ToString();
        }

        private static char GetNumericListSeparator(IFormatProvider provider)
        {
            NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
            return instance.NumberDecimalSeparator.Length > 0 && instance.NumberDecimalSeparator[0] == ',' ? ';' : ',';
        }

        private static Int16Size CreateEmptySize()
        {
            return new Int16Size
            {
                _width = short.MinValue,
                _height = short.MinValue
            };
        }
    }
}
