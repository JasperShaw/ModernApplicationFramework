using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor.Commanding.Commands
{
    public sealed class PasteCommandArgs : EditorCommandArgs
    {
        public PasteCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer)
        {
        }
    }
}