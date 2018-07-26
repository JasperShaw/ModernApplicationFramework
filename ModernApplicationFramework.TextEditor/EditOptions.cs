using ModernApplicationFramework.TextEditor.Text.Differencing;

namespace ModernApplicationFramework.TextEditor
{
    public struct EditOptions
    {
        public static readonly EditOptions None = new EditOptions();
        public static readonly EditOptions DefaultMinimalChange = new EditOptions(new StringDifferenceOptions()
        {
            DifferenceType = StringDifferenceTypes.Line | StringDifferenceTypes.Word
        });

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

        public bool ComputeMinimalChange { get; }

        public StringDifferenceOptions DifferenceOptions { get; }

        public override string ToString()
        {
            if (this == None || !ComputeMinimalChange)
                return "{none}";
            return DifferenceOptions.ToString();
        }

        public override int GetHashCode()
        {
            if (this == None || !ComputeMinimalChange)
                return 0;
            return DifferenceOptions.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EditOptions))
                return false;
            EditOptions editOptions = (EditOptions)obj;
            if (editOptions.ComputeMinimalChange != ComputeMinimalChange)
                return false;
            if (!ComputeMinimalChange)
                return true;
            return editOptions.DifferenceOptions == DifferenceOptions;
        }

        public static bool operator ==(EditOptions left, EditOptions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EditOptions left, EditOptions right)
        {
            return !(left == right);
        }
    }
}