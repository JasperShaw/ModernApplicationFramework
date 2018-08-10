using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Operations
{
    public interface IEditorOperationsFactoryService
    {
        IEditorOperations GetEditorOperations(ITextView textView);
    }
}