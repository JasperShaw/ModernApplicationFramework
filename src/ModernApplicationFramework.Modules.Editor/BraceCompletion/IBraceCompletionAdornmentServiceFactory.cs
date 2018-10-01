using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    public interface IBraceCompletionAdornmentServiceFactory
    {
        IBraceCompletionAdornmentService GetOrCreateService(ITextView textView);
    }
}