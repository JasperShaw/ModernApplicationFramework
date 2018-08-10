using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Text.Data
{
    public struct EditOptions
    {
        public static readonly EditOptions DefaultMinimalChange = new EditOptions(new StringDifferenceOptions
        {
            DifferenceType = StringDifferenceTypes.Line | StringDifferenceTypes.Word
        });

        public static readonly EditOptions None = new EditOptions();

        public bool ComputeMinimalChange { get; }

        public StringDifferenceOptions DifferenceOptions { get; }

        public EditOptions(StringDifferenceOptions differenceOptions)
        {
            ComputeMinimalChange = true;
            DifferenceOptions = differenceOptions;
        }

        public EditOptions(bool computeMinimalChange, StringDifferenceOptions differenceOptions)
        {
            ComputeMinimalChange = computeMinimalChange;
            DifferenceOptions = differenceOptions;
        }

        public static bool operator ==(EditOptions left, EditOptions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EditOptions left, EditOptions right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EditOptions))
                return false;
            var editOptions = (EditOptions) obj;
            if (editOptions.ComputeMinimalChange != ComputeMinimalChange)
                return false;
            if (!ComputeMinimalChange)
                return true;
            return editOptions.DifferenceOptions == DifferenceOptions;
        }

        public override int GetHashCode()
        {
            if (this == None || !ComputeMinimalChange)
                return 0;
            return DifferenceOptions.GetHashCode();
        }

        public override string ToString()
        {
            if (this == None || !ComputeMinimalChange)
                return "{none}";
            return DifferenceOptions.ToString();
        }
    }
}