using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewMarginProvider
    {
        ITextViewMargin CreateMargin(ITextViewHost wpfTextViewHost, ITextViewMargin marginContainer);
    }
}