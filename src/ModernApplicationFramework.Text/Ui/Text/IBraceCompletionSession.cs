using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Text
{
    public interface IBraceCompletionSession
    {
        ITrackingPoint OpeningPoint { get; }

        ITrackingPoint ClosingPoint { get; }

        ITextView TextView { get; }

        ITextBuffer SubjectBuffer { get; }

        char OpeningBrace { get; }

        char ClosingBrace { get; }

        void Start();

        void Finish();

        void PreOverType(out bool handledCommand);

        void PostOverType();

        void PreTab(out bool handledCommand);

        void PostTab();

        void PreBackspace(out bool handledCommand);

        void PostBackspace();

        void PreDelete(out bool handledCommand);

        void PostDelete();

        void PreReturn(out bool handledCommand);

        void PostReturn();
    }
}
