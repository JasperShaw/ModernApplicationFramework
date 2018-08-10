using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor.Text.Formatting;

namespace ModernApplicationFramework.TextEditor
{
    public interface ILineTransformSourceProvider
    {
        ILineTransformSource Create(ITextView textView);
    }
}