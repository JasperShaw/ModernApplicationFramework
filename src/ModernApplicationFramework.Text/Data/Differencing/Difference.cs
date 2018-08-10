using System.Globalization;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    public class Difference
    {
        public Difference(Span left, Span right, Match before, Match after)
        {
            Left = left;
            Right = right;
            Before = before;
            After = after;
            if (left.Length == 0)
                DifferenceType = DifferenceType.Add;
            else if (right.Length == 0)
                DifferenceType = DifferenceType.Remove;
            else
                DifferenceType = DifferenceType.Change;
        }

        public Span Left { get; }

        public Span Right { get; }

        public Match Before { get; }

        public Match After { get; }

        public DifferenceType DifferenceType { get; }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture.NumberFormat, "(Difference; Type: {0}, Left count: {1}, Right count: {2}", DifferenceType, Left.Length, Right.Length);
        }

        public override bool Equals(object obj)
        {
            if (obj is Difference difference && Equals(DifferenceType, difference.DifferenceType) && (Equals(Before, difference.Before) && Equals(After, difference.After)) && Equals(Left, difference.Left))
                return Equals(Right, difference.Right);
            return false;
        }

        public override int GetHashCode()
        {
            return Before.GetHashCode() << 24 ^ After.GetHashCode() << 16 ^ Left.GetHashCode() << 8 ^ Right.GetHashCode();
        }
    }
}