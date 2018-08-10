using System;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public struct LineTransform
    {
        public LineTransform(double verticalScale)
        {
            this = new LineTransform(0.0, 0.0, verticalScale, 0.0);
        }

        public LineTransform(double topSpace, double bottomSpace, double verticalScale)
        {
            this = new LineTransform(topSpace, bottomSpace, verticalScale, 0.0);
        }

        public LineTransform(double topSpace, double bottomSpace, double verticalScale, double right)
        {
            if (double.IsNaN(topSpace))
                throw new ArgumentOutOfRangeException(nameof(topSpace));
            if (double.IsNaN(bottomSpace))
                throw new ArgumentOutOfRangeException(nameof(bottomSpace));
            if (verticalScale <= 0.0 || double.IsNaN(verticalScale))
                throw new ArgumentOutOfRangeException(nameof(verticalScale));
            if (right < 0.0 || double.IsNaN(right))
                throw new ArgumentOutOfRangeException(nameof(right));
            TopSpace = topSpace;
            BottomSpace = bottomSpace;
            VerticalScale = verticalScale;
            Right = right;
        }

        public double TopSpace { get; }

        public double BottomSpace { get; }

        public double VerticalScale { get; }

        public double Right { get; }

        public static LineTransform Combine(LineTransform transform1, LineTransform transform2)
        {
            return new LineTransform(Math.Max(transform1.TopSpace, transform2.TopSpace), Math.Max(transform1.BottomSpace, transform2.BottomSpace), transform1.VerticalScale * transform2.VerticalScale, Math.Max(transform1.Right, transform2.Right));
        }

        public override int GetHashCode()
        {
            double num1 = TopSpace;
            int hashCode1 = num1.GetHashCode();
            num1 = BottomSpace;
            int hashCode2 = num1.GetHashCode();
            int num2 = hashCode1 ^ hashCode2;
            num1 = VerticalScale;
            int hashCode3 = num1.GetHashCode();
            int num3 = num2 ^ hashCode3;
            num1 = Right;
            int hashCode4 = num1.GetHashCode();
            return num3 ^ hashCode4;
        }

        public override bool Equals(object obj)
        {
            if (obj is LineTransform transform)
                return this == transform;
            return false;
        }

        public static bool operator ==(LineTransform transform1, LineTransform transform2)
        {
            if (transform1.TopSpace == transform2.TopSpace && transform1.BottomSpace == transform2.BottomSpace && transform1.VerticalScale == transform2.VerticalScale)
                return transform1.Right == transform2.Right;
            return false;
        }

        public static bool operator !=(LineTransform transform1, LineTransform transform2)
        {
            return !(transform1 == transform2);
        }
    }
}