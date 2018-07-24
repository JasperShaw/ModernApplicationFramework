namespace ModernApplicationFramework.TextEditor.Text
{
    public interface IHierarchicalDifferenceCollection : IDifferenceCollection<string>
    {
        ITokenizedStringList LeftDecomposition { get; }

        ITokenizedStringList RightDecomposition { get; }

        IHierarchicalDifferenceCollection GetContainedDifferences(int index);

        bool HasContainedDifferences(int index);
    }
}