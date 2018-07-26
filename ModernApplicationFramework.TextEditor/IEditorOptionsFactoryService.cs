namespace ModernApplicationFramework.TextEditor
{
    public interface IEditorOptionsFactoryService
    {
        IEditorOptions GetOptions(IPropertyOwner scope);

        IEditorOptions CreateOptions();

        IEditorOptions GlobalOptions { get; }
    }
}