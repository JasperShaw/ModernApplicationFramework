using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Text
{
    public interface IBraceCompletionContextProvider
    {
        bool TryCreateContext(ITextView textView, SnapshotPoint openingPoint, char openingBrace, char closingBrace,
            out IBraceCompletionContext context);
    }
}