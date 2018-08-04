namespace ModernApplicationFramework.TextEditor.Commanding
{
    public interface IEditorCommandHandlerServiceFactory
    {
        IEditorCommandHandlerService GetService(ITextView textView);

        IEditorCommandHandlerService GetService(ITextView textView, ITextBuffer subjectBuffer);
    }
}