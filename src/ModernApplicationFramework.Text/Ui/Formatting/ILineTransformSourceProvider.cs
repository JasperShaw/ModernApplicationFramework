using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ILineTransformSourceProvider
    {
        ILineTransformSource Create(ITextView textView);
    }
}