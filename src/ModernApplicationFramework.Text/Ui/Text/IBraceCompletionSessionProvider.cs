using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Text
{
    public interface IBraceCompletionSessionProvider
    {
        bool TryCreateSession(ITextView textView, SnapshotPoint openingPoint, char openingBrace, char closingBrace, out IBraceCompletionSession session);
    }
}