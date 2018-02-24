namespace ModernApplicationFramework.EditorBase.Interfaces.NewElement
{
    public interface INewElementExtensionsProvider
    {
        string Text { get; }

        uint SortOrder { get; }

        INewElementExtensionTreeNode ExtensionsTree { get; }
    }
}