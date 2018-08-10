using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor.Commanding
{
    public interface IEditorCommandHandlerServiceFactory
    {
        IEditorCommandHandlerService GetService(ITextView textView);

        IEditorCommandHandlerService GetService(ITextView textView, ITextBuffer subjectBuffer);
    }
}