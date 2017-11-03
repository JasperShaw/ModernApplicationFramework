using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls.InfoBar.SpacingControls
{
    public struct AbstractPoint
    {
        private readonly Orientation _naturalOrientation;
        private readonly Orientation _actualOrientation;
        private Point _abstractPoint;

        public AbstractPoint(Orientation naturalOrientation, Orientation actualOrientation)
        {
            _naturalOrientation = naturalOrientation;
            _actualOrientation = actualOrientation;
            _abstractPoint = new Point(0.0, 0.0);
        }

        public AbstractPoint(Orientation naturalOrientation, Orientation actualOrientation, Point realPoint)
        {
            this = new AbstractPoint(naturalOrientation, actualOrientation);
            _abstractPoint = IsNatural ? realPoint : Invert(realPoint);
        }

        public AbstractPoint(Orientation naturalOrientation, Orientation actualOrientation, double realX, double realY)
        {
            this = new AbstractPoint(naturalOrientation, actualOrientation);
            _abstractPoint = IsNatural ? new Point(realX, realY) : new Point(realY, realX);
        }

        public Orientation NaturalOrientation => _naturalOrientation;

        public Orientation ActualOrientation => _actualOrientation;

        public bool IsNatural => _naturalOrientation == _actualOrientation;

        public double AbstractX
        {
            get => _abstractPoint.X;
            set => _abstractPoint.X = value;
        }

        public double AbstractY
        {
            get => _abstractPoint.Y;
            set => _abstractPoint.Y = value;
        }

        public Point RealPoint
        {
            get => !IsNatural ? Invert(_abstractPoint) : _abstractPoint;
            set => _abstractPoint = IsNatural ? value : Invert(value);
        }

        public double RealX
        {
            get
            {
                if (!IsNatural)
                    return AbstractY;
                return AbstractX;
            }
            set
            {
                if (IsNatural)
                    AbstractX = value;
                else
                    AbstractY = value;
            }
        }

        public double RealY
        {
            get
            {
                if (!IsNatural)
                    return AbstractX;
                return AbstractY;
            }
            set
            {
                if (IsNatural)
                    AbstractY = value;
                else
                    AbstractX = value;
            }
        }

        public override string ToString()
        {
            return $"Abstract: {_abstractPoint as object}  Real: {RealPoint as object}";
        }

        public static Point Invert(Point point)
        {
            return new Point(point.Y, point.X);
        }
    }
}
