namespace ModernApplicationFramework.TextEditor
{
    public interface IEditorOperationsFactoryService
    {
        IEditorOperations GetEditorOperations(ITextView textView);
    }
}