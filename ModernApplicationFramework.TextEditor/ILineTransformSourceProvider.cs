using ModernApplicationFramework.TextEditor.Text.Formatting;

namespace ModernApplicationFramework.TextEditor
{
    public interface ILineTransformSourceProvider
    {
        ILineTransformSource Create(ITextView textView);
    }
}