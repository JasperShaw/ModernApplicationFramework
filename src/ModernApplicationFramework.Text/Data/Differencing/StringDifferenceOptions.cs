using System;
using System.Globalization;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    public struct StringDifferenceOptions
    {
        public ContinueProcessingPredicate<string> ContinueProcessingPredicate { get; }
        public StringDifferenceTypes DifferenceType { get; set; }

        public bool IgnoreTrimWhiteSpace { get; set; }

        public WordSplitBehavior WordSplitBehavior { get; }

        public StringDifferenceOptions(StringDifferenceTypes differenceType, bool ignoreTrimWhiteSpace)
        {
            this = new StringDifferenceOptions();
            DifferenceType = differenceType;
            IgnoreTrimWhiteSpace = ignoreTrimWhiteSpace;
        }

        public StringDifferenceOptions(StringDifferenceOptions other)
        {
            this = new StringDifferenceOptions();
            DifferenceType = other.DifferenceType;
            IgnoreTrimWhiteSpace = other.IgnoreTrimWhiteSpace;
            WordSplitBehavior = other.WordSplitBehavior;
            ContinueProcessingPredicate = other.ContinueProcessingPredicate;
        }

        public static bool operator ==(StringDifferenceOptions left, StringDifferenceOptions right)
        {
            if ((ValueType) left == (ValueType) right)
                return true;
            if ((ValueType) left == null || (ValueType) right == null || left.DifferenceType != right.DifferenceType ||
                left.IgnoreTrimWhiteSpace != right.IgnoreTrimWhiteSpace ||
                left.WordSplitBehavior != right.WordSplitBehavior)
                return false;
            return left.ContinueProcessingPredicate == right.ContinueProcessingPredicate;
        }

        public static bool operator !=(StringDifferenceOptions left, StringDifferenceOptions right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StringDifferenceOptions))
                return false;
            return this == (StringDifferenceOptions) obj;
        }

        public override int GetHashCode()
        {
            return DifferenceType.GetHashCode() ^ IgnoreTrimWhiteSpace.GetHashCode() ^ WordSplitBehavior.GetHashCode() ^
                   (ContinueProcessingPredicate != null ? ContinueProcessingPredicate.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "Type: {0}, IgnoreTrimWhiteSpace: {1}, WordSplitBehavior: {2}, ContinueProcessingPredicate: {3}",
                DifferenceType, IgnoreTrimWhiteSpace, WordSplitBehavior,
                ContinueProcessingPredicate);
        }
    }
}