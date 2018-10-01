using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal interface IBraceCompletionAggregator
    {
        string OpeningBraces { get; }

        string ClosingBraces { get; }

        bool IsSupportedContentType(IContentType contentType, char openingBrace);

        bool TryCreateSession(ITextView textView, SnapshotPoint openingPoint, char openingBrace, out IBraceCompletionSession session);
    }
}