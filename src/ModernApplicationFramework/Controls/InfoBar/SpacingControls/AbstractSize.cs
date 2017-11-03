using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls.InfoBar.SpacingControls
{
    public struct AbstractSize
    {
        private Size _abstractSize;

        public AbstractSize(Orientation naturalOrientation, Orientation actualOrientation)
        {
            NaturalOrientation = naturalOrientation;
            ActualOrientation = actualOrientation;
            _abstractSize = new Size(0.0, 0.0);
        }

        public AbstractSize(Orientation naturalOrientation, Orientation actualOrientation, Size realSize)
        {
            this = new AbstractSize(naturalOrientation, actualOrientation);
            _abstractSize = IsNatural ? realSize : Invert(realSize);
        }

        public AbstractSize(Orientation naturalOrientation, Orientation actualOrientation, double realWidth, double realHeight)
        {
            this = new AbstractSize(naturalOrientation, actualOrientation);
            _abstractSize = IsNatural ? new Size(realWidth, realHeight) : new Size(realHeight, realWidth);
        }

        public Orientation NaturalOrientation { get; }

        public Orientation ActualOrientation { get; }

        public bool IsNatural => NaturalOrientation == ActualOrientation;

        public double AbstractWidth
        {
            get => _abstractSize.Width;
            set => _abstractSize.Width = value;
        }

        public double AbstractHeight
        {
            get => _abstractSize.Height;
            set => _abstractSize.Height = value;
        }

        public Size RealSize
        {
            get
            {
                if (!IsNatural)
                    return Invert(_abstractSize);
                return _abstractSize;
            }
            set => _abstractSize = IsNatural ? value : Invert(value);
        }

        public double RealWidth
        {
            get
            {
                if (!IsNatural)
                    return AbstractHeight;
                return AbstractWidth;
            }
            set
            {
                if (IsNatural)
                    AbstractWidth = value;
                else
                    AbstractHeight = value;
            }
        }

        public double RealHeight
        {
            get
            {
                if (!IsNatural)
                    return AbstractWidth;
                return AbstractHeight;
            }
            set
            {
                if (IsNatural)
                    AbstractHeight = value;
                else
                    AbstractWidth = value;
            }
        }

        public override string ToString()
        {
            return $"Abstract: {_abstractSize as object}  Real: {(object) RealSize}";
        }

        public static Size Invert(Size size)
        {
            return new Size(size.Height, size.Width);
        }
    }
}
