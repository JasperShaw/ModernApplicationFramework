using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    public interface ISmartIndentProvider
    {
        ISmartIndent CreateSmartIndent(ITextView textView);
    }
}