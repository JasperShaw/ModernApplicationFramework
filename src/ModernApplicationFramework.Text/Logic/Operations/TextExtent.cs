using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Operations
{
    public struct TextExtent
    {
        public bool IsSignificant { get; }

        public SnapshotSpan Span { get; }

        public TextExtent(SnapshotSpan span, bool isSignificant)
        {
            Span = span;
            IsSignificant = isSignificant;
        }

        public TextExtent(TextExtent textExtent)
        {
            Span = textExtent.Span;
            IsSignificant = textExtent.IsSignificant;
        }

        public static bool operator ==(TextExtent extent1, TextExtent extent2)
        {
            if (extent1.Span == extent2.Span)
                return extent1.IsSignificant == extent2.IsSignificant;
            return false;
        }

        public static bool operator !=(TextExtent extent1, TextExtent extent2)
        {
            return !(extent1 == extent2);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is TextExtent extent)
                return this == extent;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}